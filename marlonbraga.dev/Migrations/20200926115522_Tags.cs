using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace marlonbraga.dev.Migrations
{
    public partial class Tags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "post");

            migrationBuilder.CreateTable(
                name: "tag",
                columns: table => new
                {
                    IdTag = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tag", x => x.IdTag);
                });

            migrationBuilder.CreateTable(
                name: "postTag",
                columns: table => new
                {
                    IdTag = table.Column<int>(nullable: false),
                    IdPost = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postTag", x => new { x.IdPost, x.IdTag });
                    table.ForeignKey(
                        name: "FK_postTag_post_IdPost",
                        column: x => x.IdPost,
                        principalTable: "post",
                        principalColumn: "IdPost",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_postTag_tag_IdTag",
                        column: x => x.IdTag,
                        principalTable: "tag",
                        principalColumn: "IdTag",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_postTag_IdTag",
                table: "postTag",
                column: "IdTag");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "postTag");

            migrationBuilder.DropTable(
                name: "tag");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "post",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
