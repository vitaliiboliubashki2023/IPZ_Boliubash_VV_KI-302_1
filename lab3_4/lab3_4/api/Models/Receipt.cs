namespace lab3_4.api.Models;

public class Receipt
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Навігаційне поле
    public User User { get; set; }
}