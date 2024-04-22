﻿using System.ComponentModel.DataAnnotations;

namespace panasonic_machine_checker.Models
{

    public class MachinesModel
    {
        public List<Machines>? MachinesList { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
    public class Machines
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Location { get; set; }
    }
}
