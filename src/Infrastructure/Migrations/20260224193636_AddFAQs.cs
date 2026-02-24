using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFAQs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description_kk",
                table: "Services",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name_kk",
                table: "Services",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FAQs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionRu = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    QuestionKk = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    QuestionEn = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    AnswerRu = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    AnswerKk = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    AnswerEn = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQs", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "description_kk", "name_kk" },
                values: new object[] { "Үй-жайды толық жинау", "Жалпы жинау" });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "description_kk", "name_kk" },
                values: new object[] { "Тұрақты жинау қызметі", "Қолдаушы жинау" });

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_IsActive_SortOrder",
                table: "FAQs",
                columns: new[] { "IsActive", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAQs");

            migrationBuilder.DropColumn(
                name: "description_kk",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "name_kk",
                table: "Services");
        }
    }
}
