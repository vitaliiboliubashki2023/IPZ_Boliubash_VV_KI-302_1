using lab3_4.api.Models;
using Microsoft.EntityFrameworkCore;

namespace lab3_4.api.Services;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(int userId, bool isSuccess)> RegisterUser(string name, string password)
    {
        // Validate password
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            throw new ArgumentException("Пароль має бути не коротшим за 8 символів.");
        }

        if (!password.Any(char.IsUpper))
        {
            throw new ArgumentException("Пароль має містити хоча б одну велику літеру.");
        }

        if (!password.Any(char.IsLower))
        {
            throw new ArgumentException("Пароль має містити хоча б одну малу літеру.");
        }

        if (!password.Any(char.IsDigit))
        {
            throw new ArgumentException("Пароль має містити хоча б одну цифру.");
        }

        if (!password.Any(c => "!@#$%^&*()-_=+[]{}|;:'\",.<>?/".Contains(c)))
        {
            throw new ArgumentException("Пароль має містити хоча б один спеціальний символ.");
        }

        try
        {
            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Name == name);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Користувач з таким ім'м вже існує.");
            }

            // Create new user
            var user = new User
            {
                Name = name,
                Password = password,
                Points = 0
            };

            _context.Users.Add(user);
            var result = await _context.SaveChangesAsync();

            return (user.Id, result > 0);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error during user registration: {ex.Message}");
            throw;
        }
    }

    public async Task<(int userId, bool isSuccess)> LoginUser(string name, string password)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Ім'я та пароль не можуть бути порожніми.");
        }

        try
        {
            // Check if user exists and password matches
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == name && u.Password == password);
            if (user != null)
            {
                return (user.Id, true); // Successful login
            }

            return (0, false); // User not found or password incorrect
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error during user login: {ex.Message}");
            throw;
        }
    }


    public async Task<(bool success, int points)> GetUserPoints(string userPhone)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == userPhone);
        if (user == null)
        {
            throw new InvalidOperationException("User does not exist.");
        }

        return (true, user.Points);
    }
}