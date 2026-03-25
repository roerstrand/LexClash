using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdSpel.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRulesAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayers_AspNetUsers_UserId",
                table: "GamePlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSessions_AspNetUsers_CurrentTurnUserId",
                table: "GameSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSessions_Categories_CategoryId",
                table: "GameSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_GameTurns_AspNetUsers_UserId",
                table: "GameTurns");

            migrationBuilder.DropIndex(
                name: "IX_GameSessions_CurrentTurnUserId",
                table: "GameSessions");

            migrationBuilder.DropIndex(
                name: "IX_GamePlayers_SessionId",
                table: "GamePlayers");

            migrationBuilder.DropColumn(
                name: "CurrentTurnUserId",
                table: "GameSessions");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Words",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Word",
                table: "GameTurns",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StartWord",
                table: "GameSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GameCode",
                table: "GameSessions",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentUserId",
                table: "GameSessions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Words_CategoryId_Text",
                table: "Words",
                columns: new[] { "CategoryId", "Text" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_CurrentUserId",
                table: "GameSessions",
                column: "CurrentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_GameCode",
                table: "GameSessions",
                column: "GameCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayers_SessionId_PlayerOrder",
                table: "GamePlayers",
                columns: new[] { "SessionId", "PlayerOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayers_SessionId_UserId",
                table: "GamePlayers",
                columns: new[] { "SessionId", "UserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayers_AspNetUsers_UserId",
                table: "GamePlayers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSessions_AspNetUsers_CurrentUserId",
                table: "GameSessions",
                column: "CurrentUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSessions_Categories_CategoryId",
                table: "GameSessions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GameTurns_AspNetUsers_UserId",
                table: "GameTurns",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Categories_CategoryId",
                table: "Words",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayers_AspNetUsers_UserId",
                table: "GamePlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSessions_AspNetUsers_CurrentUserId",
                table: "GameSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSessions_Categories_CategoryId",
                table: "GameSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_GameTurns_AspNetUsers_UserId",
                table: "GameTurns");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_Categories_CategoryId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_CategoryId_Text",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_GameSessions_CurrentUserId",
                table: "GameSessions");

            migrationBuilder.DropIndex(
                name: "IX_GameSessions_GameCode",
                table: "GameSessions");

            migrationBuilder.DropIndex(
                name: "IX_GamePlayers_SessionId_PlayerOrder",
                table: "GamePlayers");

            migrationBuilder.DropIndex(
                name: "IX_GamePlayers_SessionId_UserId",
                table: "GamePlayers");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Words",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Word",
                table: "GameTurns",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StartWord",
                table: "GameSessions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "GameCode",
                table: "GameSessions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentUserId",
                table: "GameSessions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentTurnUserId",
                table: "GameSessions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_CurrentTurnUserId",
                table: "GameSessions",
                column: "CurrentTurnUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayers_SessionId",
                table: "GamePlayers",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayers_AspNetUsers_UserId",
                table: "GamePlayers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSessions_AspNetUsers_CurrentTurnUserId",
                table: "GameSessions",
                column: "CurrentTurnUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSessions_Categories_CategoryId",
                table: "GameSessions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameTurns_AspNetUsers_UserId",
                table: "GameTurns",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
