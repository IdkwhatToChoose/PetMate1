using System;
using System.Collections.Generic;

namespace PetMate.Model;

public partial class VirtualMeeting
{
    public string Url { get; set; } = null!;

    public int UserId { get; set; }

    public int ShelterId { get; set; }

    public DateTime Datetime { get; set; }

    public int Id { get; set; }
}
