using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentInfoSys.Data.Entities;

namespace StudentInfoSys.Data.Configurations
{
    public class ExamConfiguration : BaseConfiguration<ExamEntity>
    {
        public override void Configure(EntityTypeBuilder<ExamEntity> builder)
        {
            base.Configure(builder);

            builder.Property(ex => ex.Title).IsRequired();
            builder.Property(ex => ex.ExamDate).IsRequired();
            builder.Property(ex => ex.Grade).IsRequired(false);

            builder.HasOne(ex => ex.Enrollment)
                   .WithMany(e => e.Exams)
                   .HasForeignKey(ex => ex.EnrollmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}