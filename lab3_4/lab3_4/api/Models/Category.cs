namespace lab3_4.api.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Item> Items { get; set; } // Зв'язок один до багатьох
}