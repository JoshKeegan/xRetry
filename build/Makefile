XRETRY_VERSION			=1.9.0#
XRETRY_SPECFLOW_VERSION	=1.9.0#
XRETRY_REQNOLL_VERSION	=1.0.0#
XRETRY_V3_VERSION		=1.0.0-rc1#

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
	cd ../src/xRetry.v3 && \
    		dotnet build -c Release --no-restore -p:Version=$(XRETRY_V3_VERSION)

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
	
	cd ../test/UnitTests.v3 && \
    		dotnet build -c Release --no-restore
    
	cd ../test/UnitTests.SingleThreaded.v3 && \
		dotnet build -c Release --no-restore

.PHONY: unit-tests-run
unit-tests-run: \
	unit-tests-run-main \
	unit-tests-run-single-threaded \
	unit-tests-run-specflow \
	unit-tests-run-reqnroll \
	unit-tests-run-main-v3 \
	unit-tests-run-single-threaded-v3

.PHONY: unit-tests-run-main
unit-tests-run-main:
	cd ../test/UnitTests && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.trx

.PHONY: unit-tests-run-single-threaded
unit-tests-run-single-threaded:
#  Run the single threaded tests with a timeout, as they test deadlock scenarios which would never return if failing
#	You can tell if this times out as it returns exit code 124, which make prints as "Error 124"
	cd ../test/UnitTests.SingleThreaded && \
		timeout 10 dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.SingleThreaded.trx

.PHONY: unit-tests-run-specflow
unit-tests-run-specflow:
	cd ../test/UnitTests.SpecFlow && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.SpecFlow.trx

.PHONY: unit-tests-run-reqnroll
unit-tests-run-reqnroll:
	cd ../test/UnitTests.Reqnroll && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.Reqnroll.trx

.PHONY: unit-tests-run-main-v3
unit-tests-run-main-v3:
	cd ../test/UnitTests.v3 && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.v3.trx

.PHONY: unit-tests-run-single-threaded-v3
unit-tests-run-single-threaded-v3:
#  Run the single threaded tests with a timeout, as they test deadlock scenarios which would never return if failing
#	You can tell if this times out as it returns exit code 124, which make prints as "Error 124"
	cd ../test/UnitTests.SingleThreaded.v3 && \
		timeout 10 dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.SingleThreaded.v3.trx

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
	
	dotnet pack ../src/xRetry.v3 \
		-p:Version=$(XRETRY_V3_VERSION) \
		-p:NuspecFile=xRetry.v3.nuspec \
		--no-build \
		-c Release \
		-o ../artefacts/nuget

.PHONY: ci
ci: lint build unit-tests-run docs nuget-create