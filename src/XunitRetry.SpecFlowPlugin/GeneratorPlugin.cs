using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.UnitTestProvider;
using XunitRetry.SpecFlowPlugin;

[assembly: GeneratorPlugin(typeof(GeneratorPlugin))]
namespace XunitRetry.SpecFlowPlugin
{
    public class GeneratorPlugin : IGeneratorPlugin
    {
        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters,
            UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            unitTestProviderConfiguration.UseUnitTestProvider("xunit");

            generatorPluginEvents.CustomizeDependencies += CustomiseDependencies;
        }

        private void CustomiseDependencies(object sender, CustomizeDependenciesEventArgs eventArgs)
        {
            eventArgs.ObjectContainer.RegisterTypeAs<TestGeneratorProvider, IUnitTestGeneratorProvider>();
        }
    }
}
