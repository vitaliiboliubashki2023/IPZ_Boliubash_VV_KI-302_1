using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using lab3_4.api.Models;
using lab3_4.api.Services;
using lab3_7.api.Services;

public class TcpServer
{
    private readonly UserService _userService;
    private readonly ItemService _itemService;
    private readonly ILogger _logger;
    private readonly CartService _cartService;

    public TcpServer(UserService userService, ItemService itemService, CartService cartService,
        ILogger<TcpServer> logger)
    {
        _userService = userService;
        _itemService = itemService;
        _cartService = cartService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        _logger.LogInformation("TCP Server started on port 5000.");

        while (!cancellationToken.IsCancellationRequested)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = HandleClientAsync(client);
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        _logger.LogInformation("Client connected.");
        using (client)
        using (var stream = client.GetStream())
        {
            var buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine(request);

            try
            {
                // Десеріалізація запиту
                var requestData = JsonSerializer.Deserialize<Dictionary<string, string>>(request);
                if (requestData == null)
                {
                    var response = new { message = "Invalid command." };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                    return;
                }

                var command = requestData["command"].ToLower();
                if (command == "register")
                {
                    var (userId, registerSuccess) = await _userService.RegisterUser(requestData["name"], requestData["password"]);
                    var response = new
                    {
                        success = registerSuccess,
                        message = registerSuccess ? "User logged successfully." : "Invalid phone or password.",
                        userId = userId
                    };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                } else if (command == "login")
                {
                    var (userId, loginSuccess) = await _userService.LoginUser(requestData["name"], requestData["password"]);
                    var response = new
                    {
                        success = loginSuccess,
                        message = loginSuccess ? "User logged successfully." : "Invalid phone or password.",
                        userId = userId
                    };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                }
                else if (command == "createcategory")
                {
                    bool createSuccess = await _itemService.CreateCategory(requestData["Name"]);
                    var response = new
                    {
                        success = createSuccess,
                        message = createSuccess ? "Category created successfully." : "Invalid Name."
                    };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                }
                else if (command == "createitem")
                {
                    // Отримання назви та назви категорії з запиту
                    string itemName = requestData["Name"];
                    string categoryName = requestData["CategoryName"];

                    bool createSuccess = await _itemService.CreateItem(itemName, categoryName);
                    var response = new
                    {
                        success = createSuccess,
                        message = createSuccess ? "Item created successfully." : "Invalid Name."
                    };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                }
                else if (command == "addtocart")
                {
                    string userId = requestData["UserId"];
                    string itemId = requestData["ItemId"];

                    bool addSuccess = await _cartService.AddToCart(userId, itemId);
                    var response = new
                    {
                        success = addSuccess,
                        message = addSuccess
                            ? "Item added to cart successfully."
                            : "Item already in cart or does not exist."
                    };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                }
                else if (command == "removefromcart")
                {
                    string userId = requestData["UserId"];
                    string itemId = requestData["ItemId"];

                    bool removeSuccess = await _cartService.RemoveFromCart(userId, itemId);
                    var response = new
                    {
                        success = removeSuccess,
                        message = removeSuccess
                            ? "Item removed from cart successfully."
                            : "Item not found in cart or does not exist."
                    };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                }
                else if (command == "getcategories")
                {
                    var categories = await _itemService.GetAllCategories();
                    var response = new
                    {
                        success = true,
                        message = "Categories",
                        categories = categories
                    };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                }
                else if (command == "getitems")
                {
                    var (success, items) = await _itemService.GetAllItems();
                    var response = new
                    {
                        success = success,
                        message = "Items",
                        items = items.Select(i => new
                            { i.Id, i.Name, Type = i.Category.Name })
                    };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                }
                else if (command == "getcartitems")
                {
                    string userId = requestData["id"];
                    var (success, cartItems) = await _cartService.GetCartItemsByUserPhone(userId);
                    var response = new
                    {
                        success = success,
                        message = "Cart items",
                        items = cartItems
                    };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                }
                else if (command == "getreceipt")
                {
                    string userId = requestData["id"];
                    var (success, OrderNumber, Points) = await _cartService.GetReceipt(userId);
                    var response = new
                    {
                        success = success,
                        OrderNumber = OrderNumber,
                        Points = Points
                    };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                }
                else if (command == "getpoints")
                {
                    var (success, points) = await _userService.GetUserPoints(requestData["Name"]);
                    var response = new
                    {
                        success = success,
                        points = points
                    };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                }
                else
                {
                    var response = new { message = "Invalid command." };
                    var jsonResponse = JsonSerializer.Serialize(response);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
                }
            }
            catch (JsonException ex)
            {
                var response = new { success = false, message = $"Invalid JSON format: {ex.Message}" };
                var jsonResponse = JsonSerializer.Serialize(response);
                await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
            }
            catch (InvalidOperationException ex)
            {
                var response = new
                    { success = false, message = $"Error InvalidOperationException: {ex.Message}" };
                var jsonResponse = JsonSerializer.Serialize(response);
                await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
            }
            catch (Exception ex)
            {
                var response = new { success = false, message = $"Error: {ex.Message}" };
                var jsonResponse = JsonSerializer.Serialize(response);
                await stream.WriteAsync(Encoding.UTF8.GetBytes(jsonResponse));
            }
        }
    }
}