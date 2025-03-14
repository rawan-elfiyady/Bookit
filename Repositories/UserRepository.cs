using System;
using System.Threading.Tasks;
using Bookit.Data;
using Bookit.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookit.Repositories;

public class UserRepository
{
    private readonly LibraryDbContext _context;

    public UserRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmail(String email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserById(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }
    public async Task<User?> GetUserById_Role(int userId, string role)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Role == role);
    }



    public async void SaveUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async void UpdateUser(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> deleteUser(int userId)
    {
        var user = _context.Users.Find(userId);

        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;

    }

    public async Task<bool> ApproveLibrarian(int LibrarianId)
    {
        var librarian = await _context.Users.FindAsync(LibrarianId);

        if (librarian != null)
        {
            librarian.IsApproved = true;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }


    public async Task<List<User>> GetPendingLibrarians()
    {
        return await _context.Users
                    .Where(l => l.IsApproved == false && l.Role == "Librarian")
                    .ToListAsync();
    }
    public async Task<List<User>> GetLibrarians()
    {
        return await _context.Users
                    .Where(l => l.Role.Equals("Librarian"))
                    .ToListAsync();
    }
    public async Task<List<User>> GetUsers()
    {
        return await _context.Users
                    .Where(l => l.Role.Equals("User"))
                    .ToListAsync();
    }


}
