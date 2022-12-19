GENERATED_DISCLAIMER := "[//]: \# (This file is auto-generated, do not modify it directly. Instead, update the files under docs/)"

# Appends a markdown file, padding with newlines to prevent formatting issues.
# Args:
#	- source
#	- destination
define append
	printf "\n\n[//]: \# (Src: $(1))\n\n" >> $(2)
	cat $(1) >> $(2)
endef

.PHONY: repo
repo:
	echo $(GENERATED_DISCLAIMER) > ../README.md
	$(call append,repo/header.md,../README.md)
	$(call append,ciBadge.md,../README.md)
	$(call append,whenToUse.md,../README.md)
	$(call append,xRetry.SpecFlow/usage.md,../README.md)
	$(call append,xRetry/usage.md,../README.md)
	$(call append,logs.md,../README.md)
	$(call append,contributing.md,../README.md)
	$(call append,repo/footer.md,../README.md)

.PHONY: xRetry
xRetry:
	echo $(GENERATED_DISCLAIMER) > ../src/xRetry/README.md
	$(call append,xRetry/header.md,../src/xRetry/README.md)
	$(call append,ciBadge.md,../src/xRetry/README.md)
	$(call append,whenToUse.md,../src/xRetry/README.md)
	$(call append,xRetry/usage.md,../src/xRetry/README.md)
	$(call append,logs.md,../src/xRetry/README.md)
	$(call append,issues.md,../src/xRetry/README.md)

.PHONY: xRetry.SpecFlow
xRetry.SpecFlow:
	echo $(GENERATED_DISCLAIMER) > ../src/xRetry.SpecFlow/README.md
	$(call append,xRetry.SpecFlow/header.md,../src/xRetry.SpecFlow/README.md)
	$(call append,ciBadge.md,../src/xRetry.SpecFlow/README.md)
	$(call append,whenToUse.md,../src/xRetry.SpecFlow/README.md)
	$(call append,xRetry.SpecFlow/usage.md,../src/xRetry.SpecFlow/README.md)
	$(call append,logs.md,../src/xRetry.SpecFlow/README.md)
	$(call append,issues.md,../src/xRetry.SpecFlow/README.md)

.PHONY: all
all: repo xRetry xRetry.SpecFlow