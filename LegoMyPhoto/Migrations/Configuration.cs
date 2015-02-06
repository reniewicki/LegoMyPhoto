namespace LegoMyPhoto.Migrations
{
    using LegoMyPhoto.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<LegoMyPhoto.Models.LegoMyPhotoDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(LegoMyPhoto.Models.LegoMyPhotoDb context)
        {
            //context.UserProfiles.AddOrUpdate(r => r.UserName,
            //    new UserProfile { UserName = "Rich", 
            //        MyPhotos = new List<PhotoPack> {
            //    new PhotoPack { 
            //        PhotoName = "3 Amigos", 
            //        newFileName = "6eaa4c16-0873-4581-86fa-5ca9e82b02fd_3Amigos.jpg", 
            //        Original = true, 
            //        SelectedSize = 16, 
            //        ShareThisPhoto = true, 
            //         }
            //        }
            //    });
            SeedMembership();
        }
        private void SeedMembership()
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("DefaultConnection",
                    "UserProfile", "UserId", "UserName", autoCreateTables: true);
            }

            var roles = (SimpleRoleProvider)Roles.Provider;
            var membership = (SimpleMembershipProvider)Membership.Provider;

            if (!roles.RoleExists("SuperAdmin"))
            {
                roles.CreateRole("SuperAdmin");
            }
            if (membership.GetUser("reniewicki", false) == null)
            {
                membership.CreateUserAndAccount("reniewicki", "imalittleteapot");
            }
            if (!roles.GetRolesForUser("reniewicki").Contains("SuperAdmin"))
            {
                roles.AddUsersToRoles(new[] { "reniewicki" }, new[] { "SuperAdmin" });
            }


            
        }
    }
}
