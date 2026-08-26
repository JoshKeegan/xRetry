# Package smoke tests

These projects are deliberately kept in their own `PackageTests.sln`. Do not add them to the root `xRetry.sln`.

The package smoke tests act as consumers of the packed `xRetry` NuGet packages in `../../artefacts/nuget`. The main build cannot restore them correctly: `make build` runs `dotnet restore` before `nuget-create` has produced those packages. If these projects are added to `xRetry.sln`, restore could resolve the current versions from nuget.org instead of testing the packages just built, or fail as soon as the local package versions are bumped beyond the published versions.

`NuGet.config` makes package resolution deterministic by mapping `xRetry*` packages to the local artefacts directory and all other packages to nuget.org.

From the `build` directory, create the packages and then run the smoke tests:

```bash
make nuget-create
make package-tests-run
```
