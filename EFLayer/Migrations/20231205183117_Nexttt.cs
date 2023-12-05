using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFLayer.Migrations
{
    /// <inheritdoc />
    public partial class Nexttt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "Discounds",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Discounds");
        }
    }
}
