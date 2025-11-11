XRETRY_VERSION			=1.9.0#
XRETRY_SPECFLOW_VERSION	=1.9.0#
XRETRY_REQNOLL_VERSION	=1.0.0#
XRETRY_V3_VERSION		=1.0.0-rc2#

# Optional: xUnit v3 package can be configured to allow tests to be run targeting different MTP versions
XUNIT_V3_PACKAGE		?=

XUNIT_V3_ARGS :=
ifneq ($(strip $(XUNIT_V3_PACKAGE)),)
XUNIT_V3_ARGS += -p:XunitV3Package=$(XUNIT_V3_PACKAGE)
endif

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
	unit-tests-run-v3-main-all-mtps \
	unit-tests-run-v3-single-threaded-all-mtps

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
unit-tests-run-v3-main:
	cd ../test/UnitTests.v3 && \
		dotnet test -c Release $(XUNIT_V3_ARGS) --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.v3.mtp.trx

.PHONY: unit-tests-run-single-threaded-v3
unit-tests-run-v3-single-threaded:
	cd ../test/UnitTests.SingleThreaded.v3 && \
		timeout 10 dotnet test -c Release $(XUNIT_V3_ARGS) --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.SingleThreaded.v3.mtp.trx

# Convenience targets to run the v3 test packs against both supported MTP versions
.PHONY: unit-tests-run-v3-mtp-v1
unit-tests-run-v3-main-mtp-v1:
	make unit-tests-run-v3-main XUNIT_V3_PACKAGE=xunit.v3.mtp-v1

.PHONY: unit-tests-run-v3-mtp-v2
unit-tests-run-v3-main-mtp-v2:
	make unit-tests-run-v3-main XUNIT_V3_PACKAGE=xunit.v3.mtp-v2

.PHONY: unit-tests-run-v3-all-mtps
unit-tests-run-v3-main-all-mtps: \
	unit-tests-run-v3-main-mtp-v1 \
	unit-tests-run-v3-main-mtp-v2

.PHONY: unit-tests-run-v3-mtp-v1
unit-tests-run-v3-single-threaded-mtp-v1:
	make unit-tests-run-v3-single-threaded XUNIT_V3_PACKAGE=xunit.v3.mtp-v1

.PHONY: unit-tests-run-v3-mtp-v2
unit-tests-run-v3-single-threaded-mtp-v2:
	make unit-tests-run-v3-single-threaded XUNIT_V3_PACKAGE=xunit.v3.mtp-v2

.PHONY: unit-tests-run-v3-all-mtps
unit-tests-run-v3-single-threaded-all-mtps: \
	unit-tests-run-v3-single-threaded-mtp-v1 \
	unit-tests-run-v3-single-threaded-mtp-v2

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