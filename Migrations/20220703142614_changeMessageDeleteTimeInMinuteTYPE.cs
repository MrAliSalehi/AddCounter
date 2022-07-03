using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AddCounter.Migrations
{
    public partial class changeMessageDeleteTimeInMinuteTYPE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ushort>(
                name: "MessageDeleteTimeInMinute",
                table: "Groups",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MessageDeleteTimeInMinute",
                table: "Groups",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(ushort),
                oldType: "INTEGER");
        }
    }
}
