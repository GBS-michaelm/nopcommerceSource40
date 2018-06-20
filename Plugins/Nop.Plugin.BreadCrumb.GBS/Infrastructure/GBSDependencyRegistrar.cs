using Autofac;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Web.Factories;
using Nop.Plugin.BreadCrumb.GBS.Factories;

namespace Nop.Services.BreadCrumb.GBS
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            var pluginFinder = new PluginFinder();
            pluginFinder.ReloadPlugins();

            var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName("BreadCrumb.GBS");
            if (pluginDescriptor != null)
            {
                builder.RegisterType<GBSProductModelFactory>().As<IProductModelFactory>().InstancePerLifetimeScope();
            }
        }

        public int Order
        {
            get
            {
                return 1000;
            }
        }
    }
}
