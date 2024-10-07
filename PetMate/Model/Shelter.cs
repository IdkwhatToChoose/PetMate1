using System;
using System.Collections.Generic;

namespace PetMate.Model;

public partial class Shelter
{
    public long PetCount { get; set; }

    public int Id { get; set; }

    public string Address { get; set; } = null!;

    public int Type { get; set; }

    public TimeSpan WorkingTime { get; set; }

    public TimeSpan VisitorsTime { get; set; }
}
