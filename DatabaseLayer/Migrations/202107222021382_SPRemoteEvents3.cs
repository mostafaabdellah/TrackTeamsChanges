namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SPRemoteEvents3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteEvents", "WebUrl", c => c.String());
            AddColumn("dbo.RemoteEvents", "ListId", c => c.Guid(nullable: false));
            AddColumn("dbo.RemoteEvents", "ListTitle", c => c.String());
            AddColumn("dbo.RemoteEvents", "ListItemId", c => c.Int());
            AddColumn("dbo.RemoteEvents", "Versionless", c => c.Boolean());
            AddColumn("dbo.RemoteEvents", "UserDisplayName", c => c.String());
            AddColumn("dbo.RemoteEvents", "UserLoginName", c => c.String());
            AddColumn("dbo.RemoteEvents", "IsBackgroundSave", c => c.Boolean());
            AddColumn("dbo.RemoteEvents", "CurrentUserId", c => c.Int());
            AddColumn("dbo.RemoteEvents", "BeforeUrl", c => c.String());
            AddColumn("dbo.RemoteEvents", "AfterUrl", c => c.String());
            DropColumn("dbo.RemoteEvents", "ItemEventProperties_WebUrl");
            DropColumn("dbo.RemoteEvents", "ItemEventProperties_ListId");
            DropColumn("dbo.RemoteEvents", "ItemEventProperties_ListTitle");
            DropColumn("dbo.RemoteEvents", "ItemEventProperties_ListItemId");
            DropColumn("dbo.RemoteEvents", "ItemEventProperties_Versionless");
            DropColumn("dbo.RemoteEvents", "ItemEventProperties_UserDisplayName");
            DropColumn("dbo.RemoteEvents", "ItemEventProperties_UserLoginName");
            DropColumn("dbo.RemoteEvents", "ItemEventProperties_IsBackgroundSave");
            DropColumn("dbo.RemoteEvents", "ItemEventProperties_CurrentUserId");
            DropColumn("dbo.RemoteEvents", "ItemEventProperties_BeforeUrl");
            DropColumn("dbo.RemoteEvents", "ItemEventProperties_AfterUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RemoteEvents", "ItemEventProperties_AfterUrl", c => c.String());
            AddColumn("dbo.RemoteEvents", "ItemEventProperties_BeforeUrl", c => c.String());
            AddColumn("dbo.RemoteEvents", "ItemEventProperties_CurrentUserId", c => c.Int());
            AddColumn("dbo.RemoteEvents", "ItemEventProperties_IsBackgroundSave", c => c.Boolean());
            AddColumn("dbo.RemoteEvents", "ItemEventProperties_UserLoginName", c => c.String());
            AddColumn("dbo.RemoteEvents", "ItemEventProperties_UserDisplayName", c => c.String());
            AddColumn("dbo.RemoteEvents", "ItemEventProperties_Versionless", c => c.Boolean());
            AddColumn("dbo.RemoteEvents", "ItemEventProperties_ListItemId", c => c.Int());
            AddColumn("dbo.RemoteEvents", "ItemEventProperties_ListTitle", c => c.String());
            AddColumn("dbo.RemoteEvents", "ItemEventProperties_ListId", c => c.Guid(nullable: false));
            AddColumn("dbo.RemoteEvents", "ItemEventProperties_WebUrl", c => c.String());
            DropColumn("dbo.RemoteEvents", "AfterUrl");
            DropColumn("dbo.RemoteEvents", "BeforeUrl");
            DropColumn("dbo.RemoteEvents", "CurrentUserId");
            DropColumn("dbo.RemoteEvents", "IsBackgroundSave");
            DropColumn("dbo.RemoteEvents", "UserLoginName");
            DropColumn("dbo.RemoteEvents", "UserDisplayName");
            DropColumn("dbo.RemoteEvents", "Versionless");
            DropColumn("dbo.RemoteEvents", "ListItemId");
            DropColumn("dbo.RemoteEvents", "ListTitle");
            DropColumn("dbo.RemoteEvents", "ListId");
            DropColumn("dbo.RemoteEvents", "WebUrl");
        }
    }
}
