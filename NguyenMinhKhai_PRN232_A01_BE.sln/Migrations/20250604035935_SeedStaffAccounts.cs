using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Migrations
{
    /// <inheritdoc />
    public partial class SeedStaffAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "Email", "FullName", "Password", "Role" },
                values: new object[,]
                {
                    { 2, "staff1@FUNewsManagementSystem.org", "Staff Member 1", "Staff123@@", 1 },
                    { 3, "staff2@FUNewsManagementSystem.org", "Staff Member 2", "Staff123@@", 1 },
                    { 4, "staff3@FUNewsManagementSystem.org", "Staff Member 3", "Staff123@@", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 4);
        }
    }
}
