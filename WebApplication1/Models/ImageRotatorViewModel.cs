using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace WebApplication1.Models
{
    public class ImageRotatorViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string FileData { get; set; }
    }

    
}