using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Widgets.MarketCenters.GBS.Data;
using Nop.Plugin.Widgets.MarketCenters.GBS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc;
using Nop.Data;
using Nop.Core.Data;
using Autofac.Core;
using Nop.Plugin.Widgets.MarketCenters.GBS.Domain;

namespace Nop.Plugin.Widgets.MarketCenters.GBS
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
            builder.RegisterType<MarketCentersService>().As<IMarketCentersService>().InstancePerDependency();

            //data context
            this.RegisterPluginDataContext<MarketCentersObjectContext>(builder, "plugin_object_context_marketcenter");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<MC_Client>>()
                .As<IRepository<MC_Client>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("plugin_object_context_marketcenter"))
                .InstancePerLifetimeScope();            
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2; }
        }
    }
}
