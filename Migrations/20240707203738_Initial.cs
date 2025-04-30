using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Replay.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContractName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessBlueprint",
                columns: table => new
                {
                    ProcessBlueprintName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessBlueprint", x => x.ProcessBlueprintName);
                });

            /*migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    IsBlocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Email);
                });*/

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DepartmentName = table.Column<string>(type: "TEXT", nullable: false),
                    UserEmail = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Department_User_UserEmail",
                        column: x => x.UserEmail,
                        principalTable: "User",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "ProcessInstance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcessInstanceName = table.Column<string>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContractId = table.Column<int>(type: "INTEGER", nullable: false),
                    ResponsibleUserEmail = table.Column<string>(type: "TEXT", nullable: false),
                    ReferenceUserEmail = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessInstance_ContractType_ContractId",
                        column: x => x.ContractId,
                        principalTable: "ContractType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessInstance_User_ReferenceUserEmail",
                        column: x => x.ReferenceUserEmail,
                        principalTable: "User",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessInstance_User_ResponsibleUserEmail",
                        column: x => x.ResponsibleUserEmail,
                        principalTable: "User",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskInstance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TaskName = table.Column<string>(type: "TEXT", nullable: false),
                    Instructions = table.Column<string>(type: "TEXT", nullable: false),
                    AssignedUserEmail = table.Column<string>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    TaskStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcessInstanceId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskInstance_ProcessInstance_ProcessInstanceId",
                        column: x => x.ProcessInstanceId,
                        principalTable: "ProcessInstance",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskInstance_User_AssignedUserEmail",
                        column: x => x.AssignedUserEmail,
                        principalTable: "User",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleName = table.Column<string>(type: "TEXT", nullable: false),
                    TaskInstanceId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserEmail = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_TaskInstance_TaskInstanceId",
                        column: x => x.TaskInstanceId,
                        principalTable: "TaskInstance",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Role_User_UserEmail",
                        column: x => x.UserEmail,
                        principalTable: "User",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "ProcessBlueprintRoles",
                columns: table => new
                {
                    PermittedRolesId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcessBlueprintsProcessBlueprintName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessBlueprintRoles", x => new { x.PermittedRolesId, x.ProcessBlueprintsProcessBlueprintName });
                    table.ForeignKey(
                        name: "FK_ProcessBlueprintRoles_ProcessBlueprint_ProcessBlueprintsProcessBlueprintName",
                        column: x => x.ProcessBlueprintsProcessBlueprintName,
                        principalTable: "ProcessBlueprint",
                        principalColumn: "ProcessBlueprintName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessBlueprintRoles_Role_PermittedRolesId",
                        column: x => x.PermittedRolesId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskBlueprint",
                columns: table => new
                {
                    TaskName = table.Column<string>(type: "TEXT", nullable: false),
                    DaysRelativeToDueDate = table.Column<int>(type: "INTEGER", nullable: false),
                    Instructions = table.Column<string>(type: "TEXT", nullable: true),
                    PermittedRoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcessBlueprintName = table.Column<string>(type: "TEXT", nullable: true),
                    ProcessBlueprintName1 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskBlueprint", x => x.TaskName);
                    table.ForeignKey(
                        name: "FK_TaskBlueprint_ProcessBlueprint_ProcessBlueprintName1",
                        column: x => x.ProcessBlueprintName1,
                        principalTable: "ProcessBlueprint",
                        principalColumn: "ProcessBlueprintName");
                    table.ForeignKey(
                        name: "FK_TaskBlueprint_Role_PermittedRoleId",
                        column: x => x.PermittedRoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskBlueprintContractTypes",
                columns: table => new
                {
                    PermittedContractTypesId = table.Column<int>(type: "INTEGER", nullable: false),
                    TasksTaskName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskBlueprintContractTypes", x => new { x.PermittedContractTypesId, x.TasksTaskName });
                    table.ForeignKey(
                        name: "FK_TaskBlueprintContractTypes_ContractType_PermittedContractTypesId",
                        column: x => x.PermittedContractTypesId,
                        principalTable: "ContractType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskBlueprintContractTypes_TaskBlueprint_TasksTaskName",
                        column: x => x.TasksTaskName,
                        principalTable: "TaskBlueprint",
                        principalColumn: "TaskName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskBlueprintDepartments",
                columns: table => new
                {
                    PermittedDepartmentsId = table.Column<int>(type: "INTEGER", nullable: false),
                    TasksTaskName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskBlueprintDepartments", x => new { x.PermittedDepartmentsId, x.TasksTaskName });
                    table.ForeignKey(
                        name: "FK_TaskBlueprintDepartments_Department_PermittedDepartmentsId",
                        column: x => x.PermittedDepartmentsId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskBlueprintDepartments_TaskBlueprint_TasksTaskName",
                        column: x => x.TasksTaskName,
                        principalTable: "TaskBlueprint",
                        principalColumn: "TaskName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_UserEmail",
                table: "Department",
                column: "UserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessBlueprintRoles_ProcessBlueprintsProcessBlueprintName",
                table: "ProcessBlueprintRoles",
                column: "ProcessBlueprintsProcessBlueprintName");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInstance_ContractId",
                table: "ProcessInstance",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInstance_ReferenceUserEmail",
                table: "ProcessInstance",
                column: "ReferenceUserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessInstance_ResponsibleUserEmail",
                table: "ProcessInstance",
                column: "ResponsibleUserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Role_TaskInstanceId",
                table: "Role",
                column: "TaskInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_UserEmail",
                table: "Role",
                column: "UserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBlueprint_PermittedRoleId",
                table: "TaskBlueprint",
                column: "PermittedRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBlueprint_ProcessBlueprintName1",
                table: "TaskBlueprint",
                column: "ProcessBlueprintName1");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBlueprintContractTypes_TasksTaskName",
                table: "TaskBlueprintContractTypes",
                column: "TasksTaskName");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBlueprintDepartments_TasksTaskName",
                table: "TaskBlueprintDepartments",
                column: "TasksTaskName");

            migrationBuilder.CreateIndex(
                name: "IX_TaskInstance_AssignedUserEmail",
                table: "TaskInstance",
                column: "AssignedUserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_TaskInstance_ProcessInstanceId",
                table: "TaskInstance",
                column: "ProcessInstanceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ProcessBlueprintRoles");

            migrationBuilder.DropTable(
                name: "TaskBlueprintContractTypes");

            migrationBuilder.DropTable(
                name: "TaskBlueprintDepartments");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "TaskBlueprint");

            migrationBuilder.DropTable(
                name: "ProcessBlueprint");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "TaskInstance");

            migrationBuilder.DropTable(
                name: "ProcessInstance");

            migrationBuilder.DropTable(
                name: "ContractType");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
