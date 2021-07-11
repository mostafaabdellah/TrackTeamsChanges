namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTeamsSchema12 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Teams", "TeamId1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Teams", "TeamId1", c => c.String());
        }
    }
}
