using Reqnroll.Generator.Plugins;
using Reqnroll.Generator.UnitTestProvider;
using Reqnroll.Infrastructure;
using Reqnroll.UnitTestProvider;
using xRetry.v3.Reqnroll;
using xRetry.v3.Reqnroll.Parsers;

[assembly: GeneratorPlugin(typeof(GeneratorPlugin))]

namespace xRetry.v3.Reqnroll;

public class GeneratorPlugin : IGeneratorPlugin
{
    public void Initialize(GeneratorPluginEvents generatorPluginEvents,
        GeneratorPluginParameters generatorPluginParameters,
        UnitTestProviderConfiguration unitTestProviderConfiguration)
    {
        unitTestProviderConfiguration.UseUnitTestProvider("xunit3");
        generatorPluginEvents.CustomizeDependencies += CustomizeDependencies;
    }

    private void CustomizeDependencies(object sender, CustomizeDependenciesEventArgs eventArgs)
    {
        eventArgs.ObjectContainer.RegisterTypeAs<RetryTagParser, IRetryTagParser>();
        eventArgs.ObjectContainer.RegisterTypeAs<TestGeneratorProvider, IUnitTestGeneratorProvider>();
    }
}