using System;
using Bookit.Data;
using Bookit.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookit.Repositories;

public class BorrowedBooksRepository
{
    private readonly LibraryDbContext _context;

    public BorrowedBooksRepository(LibraryDbContext context)
    {
        _context = context;
    }

    // Create Book
    public async Task SaveBook(BorrowedBook book)
    {
        book.ConvertDatesToUtc();
        _context.BorrowedBooks.Add(book);
        await _context.SaveChangesAsync();
    }

    // Get Book By ID
    public async Task<BorrowedBook?> GetBookById(int bookId)
    {
        return await _context.BorrowedBooks.FindAsync(bookId);
    }
    // Get Book By BookId & UserId
    public async Task<BorrowedBook?> GetBookByUserId(int bookId, int userId)
    {
        return await _context.BorrowedBooks
                            .Where(b => b.BookId == bookId
                            && b.UserId == userId).FirstOrDefaultAsync();
    }

    // Get All Borrowed Books
    public async Task<List<BorrowedBook>> GetBorrowedBooks()
    {
        return await _context.BorrowedBooks.ToListAsync();
    }

    // Get Pending Return Requests
    public async Task<List<BorrowedBook>> GetPendingReturnRequests()
    {
        return await _context.BorrowedBooks
                            .Where(b =>
                                    b.IsReturned == false &&
                                    b.IsRequestedToBeReturned == true)
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
    // Update Book
    public async Task UpdateBook(BorrowedBook book)
    {
        book.ConvertDatesToUtc();
        _context.BorrowedBooks.Update(book);
        await _context.SaveChangesAsync();
    }
    // Delete Borrowed Book
    public async Task<bool> DeleteBorrowedBook(int bookId, int userId)
    {
        var book = await _context.BorrowedBooks
                            .Where(b => b.BookId == bookId
                            && b.UserId == userId).FirstOrDefaultAsync();

        if (book != null)
        {
            _context.BorrowedBooks.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;

    }

    // Approve Borrow Request
    public async Task<bool> ApproveBorrowRequest(int bookId, int userId)
    {
        var book = await _context.BorrowedBooks
                            .Where(b => b.BookId == bookId
                            && b.UserId == userId).FirstOrDefaultAsync();

        if (book != null)
        {
            book.IsConfirmed = true;
            book.BorrowDate = DateTime.Now;
            book.DueDate = DateTime.Now.AddDays(14);
            book.BorrowDate = book.BorrowDate.ToUniversalTime();
            book.DueDate = book.DueDate.ToUniversalTime();
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
    // Approve Return Request
    public async Task<bool> ApproveReturnRequest(int bookId, int userId)
    {
        var book = await _context.BorrowedBooks
                            .Where(b => b.BookId == bookId
                            && b.UserId == userId).FirstOrDefaultAsync();

        if (book != null)
        {
            book.IsReturned = true;
            book.ReturnDate = DateTime.Now;
            book.ReturnDate = book.ReturnDate.ToUniversalTime();

            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
