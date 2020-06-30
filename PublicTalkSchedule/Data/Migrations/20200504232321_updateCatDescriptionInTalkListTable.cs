using Microsoft.EntityFrameworkCore.Migrations;

namespace PublicTalkSchedule.Data.Migrations
{
    public partial class updateCatDescriptionInTalkListTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TalkList_TalkCategory_CatNum",
                table: "TalkList");

            migrationBuilder.DropIndex(
                name: "IX_TalkList_CatNum",
                table: "TalkList");

            migrationBuilder.DropColumn(
                name: "CatNum",
                table: "TalkList");

            migrationBuilder.AddColumn<string>(
                name: "CatDescription",
                table: "TalkList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CatDescription",
                table: "TalkList");

            migrationBuilder.AddColumn<int>(
                name: "CatNum",
                table: "TalkList",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TalkList_CatNum",
                table: "TalkList",
                column: "CatNum");

            migrationBuilder.AddForeignKey(
                name: "FK_TalkList_TalkCategory_CatNum",
                table: "TalkList",
                column: "CatNum",
                principalTable: "TalkCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
