using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PetMate.Model;

public partial class PetMateContext : DbContext
{
    public PetMateContext()
    {
    }

    public PetMateContext(DbContextOptions<PetMateContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Character> Characters { get; set; }

    public virtual DbSet<GuideVid> GuideVids { get; set; }

    public virtual DbSet<Pet> Pets { get; set; }

    public virtual DbSet<PhotoOfPet> PhotoOfPets { get; set; }

    public virtual DbSet<Shelter> Shelters { get; set; }

    public virtual DbSet<Sponsoring> Sponsorings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VirtualMeeting> VirtualMeetings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOPCOMP;Initial Catalog=PetMate;Trusted_Connection=True;Encrypt=False;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Character>(entity =>
        {
            entity.ToTable("Character");

            entity.Property(e => e.Character1)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("character");
        });

        modelBuilder.Entity<GuideVid>(entity =>
        {
            entity.Property(e => e.VideoName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("videoName");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("videoUrl");
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.Property(e => e.AdopterId).HasColumnName("adopterID");
            entity.Property(e => e.Breed)
                .HasMaxLength(50)
                .HasColumnName("breed");
            entity.Property(e => e.Castrated).HasColumnName("castrated");
            entity.Property(e => e.Character)
                .HasMaxLength(350)
                .HasColumnName("character");
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .HasColumnName("gender");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.ShelterId).HasColumnName("shelterID");
            entity.Property(e => e.Size)
                .HasMaxLength(50)
                .HasColumnName("size");

            entity.HasOne(d => d.Shelter).WithMany(p => p.Pets)
                .HasForeignKey(d => d.ShelterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pets_Shelters");
        });

        modelBuilder.Entity<PhotoOfPet>(entity =>
        {
            entity.ToTable("photoOfPet");

            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.ImageName)
                .HasMaxLength(350)
                .IsUnicode(false)
                .HasColumnName("imageName");
            entity.Property(e => e.PetId).HasColumnName("petID");
        });

        modelBuilder.Entity<Shelter>(entity =>
        {
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.PetCount).HasColumnName("petCount");
            entity.Property(e => e.ShelterName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ShelterPassword)
                .IsUnicode(false)
                .HasColumnName("shelterPassword");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.VisitorsTime)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("visitorsTime");
            entity.Property(e => e.WorkingTime)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("workingTime");
        });

        modelBuilder.Entity<Sponsoring>(entity =>
        {
            entity.ToTable("Sponsoring");

            entity.Property(e => e.PetId).HasColumnName("petID");
            entity.Property(e => e.UserId).HasColumnName("userID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Character)
                .HasMaxLength(350)
                .IsUnicode(false)
                .HasColumnName("character");
            entity.Property(e => e.Email)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(350)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<VirtualMeeting>(entity =>
        {
            entity.ToTable("virtualMeetings");

            entity.Property(e => e.Datetime)
                .HasColumnType("datetime")
                .HasColumnName("datetime");
            entity.Property(e => e.ShelterId).HasColumnName("shelterID");
            entity.Property(e => e.Url)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("url");
            entity.Property(e => e.UserId).HasColumnName("userID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
