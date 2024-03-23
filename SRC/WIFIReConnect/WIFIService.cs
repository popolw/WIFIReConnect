using ManagedNativeWifi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WIFIReConnect
{
    public class WIFIService : BackgroundService
    {
        protected readonly ILogger _logger;
        private readonly string _wifi;
        private readonly int _checkCycle;
        public WIFIService(IConfiguration configuration, ILogger<WIFIService> logger)
        {
            this._wifi = configuration["WIFI"] ?? string.Empty;
            this._checkCycle = configuration.GetValue<int>("CheckCycle");
            this._logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"WIFIReConnectService Started,WIFI {_wifi}");
            return base.StartAsync(cancellationToken);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                var profile = NativeWifi.EnumerateProfileRadios().Where(p => p.Document.Ssid.ToString() == _wifi).FirstOrDefault();
                if (profile!=null && !profile.Interface.IsConnected)
                {
                    _logger.LogInformation($"WIFI {_wifi} 未链接,信号质量:{profile.SignalQuality},尝试链接到该WIFI");
                    var success = await NativeWifi.ConnectNetworkAsync(profile.Interface.Id, profile.Name, profile.Document.BssType, TimeSpan.FromSeconds(4));
                    if (success)
                    {
                        _logger.LogInformation($"WIFI {profile.Document.Ssid} 链接成功");
                    }
                    else
                    {
                        _logger.LogWarning($"WIFI {profile.Document.Ssid} 链接失败");
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(this._checkCycle));
            }

        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("WIFIReConnectService Stoped");
            return base.StopAsync(cancellationToken);
        }
    }
}
