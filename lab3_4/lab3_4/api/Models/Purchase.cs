namespace lab3_4.api.Models;

public class Purchase
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public int ReceiptId { get; set; }
    public DateTime PurchasedAt { get; set; }

    public User User { get; set; }
    public Item Item { get; set; }
    public Receipt Receipt { get; set; }
}