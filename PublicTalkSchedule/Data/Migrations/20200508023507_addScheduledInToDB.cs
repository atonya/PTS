using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PublicTalkSchedule.Data.Migrations
{
    public partial class addScheduledInToDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleOut",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DOT = table.Column<DateTime>(nullable: false),
                    SpkNum = table.Column<int>(nullable: true),
                    SpkTalkNum = table.Column<int>(nullable: true),
                    hostCongNum = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleOut", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleOut_Speaker_SpkNum",
                        column: x => x.SpkNum,
                        principalTable: "Speaker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduleOut_TalkList_SpkTalkNum",
                        column: x => x.SpkTalkNum,
                        principalTable: "TalkList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduleOut_Congregation_hostCongNum",
                        column: x => x.hostCongNum,
                        principalTable: "Congregation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleOut_SpkNum",
                table: "ScheduleOut",
                column: "SpkNum");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleOut_SpkTalkNum",
                table: "ScheduleOut",
                column: "SpkTalkNum");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleOut_hostCongNum",
                table: "ScheduleOut",
                column: "hostCongNum");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleOut");
        }
    }
}
