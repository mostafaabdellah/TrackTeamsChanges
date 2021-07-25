namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SPRemoteEvents6 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.RemoteEventReceivers");
            AddColumn("dbo.RemoteEventReceivers", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.RemoteEventReceivers", "ReceiverId", c => c.String());
            AddPrimaryKey("dbo.RemoteEventReceivers", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.RemoteEventReceivers");
            AlterColumn("dbo.RemoteEventReceivers", "ReceiverId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.RemoteEventReceivers", "Id");
            AddPrimaryKey("dbo.RemoteEventReceivers", "ReceiverId");
        }
    }
}
