﻿using System.ComponentModel.DataAnnotations;

namespace panasonic_machine_checker.Models
{

    public class StatusModel
    {
        public List<StatusCases>? StatusList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class StatusCases
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public required string Name { get; set; }
    }
}
