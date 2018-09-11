using KodiFolderMovieSets.Models;
using KodiFolderMovieSets.Models.Art;
using Microsoft.EntityFrameworkCore;

namespace KodiFolderMovieSets
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base (options)
        {
            
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Path> Paths { get; set; }
        public DbSet<MovieSet> MovieSets { get; set; }
        public DbSet<MovieSetArt> MovieSetArts { get; set; }
    }
}
