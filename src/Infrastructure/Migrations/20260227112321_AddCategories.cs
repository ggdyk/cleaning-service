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
            // 1. Удаляем seed-данные Services (только если таблица существует)
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'Services') THEN
                        DELETE FROM ""Services"" WHERE ""Id"" IN (1, 2);
                    END IF;
                END $$;");

            // 2. Дропаем старый uuid-столбец (только если таблица и столбец существуют)
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF EXISTS (
                        SELECT FROM information_schema.columns
                        WHERE table_name = 'Services' AND column_name = 'CategoryId'
                    ) THEN
                        ALTER TABLE ""Services"" DROP COLUMN ""CategoryId"";
                    END IF;
                END $$;");

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

            // 5–7. Добавляем CategoryId в Services, сидируем, делаем NOT NULL
            // (только если таблица Services существует)
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'Services') THEN
                        ALTER TABLE ""Services"" ADD COLUMN IF NOT EXISTS ""CategoryId"" integer NULL;
                        INSERT INTO ""Services"" (""Id"", ""CategoryId"", ""BasePrice"", ""DurationMinutes"", ""IsActive"", ""MinArea"", ""SortOrder"", ""Unit"", description_en, description_kk, description_ru, name_en, name_kk, name_ru)
                        VALUES
                            (1, 1, 1000, 120, true, null, 1, 'service', 'Full cleaning of the premises', 'Үй-жайды толық жинау', 'Полная уборка помещения', 'General cleaning', 'Жалпы жинау', 'Генеральная уборка'),
                            (2, 1, 500,   60, true, 30.0, 2, 'sqm',     'Regular cleaning service',      'Тұрақты жинау қызметі', 'Регулярная уборка',       'Maintenance cleaning', 'Қолдаушы жинау', 'Поддерживающая уборка')
                        ON CONFLICT DO NOTHING;
                        ALTER TABLE ""Services"" ALTER COLUMN ""CategoryId"" SET NOT NULL;
                    END IF;
                END $$;");

            // 8. Индекс и FK (только если таблица Services существует)
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'Services') THEN
                        IF NOT EXISTS (
                            SELECT FROM pg_indexes WHERE tablename = 'Services' AND indexname = 'IX_Services_CategoryId'
                        ) THEN
                            CREATE INDEX ""IX_Services_CategoryId"" ON ""Services"" (""CategoryId"");
                        END IF;
                        IF NOT EXISTS (
                            SELECT FROM information_schema.table_constraints
                            WHERE constraint_name = 'FK_Services_Categories_CategoryId'
                        ) THEN
                            ALTER TABLE ""Services"" ADD CONSTRAINT ""FK_Services_Categories_CategoryId""
                            FOREIGN KEY (""CategoryId"") REFERENCES ""Categories"" (""Id"") ON DELETE RESTRICT;
                        END IF;
                    END IF;
                END $$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$ BEGIN
                    IF EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'Services') THEN
                        IF EXISTS (
                            SELECT FROM information_schema.table_constraints
                            WHERE constraint_name = 'FK_Services_Categories_CategoryId'
                        ) THEN
                            ALTER TABLE ""Services"" DROP CONSTRAINT ""FK_Services_Categories_CategoryId"";
                        END IF;
                        DROP INDEX IF EXISTS ""IX_Services_CategoryId"";
                        DELETE FROM ""Services"" WHERE ""Id"" IN (1, 2);
                        IF EXISTS (
                            SELECT FROM information_schema.columns
                            WHERE table_name = 'Services' AND column_name = 'CategoryId'
                        ) THEN
                            ALTER TABLE ""Services"" DROP COLUMN ""CategoryId"";
                        END IF;
                        ALTER TABLE ""Services"" ADD COLUMN ""CategoryId"" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
                        UPDATE ""Services"" SET ""CategoryId"" = '11111111-1111-1111-1111-111111111111' WHERE ""Id"" IN (1, 2);
                    END IF;
                END $$;");

            migrationBuilder.DropTable(name: "Categories");
        }
    }
}
