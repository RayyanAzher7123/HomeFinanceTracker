using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeFinance.web.Migrations
{
    /// <inheritdoc />
    public partial class AddAppUserWithNullableFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_AppUserId",
                table: "Expenses",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_AppUsers_AppUserId",
                table: "Expenses",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_AppUsers_AppUserId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_AppUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Expenses");
        }
    }
}
