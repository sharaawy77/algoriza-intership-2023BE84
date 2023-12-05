using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFLayer.Migrations
{
    /// <inheritdoc />
    public partial class wwe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Times_TimeId",
                schema: "Security",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Times_TimeId",
                schema: "Security",
                table: "Users",
                column: "TimeId",
                principalTable: "Times",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Times_TimeId",
                schema: "Security",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Times_TimeId",
                schema: "Security",
                table: "Users",
                column: "TimeId",
                principalTable: "Times",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
