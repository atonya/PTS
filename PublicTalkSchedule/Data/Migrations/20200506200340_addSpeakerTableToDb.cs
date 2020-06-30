using Microsoft.EntityFrameworkCore.Migrations;

namespace PublicTalkSchedule.Data.Migrations
{
    public partial class addSpeakerTableToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Speaker",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpkNum = table.Column<int>(nullable: false),
                    spkLastName = table.Column<string>(nullable: false),
                    spkFirstName = table.Column<string>(nullable: false),
                    spkMobilePhone = table.Column<string>(maxLength: 14, nullable: true),
                    spkHomePhone = table.Column<string>(maxLength: 14, nullable: true),
                    spkEmail = table.Column<string>(nullable: true),
                    EldMs = table.Column<string>(nullable: true),
                    CongName = table.Column<string>(nullable: true),
                    CongId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Speaker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Speaker_Congregation_CongId",
                        column: x => x.CongId,
                        principalTable: "Congregation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Speaker_CongId",
                table: "Speaker",
                column: "CongId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Speaker");
        }
    }
}
