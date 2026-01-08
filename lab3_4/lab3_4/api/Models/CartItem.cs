namespace lab3_4.api.Models;


public class CartItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public Item Item { get; set; }
}
