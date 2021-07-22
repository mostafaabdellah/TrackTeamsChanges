namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class schema5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RemoteEventReceivers",
                c => new
                    {
                        ReceiverId = c.String(nullable: false, maxLength: 128),
                        TeamId = c.String(),
                        ReceiverUrl = c.String(),
                        EventType = c.Int(nullable: false),
                        Oid = c.String(),
                        Metadata = c.String(),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.ReceiverId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RemoteEventReceivers");
        }
    }
}
