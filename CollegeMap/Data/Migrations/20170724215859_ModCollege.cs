using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollegeMap.Data.Migrations
{
    public partial class ModCollege : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_CollegeTypes_CollegeTypeID",
                table: "Colleges");

            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_DegreeTypes_DegreeTypeID",
                table: "Colleges");

            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_Colleges_TypeID",
                table: "Colleges");

            migrationBuilder.DropIndex(
                name: "IX_Colleges_CollegeTypeID",
                table: "Colleges");

            migrationBuilder.DropColumn(
                name: "CollegeTypeID",
                table: "Colleges");

            migrationBuilder.RenameColumn(
                name: "DegreeTypeID",
                table: "Colleges",
                newName: "HighestDegreeOfferedID");

            migrationBuilder.RenameIndex(
                name: "IX_Colleges_DegreeTypeID",
                table: "Colleges",
                newName: "IX_Colleges_HighestDegreeOfferedID");

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_DegreeTypes_HighestDegreeOfferedID",
                table: "Colleges",
                column: "HighestDegreeOfferedID",
                principalTable: "DegreeTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_CollegeTypes_TypeID",
                table: "Colleges",
                column: "TypeID",
                principalTable: "CollegeTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_DegreeTypes_HighestDegreeOfferedID",
                table: "Colleges");

            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_CollegeTypes_TypeID",
                table: "Colleges");

            migrationBuilder.RenameColumn(
                name: "HighestDegreeOfferedID",
                table: "Colleges",
                newName: "DegreeTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_Colleges_HighestDegreeOfferedID",
                table: "Colleges",
                newName: "IX_Colleges_DegreeTypeID");

            migrationBuilder.AddColumn<int>(
                name: "CollegeTypeID",
                table: "Colleges",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colleges_CollegeTypeID",
                table: "Colleges",
                column: "CollegeTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_CollegeTypes_CollegeTypeID",
                table: "Colleges",
                column: "CollegeTypeID",
                principalTable: "CollegeTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_DegreeTypes_DegreeTypeID",
                table: "Colleges",
                column: "DegreeTypeID",
                principalTable: "DegreeTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_Colleges_TypeID",
                table: "Colleges",
                column: "TypeID",
                principalTable: "Colleges",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
