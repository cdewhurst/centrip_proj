using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecureBeaconService
{
    internal class UiCommsServer
    {
        private readonly List<StreamWriter> _activeClients = new List<StreamWriter>();
        private readonly object _clientLock = new object();

        private readonly int _port;
        private TcpListener _listener;
        private CancellationTokenSource _cts;

        public event Action<UiCommand> OnMessageReceived;

        public UiCommsServer(int port)
        {
            _port = port;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Loopback, _port);
            _listener.Start();

            Task.Run(() => AcceptClientsAsync(_cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
            _listener?.Stop();
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Listener was stopped, exit the loop
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TcpCommandServer] Accept failed: {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                // Add the client to the active clients list.
                lock (_clientLock)
                {
                    _activeClients.Add(writer);
                }

                try
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null && !cancellationToken.IsCancellationRequested)
                    {
                        _SendMessageToService(line);
                    }
                }
                finally
                {
                    // Remove the client when it disconnects.
                    lock (_clientLock)
                    {
                        _activeClients.Remove(writer);
                    }
                    writer.Dispose();
                }
            }
        }

        private void _SendMessageToService(string line)
        {
            try {
                var message = JsonConvert.DeserializeObject<UiCommand>(line);
                OnMessageReceived?.Invoke(message);
            }
            catch (Exception ex)
            {
                // A full implementation would catch all the different exceptions that can occur, such as JsonReaderException, JsonSerializationException, etc.
                Console.WriteLine($"[TcpCommandServer] Error processing message: {ex.Message}");
            }
        }

        public void SendMessageToClients(JObject message)
        {
            lock (_clientLock)
            {
                foreach (var writer in _activeClients.ToList())
                {
                    try
                    {
                        writer.WriteLine(message.ToString(Newtonsoft.Json.Formatting.None));
                    }
                    catch
                    {
                        // Remove the failed writer - has probably disconnected.
                        _activeClients.Remove(writer);
                    }
                }
            }
        }
    }
}