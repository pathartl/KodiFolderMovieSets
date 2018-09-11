using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KodiFolderMovieSets.Models
{
    [Table("sets")]
    public class MovieSet
    {
        [Key]
        [Column("idSet")]
        public int Id { get; set; }

        [Column("strSet")]
        public string Name { get; set; }

        public virtual IEnumerable<Movie> Movies { get; set; }
    }
}
