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
    }
}
