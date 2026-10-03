using Microsoft.EntityFrameworkCore;
using SkillMatch.API.Models;

namespace SkillMatch.API.Data;

public class SkillMatchDbContext(DbContextOptions<SkillMatchDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<ResumeSkill> ResumeSkills => Set<ResumeSkill>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobSkill> JobSkills => Set<JobSkill>();
    public DbSet<JobMatch> JobMatches => Set<JobMatch>();
    public DbSet<JobMatchSkill> JobMatchSkills => Set<JobMatchSkill>();
    public DbSet<SkillRecommendation> SkillRecommendations => Set<SkillRecommendation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.Email).HasMaxLength(256);
            entity.Property(user => user.FullName).HasMaxLength(150);
        });

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.Property(resume => resume.FileName).HasMaxLength(255);
            entity.Property(resume => resume.FilePath).HasMaxLength(500);
            entity.HasOne(resume => resume.User)
                .WithMany(user => user.Resumes)
                .HasForeignKey(resume => resume.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasIndex(skill => skill.Name).IsUnique();
            entity.Property(skill => skill.Name).HasMaxLength(100);
            entity.Property(skill => skill.Category).HasMaxLength(100);
            entity.HasData(SkillSeedData.All);
        });

        modelBuilder.Entity<ResumeSkill>(entity =>
        {
            entity.HasKey(resumeSkill => new { resumeSkill.ResumeId, resumeSkill.SkillId });
            entity.HasOne(resumeSkill => resumeSkill.Resume)
                .WithMany(resume => resume.ResumeSkills)
                .HasForeignKey(resumeSkill => resumeSkill.ResumeId);
            entity.HasOne(resumeSkill => resumeSkill.Skill)
                .WithMany(skill => skill.ResumeSkills)
                .HasForeignKey(resumeSkill => resumeSkill.SkillId);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.Property(job => job.Title).HasMaxLength(200);
            entity.Property(job => job.Company).HasMaxLength(200);
            entity.HasOne(job => job.User)
                .WithMany(user => user.Jobs)
                .HasForeignKey(job => job.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobSkill>(entity =>
        {
            entity.HasKey(jobSkill => new { jobSkill.JobId, jobSkill.SkillId });
            entity.Property(jobSkill => jobSkill.Importance).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(jobSkill => jobSkill.Job)
                .WithMany(job => job.JobSkills)
                .HasForeignKey(jobSkill => jobSkill.JobId);
            entity.HasOne(jobSkill => jobSkill.Skill)
                .WithMany(skill => skill.JobSkills)
                .HasForeignKey(jobSkill => jobSkill.SkillId);
        });

        modelBuilder.Entity<JobMatch>(entity =>
        {
            entity.Property(jobMatch => jobMatch.MatchScore).HasPrecision(5, 2);
            entity.HasOne(jobMatch => jobMatch.User)
                .WithMany(user => user.JobMatches)
                .HasForeignKey(jobMatch => jobMatch.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(jobMatch => jobMatch.Resume)
                .WithMany(resume => resume.JobMatches)
                .HasForeignKey(jobMatch => jobMatch.ResumeId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(jobMatch => jobMatch.Job)
                .WithMany(job => job.JobMatches)
                .HasForeignKey(jobMatch => jobMatch.JobId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<JobMatchSkill>(entity =>
        {
            entity.HasKey(matchSkill => new { matchSkill.JobMatchId, matchSkill.SkillId });
            entity.HasOne(matchSkill => matchSkill.JobMatch)
                .WithMany(jobMatch => jobMatch.MatchSkills)
                .HasForeignKey(matchSkill => matchSkill.JobMatchId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(matchSkill => matchSkill.Skill)
                .WithMany(skill => skill.JobMatchSkills)
                .HasForeignKey(matchSkill => matchSkill.SkillId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<SkillRecommendation>(entity =>
        {
            entity.Property(recommendation => recommendation.Title).HasMaxLength(200);
            entity.Property(recommendation => recommendation.LearningPriority)
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.HasOne(recommendation => recommendation.Skill)
                .WithMany(skill => skill.Recommendations)
                .HasForeignKey(recommendation => recommendation.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasData(RecommendationSeedData.All);
        });
    }
}
