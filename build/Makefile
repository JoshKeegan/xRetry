XRETRY_VERSION				=1.9.0#
XRETRY_VERSION_MAX			:= $(shell expr $(shell echo $(XRETRY_VERSION) | cut -d. -f1) + 1).0.0
XRETRY_SPECFLOW_VERSION		=1.9.0#
XRETRY_REQNOLL_VERSION		=1.1.0#
XRETRY_V3_VERSION			=1.0.0-rc3#
XRETRY_V3_VERSION_MAX		:= $(shell expr $(shell echo $(XRETRY_V3_VERSION) | cut -d. -f1) + 1).0.0
XRETRY_V3_REQNROLL_VERSION	=0.1.0-alpha1#
.PHONY: clean
clean:
	rm -r ../artefacts || true
	rm -r ../obj || true
	rm -r ../*/*/obj || true
	rm -r ../*/*/bin || true
	rm -r ../*/*/*/obj || true
	rm -r ../*/*/*/bin || true

	mkdir -p ../artefacts/nuget

.PHONY: lint
lint:
	dotnet format --verify-no-changes ../ || \
		(echo "Lint check failed. You can automatically fix all formatting issues by running \"dotnet format\""; exit 1)

# nuget-create must run before this target. It's not a dependency since CI invokes the targets separately, which would
# create the packages twice.
.PHONY: lint-package-tests
lint-package-tests:
	dotnet format --verify-no-changes ../test/PackageTests

.PHONY: build
build: clean
	dotnet restore ../

# SpecFlow & Reqnroll plugins must be built before the tests
	cd ../src/xRetry && \
		dotnet build -c Release --no-restore -p:Version=$(XRETRY_VERSION)

	cd ../src/xRetry.v3 && \
    	dotnet build -c Release --no-restore -p:Version=$(XRETRY_V3_VERSION)

	cd ../src/xRetry.SpecFlow.Runtime && \
		dotnet build -c Release --no-restore

	cd ../src/xRetry.Reqnroll.Runtime && \
		dotnet build -c Release --no-restore

	cd ../src/xRetry.v3.Reqnroll.Runtime && \
		dotnet build -c Release --no-restore

	cd ../src/xRetry.SpecFlow && \
		dotnet build -c Release --no-restore -p:Version=$(XRETRY_SPECFLOW_VERSION)
	
	cd ../src/xRetry.Reqnroll && \
		dotnet build -c Release --no-restore -p:Version=$(XRETRY_REQNOLL_VERSION)
		
	cd ../src/xRetry.v3.Reqnroll && \
    	dotnet build -c Release --no-restore -p:Version=$(XRETRY_V3_REQNROLL_VERSION)

	cd ../test/UnitTests && \
		dotnet build -c Release --no-restore

	cd ../test/UnitTests.SingleThreaded && \
		dotnet build -c Release --no-restore

	cd ../test/UnitTests.GlobalConfig && \
		dotnet build -c Release --no-restore
	
	cd ../test/UnitTests.SpecFlow && \
		dotnet build -c Release --no-restore

	cd ../test/UnitTests.SpecFlow.GlobalConfig && \
		dotnet build -c Release --no-restore

	cd ../test/UnitTests.SpecFlow.GlobalConfig.RetryUntagged && \
		dotnet build -c Release --no-restore
	
	cd ../test/UnitTests.Reqnroll && \
		dotnet build -c Release --no-restore

	cd ../test/UnitTests.Reqnroll.GlobalConfig && \
		dotnet build -c Release --no-restore

	cd ../test/UnitTests.Reqnroll.GlobalConfig.RetryUntagged && \
		dotnet build -c Release --no-restore
	
	cd ../test/UnitTests.v3 && \
    	dotnet build -c Release --no-restore

	cd ../test/UnitTests.v3.GlobalConfig && \
		dotnet build -c Release --no-restore
    
	cd ../test/UnitTests.SingleThreaded.v3 && \
		dotnet build -c Release --no-restore
		
	cd ../test/UnitTests.v3.Reqnroll && \
		dotnet build -c Release --no-restore

	cd ../test/UnitTests.v3.Reqnroll.GlobalConfig && \
		dotnet build -c Release --no-restore

	cd ../test/UnitTests.v3.Reqnroll.GlobalConfig.RetryUntagged && \
		dotnet build -c Release --no-restore

.PHONY: unit-tests-run
unit-tests-run: \
	unit-tests-run-main \
	unit-tests-run-single-threaded \
	unit-tests-run-main-global-config \
	unit-tests-run-specflow \
	unit-tests-run-specflow-global-config \
	unit-tests-run-specflow-global-config-retry-untagged \
	unit-tests-run-reqnroll \
	unit-tests-run-reqnroll-global-config \
	unit-tests-run-reqnroll-global-config-retry-untagged \
	unit-tests-run-main-v3 \
	unit-tests-run-main-v3-global-config \
	unit-tests-run-single-threaded-v3 \
	unit-tests-run-reqnroll-v3 \
	unit-tests-run-reqnroll-v3-global-config \
	unit-tests-run-reqnroll-v3-global-config-retry-untagged

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

.PHONY: unit-tests-run-main-global-config
unit-tests-run-main-global-config:
	cd ../test/UnitTests.GlobalConfig && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.GlobalConfig.trx

.PHONY: unit-tests-run-specflow
unit-tests-run-specflow:
	cd ../test/UnitTests.SpecFlow && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.SpecFlow.trx

.PHONY: unit-tests-run-specflow-global-config
unit-tests-run-specflow-global-config:
	cd ../test/UnitTests.SpecFlow.GlobalConfig && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.SpecFlow.GlobalConfig.trx

.PHONY: unit-tests-run-specflow-global-config-retry-untagged
unit-tests-run-specflow-global-config-retry-untagged:
	cd ../test/UnitTests.SpecFlow.GlobalConfig.RetryUntagged && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.SpecFlow.GlobalConfig.RetryUntagged.trx

.PHONY: unit-tests-run-reqnroll
unit-tests-run-reqnroll:
	cd ../test/UnitTests.Reqnroll && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.Reqnroll.trx

.PHONY: unit-tests-run-reqnroll-global-config
unit-tests-run-reqnroll-global-config:
	cd ../test/UnitTests.Reqnroll.GlobalConfig && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.Reqnroll.GlobalConfig.trx

.PHONY: unit-tests-run-reqnroll-global-config-retry-untagged
unit-tests-run-reqnroll-global-config-retry-untagged:
	cd ../test/UnitTests.Reqnroll.GlobalConfig.RetryUntagged && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.Reqnroll.GlobalConfig.RetryUntagged.trx

.PHONY: unit-tests-run-main-v3
unit-tests-run-main-v3:
	cd ../test/UnitTests.v3 && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.v3.trx

.PHONY: unit-tests-run-main-v3-global-config
unit-tests-run-main-v3-global-config:
	cd ../test/UnitTests.v3.GlobalConfig && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.v3.GlobalConfig.trx

.PHONY: unit-tests-run-single-threaded-v3
unit-tests-run-single-threaded-v3:
#  Run the single threaded tests with a timeout, as they test deadlock scenarios which would never return if failing
#	You can tell if this times out as it returns exit code 124, which make prints as "Error 124"
	cd ../test/UnitTests.SingleThreaded.v3 && \
		timeout 10 dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.SingleThreaded.v3.trx

.PHONY: unit-tests-run-reqnroll-v3
unit-tests-run-reqnroll-v3:
	cd ../test/UnitTests.v3.Reqnroll && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.v3.Reqnroll.trx

.PHONY: unit-tests-run-reqnroll-v3-global-config
unit-tests-run-reqnroll-v3-global-config:
	cd ../test/UnitTests.v3.Reqnroll.GlobalConfig && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.v3.Reqnroll.GlobalConfig.trx

.PHONY: unit-tests-run-reqnroll-v3-global-config-retry-untagged
unit-tests-run-reqnroll-v3-global-config-retry-untagged:
	cd ../test/UnitTests.v3.Reqnroll.GlobalConfig.RetryUntagged && \
		dotnet test --no-build -c Release --logger:trx\;logfilename=../../../artefacts/testResults/UnitTests.v3.Reqnroll.GlobalConfig.RetryUntagged.trx

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
		-p:xRetryVersionMin=$(XRETRY_VERSION) \
		-p:xRetryVersionMax=$(XRETRY_VERSION_MAX) \
		-p:NuspecFile=xRetry.SpecFlow.nuspec \
		--no-build \
		-c Release \
		-o ../artefacts/nuget
	
	dotnet pack ../src/xRetry.Reqnroll \
		-p:Version=$(XRETRY_REQNOLL_VERSION) \
		-p:xRetryVersionMin=$(XRETRY_VERSION) \
		-p:xRetryVersionMax=$(XRETRY_VERSION_MAX) \
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
	
	dotnet pack ../src/xRetry.v3.Reqnroll \
		-p:Version=$(XRETRY_V3_REQNROLL_VERSION) \
		-p:xRetryV3VersionMin=$(XRETRY_V3_VERSION) \
		-p:xRetryV3VersionMax=$(XRETRY_V3_VERSION_MAX) \
		-p:NuspecFile=xRetry.v3.Reqnroll.nuspec \
		--no-build \
		-c Release \
		-o ../artefacts/nuget

.PHONY: package-tests-run
package-tests-run: \
	package-tests-run-specflow \
	package-tests-run-reqnroll \
	package-tests-run-reqnroll-v3

.PHONY: package-tests-run-specflow
package-tests-run-specflow:
	cd ../test/PackageTests/SpecFlow && \
		dotnet test -c Release -p:xRetrySpecFlowVersion=$(XRETRY_SPECFLOW_VERSION)

.PHONY: package-tests-run-reqnroll
package-tests-run-reqnroll:
	cd ../test/PackageTests/Reqnroll && \
		dotnet test -c Release -p:xRetryReqnrollVersion=$(XRETRY_REQNOLL_VERSION)

.PHONY: package-tests-run-reqnroll-v3
package-tests-run-reqnroll-v3:
	cd ../test/PackageTests/v3.Reqnroll && \
		dotnet test -c Release -p:xRetryV3ReqnrollVersion=$(XRETRY_V3_REQNROLL_VERSION)

.PHONY: ci
ci: lint build unit-tests-run docs nuget-create lint-package-tests package-tests-run
