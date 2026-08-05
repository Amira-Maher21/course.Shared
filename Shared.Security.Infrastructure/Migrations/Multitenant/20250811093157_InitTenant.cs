using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NDS.Shared.Security.Infrastructure.Migrations.Multitenant
{
    /// <inheritdoc />
    public partial class InitTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Tenant");

            migrationBuilder.CreateTable(
                name: "AppServices",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentVersion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppServices", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "TenantRefreshToken",
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRefreshToken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantRole",
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tenant_ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tenant_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: true),
                    TenantName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantUser",
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tenant_ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: true),
                    TenantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppServiceVersions",
                columns: table => new
                {
                    ServiceVersionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VersionState = table.Column<int>(type: "int", nullable: true),
                    AppServiceServiceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppServiceVersions", x => x.ServiceVersionId);
                    table.ForeignKey(
                        name: "FK_AppServiceVersions_AppServices_AppServiceServiceId",
                        column: x => x.AppServiceServiceId,
                        principalTable: "AppServices",
                        principalColumn: "ServiceId");
                });

            migrationBuilder.CreateTable(
                name: "TenantRoleClaims",
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantRoleClaims_TenantRole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Tenant",
                        principalTable: "TenantRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantDbs",
                columns: table => new
                {
                    TenantDbId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tenant_ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DbDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppServicesServiceId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantDbs", x => x.TenantDbId);
                    table.ForeignKey(
                        name: "FK_TenantDbs_AppServices_AppServicesServiceId",
                        column: x => x.AppServicesServiceId,
                        principalTable: "AppServices",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantDbs_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TenantUserClaims",
                schema: "Tenant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUserClaims_TenantUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "Tenant",
                        principalTable: "TenantUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUserLogins",
                schema: "Tenant",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_TenantUserLogins_TenantUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "Tenant",
                        principalTable: "TenantUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUserRoles",
                schema: "Tenant",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_TenantRole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Tenant",
                        principalTable: "TenantRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUserRoles_TenantUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "Tenant",
                        principalTable: "TenantUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUserTokens",
                schema: "Tenant",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_TenantUserTokens_TenantUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "Tenant",
                        principalTable: "TenantUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AppServiceVersions",
                columns: new[] { "ServiceVersionId", "AppServiceServiceId", "ServiceId", "Version", "VersionState" },
                values: new object[,]
                {
                    { 1, null, 1, "1.0", 1 },
                    { 2, null, 2, "1.0", 1 }
                });

            migrationBuilder.InsertData(
                table: "AppServices",
                columns: new[] { "ServiceId", "CurrentVersion", "ServiceDescription", "ServiceName" },
                values: new object[,]
                {
                    { 1, "1.0", "General Accounts", "Accounts" },
                    { 2, "1.0", "Sales", "Sales" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppServiceVersions_AppServiceServiceId",
                table: "AppServiceVersions",
                column: "AppServiceServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantDbs_AppServicesServiceId",
                table: "TenantDbs",
                column: "AppServicesServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantDbs_TenantId",
                table: "TenantDbs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Tenant",
                table: "TenantRole",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TenantRoleClaims_RoleId",
                schema: "Tenant",
                table: "TenantRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "Tenant",
                table: "TenantUser",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Tenant",
                table: "TenantUser",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserClaims_UserId",
                schema: "Tenant",
                table: "TenantUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserLogins_UserId",
                schema: "Tenant",
                table: "TenantUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUserRoles_RoleId",
                schema: "Tenant",
                table: "TenantUserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppServiceVersions");

            migrationBuilder.DropTable(
                name: "TenantDbs");

            migrationBuilder.DropTable(
                name: "TenantRefreshToken",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "TenantRoleClaims",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "TenantUserClaims",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "TenantUserLogins",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "TenantUserRoles",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "TenantUserTokens",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "AppServices");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "TenantRole",
                schema: "Tenant");

            migrationBuilder.DropTable(
                name: "TenantUser",
                schema: "Tenant");
        }
    }
}
