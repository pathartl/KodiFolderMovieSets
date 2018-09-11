using KodiFolderMovieSets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KodiFolderMovieSets.Services
{
    public class MovieService
    {
        private DatabaseContext _context { get; set; }
        private Repository<Movie> _movieRepo { get; set; }
        private Repository<File> _fileRepo { get; set; }
        private Repository<Path> _pathRepo { get; set; }
        private Repository<MovieSet> _setRepo { get; set; }

        public MovieService(DatabaseContext Context)
        {
            _context = Context;
            _movieRepo = new Repository<Movie>(Context);
            _fileRepo = new Repository<File>(Context);
            _pathRepo = new Repository<Path>(Context);
            _setRepo = new Repository<MovieSet>(Context);
        }

        public IEnumerable<Movie> GetMovies()
        {
            return _movieRepo.All(true, new string[] { "MovieSet", "File", "File.Path" }).ToList();
        }
    }
}
