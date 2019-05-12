VERSION=0.1.1-alpha02#

clean:
	rm -r ../artefacts || true
	rm -r ../*/*/obj || true
	rm -r ../*/*/bin || true

	mkdir -p ../artefacts/nuget

build: clean
	dotnet restore ../

# SpecFlowPlugin must be built before the tests
	dotnet build -c Release --no-restore /p:Version=$(VERSION) ../src/xRetry.SpecFlowPlugin

# When building from Visual Studio (assume full fat MSBuild), you need the dependencies of the generator plugin in the
#	same directory as the plugin, so publish now & the nuget pack will use them later
	dotnet publish -c Release --no-build /p:Version=$(VERSION) ../src/xRetry.SpecFlowPlugin

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