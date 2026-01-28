GENERATED_DISCLAIMER := "[//]: \# (This file is auto-generated, do not modify it directly. Instead, update the files under docs/)"

# Appends a markdown file, padding with newlines to prevent formatting issues.
# Args:
#	- source
#	- destination
# 	- skip src comment [0/1] - required if you need to have no line break between source files
define append
	if [ "$(3)" -eq "0" ]; then \
		printf "\n\n[//]: \# (Src: $(1))\n\n" >> $(2); \
	fi
	cat $(1) >> $(2)
endef

.PHONY: repo
repo:
	echo $(GENERATED_DISCLAIMER) > ../README.md
	$(call append,repo/header.md,../README.md,0)
	$(call append,ciBadge.md,../README.md,0)
	$(call append,nugetBadges.md,../README.md,0)
	$(call append,whenToUse.md,../README.md,0)
	$(call append,xRetry.v3/usage.md,../README.md,0)
	$(call append,xRetry/usage.md,../README.md,0)
	$(call append,xRetry.SpecFlow/usage.md,../README.md,0)
	$(call append,xRetry.Reqnroll/usage.md,../README.md,0)
	$(call append,xRetry.v3.Reqnroll/usage.md,../README.md,0)
	$(call append,logs.md,../README.md,0)
	$(call append,contributing.md,../README.md,0)
	$(call append,repo/footer.md,../README.md,0)

.PHONY: xRetry
xRetry:
	echo $(GENERATED_DISCLAIMER) > ../src/xRetry/README.md
	$(call append,xRetry/header.md,../src/xRetry/README.md,0)
	$(call append,ciBadge.md,../src/xRetry/README.md,0)
	$(call append,whenToUse.md,../src/xRetry/README.md,0)
	$(call append,xRetry/usage.md,../src/xRetry/README.md,0)
	$(call append,logs.md,../src/xRetry/README.md,0)
	$(call append,issues.md,../src/xRetry/README.md,0)

.PHONY: xRetry.SpecFlow
xRetry.SpecFlow:
	echo $(GENERATED_DISCLAIMER) > ../src/xRetry.SpecFlow/README.md
	$(call append,xRetry.SpecFlow/header.md,../src/xRetry.SpecFlow/README.md,0)
	$(call append,ciBadge.md,../src/xRetry.SpecFlow/README.md,0)
	$(call append,whenToUse.md,../src/xRetry.SpecFlow/README.md,0)
	$(call append,xRetry.SpecFlow/usage.md,../src/xRetry.SpecFlow/README.md,0)
	$(call append,logs.md,../src/xRetry.SpecFlow/README.md,0)
	$(call append,issues.md,../src/xRetry.SpecFlow/README.md,0)

.PHONY: xRetry.Reqnroll
xRetry.Reqnroll:
	echo $(GENERATED_DISCLAIMER) > ../src/xRetry.Reqnroll/README.md
	$(call append,xRetry.Reqnroll/header.md,../src/xRetry.Reqnroll/README.md,0)
	$(call append,ciBadge.md,../src/xRetry.Reqnroll/README.md,0)
	$(call append,whenToUse.md,../src/xRetry.Reqnroll/README.md,0)
	$(call append,xRetry.Reqnroll/usage.md,../src/xRetry.Reqnroll/README.md,0)
	$(call append,logs.md,../src/xRetry.Reqnroll/README.md,0)
	$(call append,issues.md,../src/xRetry.Reqnroll/README.md,0)
	
.PHONY: xRetry.v3.Reqnroll
xRetry.v3.Reqnroll:
	echo $(GENERATED_DISCLAIMER) > ../src/xRetry.v3.Reqnroll/README.md
	$(call append,xRetry.v3.Reqnroll/header.md,../src/xRetry.v3.Reqnroll/README.md,0)
	$(call append,ciBadge.md,../src/xRetry.v3.Reqnroll/README.md,0)
	$(call append,whenToUse.md,../src/xRetry.v3.Reqnroll/README.md,0)
	$(call append,xRetry.v3.Reqnroll/usage.md,../src/xRetry.v3.Reqnroll/README.md,0)
	$(call append,logs.md,../src/xRetry.v3.Reqnroll/README.md,0)
	$(call append,issues.md,../src/xRetry.v3.Reqnroll/README.md,0)

.PHONY: xRetry.v3
xRetry.v3:
	echo $(GENERATED_DISCLAIMER) > ../src/xRetry.v3/README.md
	$(call append,xRetry.v3/header.md,../src/xRetry.v3/README.md,0)
	$(call append,ciBadge.md,../src/xRetry.v3/README.md,0)
	$(call append,whenToUse.md,../src/xRetry.v3/README.md,0)
	$(call append,xRetry.v3/usage.md,../src/xRetry.v3/README.md,0)
	$(call append,logs.md,../src/xRetry.v3/README.md,0)
	$(call append,issues.md,../src/xRetry.v3/README.md,0)

.PHONY: all
all: repo xRetry xRetry.SpecFlow xRetry.Reqnroll xRetry.v3 xRetry.v3.Reqnroll