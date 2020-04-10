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

	export MSBUILDSINGLELOADCONTEXT=1 && \
		cd ../test/UnitTests.SingleThreaded && \
		dotnet build -c Release --no-restore -p:Version=$(VERSION)

unit-tests-run:
	cd ../test/UnitTests && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.trx

#  Run the single threaded tests with a timeout, as they test deadlock scenarios which would never return if failing
#	You can tell if this times out as it returns exit code 124, which make prints as "Error 124"
	cd ../test/UnitTests.SingleThreaded && \
		timeout 10 dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.SingleThreaded.trx

nuget-create:
	dotnet pack ../src/xRetry \
		-p:Version=$(VERSION) \
		-p:NuspecFile=xRetry.nuspec \
		--no-build \
		-c Release \
		-o ../artefacts/nuget

	dotnet pack ../src/xRetry.SpecFlow \
		-p:Version=$(VERSION) \
		-p:NuspecFile=xRetry.SpecFlow.nuspec \
		--no-build \
		-c Release \
		-o ../artefacts/nuget