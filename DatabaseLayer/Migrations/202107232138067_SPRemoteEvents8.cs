namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SPRemoteEvents8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteEvents", "Processed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RemoteEvents", "Processed");
        }
    }
}
