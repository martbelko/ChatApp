using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace waapi.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Passwords",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passwords_UserId1",
                table: "Passwords",
                column: "UserId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Passwords_Users_UserId1",
                table: "Passwords",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passwords_Users_UserId1",
                table: "Passwords");

            migrationBuilder.DropIndex(
                name: "IX_Passwords_UserId1",
                table: "Passwords");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Passwords");
        }
    }
}
