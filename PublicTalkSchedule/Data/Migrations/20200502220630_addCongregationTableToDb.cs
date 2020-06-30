using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PublicTalkSchedule.Data.Migrations
{
    public partial class addCongregationTableToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Congregation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    CongName = table.Column<string>(nullable: false),
                    CircuitNum = table.Column<string>(nullable: true),
                    khAddress = table.Column<string>(nullable: true),
                    khCity = table.Column<string>(nullable: true),
                    khPhone = table.Column<string>(maxLength: 14, nullable: true),
                    tcLastName = table.Column<string>(nullable: true),
                    tcFirstName = table.Column<string>(nullable: true),
                    tcMobilePhone = table.Column<string>(maxLength: 14, nullable: true),
                    tcHomePhone = table.Column<string>(maxLength: 14, nullable: true),
                    tcEmail = table.Column<string>(nullable: true),
                    cobeLastName = table.Column<string>(nullable: true),
                    cobeFirstName = table.Column<string>(nullable: true),
                    cobeMobilePhone = table.Column<string>(maxLength: 14, nullable: true),
                    cobeHomePhone = table.Column<string>(maxLength: 14, nullable: true),
                    MtgDay = table.Column<int>(nullable: false),
                    MtgTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Congregation", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Congregation");
        }
    }
}
