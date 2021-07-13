namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeSchema2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Changes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TeamId = c.String(),
                        SubscriptionId = c.String(),
                        Iteration = c.Int(nullable: false),
                        ChangeType = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        RestNotificationOn = c.DateTime(),
                        GraphNotificationOn = c.DateTime(),
                        RestLatency = c.Int(nullable: false),
                        GraphLatency = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Changes");
        }
    }
}
