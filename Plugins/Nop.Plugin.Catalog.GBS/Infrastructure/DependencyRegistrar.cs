using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc;
using Nop.Core.Data;
using Autofac.Core;
using Nop.Plugin.Catalog.GBS.Factories;
using Nop.Services.Catalog;
using Nop.Plugin.Catalog.GBS.Services;
using Nop.Web.Factories;

namespace Nop.Plugin.Catalog.GBS
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {            
            builder.RegisterType<CatalogModelFactoryCustom>().As<ICatalogModelFactoryCustom>().InstancePerDependency();
            builder.RegisterType<GBSProductAttributeFormatter>().As<IProductAttributeFormatter>();
            builder.RegisterType<Nop.Plugin.Catalog.GBS.Factories.CatalogModelFactory>().As<ICatalogModelFactory>().InstancePerDependency();
            builder.RegisterType<Nop.Plugin.Catalog.GBS.Factories.ProductModelFactory>().As<IProductModelFactory>().InstancePerDependency();

        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2000; }
        }
    }
}
