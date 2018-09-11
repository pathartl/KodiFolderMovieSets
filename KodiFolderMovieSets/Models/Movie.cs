using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KodiFolderMovieSets.Models
{
    [Table("movie")]
    public class Movie
    {
        [Key]
        [Column("idMovie")]
        public int Id { get; set; }

        [Column("idFile")]
        public int FileId { get; set; }

        [ForeignKey("FileId")]
        public virtual File File { get; set; }

        [Column("c00")]
        public string Title { get; set; }

        [Column("idSet")]
        public int? SetId { get; set; }

        [ForeignKey("SetId")]
        public virtual MovieSet MovieSet { get; set; }
    }
}
