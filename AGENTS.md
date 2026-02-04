# Instructions for AI Agents
This repository contains code for multiple NuGet packages. They are all variants for xRetry, enabling tests to be 
retried in different testing frameworks.

Instructions:
 - Read the "Contributing" section of the main README.md for general guidance for developers.
 - Build scripts use `make`, are under `build/`, and should be run from this directory.
   - If using Linux, these should work with a bash terminal.
   - If using Mac, these should work with a zsh terminal.
   - If using Windows, these will not work with Powershell or cmd. You can either:
     - Use bash, e.g. via Git Bash or WSL.
     - Use docker to run them in Linux, volume mounting the repo into the container.
   - Regardless of OS, you can use docker to get the same environment as CI with 
     `docker run --rm -it -v $(pwd):/src -w /src/build joshkeegan/dotnet-mixed-build:8.0`
 - When editing documentation, many of the files are generated. Generated files will have
    "This file is auto-generated..." at the top. Look under `docs/` for originals and use `make docs` to generate.
 - Due to the different test frameworks supported, many projects in this repo are quite similar. When making changes to
    one project, you may want to consider whether the same change should be applied to others. Here are some projects
    that should be considered similar, and how they differ:
   - `xRetry` and `xRetry.v3`: same high-level functionality but targeting different frameworks. Implementations may be
        quite different.
   - `xRetry.SpecFlow` and `xRetry.Reqnroll`: Reqnroll is a fork of SpecFlow. Both of these packages target the xUnit v2
       runners. These are almost identical in implementation, and should also have feature parity where possible.
   - `xRetry.Reqnroll` and `xRetry.v3.Reqnroll`: These are both Reqnroll, but targeting xUnit v2 and xUnit v3 
       respectively. Implementations have some differences, but these should always have feature parity.
