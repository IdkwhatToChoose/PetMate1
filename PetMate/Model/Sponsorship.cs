using PetMate.Helpers;
using PetMate.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetMate.Model;

public partial class Sponsorship
{

    public int Id { get; set; }

    public int PetId { get; set; }

    public int UserId { get; set; }

    public long Amount { get; set; }

    public bool KeepUpdated { get; set; }

 


}
public class Donation
{
    private readonly PetMateContext _context = new PetMateContext();
    public string? StripeToken { get; set; }

    public string Amount { get; set; } = null!;

    public int? PetId { get; set; }

    public string? Username { get; set; }

    public string? UserEmail { get; set; } 

    public bool SendUpdates { get; set; }

    public virtual Pet? Pet { get; set; }

    public virtual User? User { get; set; }

    public virtual Shelter? Shelter { get; set; }

    public Donation()
    {

    }
    public Donation(Sponsorship sps)
    {
        Pet = _context.Pets.Find(sps.PetId);
        SendUpdates = sps.KeepUpdated;
        User = _context.Users.Find(sps.UserId);
        Amount = sps.Amount.ToString();
        Shelter = _context.Shelters.Find(Pet.ShelterId);

    }
}