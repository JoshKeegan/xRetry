#
# Deploy script
# Args:
#	NuGet API Key
#

apiKey=$1

dotnet nuget push --source https://api.nuget.org/v3/index.json --api-key $apiKey ../artefacts/nuget/\*.nupkg