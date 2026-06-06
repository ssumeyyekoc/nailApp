using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NailAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedDataWithCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 5, 18, 11, 7, 1, 996, DateTimeKind.Local).AddTicks(6060), null, "Tırnak Bakımı" });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "DurationMinutes", "IsActive", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 5, 18, 11, 7, 1, 996, DateTimeKind.Local).AddTicks(6150), null, 0, true, "Kalıcı Oje", 400m, null },
                    { 2, 1, new DateTime(2026, 5, 18, 11, 7, 1, 996, DateTimeKind.Local).AddTicks(6150), null, 0, true, "Protez Tırnak", 800m, null },
                    { 3, 1, new DateTime(2026, 5, 18, 11, 7, 1, 996, DateTimeKind.Local).AddTicks(6150), null, 0, true, "Manikür", 300m, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 5, 4, 8, 10, 23, 527, DateTimeKind.Local).AddTicks(9740), "Administrator", "Admin", "ADMIN" },
                    { 2, null, new DateTime(2026, 5, 4, 8, 10, 23, 527, DateTimeKind.Local).AddTicks(9808), "Registered Customer", "Customer", "CUSTOMER" },
                    { 3, null, new DateTime(2026, 5, 4, 8, 10, 23, 527, DateTimeKind.Local).AddTicks(9812), "Guest User", "Guest", "GUEST" }
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 5, 4, 8, 10, 23, 528, DateTimeKind.Local).AddTicks(44), "Jel tırnak hizmetleri", "Jel" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 2, new DateTime(2026, 5, 4, 8, 10, 23, 528, DateTimeKind.Local).AddTicks(49), "Protez tırnak hizmetleri", true, "Protez" },
                    { 3, new DateTime(2026, 5, 4, 8, 10, 23, 528, DateTimeKind.Local).AddTicks(50), "Nail Art tasarımları", true, "Nail Art" },
                    { 4, new DateTime(2026, 5, 4, 8, 10, 23, 528, DateTimeKind.Local).AddTicks(51), "Kirpik lifting hizmetleri", true, "Kirpik Lifting" }
                });
        }
    }
}
