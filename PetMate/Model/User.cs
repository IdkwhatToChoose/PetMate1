using System;
using System.Collections.Generic;

namespace PetMate.Model;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public string? Character { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
