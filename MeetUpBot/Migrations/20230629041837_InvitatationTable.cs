using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetUpBot.Migrations
{
    /// <inheritdoc />
    public partial class InvitatationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeetUpUser");

            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    MeetUpId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TimeApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => new { x.MeetUpId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Invitations_MeetUps_MeetUpId",
                        column: x => x.MeetUpId,
                        principalTable: "MeetUps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_UserId",
                table: "Invitations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.CreateTable(
                name: "MeetUpUser",
                columns: table => new
                {
                    MeetingsId = table.Column<int>(type: "int", nullable: false),
                    ParticipantsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetUpUser", x => new { x.MeetingsId, x.ParticipantsId });
                    table.ForeignKey(
                        name: "FK_MeetUpUser_MeetUps_MeetingsId",
                        column: x => x.MeetingsId,
                        principalTable: "MeetUps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetUpUser_Users_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetUpUser_ParticipantsId",
                table: "MeetUpUser",
                column: "ParticipantsId");
        }
    }
}
