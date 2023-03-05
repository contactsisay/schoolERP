﻿using System.ComponentModel.DataAnnotations;

namespace BALibrary.Report
{
    public class StockReport
    {
        [Key]
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Report Date")]
        public DateTime ReportDate { get; set; }

        [Display(Name = "From")]
        public DateTime FromDate { get; set; }

        [Display(Name = "To")]
        public DateTime ToDate { get; set; }

        [ScaffoldColumn(false)]
        public string GeneratedQuery { get; set; }

        [Display(Name = "Generated By")]
        public int EmployeeId { get; set; }

        [ScaffoldColumn(false)]
        public int Status { get; set; }
    }
}
