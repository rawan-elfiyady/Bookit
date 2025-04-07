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

    // Get All Borrowed Books
    public async Task<List<BorrowedBook>> GetBorrowedBooks()
    {
        return await _context.BorrowedBooks.ToListAsync();
    }

    // Get Borrowed Book
    public async Task<BorrowedBook?> GetBorrowedBook(int bookId)
    {
        return await _context.BorrowedBooks.FindAsync(bookId);
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
    // Get Returned Books
    public async Task<List<BorrowedBook>> GetReturnedBooks()
    {
        return await _context.BorrowedBooks
                    .Where(b => b.IsReturned == true)
                    .ToListAsync();
    }

    // Get User Borrowed Books
    public async Task<List<BorrowedBook>> GetUserBorrowedBooks(int userId)
    {
        return await _context.BorrowedBooks
                    .Where(b => b.UserId == userId)
                    .ToListAsync();
    }

    // Get Pending Borrow Requests
    public async Task<List<BorrowedBook>> GetPendingBorrowRequests()
    {
        return await _context.BorrowedBooks
                    .Where(b => b.IsConfirmed == false)
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

    // Delete Borrowed Book
    public async Task<bool> DeleteBorrowedBook(int bookId)
    {
        var book = _context.BorrowedBooks.Find(bookId);

        if (book != null)
        {
            _context.BorrowedBooks.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;

    }

    // Approve Borrow Request
    public async Task<bool> ApproveBorrowRequest(int bookId)
    {
        var book = await _context.BorrowedBooks.FindAsync(bookId);

        if (book != null)
        {
            book.IsConfirmed = true;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    // Approve Return Request
    public async Task<bool> ApproveReturnRequest(int bookId)
    {
        var book = await _context.BorrowedBooks.FindAsync(bookId);

        if (book != null)
        {
            book.IsReturned = true;
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