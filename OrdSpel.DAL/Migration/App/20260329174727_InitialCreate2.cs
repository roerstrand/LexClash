using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdSpel.DAL.Migration.App
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Words_Categories_CategoryId",
                table: "Words");

            migrationBuilder.RenameColumn(
                name: "CurrentUserId",
                table: "GameSessions",
                newName: "CurrentTurnUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Categories_CategoryId",
                table: "Words",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Words_Categories_CategoryId",
                table: "Words");

            migrationBuilder.RenameColumn(
                name: "CurrentTurnUserId",
                table: "GameSessions",
                newName: "CurrentUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Categories_CategoryId",
                table: "Words",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
