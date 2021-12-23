namespace DWD.UI.PredicateBuilder.Tests;

using System;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Database context for ClaimantWage.
/// </summary>
internal partial class AnEntityContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnEntityContext"/> class.
    /// </summary>
    /// <param name="options">Options for configuring the context.</param>
#pragma warning disable CS8618 // Non-nullable field AnEntities is initialized in the test
    public AnEntityContext(DbContextOptions<AnEntityContext> options)
#pragma warning restore CS8618
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets AnEntity DB set.
    /// </summary>
    public virtual DbSet<AnEntity> AnEntities { get; set; }

    /// <summary>
    /// Handler for the OnConfiguring event.
    /// </summary>
    /// <param name="optionsBuilder">Object used to configure the context.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    /// <summary>
    /// Handler for the OnModelCreating event.
    /// </summary>
    /// <param name="modelBuilder">Model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (null == modelBuilder)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        _ = modelBuilder.HasAnnotation("Relational:Collation", "en_US.UTF8");

        modelBuilder.Entity<AnEntity>(entity =>
        {
            entity.ToTable("AnEntities");

            entity.HasIndex(e => new { e.Fk, e.P1, e.P2 }, "an_entity_idx")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Fk).HasColumnName("fk");

            entity.Property(e => e.P1).HasColumnName("p1");

            entity.Property(e => e.P2).HasColumnName("p2");
        });

        this.OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
