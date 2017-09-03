using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollegeMap.Data.Migrations
{
    public partial class BunchMoreCollegeData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AnnualRoomAndBoard",
                table: "Colleges",
                newName: "AvgNetPrice");

            migrationBuilder.AddColumn<float>(
                name: "AcceptanceRate",
                table: "Colleges",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "AnnualTuitionOut",
                table: "Colleges",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptanceRate",
                table: "Colleges");

            migrationBuilder.DropColumn(
                name: "AnnualTuitionOut",
                table: "Colleges");

            migrationBuilder.RenameColumn(
                name: "AvgNetPrice",
                table: "Colleges",
                newName: "AnnualRoomAndBoard");
        }
    }
}
