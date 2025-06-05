using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "Email", "FullName", "Password", "Role" },
                values: new object[] { 1, "admin@FUNewsManagementSystem.org", "System Administrator", "@@abc123@@", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 1);
        }
    }
}
