namespace LegoMyPhoto.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PhotoPacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        newFileName = c.String(),
                        ViewFileName = c.String(),
                        ShareThisPhoto = c.Boolean(nullable: false),
                        PhotoName = c.String(),
                        SelectedSize = c.Int(nullable: false),
                        PhotoType = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        AlwaysShare = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.PhotoPacks", new[] { "UserId" });
            DropForeignKey("dbo.PhotoPacks", "UserId", "dbo.UserProfile");
            DropTable("dbo.UserProfile");
            DropTable("dbo.PhotoPacks");
        }
    }
}
