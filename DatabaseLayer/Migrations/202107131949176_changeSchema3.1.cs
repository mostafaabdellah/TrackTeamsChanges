namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeSchema31 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Changes", "RestLatency", c => c.Int());
            AlterColumn("dbo.Changes", "GraphLatency", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Changes", "GraphLatency", c => c.Int(nullable: false));
            AlterColumn("dbo.Changes", "RestLatency", c => c.Int(nullable: false));
        }
    }
}
