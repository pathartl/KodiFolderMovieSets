using KodiFolderMovieSets.Models;
using KodiFolderMovieSets.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace KodiFolderMovieSets
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = configurationBuilder.Build();

            var connectionString = configuration.GetConnectionString("Kodi");

            var services = new ServiceCollection()
                .AddDbContextPool<DatabaseContext>(
                    options => options.UseMySql(connectionString)
                );

            var serviceProvider = services.BuildServiceProvider();

            var prefixes = configuration.GetSection("PathPrefixes").Get<string[]>();

            using (var context = serviceProvider.GetService<DatabaseContext>())
            {
                var movieSetService = new MovieSetService(context);

                foreach (var prefix in prefixes)
                {
                    movieSetService.AssignMovieSetsByPath(prefix);
                    movieSetService.UpdateMovieSetArt(prefix);
                }

                movieSetService.CleanMovieSets();
                movieSetService.CleanMovieSetArt();
            }

            Console.WriteLine("Done syncing!");
        }
    }
}
