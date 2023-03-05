﻿using System.ComponentModel.DataAnnotations;

namespace BALibrary.HR
{
    public class DocumentType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
