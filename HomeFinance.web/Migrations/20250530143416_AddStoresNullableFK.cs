using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeFinance.web.Migrations
{
    /// <inheritdoc />
    public partial class AddStoresNullableFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Stores_StoreId",
                table: "Expenses");

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "Expenses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Stores_StoreId",
                table: "Expenses",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "StoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Stores_StoreId",
                table: "Expenses");

            migrationBuilder.AlterColumn<int>(
                name: "StoreId",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Stores_StoreId",
                table: "Expenses",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "StoreId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
