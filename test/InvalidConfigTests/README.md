# Invalid configuration output tests

These projects deliberately contain an invalid `xretry.json` and therefore fail when run directly. They verify that
`dotnet test` reports a clear xRetry configuration error for each discovered test, rather than an exception from
attribute construction or no discovered tests. They also verify that the error remains visible when tests are filtered.

The projects use source project references because this check runs with the unit tests, before NuGet packages are
created. They have a separate solution so they do not make "Run All" fail in the main `xRetry.sln`.

Run the assertions from the `build` directory:

```bash
make unit-tests-run-invalid-config
```

The assertion covers xUnit v2 and v3 on every target framework declared by the projects.
