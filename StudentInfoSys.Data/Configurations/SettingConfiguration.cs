using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSys.Data.Entities;

namespace StudentInfoSys.Data.Configurations
{
    public class SettingConfiguration : BaseConfiguration<SettingEntity>
    {
        public override void Configure(EntityTypeBuilder<SettingEntity> builder)
        {
            base.Configure(builder);

            builder.Property(s => s.MaintenanceMode).IsRequired();

            builder.HasData(new SettingEntity
            {
                Id = 1,
                MaintenanceMode = false,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            });
        }
    }
}