using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NDS.Shared.Security.Infrastructure.Migrations.System
{
    /// <inheritdoc />
    public partial class InitSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "System");

            migrationBuilder.CreateTable(
                name: "SystemRefreshToken",
                schema: "System",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRefreshToken", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemRole",
                schema: "System",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemUser",
                schema: "System",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_SystemUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemRoleClaims",
                schema: "System",
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
                    table.PrimaryKey("PK_SystemRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemRoleClaims_SystemRole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "System",
                        principalTable: "SystemRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemUserClaims",
                schema: "System",
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
                    table.PrimaryKey("PK_SystemUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemUserClaims_SystemUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "System",
                        principalTable: "SystemUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemUserLogins",
                schema: "System",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_SystemUserLogins_SystemUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "System",
                        principalTable: "SystemUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemUserRoles",
                schema: "System",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_SystemUserRoles_SystemRole_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "System",
                        principalTable: "SystemRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemUserRoles_SystemUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "System",
                        principalTable: "SystemUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemUserTokens",
                schema: "System",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_SystemUserTokens_SystemUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "System",
                        principalTable: "SystemUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "System",
                table: "SystemRole",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SystemRoleClaims_RoleId",
                schema: "System",
                table: "SystemRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "System",
                table: "SystemUser",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "System",
                table: "SystemUser",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserClaims_UserId",
                schema: "System",
                table: "SystemUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserLogins_UserId",
                schema: "System",
                table: "SystemUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUserRoles_RoleId",
                schema: "System",
                table: "SystemUserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemRefreshToken",
                schema: "System");

            migrationBuilder.DropTable(
                name: "SystemRoleClaims",
                schema: "System");

            migrationBuilder.DropTable(
                name: "SystemUserClaims",
                schema: "System");

            migrationBuilder.DropTable(
                name: "SystemUserLogins",
                schema: "System");

            migrationBuilder.DropTable(
                name: "SystemUserRoles",
                schema: "System");

            migrationBuilder.DropTable(
                name: "SystemUserTokens",
                schema: "System");

            migrationBuilder.DropTable(
                name: "SystemRole",
                schema: "System");

            migrationBuilder.DropTable(
                name: "SystemUser",
                schema: "System");
        }
    }
}
