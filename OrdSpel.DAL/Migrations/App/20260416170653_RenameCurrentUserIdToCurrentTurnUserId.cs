using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdSpel.DAL.Migrations.App
{
    /// <inheritdoc />
    public partial class RenameCurrentUserIdToCurrentTurnUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentUserId",
                table: "GameSessions",
                newName: "CurrentTurnUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentTurnUserId",
                table: "GameSessions",
                newName: "CurrentUserId");
        }
    }
}
