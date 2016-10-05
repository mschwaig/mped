
using System.Data.Entity;

namespace at.mschwaig.mped.persistence
{
    public class ThesisDbContext : DbContext
    {
        public ThesisDbContext() : base()
        {
            this.Configuration.LazyLoadingEnabled = true;
        }

        public DbSet<Problem> Problems { get; set; }

        public DbSet<Result> Results { get; set; }

        public DbSet<HeuristicRun> HeuristicRun { get; set; }

        public DbSet<Experiment> Experiments { get; set; }
    }
}
