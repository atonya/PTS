using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PublicTalkSchedule.Data.Migrations
{
    public partial class addScheduleInTableToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleIn",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DOT = table.Column<DateTime>(nullable: false),
                    SpkNum = table.Column<int>(nullable: true),
                    SpeakerName = table.Column<string>(nullable: true),
                    SpkCongName = table.Column<string>(nullable: true),
                    SpkCongNum = table.Column<int>(nullable: true),
                    SpkTalkNum = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleIn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleIn_Speaker_SpkNum",
                        column: x => x.SpkNum,
                        principalTable: "Speaker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleIn_SpkNum",
                table: "ScheduleIn",
                column: "SpkNum");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleIn");
        }
    }
}
