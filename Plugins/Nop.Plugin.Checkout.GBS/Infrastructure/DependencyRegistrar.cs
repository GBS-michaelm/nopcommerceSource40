using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Checkout.GBS.Controllers;

namespace Nop.Plugin.Checkout.GBS
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig nopConfig)
        {

            //builder.RegisterType<CheckoutController>().As<Nop.Web.Controllers.CheckoutController>();
            //builder.RegisterType<LoadFilters>().As<System.Web.Mvc.IFilterProvider>();
            //builder.RegisterType<PostFilters>().As<System.Web.Mvc.IFilterProvider>();
        }

        public int Order
        {
            get { return 10; }
        }
    }
}