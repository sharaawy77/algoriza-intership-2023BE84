using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFLayer.Migrations
{
    /// <inheritdoc />
    public partial class Vezeetaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Discounds_Code",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_Code",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Bookings",
                newName: "DisCodeId");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Appointments",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DisCodeId",
                table: "Bookings",
                column: "DisCodeId",
                unique: true,
                filter: "[DisCodeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Discounds_DisCodeId",
                table: "Bookings",
                column: "DisCodeId",
                principalTable: "Discounds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Discounds_DisCodeId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_DisCodeId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "DisCodeId",
                table: "Bookings",
                newName: "Code");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Appointments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Code",
                table: "Bookings",
                column: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Discounds_Code",
                table: "Bookings",
                column: "Code",
                principalTable: "Discounds",
                principalColumn: "Id");
        }
    }
}
