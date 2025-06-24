using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Migrations
{
    /// <inheritdoc />
    public partial class innirt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_NewsArticles_NewsId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_NewsId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "NewsId",
                table: "Tags");

            migrationBuilder.CreateTable(
                name: "NewsTags",
                columns: table => new
                {
                    NewsId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsTags", x => new { x.NewsId, x.TagId });
                    table.ForeignKey(
                        name: "FK_NewsTags_NewsArticles_NewsId",
                        column: x => x.NewsId,
                        principalTable: "NewsArticles",
                        principalColumn: "NewsId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsTags_TagId",
                table: "NewsTags",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsTags");

            migrationBuilder.AddColumn<int>(
                name: "NewsId",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "TagId",
                keyValue: 1,
                column: "NewsId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "TagId",
                keyValue: 2,
                column: "NewsId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "TagId",
                keyValue: 3,
                column: "NewsId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Tags",
                keyColumn: "TagId",
                keyValue: 4,
                column: "NewsId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_NewsId",
                table: "Tags",
                column: "NewsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_NewsArticles_NewsId",
                table: "Tags",
                column: "NewsId",
                principalTable: "NewsArticles",
                principalColumn: "NewsId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
