using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LegoMyPhoto.Models
{
    public class LegoMyPhotoDb : DbContext
    {
        public LegoMyPhotoDb() : base("name=DefaultConnection")
        {

        }


        public DbSet<PhotoPack> PhotoPacks { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}