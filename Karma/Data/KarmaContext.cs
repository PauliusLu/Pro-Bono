using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Karma.Models;

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

        public DbSet<Karma.Models.Role> Role { get; set; }
    }
}
