using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Удаляем seed-данные Services (CategoryId был Guid — несовместимый тип)
            migrationBuilder.DeleteData(table: "Services", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Services", keyColumn: "Id", keyValue: 2);

            // 2. Дропаем старый uuid-столбец
            migrationBuilder.DropColumn(name: "CategoryId", table: "Services");

            // 3. Создаём таблицу Categories
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name_ru = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name_kk = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    name_en = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description_ru = table.Column<string>(type: "text", nullable: false),
                    description_kk = table.Column<string>(type: "text", nullable: false),
                    description_en = table.Column<string>(type: "text", nullable: false),
                    IconUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            // 4. Seed категории
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "IconUrl", "IsActive", "SortOrder", "description_en", "description_kk", "description_ru", "name_en", "name_kk", "name_ru" },
                values: new object[] { 1, null, true, 1, "Residential cleaning services", "Тұрғын үй-жайларды тазалау", "Уборка жилых помещений", "Apartment cleaning", "Пәтерлерді тазалау", "Уборка квартир" });

            // 5. Добавляем новый int-столбец (nullable — чтобы избежать проблем с существующими строками)
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Services",
                type: "integer",
                nullable: true);

            // 6. Повторно сидируем Services с корректным int CategoryId
            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "CategoryId", "BasePrice", "DurationMinutes", "IsActive", "MinArea", "SortOrder", "Unit", "description_en", "description_kk", "description_ru", "name_en", "name_kk", "name_ru" },
                values: new object[,]
                {
                    { 1, 1, 1000m, 120, true, null, 1, "service", "Full cleaning of the premises", "Үй-жайды толық жинау", "Полная уборка помещения", "General cleaning", "Жалпы жинау", "Генеральная уборка" },
                    { 2, 1, 500m,   60, true, 30.0, 2, "sqm",     "Regular cleaning service",      "Тұрақты жинау қызметі", "Регулярная уборка",       "Maintenance cleaning", "Қолдаушы жинау", "Поддерживающая уборка" }
                });

            // 7. Делаем столбец NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Services",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // 8. Индекс и FK
            migrationBuilder.CreateIndex(
                name: "IX_Services_CategoryId",
                table: "Services",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Categories_CategoryId",
                table: "Services",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Services_Categories_CategoryId", table: "Services");
            migrationBuilder.DropTable(name: "Categories");
            migrationBuilder.DropIndex(name: "IX_Services_CategoryId", table: "Services");

            migrationBuilder.DeleteData(table: "Services", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Services", keyColumn: "Id", keyValue: 2);

            migrationBuilder.DropColumn(name: "CategoryId", table: "Services");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Services",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                column: "CategoryId",
                value: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                column: "CategoryId",
                value: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
