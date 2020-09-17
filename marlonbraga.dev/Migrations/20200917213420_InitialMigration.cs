using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace marlonbraga.dev.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "post",
                columns: table => new
                {
                    IdPost = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PublicationDate = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(nullable: false),
                    TumbNail = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post", x => x.IdPost);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "post");
        }
    }
}
