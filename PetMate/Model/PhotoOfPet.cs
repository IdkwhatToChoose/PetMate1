using System;
using System.Collections.Generic;

namespace PetMate.Model;

public partial class PhotoOfPet
{
    public byte[]? Image { get; set; }

    public string? ImageName { get; set; }

    public int Id { get; set; }

    public int PetId { get; set; }
}
