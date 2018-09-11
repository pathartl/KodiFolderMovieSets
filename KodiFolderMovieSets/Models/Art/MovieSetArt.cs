using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KodiFolderMovieSets.Models.Art
{
    [Table("art")]
    public class MovieSetArt : ArtDefinition
    {
        public static string DefaultMediaType = "set";

        [ForeignKey("MediaId")]
        public virtual MovieSet MovieSet { get; set; }
    }
}
