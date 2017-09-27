using System.Web.Mvc;
using Autofac;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
//using Nop.Plugin.Shipping.GBS.Filters;
using Nop.Core.Configuration;
using Nop.Services.Shipping;

namespace Nop.Plugin.Shipping.GBS.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig nopConfig)
        {
            //FilterProviders.Providers.Add(new NopFilterProvider());
            builder.RegisterType<GBSShippingComputationPlugin>().As<IShippingRateComputationMethod>().InstancePerRequest();
        }

        public int Order => 0;
    }
}