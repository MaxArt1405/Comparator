namespace Comparator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PDFfiles", "CreateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PDFfiles", "CreateTime");
        }
    }
}
