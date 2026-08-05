using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shared.Security.Infrastructure.Multitenant.IdentityModels;
using Shared.Domain.Models.AppSystem;
using Shared.Domain.Models.Multitenant;
using Shared.Security.Domain.Models.Identity;
using Shared.Security.Infrastructure.Multitenant.DataContext.EntityConfigurations;
using Shared.Security.Infrastructure.Multitenant.IdentityModels;

namespace  Shared.Security.Infrastructure.Multitenant.DataContext
{
    public class TenantDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {

        public TenantDbContext()
        {

        }
        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("TenantUser", "Tenant");
            builder.Entity<ApplicationRole>().ToTable("TenantRole", "Tenant");
            builder.Entity<UserRefreshToken>().ToTable("TenantRefreshToken", "Tenant");
            builder.Entity<IdentityUserRole<string>>().ToTable("TenantUserRoles", "Tenant");
            builder.Entity<IdentityUserClaim<string>>().ToTable("TenantUserClaims", "Tenant");
            builder.Entity<IdentityUserLogin<string>>().ToTable("TenantUserLogins", "Tenant");
            builder.Entity<IdentityUserToken<string>>().ToTable("TenantUserTokens", "Tenant");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("TenantRoleClaims", "Tenant");

            builder.ApplyConfiguration(new AppServiceConfiguration());
            builder.ApplyConfiguration(new AppServiceVersionConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseSqlServer("Data Source=Circleserp.com;Initial Catalog=NDSSecuirty;Persist Security Info=True;User ID=mahmoud;Password=50%admin;Trust Server Certificate=True");
            optionsBuilder.UseSqlServer("Server = test.server.local; Database = NDSSecuirty; User Id = remote; Password = remote@1234; TrustServerCertificate = True");
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<AppService> AppServices { get; set; }
        public DbSet<AppServiceVersion> AppServiceVersions { get; set; }
        public DbSet<TenantDb> TenantDbs { get; set; }
        public DbSet<UserRefreshToken> RefreshTokens { get; set; }
    }
}
