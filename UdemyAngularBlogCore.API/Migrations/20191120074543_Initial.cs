using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UdemyAngularBlogCore.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(maxLength: 500, nullable: false),
                    content_summary = table.Column<string>(maxLength: 500, nullable: false),
                    content_main = table.Column<string>(nullable: false),
                    publish_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    picture = table.Column<string>(maxLength: 300, nullable: true),
                    category_id = table.Column<int>(nullable: false),
                    viewCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.id);
                    table.ForeignKey(
                        name: "FK_Article_Category",
                        column: x => x.category_id,
                        principalTable: "Category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    article_id = table.Column<int>(nullable: false),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    content_main = table.Column<string>(nullable: false),
                    publish_date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.id);
                    table.ForeignKey(
                        name: "FK_Comment_Article",
                        column: x => x.article_id,
                        principalTable: "Article",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "id", "name" },
                values: new object[] { 1, "Asp.Net Core" });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "id", "name" },
                values: new object[] { 2, "Angular 8" });

            migrationBuilder.InsertData(
                table: "Article",
                columns: new[] { "id", "category_id", "content_main", "content_summary", "picture", "publish_date", "title", "viewCount" },
                values: new object[] { 1, 1, "Makale içerik 1", "Makale özet 1", null, new DateTime(2019, 11, 20, 10, 45, 43, 269, DateTimeKind.Local).AddTicks(9658), "Makale 1", 0 });

            migrationBuilder.InsertData(
                table: "Article",
                columns: new[] { "id", "category_id", "content_main", "content_summary", "picture", "publish_date", "title", "viewCount" },
                values: new object[] { 2, 2, "Makale içerik 2", "Makale özet 2", null, new DateTime(2019, 11, 20, 10, 45, 43, 271, DateTimeKind.Local).AddTicks(7345), "Makale 2", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Article_category_id",
                table: "Article",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_article_id",
                table: "Comment",
                column: "article_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
