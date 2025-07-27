using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSys.Data.Entities;

namespace StudentInfoSys.Data.Configurations
{
    public class TeacherConfiguration : BaseConfiguration<TeacherEntity>
    {
        public override void Configure(EntityTypeBuilder<TeacherEntity> builder)
        {
            base.Configure(builder);

            builder.Property(t => t.Department).IsRequired();

            builder.HasOne(t => t.User)
                   .WithOne(u => u.Teacher)
                   .HasForeignKey<TeacherEntity>(t => t.UserId);

            builder.HasMany(t => t.Courses)
                   .WithOne(c => c.Teacher)
                   .HasForeignKey(c => c.TeacherId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}