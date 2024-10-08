XRETRY_VERSION			=1.9.0#
XRETRY_SPECFLOW_VERSION	=1.9.0#
XRETRY_REQNOLL_VERSION	=1.0.0#

.PHONY: clean
clean:
	rm -r ../artefacts || true
	rm -r ../*/*/obj || true
	rm -r ../*/*/bin || true

	mkdir -p ../artefacts/nuget

.PHONY: lint
lint:
	dotnet format --verify-no-changes ../ || \
		(echo "Lint check failed. You can automatically fix all formatting issues by running \"dotnet format\""; exit 1)

.PHONY: build
build: clean
	dotnet restore ../

# SpecFlow & Reqnroll plugins must be built before the tests
	cd ../src/xRetry.SpecFlow && \
		dotnet build -c Release --no-restore -p:Version=$(XRETRY_SPECFLOW_VERSION)
	
	cd ../src/xRetry.Reqnroll && \
		dotnet build -c Release --no-restore -p:Version=$(XRETRY_REQNOLL_VERSION)

	cd ../test/UnitTests && \
		dotnet build -c Release --no-restore

	cd ../test/UnitTests.SingleThreaded && \
		dotnet build -c Release --no-restore
	
	cd ../test/UnitTests.SpecFlow && \
		dotnet build -c Release --no-restore
	
	cd ../test/UnitTests.Reqnroll && \
		dotnet build -c Release --no-restore

.PHONY: unit-tests-run
unit-tests-run:
	cd ../test/UnitTests && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.trx

#  Run the single threaded tests with a timeout, as they test deadlock scenarios which would never return if failing
#	You can tell if this times out as it returns exit code 124, which make prints as "Error 124"
	cd ../test/UnitTests.SingleThreaded && \
		timeout 10 dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.SingleThreaded.trx
	
	cd ../test/UnitTests.SpecFlow && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.SpecFlow.trx
	
	cd ../test/UnitTests.Reqnroll && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.Reqnroll.trx

.PHONY: docs
docs:
	cd ../docs && \
		make all

.PHONY: nuget-create
nuget-create:
	dotnet pack ../src/xRetry \
		-p:Version=$(XRETRY_VERSION) \
		-p:NuspecFile=xRetry.nuspec \
		--no-build \
		-c Release \
		-o ../artefacts/nuget

	dotnet pack ../src/xRetry.SpecFlow \
		-p:Version=$(XRETRY_SPECFLOW_VERSION) \
		-p:xRetryVersion=$(XRETRY_VERSION) \
		-p:NuspecFile=xRetry.SpecFlow.nuspec \
		--no-build \
		-c Release \
		-o ../artefacts/nuget
	
	dotnet pack ../src/xRetry.Reqnroll \
		-p:Version=$(XRETRY_REQNOLL_VERSION) \
		-p:xRetryVersion=$(XRETRY_VERSION) \
		-p:NuspecFile=xRetry.Reqnroll.nuspec \
		--no-build \
		-c Release \
		-o ../artefacts/nuget

.PHONY: ci
ci: lint build unit-tests-run docs nuget-create