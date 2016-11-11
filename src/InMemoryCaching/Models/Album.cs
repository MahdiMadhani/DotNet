using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InMemoryCaching.Models
{
    [Table("tblAlbums")]
    public class Album
    {
        public Album(){ }


        [Key]
        public int Id { get; set; }

        [Required]
        public string Titel { get; set; }        

        [NotMapped]
        public string Src { get; set; }
        public byte[] AssetData { get; set; }      
        public string AssetType { get; set; }

        
        public virtual List<Photo> Photo { get; set; }
    }
}
