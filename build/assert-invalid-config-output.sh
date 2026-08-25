#!/usr/bin/env bash

set -euo pipefail

if [[ $# -ne 1 ]]; then
    echo "Usage: $0 <test-project>" >&2
    exit 2
fi

project="$1"
output_directory="$(mktemp -d)"
trap 'rm -rf "$output_directory"' EXIT

target_frameworks="$(dotnet msbuild "$project" -getProperty:TargetFrameworks -nologo)"
target_frameworks="${target_frameworks//$'\r'/}"
if [[ -z "$target_frameworks" ]]; then
    echo "Could not read TargetFrameworks from $project"
    exit 1
fi

IFS=';' read -r -a frameworks <<<"$target_frameworks"

assert_output() {
    local framework="$1"
    local run_name="$2"
    local expected_tests="$3"
    local filter="${4:-}"
    local output_file="$output_directory/$framework-$run_name.txt"
    local test_command=(
        dotnet test "$project"
        -f "$framework"
        --no-build
        -c Release
        --logger "console;verbosity=normal"
    )

    if [[ -n "$filter" ]]; then
        test_command+=(--filter "$filter")
    fi

    if [[ "$framework" == "net48" && "${OS:-}" != "Windows_NT" ]]; then
        test_command+=(-- xUnit.ParallelizeTestCollections=false)
    fi

    echo "Asserting $project ($framework, $run_name)"

    if "${test_command[@]}" >"$output_file" 2>&1; then
        cat "$output_file"
        echo "Expected malformed xretry.json to fail the test run"
        exit 1
    fi

    cat "$output_file"

    grep -Fq 'xRetry configuration file "' "$output_file"
    grep -Fq 'is invalid: maxRetries must be >= 1.' "$output_file"
    grep -Eq "Total tests:[[:space:]]+$expected_tests" "$output_file"
    grep -Eq "Failed:[[:space:]]+$expected_tests" "$output_file"

    if grep -Eq 'TargetInvocationException|CustomAttributeFormatException|No test is available' "$output_file"; then
        echo "Malformed configuration was obscured by a discovery error"
        exit 1
    fi
}

for framework in "${frameworks[@]}"; do
    assert_output "$framework" all 3
    assert_output "$framework" filtered 1 'FullyQualifiedName~ScenarioTwo'
done
