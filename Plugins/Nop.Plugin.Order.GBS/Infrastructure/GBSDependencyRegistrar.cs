﻿using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Plugins;
using Nop.Plugin.Order.GBS;
using Nop.Plugin.Order.GBS.Controllers;
using Nop.Plugin.Order.GBS.Factories;
using Nop.Plugin.Order.GBS.Orders;
using Nop.Services.Common;
using Nop.Services.Custom.Common;
using Nop.Services.Custom.Orders;
using Nop.Services.Messages;
using Nop.Services.Orders;
using NW = Nop.Web.Controllers;
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
               // builder.RegisterType<OrderController>().As<NW.OrderController>();
                builder.RegisterType<GBSOrderProcessingService>().As<IOrderProcessingService>().InstancePerLifetimeScope();
                builder.RegisterType<CustomTokenProvider>().As<IMessageTokenProvider>().InstancePerLifetimeScope();
                builder.RegisterType<GBSPdfService>().As<IPdfService>().InstancePerLifetimeScope();
                builder.RegisterType<GBSOrderModelFactory>().As<Plugin.Order.GBS.Factories.IOrderModelFactory>().InstancePerLifetimeScope();
                builder.RegisterType<GBSOrderService>().As<IGBSOrderService>().InstancePerLifetimeScope();

            }

        }
    }

}
