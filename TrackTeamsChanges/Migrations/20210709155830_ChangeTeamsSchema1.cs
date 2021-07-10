using Microsoft.EntityFrameworkCore.Migrations;

namespace TrackTeamsChanges.Migrations
{
    public partial class ChangeTeamsSchema1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ListId",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteId",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "Teams");
        }
    }
}
