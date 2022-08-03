using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace DataGen
{
    public class Worker : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<Worker> _logger;

        public Worker(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<Worker> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                // random to seed data into the system
                var rand = new Random();
                var timeUpPeriod = rand.Next(3, 5);
                _logger.LogInformation("CoffeeShop URL: {url}", _config.GetValue<string>("CoffeeShopApi"));
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri(_config.GetValue<string>("CoffeeShopApi"), UriKind.Absolute);

                var orderCommand = new PlaceOrderCommand();
                orderCommand.BaristaItems.Add(new CommandItem
                {
                    ItemType = (ItemType)rand.Next(0, 5),
                });
                orderCommand.KitchenItems.Add(new CommandItem
                {
                    ItemType = (ItemType)rand.Next(6, 9),
                });

                var orderCommandJson = new StringContent(
                    JsonSerializer.Serialize(orderCommand),
                    Encoding.UTF8,
                    Application.Json);

                using var httpResponseMessage = await httpClient.PostAsync("/apis/counter/v1/api/orders", orderCommandJson);
                httpResponseMessage.EnsureSuccessStatusCode();

                await Task.Delay(TimeSpan.FromSeconds(timeUpPeriod), stoppingToken);
            }
        }
    }

    public class PlaceOrderCommand
    {
        public int CommandType { get; set; } = 0;
        public int OrderSource { get; set; } = 0;
        public int Location { get; set; } = 0;
        public Guid LoyaltyMemberId { get; set; } = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        public List<CommandItem> BaristaItems { get; set; } = new();
        public List<CommandItem> KitchenItems { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class CommandItem
    {
        public ItemType ItemType { get; set; }
    }

    public enum ItemType
    {
        // Beverages
        CAPPUCCINO,
        COFFEE_BLACK,
        COFFEE_WITH_ROOM,
        ESPRESSO,
        ESPRESSO_DOUBLE,
        LATTE,
        // Food
        CAKEPOP,
        CROISSANT,
        MUFFIN,
        CROISSANT_CHOCOLATE
    }
}