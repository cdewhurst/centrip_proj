using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecureBeaconService
{
    public class BeaconService : ServiceBase
    {
        private UiCommsServer _uiCommsServer = new UiCommsServer(30341);
        private static readonly HttpClient _client = new HttpClient();
        private readonly string _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "service.log");
        private CancellationTokenSource _cts = new CancellationTokenSource();

        protected override void OnStart(string[] args)
        {
            Log("Service started.");
            Task.Run(() => Run());
        }
        protected override void OnStop()
        {
            _uiCommsServer.Stop();
            _cts.Cancel();
            Log("Service stopped.");
        }

        public async Task Run()
        {
            _uiCommsServer.Start();
            _uiCommsServer.OnMessageReceived += _OnUiCommand;

            while (_cts.Token.IsCancellationRequested == false)
            {
                try
                {
                    // Wait for a command from the UI
                    await Task.Delay(10000, _cts.Token);
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled, exit the loop
                    break;
                }

                var payload = new
                {
                    hostname = Environment.MachineName,
                    ip = Utils.GetLikelyPublicIPAddress(),
                    timestamp = DateTimeOffset.Now.ToString("O") // Give TZ information as well as accurate time.
                };

                try
                {
                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    if (_client != null)
                    {
                        var response = await _client.PostAsync("https://httpbin.org/post", content);
                        Log($"Sent: {json} - Response: {response.StatusCode}");
                    }
                    else
                    {
                        Log($"No HttpClient to post request.");
                    }
                }
                catch (Exception ex)
                {
                    Log($"Error sending beacon: {ex.Message}");
                }
            }
        }

        private void _OnUiCommand(UiCommand cmd)
        {
            switch (cmd.Command)
            {
                case CommandType.Start:
                    Log("Received Start command from UI.");
                    break;
                case CommandType.Stop:
                    Log("Received Stop command from UI.");
                    _cts.Cancel();
                    break;
                default:
                    Log($"Unknown command received: {cmd.Command}");
                    break;
            }
        }

        private void Log(string message)
        {
            _uiCommsServer.SendMessageToClients(JObject.FromObject(new
            {
                Type = "Log",
                Message = message
            }));

            //File.AppendAllText(_logPath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}
