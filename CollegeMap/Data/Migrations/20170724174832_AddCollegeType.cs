using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CollegeMap.Data.Migrations
{
    public partial class AddCollegeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CollegeTypeID",
                table: "Colleges",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CollegeTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollegeTypes", x => x.ID);
                });

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_CollegeTypes_CollegeTypeID",
                table: "Colleges");

            migrationBuilder.DropTable(
                name: "CollegeTypes");

            migrationBuilder.DropIndex(
                name: "IX_Colleges_CollegeTypeID",
                table: "Colleges");

            migrationBuilder.DropColumn(
                name: "CollegeTypeID",
                table: "Colleges");
        }
    }
}
