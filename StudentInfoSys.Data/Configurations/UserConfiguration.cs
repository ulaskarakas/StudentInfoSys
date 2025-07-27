using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSys.Data.Entities;

namespace StudentInfoSys.Data.Configurations
{
    public class UserConfiguration : BaseConfiguration<UserEntity>
    {
        public override void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            base.Configure(builder);

            builder.Property(u => u.FirstName).IsRequired();
            builder.Property(u => u.LastName).IsRequired();
            builder.Property(u => u.Password).IsRequired();
            builder.Property(u => u.Email).IsRequired();
            builder.Property(u => u.BirthDate).IsRequired();

            // One-to-one User-Student
            builder.HasOne(u => u.Student)
                   .WithOne(s => s.User)
                   .HasForeignKey<StudentEntity>(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // One-to-one User-Teacher
            builder.HasOne(u => u.Teacher)
                   .WithOne(t => t.User)
                   .HasForeignKey<TeacherEntity>(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // One-to-many User-UserRoles
            builder.HasMany(u => u.UserRoles)
                   .WithOne(ur => ur.User)
                   .HasForeignKey(ur => ur.UserId);
        }
    }
}