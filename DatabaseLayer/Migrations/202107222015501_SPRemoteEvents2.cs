namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SPRemoteEvents2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RemoteEvents", "ItemEventProperties_ListItemId", c => c.Int());
            AlterColumn("dbo.RemoteEvents", "ItemEventProperties_Versionless", c => c.Boolean());
            AlterColumn("dbo.RemoteEvents", "ItemEventProperties_IsBackgroundSave", c => c.Boolean());
            AlterColumn("dbo.RemoteEvents", "ItemEventProperties_CurrentUserId", c => c.Int());
            DropColumn("dbo.RemoteEvents", "ItemEventProperties_ExternalNotificationMessage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RemoteEvents", "ItemEventProperties_ExternalNotificationMessage", c => c.Binary());
            AlterColumn("dbo.RemoteEvents", "ItemEventProperties_CurrentUserId", c => c.Int(nullable: false));
            AlterColumn("dbo.RemoteEvents", "ItemEventProperties_IsBackgroundSave", c => c.Boolean(nullable: false));
            AlterColumn("dbo.RemoteEvents", "ItemEventProperties_Versionless", c => c.Boolean(nullable: false));
            AlterColumn("dbo.RemoteEvents", "ItemEventProperties_ListItemId", c => c.Int(nullable: false));
        }
    }
}
