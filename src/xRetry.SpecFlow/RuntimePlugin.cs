using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;
using xRetry.SpecFlow;

[assembly: RuntimePlugin(typeof(RuntimePlugin))]
namespace xRetry.SpecFlow
{
    public class RuntimePlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters,
            UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            runtimePluginEvents.CustomizeGlobalDependencies += customiseGlobalDependencies;
        }

        private void customiseGlobalDependencies(object sender, CustomizeGlobalDependenciesEventArgs eventArgs)
        {
            eventArgs.ObjectContainer.RegisterTypeAs<TestRuntimeProvider, IUnitTestRuntimeProvider>();
        }
    }
}
