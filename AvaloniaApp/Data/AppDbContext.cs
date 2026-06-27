using System;
using System.Collections.Generic;
using System.IO;
using Abituria.Models;
using Microsoft.EntityFrameworkCore;

namespace Abituria.Data;

public sealed class LocalProfileEntity
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public ProfileKind Kind { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public int? PasswordIterations { get; set; }
    public byte[]? RecoveryCodeHash { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? LastLoginUtc { get; set; }
    public ICollection<ExerciseProgressEntity> Progress { get; set; } = [];
}

public sealed class ExerciseProgressEntity
{
    public long Id { get; set; }
    public Guid ProfileId { get; set; }
    public string ExerciseId { get; set; } = string.Empty;
    public DateTime CompletedUtc { get; set; }
    public LocalProfileEntity Profile { get; set; } = null!;
}

public sealed class AppMetadataEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<LocalProfileEntity> Profiles => Set<LocalProfileEntity>();
    public DbSet<ExerciseProgressEntity> ExerciseProgress => Set<ExerciseProgressEntity>();
    public DbSet<AppMetadataEntity> Metadata => Set<AppMetadataEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LocalProfileEntity>(entity =>
        {
            entity.ToTable("Profiles");
            entity.HasKey(profile => profile.Id);
            entity.HasIndex(profile => profile.NormalizedName).IsUnique();
            entity.Property(profile => profile.DisplayName).HasMaxLength(30).IsRequired();
            entity.Property(profile => profile.NormalizedName).HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<ExerciseProgressEntity>(entity =>
        {
            entity.ToTable("ExerciseProgress");
            entity.HasKey(progress => progress.Id);
            entity.HasIndex(progress => new { progress.ProfileId, progress.ExerciseId }).IsUnique();
            entity.Property(progress => progress.ExerciseId).HasMaxLength(80).IsRequired();
            entity.HasOne(progress => progress.Profile)
                .WithMany(profile => profile.Progress)
                .HasForeignKey(progress => progress.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AppMetadataEntity>(entity =>
        {
            entity.ToTable("AppMetadata");
            entity.HasKey(item => item.Key);
            entity.Property(item => item.Key).HasMaxLength(80);
        });
    }
}

public sealed class AppDbContextFactory
{
    private readonly DbContextOptions<AppDbContext> _options;

    public AppDbContextFactory(string? databasePath = null)
    {
        DatabasePath = databasePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Abituria",
            "abituria.db");
        Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite($"Data Source={DatabasePath}")
            .Options;
    }

    public string DatabasePath { get; }
    public AppDbContext CreateDbContext() => new(_options);
}
