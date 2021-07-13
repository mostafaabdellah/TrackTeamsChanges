namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeSchema11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SPWebhookNotifications", "Content", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SPWebhookNotifications", "Content");
        }
    }
}
