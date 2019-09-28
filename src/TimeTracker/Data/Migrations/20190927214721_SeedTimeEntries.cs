using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker.Data.Migrations
{
    public partial class SeedTimeEntries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TimeEntries",
                columns: new[] { "Id", "Description", "EntryDate", "HourRate", "Hours", "ProjectId", "UserId" },
                values: new object[] { 1L, "Time entry description 1", new DateTime(2019, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 25m, 5, 1L, 1L });

            migrationBuilder.InsertData(
                table: "TimeEntries",
                columns: new[] { "Id", "Description", "EntryDate", "HourRate", "Hours", "ProjectId", "UserId" },
                values: new object[] { 2L, "Time entry description 2", new DateTime(2019, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 25m, 2, 2L, 1L });

            migrationBuilder.InsertData(
                table: "TimeEntries",
                columns: new[] { "Id", "Description", "EntryDate", "HourRate", "Hours", "ProjectId", "UserId" },
                values: new object[] { 3L, "Time entry description 3", new DateTime(2019, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 25m, 1, 3L, 1L });

            migrationBuilder.InsertData(
                table: "TimeEntries",
                columns: new[] { "Id", "Description", "EntryDate", "HourRate", "Hours", "ProjectId", "UserId" },
                values: new object[] { 4L, "Time entry description 4", new DateTime(2019, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30m, 8, 3L, 2L });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TimeEntries",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "TimeEntries",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "TimeEntries",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "TimeEntries",
                keyColumn: "Id",
                keyValue: 4L);
        }
    }
}
