using lab3_4.api.Services;

namespace lab3_4;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var tcpServer = scope.ServiceProvider.GetRequiredService<TcpServer>();
            await tcpServer.StartAsync(stoppingToken);
        }
    }
}