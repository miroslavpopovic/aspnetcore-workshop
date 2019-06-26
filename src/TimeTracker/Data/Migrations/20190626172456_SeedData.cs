using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker.Data.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1L, "Client 1" });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2L, "Client 2" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "HourRate", "Name" },
                values: new object[] { 1L, 25m, "John Doe" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "HourRate", "Name" },
                values: new object[] { 2L, 30m, "Joan Doe" });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "ClientId", "Name" },
                values: new object[] { 1L, 1L, "Project 1" });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "ClientId", "Name" },
                values: new object[] { 2L, 1L, "Project 2" });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "ClientId", "Name" },
                values: new object[] { 3L, 2L, "Project 3" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2L);
        }
    }
}
