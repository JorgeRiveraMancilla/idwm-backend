using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tienda_UCN_api.src.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderItemProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderItems",
                newName: "DiscountAtMoment");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAtMoment",
                table: "OrderItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageAtMoment",
                table: "OrderItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleAtMoment",
                table: "OrderItems",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAtMoment",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ImageAtMoment",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "TitleAtMoment",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "DiscountAtMoment",
                table: "OrderItems",
                newName: "ProductId");

            migrationBuilder.AddColumn<int>(
                name: "Discount",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
