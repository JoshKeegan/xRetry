#!/bin/bash
set -euxo pipefail

#
# Deploy script
# Args:
# - Package name (e.g. xRetry)
# - NuGet API Key

package=$1
apiKey=$2

# Must explicitly say there are no symbols for the nuget package, or it will error saying there aren't any accompanying *.snupkg files
dotnet nuget push \
	--source https://api.nuget.org/v3/index.json \
	--api-key $apiKey \
	--no-symbols \
	../artefacts/nuget/$package.\*.nupkg