using Microsoft.EntityFrameworkCore;
using StudentInfoSys.Data.Entities;

namespace StudentInfoSys.Data.Context
{
    public class StudentInfoSysDbContext : DbContext
    {
        public StudentInfoSysDbContext(DbContextOptions<StudentInfoSysDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for UserRole
            modelBuilder.Entity<UserRoleEntity>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRoleEntity>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRoleEntity>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // User - Student one-to-one
            modelBuilder.Entity<UserEntity>()
                .HasOne(u => u.Student)
                .WithOne(s => s.User)
                .HasForeignKey<StudentEntity>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Teacher one-to-one
            modelBuilder.Entity<UserEntity>()
                .HasOne(u => u.Teacher)
                .WithOne(t => t.User)
                .HasForeignKey<TeacherEntity>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Teacher - Course one-to-many
            modelBuilder.Entity<TeacherEntity>()
                .HasMany(t => t.Courses)
                .WithOne(c => c.Teacher)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student - Enrollment one-to-many
            modelBuilder.Entity<StudentEntity>()
                .HasMany(s => s.Enrollments)
                .WithOne(e => e.Student)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course - Enrollment one-to-many
            modelBuilder.Entity<CourseEntity>()
                .HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrollment - Exam one-to-many
            modelBuilder.Entity<EnrollmentEntity>()
                .HasMany(e => e.Exams)
                .WithOne(ex => ex.Enrollment)
                .HasForeignKey(ex => ex.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<UserRoleEntity> UserRoles { get; set; }
        public DbSet<StudentEntity> Students { get; set; }
        public DbSet<TeacherEntity> Teachers { get; set; }
        public DbSet<CourseEntity> Courses { get; set; }
        public DbSet<EnrollmentEntity> Enrollments { get; set; }
        public DbSet<ExamEntity> Exams { get; set; }
    }
}