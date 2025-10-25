using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerMVC.Migrations
{
    /// <inheritdoc />
    public partial class TasksUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserCreatorId",
                table: "Tasks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserCreatorId",
                table: "Tasks",
                column: "UserCreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_UserCreatorId",
                table: "Tasks",
                column: "UserCreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_UserCreatorId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserCreatorId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UserCreatorId",
                table: "Tasks");
        }
    }
}
