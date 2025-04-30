using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Replay.Migrations
{
    public partial class newlatest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_TaskInstance_TaskInstanceId",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskInstance_User_AssignedUserEmail",
                table: "TaskInstance");

            migrationBuilder.DropIndex(
                name: "IX_Role_TaskInstanceId",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "TaskInstanceId",
                table: "Role");

            migrationBuilder.AlterColumn<string>(
                name: "AssignedUserEmail",
                table: "TaskInstance",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "AssignedRoleId",
                table: "TaskInstance",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TaskInstance_AssignedRoleId",
                table: "TaskInstance",
                column: "AssignedRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskInstance_Role_AssignedRoleId",
                table: "TaskInstance",
                column: "AssignedRoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskInstance_User_AssignedUserEmail",
                table: "TaskInstance",
                column: "AssignedUserEmail",
                principalTable: "User",
                principalColumn: "Email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskInstance_Role_AssignedRoleId",
                table: "TaskInstance");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskInstance_User_AssignedUserEmail",
                table: "TaskInstance");

            migrationBuilder.DropIndex(
                name: "IX_TaskInstance_AssignedRoleId",
                table: "TaskInstance");

            migrationBuilder.DropColumn(
                name: "AssignedRoleId",
                table: "TaskInstance");

            migrationBuilder.AlterColumn<string>(
                name: "AssignedUserEmail",
                table: "TaskInstance",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskInstanceId",
                table: "Role",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_TaskInstanceId",
                table: "Role",
                column: "TaskInstanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_TaskInstance_TaskInstanceId",
                table: "Role",
                column: "TaskInstanceId",
                principalTable: "TaskInstance",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskInstance_User_AssignedUserEmail",
                table: "TaskInstance",
                column: "AssignedUserEmail",
                principalTable: "User",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
