using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NDS.Shared.Security.Infrastructure.Migrations.Multitenant
{
    /// <inheritdoc />
    public partial class AddERPUserCodeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ERPUserCodeId",
                schema: "Tenant",
                table: "TenantUser",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ERPUserCodeId",
                schema: "Tenant",
                table: "TenantUser");
        }
    }
}
