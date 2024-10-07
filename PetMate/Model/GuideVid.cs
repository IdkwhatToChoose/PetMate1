using System;
using System.Collections.Generic;

namespace PetMate.Model;

public partial class GuideVid
{
    public string VideoName { get; set; } = null!;

    public string VideoUrl { get; set; } = null!;

    public int Id { get; set; }
}
