using System;
using System.Collections.Generic;

namespace PetMate.Model;

public partial class Shelter
{
    public long PetCount { get; set; }

    public int Id { get; set; }

    public string Address { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string WorkingTime { get; set; } = null!;

    public string VisitorsTime { get; set; } = null!;

    public string? ShelterName { get; set; }

    public string ShelterPassword { get; set; } = null!;

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
