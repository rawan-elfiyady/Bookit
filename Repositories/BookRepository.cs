using System;
using Bookit.Data;
using Bookit.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookit.Repositories;

public class BookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context)
    {
        _context = context;
    }

    // Get Book By ID
    public async Task<Book?> GetBookById(int bookId)
    {
        return await _context.Books.FindAsync(bookId);
    }

    // Get Book By Name
    public async Task<Book?> GetBookByName(String name)
    {
        return await _context.Books.FirstOrDefaultAsync(b => b.Name == name);
    }

    // Get Book By Author
    public async Task<List<Book>> GetBooksByAuthor(string authorName)
    {
        return await _context.Books
                    .Where(b => b.Author == authorName)
                    .ToListAsync();
    }

    // Get Book By Category
    public async Task<List<Book>> GetBooksByCategory(string category)
    {
        return await _context.Books
                    .Where(b => b.Category == category)
                    .ToListAsync();

    }
    // Get All Books
    public async Task<List<Book>> GetAllBooks()
    {
        return await _context.Books.ToListAsync();
    }

    // Get Available Books
    public async Task<List<Book>> GetAvailableBooks()
    {
        return await _context.Books
                    .Where(b => b.AvailabilityStatus == true)
                    .ToListAsync();
    }
    
    // Delete Book
    public async Task<bool> deleteBook(int bookId)
    {
        var book = _context.Books.Find(bookId);

        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;

    }

    

    

    // Create Book
    public async void SaveBook(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    // Update Book
    public async void UpdateBook(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }
}