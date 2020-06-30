using Microsoft.EntityFrameworkCore.Migrations;

namespace PublicTalkSchedule.Data.Migrations
{
    public partial class addTalkListANDTalkCateroryToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TalkCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    CatDes = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalkCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TalkList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    CatNum = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalkList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TalkList_TalkCategory_CatNum",
                        column: x => x.CatNum,
                        principalTable: "TalkCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TalkList_CatNum",
                table: "TalkList",
                column: "CatNum");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TalkList");

            migrationBuilder.DropTable(
                name: "TalkCategory");
        }
    }
}
