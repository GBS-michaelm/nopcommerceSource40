using System.Web.Mvc;
using Autofac;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

using Nop.Core.Configuration;



namespace Nop.Plugin.CanvasOverride.GBS
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig nopConfig)
        {

 
            builder.RegisterType<PostFilters>().As<System.Web.Mvc.IFilterProvider>();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}