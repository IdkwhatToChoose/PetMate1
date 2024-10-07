using System;
using System.Collections.Generic;

namespace PetMate.Model;

public partial class PetCharacter
{
    public int Id { get; set; }

    public int PetId { get; set; }

    public int? CharacterId { get; set; }
}
