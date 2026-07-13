using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tripflow.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatformAdminSupportSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "support_sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminUserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminIdentityProviderUserId = table.Column<string>(type: "text", nullable: false),
                    TargetTenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_support_sessions_tenants_TargetTenantId",
                        column: x => x.TargetTenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_support_sessions_user_profiles_AdminUserProfileId",
                        column: x => x.AdminUserProfileId,
                        principalTable: "user_profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333330023"), "platform.dashboard.read", "Visualizar dashboard administrativo da plataforma." },
                    { new Guid("33333333-3333-3333-3333-333333330024"), "platform.tenants.read", "Visualizar tenants da plataforma." },
                    { new Guid("33333333-3333-3333-3333-333333330025"), "platform.tenants.manage", "Gerenciar status de tenants da plataforma." },
                    { new Guid("33333333-3333-3333-3333-333333330026"), "platform.users.read", "Visualizar usuários cross-tenant." },
                    { new Guid("33333333-3333-3333-3333-333333330027"), "platform.users.manage", "Gerenciar usuários cross-tenant." },
                    { new Guid("33333333-3333-3333-3333-333333330028"), "platform.reports.read", "Visualizar relatórios administrativos." },
                    { new Guid("33333333-3333-3333-3333-333333330029"), "platform.support.access", "Acessar modo suporte assistido." },
                    { new Guid("33333333-3333-3333-3333-333333330030"), "platform.audit.read", "Visualizar auditoria administrativa." }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333330023"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330024"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330025"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330026"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330027"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330028"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330029"), new Guid("44444444-4444-4444-4444-444444440001") },
                    { new Guid("33333333-3333-3333-3333-333333330030"), new Guid("44444444-4444-4444-4444-444444440001") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_support_sessions_AdminUserProfileId_IsActive",
                table: "support_sessions",
                columns: new[] { "AdminUserProfileId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_support_sessions_StartedAtUtc",
                table: "support_sessions",
                column: "StartedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_support_sessions_TargetTenantId",
                table: "support_sessions",
                column: "TargetTenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "support_sessions");

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330023"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330024"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330025"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330026"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330027"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330028"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330029"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("33333333-3333-3333-3333-333333330030"), new Guid("44444444-4444-4444-4444-444444440001") });

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330023"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330024"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330025"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330026"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330027"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330028"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330029"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333330030"));
        }
    }
}
