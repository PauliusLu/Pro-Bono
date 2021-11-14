using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Karma.Models;
using Microsoft.AspNetCore.Identity;
using Karma.Models.Messaging;

namespace Karma.Data
{
    public class KarmaContext : IdentityDbContext<Karma.Models.User>
    {
        public KarmaContext (DbContextOptions<KarmaContext> options)
            : base(options)
        {

        }

        public DbSet<Karma.Models.Post> Post { get; set; }

        public DbSet<Karma.Models.Charity> Charity { get; set; }

        public DbSet<Karma.Models.User> User { get; set; }

        public DbSet<Karma.Models.UserRole> UserRole { get; set; }

        public DbSet<IdentityRole> Role { get; set; }

        public DbSet<Karma.Models.CharityAddress> CharityAddress { get; set; }

        public DbSet<Karma.Models.CharityItemType> CharityItemType { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CharityItemType>().HasKey(x => new { x.CharityId, x.ItemTypeId });
        }

        public DbSet<Karma.Models.Messaging.Chat> Chat { get; set; }

        public DbSet<Karma.Models.Messaging.Message> Message { get; set; }

        public DbSet<Karma.Models.Report> Report { get; set; }
    }
}
