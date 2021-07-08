using Microsoft.EntityFrameworkCore.Migrations;

namespace TrackTeamsChanges.Migrations
{
    public partial class ChangeTeamsSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListId",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "SiteId",
                table: "Teams",
                newName: "DriveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DriveId",
                table: "Teams",
                newName: "SiteId");

            migrationBuilder.AddColumn<string>(
                name: "ListId",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
