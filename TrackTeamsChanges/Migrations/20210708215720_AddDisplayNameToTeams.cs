using Microsoft.EntityFrameworkCore.Migrations;

namespace TrackTeamsChanges.Migrations
{
    public partial class AddDisplayNameToTeams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Teams");
        }
    }
}
