using System;
using System.Collections.Generic;

namespace PetMate.Model;

public partial class Request
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PetId { get; set; }

    public int ShelterId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime Datetime { get; set; }

    public virtual Pet Pet { get; set; } = null!;

    public virtual Shelter Shelter { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
