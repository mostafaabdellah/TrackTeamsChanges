namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTeamsSchema11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        TeamId = c.String(nullable: false, maxLength: 128),
                        TeamId1 = c.String(),
                        CreatedOn = c.DateTime(),
                        DriveId = c.String(),
                        SubscriptionId = c.String(),
                        SubscriptionExpirationDate = c.DateTime(),
                        DeltaToken = c.String(),
                        DisplayName = c.String(),
                        SiteUrl = c.String(),
                        SiteId = c.String(),
                        ListId = c.String(),
                    })
                .PrimaryKey(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Teams");
        }
    }
}
