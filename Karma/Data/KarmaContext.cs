using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Karma.Models;

namespace Karma.Data
{
    public class KarmaContext : DbContext
    {
        public KarmaContext (DbContextOptions<KarmaContext> options)
            : base(options)
        {
        }

        public DbSet<Karma.Models.Post> Post { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.IsDonation).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.ItemType).HasMaxLength(30).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(120).IsRequired();
                entity.Property(e => e.ImagePath);
                entity.Property(e => e.IsVisible).IsRequired();
            });
        }
    }
}
