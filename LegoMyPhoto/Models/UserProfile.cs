using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LegoMyPhoto.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        //public bool AlwaysShare { get; set; }
        public ICollection<PhotoPack> MyPhotos { get; set; }

        public UserProfile()
        {
            MyPhotos = new List<PhotoPack>();
        }

    }
}
