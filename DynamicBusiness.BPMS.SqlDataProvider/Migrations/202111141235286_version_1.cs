namespace DynamicBusiness.BPMS.BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    /// <summary>
    /// I Added this to fix Table-valued functions errors. It must be commented but I cannot remove this code.
    /// </summary>
    public partial class version_1 : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.sysBpmsSplit_Result",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            Data = c.String(),
            //        })
            //    .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            //DropTable("dbo.sysBpmsSplit_Result");
        }
    }
}
