using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace at.mschwaig.mped.definitions
{
    public class ThesisDbContext : DbContext
    {
        public ThesisDbContext() : base() { }

        public DbSet<Problem> Problems { get; set; }

        public DbSet<Result> Results { get; set; }

        public DbSet<Solution> Solutions { get; set; }

        public DbSet<HeuristicRun> HeuristicRun { get; set; }

        public DbSet<Experiment> Experiments { get; set; }
    }
}
