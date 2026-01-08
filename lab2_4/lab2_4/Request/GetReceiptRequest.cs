using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using lab2_4.Entity;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace lab2_4.Request;

public class GetReceiptRequest
{
    private const string ServerAddress = "localhost"; // Адреса сервера
    private const int ServerPort = 5000; // Порт сервера

    public static async Task<(bool,int,int)> Send()
    {
        var registerModel = new
        {
            command = "getreceipt",
            id = LoginEntity.userId.ToString()
        };

        var request = JsonSerializer.Serialize(registerModel);
        var bytesToSend = Encoding.UTF8.GetBytes(request);

        using (var client = new TcpClient(ServerAddress, ServerPort))
        {
            using (var stream = client.GetStream())
            {
                try
                {
                    await stream.WriteAsync(bytesToSend, 0, bytesToSend.Length);

                    using (var memoryStream = new MemoryStream())
                    {
                        var buffer = new byte[1024];
                        int bytesRead;

                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await memoryStream.WriteAsync(buffer, 0, bytesRead);
                        }

                        string response = Encoding.UTF8.GetString(memoryStream.ToArray());

                        dynamic decodedResponse = JsonConvert.DeserializeObject(response);
                        Console.WriteLine(decodedResponse);

                        var responseObject = JsonSerializer.Deserialize<ResponseWrapper>(response);
                        if (responseObject.success)
                        {
                            return (responseObject.success, responseObject.OrderNumber, responseObject.Points);
                        }

                        MessageBox.Show(responseObject.message);
                        return (false,0,0);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return (false,0,0);
                }
            }
        }
    }

    public class ResponseWrapper
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int OrderNumber { get; set; }
        public int Points { get; set; }
    }
}