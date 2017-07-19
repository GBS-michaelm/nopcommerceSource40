using Autofac;
using Nop.Core.Infrastructure.DependencyManagement;
using System;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Services.Messages;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Custom.Customers;

namespace Nop.Services.Customers.GBS
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

            var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName("Login.GBS");

            

            if (pluginDescriptor != null)  // pluginDescriptor.Installed == true
            {
                builder.RegisterType<GBSCustomerRegistrationService>().As<ICustomerRegistrationService>().InstancePerLifetimeScope();
            }

        }
    }

}
