using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InMemoryCaching.Models
{
    [Table("tblComments")]
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(300)]
        [Required]
        public string WriterName { get; set; }
        [Required]
        public string WriterEmail { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }
      
        public Boolean status { get; set; } = false;

        // [ForeignKey("AlbumID")]
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
    }
}