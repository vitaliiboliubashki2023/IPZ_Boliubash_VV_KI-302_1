using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using lab2_4.Entity;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace lab2_4.Request;

public class LoginRequest
{
    private const string ServerAddress = "localhost"; // Адреса сервера
    private const int ServerPort = 5000; // Порт сервера

    public static async Task<bool> Send(string name, string password)
    {
        var registerModel = new
        {
            command = "login",
            name = name,
            password = password
        };

        var request = JsonSerializer.Serialize(registerModel);
        var bytesToSend = Encoding.UTF8.GetBytes(request);

        try
        {
            using (var client = new TcpClient())
            {
                // Тайм-аут для підключення
                var connectTask = client.ConnectAsync(ServerAddress, ServerPort);
                if (await Task.WhenAny(connectTask, Task.Delay(5000)) != connectTask)
                {
                    MessageBox.Show("Сервер недоступний або перевищено час очікування підключення.");
                    return false;
                }

                using (var stream = client.GetStream())
                {
                    await stream.WriteAsync(bytesToSend, 0, bytesToSend.Length);

                    using (var memoryStream = new MemoryStream())
                    {
                        var buffer = new byte[1024];
                        int bytesRead;

                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await memoryStream.WriteAsync(buffer, 0, bytesRead);

                            // Перевіряємо, чи отримали повну відповідь (для оптимізації можна додати умову)
                            if (stream.DataAvailable == false)
                                break;
                        }

                        string response = Encoding.UTF8.GetString(memoryStream.ToArray());

                        dynamic decodedResponse = JsonConvert.DeserializeObject(response);
                        Console.WriteLine(decodedResponse);

                        var responseObject = JsonSerializer.Deserialize<ResponseWrapper>(response);
                        if (responseObject.success)
                        {
                            LoginEntity.userId = responseObject.userId;
                            return responseObject.success;
                        }

                        MessageBox.Show(responseObject.message);
                        return responseObject.success;
                    }
                }
            }
        }
        catch (SocketException ex)
        {
            MessageBox.Show($"Сервер недоступний: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Виникла помилка: {ex.Message}");
            return false;
        }
    }


    public class ResponseWrapper
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int userId { get; set; }
    }
}