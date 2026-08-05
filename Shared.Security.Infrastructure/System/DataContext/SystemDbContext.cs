using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using  Shared.Security.Infrastructure.System.IdentityModels;
using  Shared.Security.Domain.Models.Identity;
using  Shared.Security.Infrastructure.System.IdentityModels;

namespace  Shared.Security.Infrastructure.System.DataContext
{
    public class SystemDbContext : IdentityDbContext<SystemUser, SystemRole, string>
    {
        public SystemDbContext() { }
        public SystemDbContext(DbContextOptions<SystemDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.HasDefaultSchema("System");
            builder.Entity<SystemUser>().ToTable("SystemUser", "System");
            builder.Entity<SystemRole>().ToTable("SystemRole", "System");
            builder.Entity<UserRefreshToken>().ToTable("SystemRefreshToken", "System");
            builder.Entity<IdentityUserRole<string>>().ToTable("SystemUserRoles", "System");
            builder.Entity<IdentityUserClaim<string>>().ToTable("SystemUserClaims", "System");
            builder.Entity<IdentityUserLogin<string>>().ToTable("SystemUserLogins", "System");
            builder.Entity<IdentityUserToken<string>>().ToTable("SystemUserTokens", "System");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("SystemRoleClaims", "System");


        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseSqlServer("Data Source=Circleserp.com;Initial Catalog=NDSSecuirty;Persist Security Info=True;User ID=mahmoud;Password=50%admin;Trust Server Certificate=True");
            optionsBuilder.UseSqlServer("Server=test.server.local;Database=NDSSecuirty;User Id=remote; Password=remote@1234;TrustServerCertificate=True");
        }

        public DbSet<UserRefreshToken> RefreshTokens { get; set; }

    }
}
