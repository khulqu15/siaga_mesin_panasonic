﻿using System;
using System.Collections.Generic;

namespace panasonic_machine_checker.data;

public partial class StatusJoborder
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<JobOrder> JobOrders { get; set; } = new List<JobOrder>();
}
