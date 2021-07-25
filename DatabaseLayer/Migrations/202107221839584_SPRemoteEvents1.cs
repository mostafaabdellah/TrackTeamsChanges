namespace DatabaseLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SPRemoteEvents1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.SPRemoteEvents", newName: "RemoteEvents");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.RemoteEvents", newName: "SPRemoteEvents");
        }
    }
}
