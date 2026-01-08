using lab3_4.api.Models;
using Microsoft.EntityFrameworkCore;

namespace lab3_4.api.Services;

public class CartService
{
    private readonly AppDbContext _context;

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddToCart(string userId, string itemid)
    {
        try
        {
            var UserId = int.Parse(userId);
            var ItemId = int.Parse(itemid);
            // Знайти товар за назвою
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == ItemId);
            if (item == null)
            {
                throw new InvalidOperationException("Item does not exist.");
            }

            // Знайти користувача за номером телефону
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User does not exist.");
            }

            // Перевірка, чи вже існує товар у кошику
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == user.Id && ci.ItemId == item.Id);

            if (existingCartItem != null)
            {
                // Товар вже в кошику, можна повернути false або обновити
                return false;
            }

            // Створення нового елемента кошика
            var cartItem = new CartItem
            {
                UserId = user.Id, // Використовуємо UserId
                ItemId = item.Id
            };

            _context.CartItems.Add(cartItem);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error: {ex.Message}");
        }
    }

    public async Task<bool> RemoveFromCart(string userId, string itemId)
    {
        try
        {
            var UserId = int.Parse(userId);
            var ItemId = int.Parse(itemId);
            // Знайти товар за назвою
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == ItemId);
            if (item == null)
            {
                throw new InvalidOperationException("Item does not exist.");
            }

            // Знайти користувача за номером телефону
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User does not exist.");
            }

            // Знайти елемент кошика, який потрібно видалити
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == user.Id && ci.ItemId == item.Id);

            if (cartItem == null)
            {
                // Товар не знайдено в кошику
                return false;
            }

            // Видалення елемента з кошика
            _context.CartItems.Remove(cartItem);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error: {ex.Message}");
        }
    }

    public async Task<(bool success, List<CartItemDto> cartItems)> GetCartItemsByUserPhone(string userId)
    {
        try
        {
            var UserId = int.Parse(userId);
            // Знайти користувача за номером телефону
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if (user == null)
            {
                return (false, null);
            }

            var cartItems = await _context.CartItems
                .Include(ci => ci.Item)
                .Where(ci => ci.UserId == user.Id)
                .ToListAsync();

            // Формування результату у вигляді DTO
            var result = cartItems.Select<CartItem, CartItemDto>(ci => new CartItemDto
            {
                Id = ci.Item.Id,
                Name = ci.Item.Name,
                Type = ci.Item.Category.Name
            }).ToList();

            return (true, result);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error: {ex.Message}");
        }
    }

    public async Task<(bool success, int receiptNumber, int pointsEarned)> GetReceipt(string userId)
    {
        try
        {
            var UserId = int.Parse(userId);
            // Знайти користувача за номером телефону
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if (user == null)
            {
                throw new InvalidOperationException("User does not exist.");
            }

            // Отримати всі елементи кошика для користувача
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == user.Id)
                .ToListAsync();
            
            // Перевірка, чи кошик не порожній
            if (cartItems.Count == 0)
            {
                throw new InvalidOperationException("Корзина не може бути порожньою.");
            }

            // Розрахунок кількості куплених товарів та балів
            int totalItems = cartItems.Count;
            int pointsEarned = totalItems * 2;

            // Додавання балів до користувача
            user.Points += pointsEarned; // Оновлення кількості балів
            _context.Users.Update(user); // Оновлення користувача в контексті

            // Створення нового чека
            var receipt = new Receipt
            {
                UserId = user.Id,
                CreatedAt = DateTime.Now // Зберігаємо тільки дату створення
            };

            // Додавання чека в базу даних
            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();

            // Збереження інформації про покупки
            foreach (var cartItem in cartItems)
            {
                var purchase = new Purchase
                {
                    UserId = user.Id,
                    ItemId = cartItem.ItemId,
                    ReceiptId = receipt.Id,
                    PurchasedAt = DateTime.Now
                };

                _context.Purchases.Add(purchase);
            }

            await _context.SaveChangesAsync(); // Зберегти всі покупки

            // Очищення кошика користувача
            await ClearCartByUserId(user.Id);

            // Повертаємо номер чека (Id чека), кількість отриманих балів
            return (true, receipt.Id, pointsEarned);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error: {ex.Message}");
        }
    }


    private async Task ClearCartByUserId(int userId)
    {
        var cartItems = await _context.CartItems
            .Where(ci => ci.UserId == userId)
            .ToListAsync();

        // Видалення всіх елементів кошика
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
    }
}