using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.UnitTestProvider;
using xRetry.SpecFlow;
using xRetry.SpecFlow.Parsers;

[assembly: GeneratorPlugin(typeof(GeneratorPlugin))]
namespace xRetry.SpecFlow
{
    public class GeneratorPlugin : IGeneratorPlugin
    {
        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters,
            UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            generatorPluginEvents.RegisterDependencies += registerDependencies;
            generatorPluginEvents.CustomizeDependencies += customiseDependencies;
        }

        private void registerDependencies(object sender, RegisterDependenciesEventArgs eventArgs)
        {
            // BoDi requires these registrations to be named as there are other implementations of these interfaces already registered
            eventArgs.ObjectContainer.RegisterTypeAs<RetryDecorator, ITestMethodDecorator>(nameof(RetryDecorator));
            eventArgs.ObjectContainer.RegisterTypeAs<RetryTagDecorator, ITestMethodTagDecorator>(nameof(RetryTagDecorator));
        }

        private void customiseDependencies(object sender, CustomizeDependenciesEventArgs eventArgs)
        {
            eventArgs.ObjectContainer.RegisterTypeAs<RetryTagParser, IRetryTagParser>();
            //eventArgs.ObjectContainer.RegisterTypeAs<TestGeneratorProvider, IUnitTestGeneratorProvider>();
        }
    }
}
