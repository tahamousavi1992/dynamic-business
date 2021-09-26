namespace DynamicBusiness.BPMS.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Design;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.Migrations.Utilities;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<DynamicBusiness.BPMS.BusinessLogic.Db_BPMSEntities>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DynamicBusiness.BPMS.BusinessLogic.Db_BPMSEntities context)
        {
            //  This method will be called after migrating to the latest version.
 
            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
    
}
