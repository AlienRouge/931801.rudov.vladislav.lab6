using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ForumIpsum.Models
{
    public class Folder
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ApplicationUserId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public Guid? FolderId { get; set; }
        public ICollection<Folder> Folders { get; set; }
        public ICollection<File> Files { get; set; }
    }


}