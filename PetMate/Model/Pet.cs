using System;
using System.Collections.Generic;

namespace PetMate.Model;

public partial class Pet
{
    public string Breed { get; set; } = null!;

    public string Size { get; set; } = null!;

    public bool Castrated { get; set; }

    public int Id { get; set; }

    public int ShelterId { get; set; }

    public int? AdopterId { get; set; }
}
