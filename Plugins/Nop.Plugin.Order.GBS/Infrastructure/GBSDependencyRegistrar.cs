using Autofac;
using Nop.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Orders;
using Nop.Services.Custom.Orders;
using Nop.Plugin.Order.GBS;
using Nop.Services.Messages;
using Nop.Core.Plugins;
using Nop.Services.Custom.Common;
using Nop.Services.Common;
using Nop.Plugin.Order.GBS.Factories;
using Nop.Web.Factories;
using Nop.Plugin.Order.GBS.Orders;

namespace Nop.Services.Order.GBS
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return 2;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {


        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {

            var pluginFinder = new PluginFinder();
            pluginFinder.ReloadPlugins();

            var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName("Order.GBS");

            

            if (pluginDescriptor != null)  // pluginDescriptor.Installed == true
            {
                builder.RegisterType<GBSOrderProcessingService>().As<IOrderProcessingService>().InstancePerLifetimeScope();
                builder.RegisterType<CustomTokenProvider>().As<IMessageTokenProvider>().InstancePerLifetimeScope();
                builder.RegisterType<GBSPdfService>().As<IPdfService>().InstancePerLifetimeScope();
                builder.RegisterType<GBSOrderModelFactory>().As<IOrderModelFactory>().InstancePerLifetimeScope();
                builder.RegisterType<GBSOrderService>().As<IOrderService>().InstancePerLifetimeScope();
                builder.RegisterType<GBSOrderTotalCalculationService>().As<IOrderTotalCalculationService>().InstancePerLifetimeScope();

            }

        }
    }

}
