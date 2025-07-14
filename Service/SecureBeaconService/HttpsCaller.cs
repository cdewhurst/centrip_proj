using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecureBeaconService
{
    internal class HttpsCaller
    {
        private static readonly HttpClient _client = new HttpClient();
        private readonly string _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "service.log");
        private CancellationTokenSource _cts;

        public bool IsRunning { get; private set; } = false;

        public async Task Start()
        {
            _cts = new CancellationTokenSource();

            IsRunning = true;

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
                        Log($"Sent: {json} - Response: {(int) response.StatusCode} ({response.StatusCode})"); 
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

        public void Stop() 
        {
            _cts?.Cancel();
            _cts = null;

            IsRunning = false;
        }

        private void Log(string message)
        {
            //_uiCommsServer.SendMessageToClients(JObject.FromObject(new
            //{
            //    Type = "Log",
            //    Message = message
            //}));

            //File.AppendAllText(_logPath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}
