using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NailAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddGalleriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Galleries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Galleries_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, "admin-stamp", new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(60), "Sistem yöneticisi", "Admin", "ADMIN" },
                    { 2, "customer-stamp", new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(120), "Kayıtlı müşteri", "Customer", "CUSTOMER" }
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(240), "Jel tırnak hizmetleri", "Jel Tırnak" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 2, new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(250), "Protez tırnak hizmetleri", true, "Protez Tırnak" },
                    { 3, new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(250), "Nail art tasarımları", true, "Nail Art" },
                    { 4, new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(250), "Kirpik lifting hizmetleri", true, "Kirpik Lifting" },
                    { 5, new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(250), "Manikür ve pedikür hizmetleri", true, "Manikür & Pedikür" }
                });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "DurationMinutes" },
                values: new object[] { 5, new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(270), "Uzun ömürlü kalıcı oje uygulaması", 45 });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "DurationMinutes" },
                values: new object[] { 2, new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(270), "Doğal görünümlü protez tırnak tasarımı", 90 });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "DurationMinutes" },
                values: new object[] { 5, new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(280), "Klasik manikür bakımı", 30 });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "DurationMinutes", "IsActive", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { 4, 1, new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(280), "Yüksek kaliteli jel malzemeleri ile uzun ömürlü uygulama", 60, true, "Jel Tırnak Uygulaması", 500m, null },
                    { 5, 3, new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(280), "Özel nail art tasarımları ve süsleme", 75, true, "Nail Art Tasarım", 600m, null },
                    { 6, 4, new DateTime(2026, 6, 5, 13, 27, 6, 326, DateTimeKind.Local).AddTicks(280), "Kirpiklerinizi kıvırma ve hacimlendirme", 45, true, "Kirpik Lifting", 350m, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_CategoryId",
                table: "Galleries",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Galleries");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 6);

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

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "DurationMinutes" },
                values: new object[] { 1, new DateTime(2026, 5, 18, 11, 7, 1, 996, DateTimeKind.Local).AddTicks(6150), null, 0 });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "DurationMinutes" },
                values: new object[] { 1, new DateTime(2026, 5, 18, 11, 7, 1, 996, DateTimeKind.Local).AddTicks(6150), null, 0 });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "CreatedAt", "Description", "DurationMinutes" },
                values: new object[] { 1, new DateTime(2026, 5, 18, 11, 7, 1, 996, DateTimeKind.Local).AddTicks(6150), null, 0 });
        }
    }
}
