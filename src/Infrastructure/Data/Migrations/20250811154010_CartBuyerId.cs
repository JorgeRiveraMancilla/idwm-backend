using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tienda_UCN_api.Src.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CartBuyerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_BuyerId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_BuyerId",
                table: "Carts");

            migrationBuilder.AlterColumn<string>(
                name: "BuyerId",
                table: "Carts",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Carts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Carts");

            migrationBuilder.AlterColumn<int>(
                name: "BuyerId",
                table: "Carts",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_BuyerId",
                table: "Carts",
                column: "BuyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_BuyerId",
                table: "Carts",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
