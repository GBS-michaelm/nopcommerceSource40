using Autofac;
using Nop.Core.Infrastructure.DependencyManagement;
using System;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Messages;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Plugin.PriceCalculation.GBS.Catalog;
using Nop.Services.Catalog;

namespace Nop.Services.PriceCalculation.GBS
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return 1000;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {


        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {

            var pluginFinder = new PluginFinder();
            pluginFinder.ReloadPlugins();

            var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName("PriceCalculation.GBS");

            if (pluginDescriptor != null)  // pluginDescriptor.Installed == true
            {
                builder.RegisterType<GBSPriceCalculationService>().As<IPriceCalculationService>().InstancePerLifetimeScope();
            }

        }
    }

}
