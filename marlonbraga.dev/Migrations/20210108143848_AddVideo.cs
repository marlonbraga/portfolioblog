using Microsoft.EntityFrameworkCore.Migrations;

namespace marlonbraga.dev.Migrations
{
    public partial class AddVideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VideoLink",
                table: "post",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VideoLink",
                table: "post");
        }
    }
}
