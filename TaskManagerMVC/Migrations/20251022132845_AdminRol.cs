using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdminRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT Id FROM AspNetRoles WHERE Id = '5b4b23a2-193e-4f14-bb6e-05b17f8d4a77')
                BEGIN
	                INSERT AspNetRoles (Id, [Name], [NormalizedName])
	                VALUES('5b4b23a2-193e-4f14-bb6e-05b17f8d4a77','admin', 'ADMIN')
                END 
                ");//Estaremos haciendo pruebas con ese id 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM AspNetRoles WHERE Id = '5b4b23a2-193e-4f14-bb6e-05b17f8d4a77'");
            //usare delete o delete from realmente es lo mismo en este caso, pero delete from es mas comun. La diferencia es que delete from se usa mas en sql puro y delete en linq. es recomienda usar delete from en sql puro porque es mas claro, aunque realmente no hay diferencia en usar uno u otro.
        }
    }
}
