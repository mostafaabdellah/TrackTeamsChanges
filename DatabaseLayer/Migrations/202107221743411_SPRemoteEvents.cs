namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SPRemoteEvents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SPRemoteEvents",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EventType = c.Int(nullable: false),
                        ItemEventProperties_WebUrl = c.String(),
                        ItemEventProperties_ListId = c.Guid(nullable: false),
                        ItemEventProperties_ListTitle = c.String(),
                        ItemEventProperties_ListItemId = c.Int(nullable: false),
                        ItemEventProperties_Versionless = c.Boolean(nullable: false),
                        ItemEventProperties_UserDisplayName = c.String(),
                        ItemEventProperties_UserLoginName = c.String(),
                        ItemEventProperties_IsBackgroundSave = c.Boolean(nullable: false),
                        ItemEventProperties_CurrentUserId = c.Int(nullable: false),
                        ItemEventProperties_BeforeUrl = c.String(),
                        ItemEventProperties_AfterUrl = c.String(),
                        ItemEventProperties_ExternalNotificationMessage = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SPRemoteEvents");
        }
    }
}
