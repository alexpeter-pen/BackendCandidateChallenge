using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace QuizService.Models;

public partial class TestDatabaseContext : DbContext
{
    public TestDatabaseContext()
    {
    }

    protected readonly IConfiguration Configuration;

    public TestDatabaseContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<QuizResponse> QuizResponses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.ToTable("Answer");

            entity.Property(e => e.Text)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_Question_Answer");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Question");

            entity.Property(e => e.Text)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasOne(d => d.CorrectAnswer).WithMany(p => p.Questions)
                .HasForeignKey(d => d.CorrectAnswerId)
                .HasConstraintName("FK_Answer_Question");

            entity.HasOne(d => d.Quiz).WithMany(p => p.Questions)
                .HasForeignKey(d => d.QuizId)
                .HasConstraintName("FK_Quiz_Question");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.ToTable("Quiz");

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(256);
        });

        modelBuilder.Entity<QuizResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__QuizResp__3214EC076541B033");

            entity.ToTable("QuizResponse");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
