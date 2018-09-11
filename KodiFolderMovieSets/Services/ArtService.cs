using KodiFolderMovieSets.Models;
using KodiFolderMovieSets.Models.Art;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KodiFolderMovieSets.Services
{
    public class ArtService
    {
        private DatabaseContext _context { get; set; }
        private Repository<Movie> _movieRepo { get; set; }
        private Repository<File> _fileRepo { get; set; }
        private Repository<Path> _pathRepo { get; set; }
        private Repository<MovieSet> _setRepo { get; set; }
        private Repository<MovieSetArt> _movieSetArtRepo { get; set; }

        public ArtService(DatabaseContext Context)
        {
            _context = Context;
            _movieRepo = new Repository<Movie>(Context);
            _fileRepo = new Repository<File>(Context);
            _pathRepo = new Repository<Path>(Context);
            _setRepo = new Repository<MovieSet>(Context);
            _movieSetArtRepo = new Repository<MovieSetArt>(Context);
        }

        public IEnumerable<MovieSetArt> GetMovieSetArts(int setId)
        {
            return _movieSetArtRepo.Get(msa => msa.MediaType == MovieSetArt.DefaultMediaType && msa.MediaId == setId).ToList();
        }
    }
}
