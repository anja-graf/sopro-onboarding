using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Replay.Migrations
{
    public partial class @new : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskInstance_ProcessInstance_ProcessInstanceId",
                table: "TaskInstance");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessInstanceId",
                table: "TaskInstance",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskInstance_ProcessInstance_ProcessInstanceId",
                table: "TaskInstance",
                column: "ProcessInstanceId",
                principalTable: "ProcessInstance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskInstance_ProcessInstance_ProcessInstanceId",
                table: "TaskInstance");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessInstanceId",
                table: "TaskInstance",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskInstance_ProcessInstance_ProcessInstanceId",
                table: "TaskInstance",
                column: "ProcessInstanceId",
                principalTable: "ProcessInstance",
                principalColumn: "Id");
        }
    }
}
