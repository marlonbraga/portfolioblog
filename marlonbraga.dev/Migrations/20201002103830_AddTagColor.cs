using Microsoft.EntityFrameworkCore.Migrations;

namespace marlonbraga.dev.Migrations
{
    public partial class AddTagColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "tag",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "tag");
        }
    }
}
