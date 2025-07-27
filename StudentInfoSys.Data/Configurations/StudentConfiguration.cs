using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSys.Data.Entities;

namespace StudentInfoSys.Data.Configurations
{
    public class StudentConfiguration : BaseConfiguration<StudentEntity>
    {
        public override void Configure(EntityTypeBuilder<StudentEntity> builder)
        {
            base.Configure(builder);

            builder.Property(s => s.StudentNumber).IsRequired();
            builder.Property(s => s.Class).IsRequired();

            builder.HasOne(s => s.User)
                   .WithOne(u => u.Student)
                   .HasForeignKey<StudentEntity>(s => s.UserId);

            builder.HasMany(s => s.Enrollments)
                   .WithOne(e => e.Student)
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
