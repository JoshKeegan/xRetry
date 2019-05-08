PACKAGE_VERSION=0.1.0-alpha1#

clean:
	rm -r ../artefacts || true
	rm -r ../*/*/obj || true
	rm -r ../*/*/bin || true

	mkdir -p ../artefacts/nuget

build: clean
	dotnet restore ../

# SpecFlowPlugin must be built before the tests
	dotnet build -c Release --no-restore ../src/xRetry.SpecFlowPlugin
	dotnet build -c Release --no-restore ../test/UnitTests

unit-tests-run:
	cd ../test/UnitTests && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.trx

nuget-create:
	dotnet pack ../src/xRetry \
		/p:Version=$(PACKAGE_VERSION) \
		--no-build \
		-c Release \
		-o ../../artefacts/nuget

	dotnet pack ../src/xRetry.SpecFlowPlugin \
		/p:Version=$(PACKAGE_VERSION) \
		--no-build \
		-c Release \
		-o ../../artefacts/nuget

# Args:
#	- apiKey
nuget-publish:
	dotnet nuget push --source https://api.nuget.org/v3/index.json --api-key $(apiKey) ../artefacts/nuget/\*.nupkg