using KodiFolderMovieSets.Models;
using KodiFolderMovieSets.Models.Art;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KodiFolderMovieSets.Services
{
    public class MovieSetService
    {
        private DatabaseContext _context { get; set; }
        private Repository<Movie> _movieRepo { get; set; }
        private Repository<File> _fileRepo { get; set; }
        private Repository<Path> _pathRepo { get; set; }
        private Repository<MovieSet> _setRepo { get; set; }
        private Repository<MovieSetArt> _movieSetArtRepo { get; set; }

        public MovieSetService(DatabaseContext Context)
        {
            _context = Context;
            _movieRepo = new Repository<Movie>(Context);
            _fileRepo = new Repository<File>(Context);
            _pathRepo = new Repository<Path>(Context);
            _setRepo = new Repository<MovieSet>(Context);
            _movieSetArtRepo = new Repository<MovieSetArt>(Context);
        }

        public MovieSet GetMovieSet(int setId)
        {
            return _setRepo.FirstOrDefault(s => s.Id == setId);
        }

        public IEnumerable<MovieSet> GetMovieSets()
        {
            var sets = _setRepo.All(true, new string[] { "Movies", "Movies.File", "Movies.File.Path" }).ToList();

            return sets;
        }

        public IEnumerable<Movie> GetMoviesFromSet(int setId)
        {
            return _movieRepo.Get(m => m.SetId == setId).ToList<Movie>();
        }

        public MovieSet GetMovieSetByName(string name)
        {
            var movieSetExists = _setRepo.Contains(s => s.Name.Trim().ToLower() == name.Trim().ToLower());

            if (movieSetExists)
            {
                return _setRepo.FirstOrDefault(s => s.Name.Trim().ToLower() == name.Trim().ToLower());
            } else
            {
                return CreateMovieSet(name);
            }
        }

        public bool MovieSetExists(int setId)
        {
            return _setRepo.Contains(s => s.Id == setId);
        }

        public MovieSet CreateMovieSet(string name)
        {
            var movieSet = new MovieSet();
            movieSet.Name = name;

            _context.Add(movieSet);
            _context.SaveChanges();

            return movieSet;
        }

        public void CleanMovieSets()
        {
            var sets = GetMovieSets();

            foreach (var set in sets)
            {
                if (set.Movies.Count() == 0)
                {
                    _context.Remove(set);
                }
            }

            _context.SaveChanges();
        }

        public void CleanMovieSetArt()
        {
            var setsWithArt = _context.MovieSetArts
                .Where(msa => msa.MediaType == MovieSetArt.DefaultMediaType)
                .Select(msa => msa.MediaId)
                .Distinct()
                .ToList<int>();

            foreach (var set in setsWithArt)
            {
                if (!MovieSetExists(set))
                {
                    _context.RemoveRange(_context.MovieSetArts.Where(msa => msa.MediaId == set).ToList());
                }
            }
        }

        public void UpdateMovieSetArt(string prefix)
        {
            prefix = prefix.TrimEnd('/');

            var setsWithPoster = _context.MovieSetArts
                .Where(
                    msa => msa.MediaType == MovieSetArt.DefaultMediaType
                    && msa.Type == "poster"
                );

            var setsWithFanart = _context.MovieSetArts
                .Where(
                    msa => msa.MediaType == MovieSetArt.DefaultMediaType
                    && msa.Type == "fanart"
                );

            var directoryMovieSets = GetMovieSetsAssignedByPath(prefix);

            foreach (var set in directoryMovieSets)
            {
                var setPoster = setsWithPoster.SingleOrDefault(msa => msa.MediaId == set.Id);
                var setFanart = setsWithFanart.SingleOrDefault(msa => msa.MediaId == set.Id);

                if (setPoster != null && setPoster.Url != String.Format("{0}/{1}/folder.jpg", prefix, set.Name))
                {
                    setPoster.Url = String.Format("{0}/{1}/folder.jpg", prefix, set.Name);
                }

                if (setFanart != null && setFanart.Url != String.Format("{0}/{1}/fanart.jpg", prefix, set.Name))
                {
                    setFanart.Url = String.Format("{0}/{1}/fanart.jpg", prefix, set.Name);
                }

                if (setPoster == null)
                {
                    setPoster = new MovieSetArt();
                    setPoster.MediaId = set.Id;
                    setPoster.MediaType = MovieSetArt.DefaultMediaType;
                    setPoster.Type = "poster";
                    setPoster.Url = String.Format("{0}/{1}/folder.jpg", prefix, set.Name);

                    _context.Add(setPoster);
                }

                if (setFanart == null)
                {
                    setFanart = new MovieSetArt();
                    setFanart.MediaId = set.Id;
                    setFanart.MediaType = MovieSetArt.DefaultMediaType;
                    setFanart.Type = "fanart";
                    setFanart.Url = String.Format("{0}/{1}/fanart.jpg", prefix, set.Name);

                    _context.Add(setFanart);
                }
            }

            _context.SaveChanges();
        }

        public string GetMovieSetDirectoryName(Movie movie, string prefix)
        {
            // Remove trailing slash is needed
            string path = movie.File.Path.FullPath.TrimEnd('/');

            if (path.StartsWith(prefix))
            {
                var splitPath = path.Replace(prefix, "").Split('/');
                var movieFileBasename = System.IO.Path.GetFileNameWithoutExtension(movie.File.Filename);

                // Check if the first directory after the prefix
                // If it's not equal to the movie's actual file name,
                // it's probably the collection name so just use that.
                if (splitPath[0] != movieFileBasename)
                {
                    return splitPath[0];
                }
            }

            return "";
        }

        public IEnumerable<MovieSet> GetMovieSetsAssignedByPath(string prefix)
        {
            var existingMovieSets = GetMovieSets();
            var directoryMovieSets = new List<MovieSet>();

            foreach (var set in existingMovieSets)
            {
                var directories = new List<String>();

                foreach (var movie in set.Movies) {
                    var dirName = GetMovieSetDirectoryName(movie, prefix);

                    if (dirName != "")
                    {
                        directories.Add(dirName);
                    }
                }

                directories = directories.Distinct().ToList();

                if (directories.Count == 1)
                {
                    directoryMovieSets.Add(set);
                }
            }

            return directoryMovieSets;
        }

        public void AssignMovieSetsByPath(string prefix)
        {
            var movieSets = GetMovieSetsAssignedByPath(prefix);

            var movieService = new MovieService(_context);
            var movies = movieService.GetMovies();

            foreach (var movie in movies)
            {
                var movieSetDirectory = GetMovieSetDirectoryName(movie, prefix);

                if (movieSetDirectory != "")
                {
                    var existingMovieSet = movieSets.SingleOrDefault(ms => ms.Name == movieSetDirectory);

                    if (existingMovieSet != null && movie.SetId != existingMovieSet.Id)
                    {
                        movie.SetId = existingMovieSet.Id;
                    }
                }
            }

            _context.SaveChanges();
        }
    }
}
