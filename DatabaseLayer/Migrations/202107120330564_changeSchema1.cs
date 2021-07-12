namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeSchema1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SPWebhookNotifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubscriptionId = c.String(),
                        ClientState = c.String(),
                        ExpirationDateTime = c.String(),
                        Resource = c.String(),
                        TenantId = c.String(),
                        SiteUrl = c.String(),
                        WebId = c.String(),
                        NotificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        SubscriptionId = c.String(nullable: false, maxLength: 128),
                        TeamId = c.String(),
                        NotificationUrl = c.String(),
                        ClientState = c.String(),
                        ExpirationDateTime = c.String(),
                        Resource = c.String(),
                        Metadata = c.String(),
                        Type = c.String(),
                        Oid = c.String(),
                        EditLink = c.String(),
                    })
                .PrimaryKey(t => t.SubscriptionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Subscriptions");
            DropTable("dbo.SPWebhookNotifications");
        }
    }
}
