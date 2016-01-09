using JetBrains.Annotations;
using Microsoft.Data.Entity;

namespace Haufwerk.Models
{
    public class Db : DbContext
    {
        [NotNull]
        // ReSharper disable once NotNullMemberIsNotInitialized
        public DbSet<Issue> Issues { get; set; }
    }
}
