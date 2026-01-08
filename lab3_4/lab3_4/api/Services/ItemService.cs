using lab3_4.api;
using lab3_4.api.Models;
using Microsoft.EntityFrameworkCore;

namespace lab3_7.api.Services;

public class ItemService
{
    private readonly AppDbContext _context;

    public ItemService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateCategory(string name)
    {
        try
        {
            // Перевірка наявності категорії
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
            if (existingCategory != null)
            {
                throw new InvalidOperationException("The category already exists.");
            }

            // Створення нової категорії
            var newCategory = new Category
            {
                Name = name
            };

            _context.Categories.Add(newCategory);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error: {ex.Message}");
        }
    }


    public async Task<bool> CreateItem(string name, string categoryName)
    {
        try
        {
            // Знайти предмет за назвою
            var existingItem = await _context.Items.FirstOrDefaultAsync(u => u.Name == name);

            if (existingItem != null)
            {
                // Кинути помилку, якщо предмет з такою назвою вже існує
                throw new InvalidOperationException("The item already exists.");
            }

            // Перевірка наявності категорії
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            if (category == null)
            {
                throw new InvalidOperationException("The specified category does not exist.");
            }

            // Створити новий предмет
            var newItem = new Item
            {
                Name = name,
                CategoryId = category.Id
            };

            _context.Items.Add(newItem);

            // Зберегти зміни
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error: {ex.Message}");
        }
    }

    public async Task<List<string>> GetAllCategories()
    {
        try
        {
            var categories = await _context.Categories
                .Select(c => c.Name)
                .ToListAsync();

            // Додаємо "Усі" як першу категорію
            categories.Insert(0, "Усі");
            return categories;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error: {ex.Message}");
        }
    }
    
    public async Task<(bool success, List<Item> items)> GetAllItems()
    {
        try
        {
            // Отримуємо всі предмети з бази даних
            var items = await _context.Items
                .Include(i => i.Category)
                .ToListAsync();

            return (true, items);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error: {ex.Message}");
        }
    }
}