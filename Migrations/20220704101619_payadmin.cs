using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddCounter.Migrations
{
    public partial class payadmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PayAdmin",
                table: "Groups",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayAdmin",
                table: "Groups");
        }
    }
}
