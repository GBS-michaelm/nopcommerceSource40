using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Plugins;
using Nop.Plugin.Logging.GBS.Logging;

namespace Nop.Services.Logging.GBS
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {


        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {

            var pluginFinder = new PluginFinder();
            pluginFinder.ReloadPlugins();

            var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName("Logging.GBS");

            

            if (pluginDescriptor != null)  // pluginDescriptor.Installed == true
            {
                builder.RegisterType<GBSLogger>().As<ILogger>().InstancePerLifetimeScope();

            }

        }
    }

}
