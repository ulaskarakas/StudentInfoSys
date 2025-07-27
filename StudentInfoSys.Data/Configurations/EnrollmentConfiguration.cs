using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSys.Data.Entities;

namespace StudentInfoSys.Data.Configurations
{
    public class EnrollmentConfiguration : BaseConfiguration<EnrollmentEntity>
    {
        public override void Configure(EntityTypeBuilder<EnrollmentEntity> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.AbsanceCount).HasDefaultValue(0);

            builder.HasOne(e => e.Student)
                   .WithMany(s => s.Enrollments)
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Course)
                   .WithMany(c => c.Enrollments)
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Exams)
                   .WithOne(ex => ex.Enrollment)
                   .HasForeignKey(ex => ex.EnrollmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
