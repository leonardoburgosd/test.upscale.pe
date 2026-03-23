using Microsoft.EntityFrameworkCore;
using TestUpscaleApp.Models;

namespace TestUpscaleApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .Property(u => u.Names)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.FathersSurname)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.MothersSurname)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.DocumentType)
                .IsRequired()
                .HasMaxLength(3); // 'DNI' o 'CE'

            modelBuilder.Entity<User>()
                .Property(u => u.DocumentNumber)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.DateOfBirth)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Nationality)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Peruana");

            modelBuilder.Entity<User>()
                .Property(u => u.Gender)
                .IsRequired()
                .HasMaxLength(1); // 'M' o 'F'

            modelBuilder.Entity<User>()
                .Property(u => u.MainEmail)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.SecondaryEmail)
                .HasMaxLength(255);

            modelBuilder.Entity<User>()
                .Property(u => u.MainPhone)
                .IsRequired()
                .HasMaxLength(11); // 9-11 caracteres

            modelBuilder.Entity<User>()
                .Property(u => u.SecondaryPhone)
                .HasMaxLength(11);

            modelBuilder.Entity<User>()
                .Property(u => u.ContractType)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<User>()
                .Property(u => u.HiringDate)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.UserPasswordEncrypt)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.UserSalt)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.CVF)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.ValidationCode)
                .IsRequired()
                .HasMaxLength(12);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.MainEmail)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.DocumentNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.MainPhone);
        }
    }
}
