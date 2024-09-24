using Microsoft.EntityFrameworkCore.Migrations;

namespace OrsaDemoWebApi.Migrations
{
    public partial class relationinstnumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstitutionNumber",
                table: "PersonnelInstitutionMediaRelation",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstitutionNumber",
                table: "PersonnelInstitutionMediaRelation");
        }
    }
}
