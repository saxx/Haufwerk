using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Haufwerk.Models
{
    public class Db : DbContext
    {
        // ReSharper disable once NotNullMemberIsNotInitialized
        public Db(DbContextOptions<Db> options) : base(options)
        {
        }

        [NotNull]
        public DbSet<Issue> Issues { get; set; }
    }
}
