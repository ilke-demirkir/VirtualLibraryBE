using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtualLibraryAPI.Migrations
{
    /// <inheritdoc />
    public partial class ReplacedIndustryIdentifierWithISBN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndustryIdentifiers",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "Isbn",
                table: "Books",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Isbn",
                table: "Books");

            migrationBuilder.AddColumn<List<string>>(
                name: "IndustryIdentifiers",
                table: "Books",
                type: "text[]",
                nullable: true);
        }
    }
}
