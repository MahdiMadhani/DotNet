using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InMemoryCaching.Models
{
    [Table("tblPhotoes")]
    public class Photo
    {
        public Photo(){ }


        [Key]
        public int Id { get; set; }

        [Required]
        public string Titel { get; set; }

        [NotMapped]
        public string Src { get; set; }
        public byte[] ImgData { get; set; }
        public string ImgType { get; set; }

        [NotMapped]
        public string idd { get; set; }

        [ForeignKey("AlbumID")]
        public int AlbumID { get; set; }
        public Album Album { get; set; }
        public virtual List<Comment> Comment { get; set; }
    }
}
