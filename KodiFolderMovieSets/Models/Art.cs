using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KodiFolderMovieSets.Models
{
    public partial class ArtDefinition
    {
        [Key]
        [Column("art_id")]
        public int Id { get; set; }

        [Column("media_id")]
        public int MediaId { get; set; }

        [Column("media_type")]
        public string MediaType { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("url")]
        public string Url { get; set; }
    }
}
