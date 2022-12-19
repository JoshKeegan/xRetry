## Viewing retry logs
By default, you won't see whether your tests are being retried as we make this information available 
via the xunit diagnostic logs but test runners will hide these detailed logs by default.  
To enable them you must configure your xUnit test project to have `diagnosticMessages` set to `true` in the `xunit.runner.json`. 
See the [xUnit docs](https://xunit.net/docs/configuration-files) for a full setup guide of their config file, or see
this projects own unit tests which has been set up with this enabled.