using Microsoft.EntityFrameworkCore.Migrations;

namespace PublicTalkSchedule.Data.Migrations
{
    public partial class reAddSpeakerTalkAndTalkShoppingCartToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpeakerTalk",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpkNum = table.Column<int>(nullable: false),
                    TalkNum = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeakerTalk", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeakerTalk_Speaker_SpkNum",
                        column: x => x.SpkNum,
                        principalTable: "Speaker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpeakerTalk_TalkList_TalkNum",
                        column: x => x.TalkNum,
                        principalTable: "TalkList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TalkShoppingCart",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpkId = table.Column<int>(nullable: false),
                    TalkId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalkShoppingCart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TalkShoppingCart_Speaker_SpkId",
                        column: x => x.SpkId,
                        principalTable: "Speaker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TalkShoppingCart_TalkList_TalkId",
                        column: x => x.TalkId,
                        principalTable: "TalkList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpeakerTalk_SpkNum",
                table: "SpeakerTalk",
                column: "SpkNum");

            migrationBuilder.CreateIndex(
                name: "IX_SpeakerTalk_TalkNum",
                table: "SpeakerTalk",
                column: "TalkNum");

            migrationBuilder.CreateIndex(
                name: "IX_TalkShoppingCart_SpkId",
                table: "TalkShoppingCart",
                column: "SpkId");

            migrationBuilder.CreateIndex(
                name: "IX_TalkShoppingCart_TalkId",
                table: "TalkShoppingCart",
                column: "TalkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpeakerTalk");

            migrationBuilder.DropTable(
                name: "TalkShoppingCart");
        }
    }
}
