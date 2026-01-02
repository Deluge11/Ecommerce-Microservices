using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authentication_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomTablesWithUniqueCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountRoles_Accounts_AccountsId",
                table: "AccountRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountRoles_Roles_RolesId",
                table: "AccountRoles");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountRoles",
                table: "AccountRoles");

            migrationBuilder.RenameTable(
                name: "AccountRoles",
                newName: "AccountRole");

            migrationBuilder.RenameColumn(
                name: "RolesId",
                table: "AccountRole",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "AccountsId",
                table: "AccountRole",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_AccountRoles_RolesId",
                table: "AccountRole",
                newName: "IX_AccountRole_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountRole",
                table: "AccountRole",
                columns: new[] { "AccountId", "RoleId" });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => new { x.PermissionId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RolePermission_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountRole_AccountId_RoleId",
                table: "AccountRole",
                columns: new[] { "AccountId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId_PermissionId",
                table: "RolePermission",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRole_Accounts_AccountId",
                table: "AccountRole",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRole_Roles_RoleId",
                table: "AccountRole",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountRole_Accounts_AccountId",
                table: "AccountRole");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountRole_Roles_RoleId",
                table: "AccountRole");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountRole",
                table: "AccountRole");

            migrationBuilder.DropIndex(
                name: "IX_AccountRole_AccountId_RoleId",
                table: "AccountRole");

            migrationBuilder.RenameTable(
                name: "AccountRole",
                newName: "AccountRoles");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "AccountRoles",
                newName: "RolesId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "AccountRoles",
                newName: "AccountsId");

            migrationBuilder.RenameIndex(
                name: "IX_AccountRole_RoleId",
                table: "AccountRoles",
                newName: "IX_AccountRoles_RolesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountRoles",
                table: "AccountRoles",
                columns: new[] { "AccountsId", "RolesId" });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    PermissionsId = table.Column<int>(type: "int", nullable: false),
                    RolesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.PermissionsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RolesId",
                table: "RolePermissions",
                column: "RolesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRoles_Accounts_AccountsId",
                table: "AccountRoles",
                column: "AccountsId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountRoles_Roles_RolesId",
                table: "AccountRoles",
                column: "RolesId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
