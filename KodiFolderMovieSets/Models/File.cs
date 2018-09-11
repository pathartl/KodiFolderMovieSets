using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KodiFolderMovieSets.Models
{
    [Table("files")]
    public class File
    {
        [Key]
        [Column("idFile")]
        public int Id { get; set; }

        [Column("idPath")]
        public int PathId { get; set; }

        [ForeignKey("PathId")]
        public virtual Path Path { get; set; }

        [Column("strFilename")]
        public string Filename { get; set; }
    }
}
