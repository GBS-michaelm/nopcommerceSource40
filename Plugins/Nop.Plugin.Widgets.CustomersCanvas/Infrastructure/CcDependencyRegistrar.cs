using Nop.Core.Infrastructure.DependencyManagement;
using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Widgets.CustomersCanvas.Data;
using Nop.Plugin.Widgets.CustomersCanvas.Domain;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Catalog;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.CustomersCanvas.Infrastructure
{
    public class CcDependencyRegistrar : IDependencyRegistrar
    {
        private const string CC_CONTEXT_NAME = "nop_object_context_product_cc_design";
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<CcProductAttributeFormatter>().As<IProductAttributeFormatter>();
            builder.RegisterType<CcService>().As<ICcService>();

            this.RegisterPluginDataContext<CcDesignObjectContext>(builder, CC_CONTEXT_NAME);
            builder.RegisterType<EfRepository<CcDesign>>()
                .As<IRepository<CcDesign>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CC_CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }
    }
}
