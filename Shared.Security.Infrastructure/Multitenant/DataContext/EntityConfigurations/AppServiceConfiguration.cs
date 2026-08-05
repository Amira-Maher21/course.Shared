using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using  Shared.Domain.Models.AppSystem;
using  Shared.Domain.SystemConstants;
using Shared.Domain.Models.AppSystem;
using Shared.Domain.SystemConstants;

namespace  Shared.Security.Infrastructure.Multitenant.DataContext.EntityConfigurations
{
    internal class AppServiceConfiguration : IEntityTypeConfiguration<AppService>
    {
        public void Configure(EntityTypeBuilder<AppService> builder)
        {
            List<AppService> appServices = new List<AppService>();
            appServices.Add(new AppService
            {
                ServiceId = 1,
                ServiceName = "Accounts",
                ServiceDescription = "General Accounts",
            });

            appServices.Add(new AppService
            {
                ServiceId = 2,
                ServiceName = "Sales",
                ServiceDescription = "Sales",
            });

            builder.HasData(appServices);
        }
    }

    internal class AppServiceVersionConfiguration : IEntityTypeConfiguration<AppServiceVersion>
    {
        public void Configure(EntityTypeBuilder<AppServiceVersion> builder)
        {
            List<AppServiceVersion> versions = new List<AppServiceVersion>();
            versions.Add(new AppServiceVersion
            {
                ServiceVersionId = 1,
                ServiceId = 1,
                Version = SystemDefaults.DefaultVersion,
                VersionState = VersionState.Current,
            });

            versions.Add(new AppServiceVersion
            {
                ServiceVersionId = 2,
                ServiceId = 2,
                Version = SystemDefaults.DefaultVersion,
                VersionState = VersionState.Current,
            });

            builder.HasData(versions);
        }
    }
}
