using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShelterApi.Models;

public partial class AnimalshelterContext : DbContext
{
    public AnimalshelterContext()
    {
    }

    public AnimalshelterContext(DbContextOptions<AnimalshelterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Shelter> Shelters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shelter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("shelters");

            entity.Property(e => e.City).HasMaxLength(40);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RegTime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
