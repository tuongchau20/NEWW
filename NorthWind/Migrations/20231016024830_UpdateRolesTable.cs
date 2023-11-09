using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NorthWind.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRolesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleName",
                table: "Roles",
                column: "RoleName",
                principalTable: "Roles",
                principalColumn: "RoleName",
                onDelete: ReferentialAction.Cascade
            );
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
