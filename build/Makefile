VERSION=0.5.0#

clean:
	rm -r ../artefacts || true
	rm -r ../*/*/obj || true
	rm -r ../*/*/bin || true

	mkdir -p ../artefacts/nuget

build: clean
	dotnet restore ../

# SpecFlow plugin must be built before the tests
	cd ../src/xRetry.SpecFlow && \
		dotnet build -c Release --no-restore -p:Version=$(VERSION)

# Env var works around bug in MSBuild &/or SpecFlow that affects SpecFlow building feature files: https://github.com/SpecFlowOSS/SpecFlow/issues/1912
	export MSBUILDSINGLELOADCONTEXT=1 && \
		cd ../test/UnitTests && \
		dotnet build -c Release --no-restore -p:Version=$(VERSION)

unit-tests-run:
	cd ../test/UnitTests && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.trx

nuget-create:
	dotnet pack ../src/xRetry \
		/p:Version=$(VERSION) \
		/p:NuspecFile=xRetry.nuspec \
		--no-build \
		-c Release \
		-o ../artefacts/nuget

	dotnet pack ../src/xRetry.SpecFlow \
		-p:Version=$(VERSION) \
		-p:NuspecFile=xRetry.SpecFlow.nuspec \
		--no-build \
		-c Release \
		-o ../artefacts/nuget