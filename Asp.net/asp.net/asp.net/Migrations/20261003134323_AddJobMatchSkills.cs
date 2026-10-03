using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillMatch.API.Migrations
{
    /// <inheritdoc />
    public partial class AddJobMatchSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobMatchSkills",
                columns: table => new
                {
                    JobMatchId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    IsMatched = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobMatchSkills", x => new { x.JobMatchId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_JobMatchSkills_JobMatches_JobMatchId",
                        column: x => x.JobMatchId,
                        principalTable: "JobMatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobMatchSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobMatchSkills_SkillId",
                table: "JobMatchSkills",
                column: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobMatchSkills");
        }
    }
}
