using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dat.Data.Migrations
{
    public partial class initialCreateDatData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    SubjectId = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.SubjectId);
                });

            migrationBuilder.CreateTable(
                name: "DatClaim",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SubjectId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    UserProfileSubjectId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatClaim_UserProfile_UserProfileSubjectId",
                        column: x => x.UserProfileSubjectId,
                        principalTable: "UserProfile",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatClaim_UserProfileSubjectId",
                table: "DatClaim",
                column: "UserProfileSubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatClaim");

            migrationBuilder.DropTable(
                name: "UserProfile");
        }
    }
}
