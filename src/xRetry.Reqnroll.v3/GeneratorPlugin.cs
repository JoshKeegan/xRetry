using System;
using System.Linq;
using Reqnroll.BoDi;
using Reqnroll.Generator.Plugins;
using Reqnroll.Generator.UnitTestProvider;
using Reqnroll.Generator.CodeDom;
using Reqnroll.Infrastructure;
using Reqnroll.UnitTestProvider;
using xRetry.Reqnroll.v3;
using xRetry.Reqnroll.v3.Parsers;

[assembly: GeneratorPlugin(typeof(GeneratorPlugin))]
namespace xRetry.Reqnroll.v3
{
    public class GeneratorPlugin : IGeneratorPlugin
    {
        private const string XUNIT3_PROVIDER_NAME = "xunit3";

        public void Initialize(GeneratorPluginEvents generatorPluginEvents, GeneratorPluginParameters generatorPluginParameters,
            UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            // Register our dependencies in the RegisterDependencies event
            // This runs before the provider is resolved
            generatorPluginEvents.RegisterDependencies += RegisterDependencies;
        }

        private void RegisterDependencies(object sender, RegisterDependenciesEventArgs eventArgs)
        {
            var container = eventArgs.ObjectContainer;
            
            // Register the retry tag parser
            container.RegisterTypeAs<RetryTagParser, IRetryTagParser>();
            
            // Register our TestGeneratorProvider using a factory that will wrap the xunit3 provider
            // The factory is called lazily when the provider is first resolved
            container.RegisterFactoryAs<IUnitTestGeneratorProvider>((ctx) =>
            {
                var codeDomHelper = ctx.Resolve<CodeDomHelper>();
                var retryTagParser = ctx.Resolve<IRetryTagParser>();
                
                // Try to create an instance of XUnit3TestGeneratorProvider
                // It's sealed and in another assembly, but we can create it via reflection
                // Or we can create a fallback with XUnit2TestGeneratorProvider
                IUnitTestGeneratorProvider innerProvider;
                
                try
                {
                    // Try to get the xunit3 provider type from the Reqnroll.xUnit3.Generator.ReqnrollPlugin assembly
                    var xunit3PluginAssembly = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a => a.GetName().Name == "Reqnroll.xUnit3.Generator.ReqnrollPlugin");
                    
                    if (xunit3PluginAssembly != null)
                    {
                        var xunit3ProviderType = xunit3PluginAssembly.GetType("Reqnroll.xUnit3.Generator.ReqnrollPlugin.XUnit3TestGeneratorProvider");
                        if (xunit3ProviderType != null)
                        {
                            innerProvider = (IUnitTestGeneratorProvider)Activator.CreateInstance(xunit3ProviderType, codeDomHelper);
                            return new TestGeneratorProvider(codeDomHelper, retryTagParser, innerProvider);
                        }
                    }
                }
                catch
                {
                    // Fall through to fallback
                }
                
                // Fallback to XUnit2TestGeneratorProvider (won't generate correct xUnit v3 code)
                innerProvider = new XUnit2TestGeneratorProvider(codeDomHelper);
                return new TestGeneratorProvider(codeDomHelper, retryTagParser, innerProvider);
                
            }, XUNIT3_PROVIDER_NAME);
        }
    }
}
