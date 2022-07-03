using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddCounter.Migrations
{
    public partial class botStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BotStatus",
                table: "Groups",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BotStatus",
                table: "Groups");
        }
    }
}
