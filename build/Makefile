VERSION=0.3.0#

clean:
	rm -r ../artefacts || true
	rm -r ../*/*/obj || true
	rm -r ../*/*/bin || true

	mkdir -p ../artefacts/nuget

build: clean
	dotnet restore ../

# SpecFlow plugin must be built before the tests
	dotnet build -c Release --no-restore /p:Version=$(VERSION) ../src/xRetry.SpecFlow

	dotnet build -c Release --no-restore /p:Version=$(VERSION) ../test/UnitTests

unit-tests-run:
	cd ../test/UnitTests && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.trx

nuget-create:
	dotnet pack ../src/xRetry \
		/p:Version=$(VERSION) \
		--no-build \
		-c Release \
		-o ../../artefacts/nuget

	dotnet pack ../src/xRetry.SpecFlow \
		/p:Version=$(VERSION) \
		/p:NuspecFile=xRetry.SpecFlow.nuspec \
		--no-build \
		-c Release \
		-o ../../artefacts/nuget