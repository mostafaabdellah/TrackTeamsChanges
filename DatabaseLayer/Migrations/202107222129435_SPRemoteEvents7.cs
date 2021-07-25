namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SPRemoteEvents7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RemoteEvents",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EventType = c.Int(nullable: false),
                        WebUrl = c.String(),
                        ListId = c.String(),
                        ListTitle = c.String(),
                        ListItemId = c.Int(),
                        Versionless = c.Boolean(),
                        UserDisplayName = c.String(),
                        UserLoginName = c.String(),
                        IsBackgroundSave = c.Boolean(),
                        CurrentUserId = c.Int(),
                        BeforeUrl = c.String(),
                        AfterUrl = c.String(),
                        BeforeProperties = c.String(),
                        AfterProperties = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.REREvents");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.REREvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventType = c.Int(),
                        WebUrl = c.String(),
                        ListId = c.String(),
                        ListItemId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.RemoteEvents");
        }
    }
}
