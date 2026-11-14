using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace p48_lessons_asp.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImageAndCategoryRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "ParentCategoryId", "Slug", "Title" },
                values: new object[] { new Guid("a2b3c4d5-e6f7-8a9b-0c1d-2e3f4a5b6c7d"), new DateTime(2026, 11, 14, 17, 44, 43, 239, DateTimeKind.Utc).AddTicks(7607), null, null, null, "", "Electronics" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a2b3c4d5-e6f7-8a9b-0c1d-2e3f4a5b6c7d"));
        }
    }
}
