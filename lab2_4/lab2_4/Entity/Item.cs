namespace lab2_4.Entity;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }

    public Item(string name, string type)
    {
        Name = name;
        Type = type;
    }
}