using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LegoMyPhoto.Models
{
    public class SearchPhotoPack
    {
        [Key]
        public int Id { get; set; }
        public string fullPath { get; set; }
        public string newFileName { get; set; } //base filaname of photo
        public string savedPath { get; set; } //path of photo to use/display
        public string oldFileName { get; set; }
        public bool savedFile { get; set; }
        public bool ShareThisPhoto { get; set; } //do they want to share this photo
        [Display(Name = "Photo Name")]
        public string PhotoName { get; set; } //Display Name of photo to save for later access
        public int SelectedSize { get; set; } //Pixel size
        public bool Original { get; set; } //type of watermark used Lego/CrossStitch
        //public WebImage Iphoto { get; set; }
        //public Bitmap Bphoto { get; set; }
        public SearchPhotoPack()
        {
            newFileName = " ";
            savedPath = " ";
            PhotoName = " ";
            ShareThisPhoto = false;
            SelectedSize = 0;
            Original = false;
        }
    }
    
}