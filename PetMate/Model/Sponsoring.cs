using System;
using System.Collections.Generic;

namespace PetMate.Model;

public partial class Sponsoring
{
    public int Id { get; set; }

    public int PetId { get; set; }

    public int? UserId { get; set; }
}
