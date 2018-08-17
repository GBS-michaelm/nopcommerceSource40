using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Nop.Core;
using Nop.Data;
using System;

namespace Nop.Plugin.Widgets.CustomersCanvas.Data
{
    public class CcDesignObjectContext : DbContext, IDbContext
    {
        public CcDesignObjectContext(): base("Data Source=|DataDirectory|\\Nop.Db.sdf;Persist Security Info=False")
        {
            
        }
        public CcDesignObjectContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        #region Implementation of IDbContext

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<CcDesignObjectContext>(null);

            modelBuilder.Configurations.Add(new CcDesignMap());

            base.OnModelCreating(modelBuilder);
        }

        public string CreateDatabaseInstallationScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public void Install()
        {
            //It's required to set initializer to null (for SQL Server Compact).
            //otherwise, you'll get something like "The model backing the 'your context name' context has changed since the database was created. Consider using Code First Migrations to update the database"
            Database.SetInitializer<CcDesignObjectContext>(null);

            var script = CreateDatabaseInstallationScript();

            try
            {
                Database.ExecuteSqlCommand(script);
            }
            catch (Exception) { }
            SaveChanges();
        }

        public void Uninstall()
        {
            Database.SetInitializer<CcDesignObjectContext>(null);

            var dbScript = "DROP TABLE CcDesign";
            Database.ExecuteSqlCommand(dbScript);
            SaveChanges();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public System.Collections.Generic.IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public void Detach(object entity)
        {
            throw new System.NotImplementedException();
        }

        public bool ProxyCreationEnabled { get; set; }
        public bool AutoDetectChangesEnabled { get; set; }
    }
}
