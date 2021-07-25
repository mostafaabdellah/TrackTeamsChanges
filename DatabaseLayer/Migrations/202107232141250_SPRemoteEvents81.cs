namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SPRemoteEvents81 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RemoteEvents", "Processed", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RemoteEvents", "Processed", c => c.Boolean(nullable: false));
        }
    }
}
