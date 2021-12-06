using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogedWebapp.Migrations
{
    public partial class FixDoubleRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ProfilesData_ProfileDataId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProfileDataId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileDataId",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileDataId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProfileDataId",
                table: "AspNetUsers",
                column: "ProfileDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ProfilesData_ProfileDataId",
                table: "AspNetUsers",
                column: "ProfileDataId",
                principalTable: "ProfilesData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
