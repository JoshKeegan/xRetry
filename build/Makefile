VERSION=0.2.0#

clean:
	rm -r ../artefacts || true
	rm -r ../*/*/obj || true
	rm -r ../*/*/bin || true

	mkdir -p ../artefacts/nuget

build: clean
	dotnet restore ../

# SpecFlowPlugin must be built before the tests
	dotnet build -c Release --no-restore /p:Version=$(VERSION) ../src/xRetry.SpecFlowPlugin

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

	dotnet pack ../src/xRetry.SpecFlowPlugin \
		/p:Version=$(VERSION) \
		/p:NuspecFile=xRetry.SpecFlowPlugin.nuspec \
		--no-build \
		-c Release \
		-o ../../artefacts/nuget