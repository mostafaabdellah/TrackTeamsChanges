namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SPRemoteEvents4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteEvents", "BeforeProperties", c => c.String());
            AddColumn("dbo.RemoteEvents", "AfterProperties", c => c.String());
            AlterColumn("dbo.RemoteEvents", "ListId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RemoteEvents", "ListId", c => c.Guid(nullable: false));
            DropColumn("dbo.RemoteEvents", "AfterProperties");
            DropColumn("dbo.RemoteEvents", "BeforeProperties");
        }
    }
}
