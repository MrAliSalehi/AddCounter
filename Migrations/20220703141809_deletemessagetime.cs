using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddCounter.Migrations
{
    public partial class deletemessagetime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MessageDeleteTimeInMinute",
                table: "Groups",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageDeleteTimeInMinute",
                table: "Groups");
        }
    }
}
