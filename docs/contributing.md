## Contributing
Feel free to open a pull request! If you want to start any sizeable chunk of work, consider 
opening an issue first to discuss, and make sure nobody else is working on the same problem.  

### Developing locally
#### In an IDE
To build and run locally, always build `xRetry.SpecFlowPlugin` with the Release profile before the tests to ensure MSBuild uses the latest version of your changes when building the UnitTests project.  

#### From the terminal
If you install `make` and go to the `build` directory, you can run the following command to run CI locally (run lint, build, run tests and create the nuget packages):
```bash
make ci
```
If that works, all is well!

### Code formatting
Code formatting rules followed for xRetry are fairly standard for C# and are enforced during CI via `dotnet format`. You can see any non-standard rules in the [.editorconfig](.editorconfig) file. If you find your build fails due to this lint check, you can fix all formatting issues by running the `dotnet format` command from the root of the project.