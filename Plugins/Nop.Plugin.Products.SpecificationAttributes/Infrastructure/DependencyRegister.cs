using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Plugin.Products.SpecificationAttributes.Controllers;
using NW = Nop.Web.Controllers;
using Autofac;
using Nop.Plugin.Products.SpecificationAttributes.ModelFactory;

namespace Nop.Plugin.Products.SpecificationAttributes
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<OrderModelFactory>().As<IOrderModelFactory>();
            builder.RegisterType<OrderController>().As<NW.OrderController>();
        }

        public int Order
        {
            get { return 10; }
        }
    }
}
