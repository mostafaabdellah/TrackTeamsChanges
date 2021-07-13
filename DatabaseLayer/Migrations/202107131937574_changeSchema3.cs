namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeSchema3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Changes", "RestSubscriptionId", c => c.String());
            AddColumn("dbo.Changes", "GraphSubscriptionId", c => c.String());
            DropColumn("dbo.Changes", "SubscriptionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Changes", "SubscriptionId", c => c.String());
            DropColumn("dbo.Changes", "GraphSubscriptionId");
            DropColumn("dbo.Changes", "RestSubscriptionId");
        }
    }
}
