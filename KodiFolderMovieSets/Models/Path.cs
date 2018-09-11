using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KodiFolderMovieSets.Models
{
    [Table("path")]
    public class Path
    {
        [Key]
        [Column("idPath")]
        public int Id { get; set; }

        [Column("strPath")]
        public string FullPath { get; set; }
    }
}
