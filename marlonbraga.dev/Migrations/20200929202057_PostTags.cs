using Microsoft.EntityFrameworkCore.Migrations;

namespace marlonbraga.dev.Migrations
{
    public partial class PostTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostIdPost",
                table: "tag",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tag_PostIdPost",
                table: "tag",
                column: "PostIdPost");

            migrationBuilder.AddForeignKey(
                name: "FK_tag_post_PostIdPost",
                table: "tag",
                column: "PostIdPost",
                principalTable: "post",
                principalColumn: "IdPost",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tag_post_PostIdPost",
                table: "tag");

            migrationBuilder.DropIndex(
                name: "IX_tag_PostIdPost",
                table: "tag");

            migrationBuilder.DropColumn(
                name: "PostIdPost",
                table: "tag");
        }
    }
}
