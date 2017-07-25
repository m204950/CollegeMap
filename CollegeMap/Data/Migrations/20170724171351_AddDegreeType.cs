using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CollegeMap.Data.Migrations
{
    public partial class AddDegreeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DegreeTypeID",
                table: "Colleges",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DegreeTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DegreeTypes", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Colleges_DegreeTypeID",
                table: "Colleges",
                column: "DegreeTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_DegreeTypes_DegreeTypeID",
                table: "Colleges",
                column: "DegreeTypeID",
                principalTable: "DegreeTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_DegreeTypes_DegreeTypeID",
                table: "Colleges");

            migrationBuilder.DropTable(
                name: "DegreeTypes");

            migrationBuilder.DropIndex(
                name: "IX_Colleges_DegreeTypeID",
                table: "Colleges");

            migrationBuilder.DropColumn(
                name: "DegreeTypeID",
                table: "Colleges");
        }
    }
}
