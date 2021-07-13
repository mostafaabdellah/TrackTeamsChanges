namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeSchema21 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Teams", "SubscriptionId");
            DropColumn("dbo.Teams", "SubscriptionExpirationDate");
            DropColumn("dbo.Teams", "DeltaToken");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Teams", "DeltaToken", c => c.String());
            AddColumn("dbo.Teams", "SubscriptionExpirationDate", c => c.DateTime());
            AddColumn("dbo.Teams", "SubscriptionId", c => c.String());
        }
    }
}
