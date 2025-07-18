using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeFinance.web.Migrations
{
    /// <inheritdoc />
    public partial class AddBillImagePathToExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillImagePath",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillImagePath",
                table: "Expenses");
        }
    }
}
