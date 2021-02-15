using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.UnitTestProvider;
using xRetry.SpecFlow;
using xRetry.SpecFlow.Parsers;

[assembly: GeneratorPlugin(typeof(GeneratorPlugin))]

namespace xRetry.SpecFlow
{
    public class GeneratorPlugin : IGeneratorPlugin
    {
        public void Initialize(
            GeneratorPluginEvents generatorPluginEvents,
            GeneratorPluginParameters generatorPluginParameters,
            UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            generatorPluginEvents.RegisterDependencies += RegisterDependencies;
            generatorPluginEvents.CustomizeDependencies += customiseDependencies;
        }

        private void RegisterDependencies(object sender, RegisterDependenciesEventArgs eventArgs)
        {
            eventArgs.ObjectContainer.RegisterTypeAs<RetryAttributeGenerator, ITestMethodDecorator>("AttributeGenerator.retry");
        }

        private void customiseDependencies(object sender, CustomizeDependenciesEventArgs eventArgs)
        {
            var retrySettings = RetrySettings.LoadConfiguration();
            eventArgs.ObjectContainer.RegisterInstanceAs<IRetrySettings>(retrySettings);
            eventArgs.ObjectContainer.RegisterTypeAs<Logger, ILogger>();
            eventArgs.ObjectContainer.RegisterTypeAs<RetryTagParser, IRetryTagParser>();
            eventArgs.ObjectContainer.RegisterTypeAs<TestGeneratorProvider, IUnitTestGeneratorProvider>();
        }
    }
}