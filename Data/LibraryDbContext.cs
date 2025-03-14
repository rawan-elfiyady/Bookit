using System;
using Bookit.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookit.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options) { }


    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BorrowedBook> BorrowedBooks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique Constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Book>()
            .HasIndex(b => b.Name)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<String>();

        // Define Many-to-Many Relationship
        modelBuilder.Entity<BorrowedBook>()
            .HasOne(bb => bb.User)
            .WithMany(u => u.BorrowedBooks)
            .HasForeignKey(bb => bb.UserId);

        modelBuilder.Entity<BorrowedBook>()
            .HasOne(bb => bb.Book)
            .WithMany(b => b.BorrowedBooks)
            .HasForeignKey(bb => bb.BookId);
    }
}
