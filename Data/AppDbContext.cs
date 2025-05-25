﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AssessmentModel> AssessmentModels { get; set; }

    public virtual DbSet<CourseModel> CourseModels { get; set; }

    public virtual DbSet<ResultModel> ResultModels { get; set; }

    public virtual DbSet<UserModel> UserModels { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\ProjectModels;Initial Catalog=Project;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssessmentModel>(entity =>
        {
            entity.HasKey(e => e.AssessmentId);

            entity.ToTable("AssessmentModel");

            entity.Property(e => e.AssessmentId).ValueGeneratedNever();
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Course).WithMany(p => p.AssessmentModels)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentModel_CourseModel");
        });

        modelBuilder.Entity<CourseModel>(entity =>
        {
            entity.HasKey(e => e.CourseId);

            entity.ToTable("CourseModel");

            entity.Property(e => e.CourseId).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MediaUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Instructor).WithMany(p => p.CourseModels)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseModel_UserModel");
        });

        modelBuilder.Entity<ResultModel>(entity =>
        {
            entity.HasKey(e => e.ResultId);

            entity.ToTable("ResultModel");

            entity.Property(e => e.ResultId).ValueGeneratedNever();
            entity.Property(e => e.AttemptDate).HasColumnType("datetime");

            entity.HasOne(d => d.Assessment).WithMany(p => p.ResultModels)
                .HasForeignKey(d => d.AssessmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResultModel_AssessmentModel");

            entity.HasOne(d => d.User).WithMany(p => p.ResultModels)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResultModel_UserModel");
        });

        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("UserModel");

            entity.HasIndex(e => e.Email, "IX_UserModel").IsUnique();

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
