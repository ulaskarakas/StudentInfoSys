using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSys.Data.Entities;

namespace StudentInfoSys.Data.Configurations
{
    public class RoleConfiguration : BaseConfiguration<RoleEntity>
    {
        public override void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            base.Configure(builder);

            builder.Property(r => r.Name).IsRequired();

            builder.HasMany(r => r.UserRoles)
                   .WithOne(ur => ur.Role)
                   .HasForeignKey(ur => ur.RoleId);

            builder.HasData(new RoleEntity
            {
                Id = 1,
                Name = "Admin",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            });
            builder.HasData(new RoleEntity
            {
                Id = 2,
                Name = "Teacher",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            });
            builder.HasData(new RoleEntity
            {
                Id = 3,
                Name = "Student",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            });
            builder.HasData(new RoleEntity
            {
                Id = 4,
                Name = "User",
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            });
        }
    }
}