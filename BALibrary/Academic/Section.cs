﻿using System.ComponentModel.DataAnnotations;

namespace BALibrary.Academic
{
    public class Section
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
