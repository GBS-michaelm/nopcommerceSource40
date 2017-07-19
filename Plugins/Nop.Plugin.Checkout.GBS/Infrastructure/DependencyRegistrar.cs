using System.Web.Mvc;
using Autofac;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

using Nop.Core.Configuration;
using Nop.Services.Shipping;
using Nop.Plugin.Checkout.Filters.GBS;
using Nop.Plugin.Checkout.GBS.Filter;

namespace Nop.Plugin.Checkout.GBS
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig nopConfig)
        {

            //builder.RegisterType<LoadFilters>().As<System.Web.Mvc.IFilterProvider>();
            //builder.RegisterType<PostFilters>().As<System.Web.Mvc.IFilterProvider>();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}