using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Services",
                columns: new[]
                {
                    "Id", "CategoryId", "BasePrice", "Unit",
                    "MinArea", "DurationMinutes", "SortOrder", "IsActive",
                    "name_ru", "name_en", "description_ru", "description_en"
                },
                values: new object[,]
                {
                    {
                        1, new Guid("11111111-1111-1111-1111-111111111111"), 1000m, "service",
                        null, 120, 1, true,
                        "Генеральная уборка", "General cleaning",
                        "Полная уборка помещения", "Full cleaning of the premises"
                    },
                    {
                        2, new Guid("11111111-1111-1111-1111-111111111111"), 500m, "sqm",
                        30.0, 60, 2, true,
                        "Поддерживающая уборка", "Maintenance cleaning",
                        "Регулярная уборка", "Regular cleaning service"
                    }
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
        }
    }
}
