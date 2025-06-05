using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CreatedDate", "Description", "Name", "Order", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "General news and announcements", "General News", 1, 1 },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Academic news and updates", "Academic", 2, 1 },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "News about student activities and achievements", "Student Life", 3, 1 },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Upcoming events and activities", "Events", 4, 1 }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "TagId", "CreatedDate", "Description", "Name", "NewsId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Important announcements", "Important", null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Academic related news", "Academic", null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Student related news", "Student", null },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Event related news", "Event", null }
                });

            migrationBuilder.InsertData(
                table: "NewsArticles",
                columns: new[] { "NewsId", "AccountId", "CategoryId", "Content", "CreatedDate", "IsFeatured", "Status", "Title", "UpdatedDate", "ViewCount" },
                values: new object[] { 1, 1, 1, "This is the first article in our news system. We are excited to bring you the latest updates and news.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 1, "Welcome to FU News Management System", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 100 });

            migrationBuilder.InsertData(
                table: "NewsArticles",
                columns: new[] { "NewsId", "AccountId", "CategoryId", "Content", "CreatedDate", "Status", "Title", "UpdatedDate", "ViewCount" },
                values: new object[] { 2, 2, 2, "The university has announced several new academic programs for the upcoming semester.", new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 1, "New Academic Programs Announced", new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), 75 });

            migrationBuilder.InsertData(
                table: "NewsArticles",
                columns: new[] { "NewsId", "AccountId", "CategoryId", "Content", "CreatedDate", "IsFeatured", "Status", "Title", "UpdatedDate", "ViewCount" },
                values: new object[] { 3, 3, 3, "Read about our outstanding students and their achievements in various fields.", new DateTime(2024, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), true, 1, "Student Success Stories", new DateTime(2024, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), 50 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "NewsArticles",
                keyColumn: "NewsId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NewsArticles",
                keyColumn: "NewsId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NewsArticles",
                keyColumn: "NewsId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "TagId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "TagId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "TagId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "TagId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3);
        }
    }
}
