using System;
using System.Collections.Generic;

namespace PetMate.Model;

public partial class UserCharacter
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? CharacterId { get; set; }
}
