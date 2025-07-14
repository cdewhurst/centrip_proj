using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecureBeaconService
{
    internal class HttpsCaller
    {
        private static readonly HttpClient _client = new HttpClient(_GetCertificateCheckingHttpHandler());
        private CancellationTokenSource _cts;
        private static int delayPeriodMinMs = 5000; // 5 seconds - standard inter-call period.
        private static int delayPeriodMaxMs = 30000; // 30 seconds
        private int delayPeriodMs = delayPeriodMinMs;

        public bool IsRunning { get; private set; } = false;

        public async Task Start(string address, int port)
        {

            _cts = new CancellationTokenSource();

            IsRunning = true;

            Log.Write("Started beacon service");
            Log.SetStatus(Status.StartRequested);

            while (_cts.Token.IsCancellationRequested == false)
            {
                var status = Status.Failed; // Fallback status if anything goes wrong.

                status = await _PostRequest(address, port, status);

                _UpdateState(status);

                try
                {
                    // Wait for a command from the UI
                    await Task.Delay(delayPeriodMs, _cts.Token);
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled, exit the loop
                    break;
                }
            }
        }

        private async Task<Status> _PostRequest(string address, int port, Status status)
        {
            var payload = new
            {
                hostname = Environment.MachineName,
                ip = Utils.GetLikelyPublicIPAddress(),
                timestamp = DateTimeOffset.Now.ToString("O") // Give TZ information as well as accurate time.
            };

            var uri = Utils.UriFromAddressAndPort(address, port);
            try
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                if (_client != null)
                {
                    var response = await _client.PostAsync(uri, content, _cts.Token);
                    Log.Write($"Sent: {json} to {uri} - Response: {(int)response.StatusCode} ({response.StatusCode})");
                    status = (response.StatusCode == System.Net.HttpStatusCode.OK) ? Status.LastCallGood : Status.Retrying;
                }
                else
                {
                    Log.Write($"No HttpClient to post request.");
                    // Can't recover from this, so we'll set the status to Failed.
                    status = Status.Failed;
                }
            }
            catch (Exception ex)
            {
                Log.Write($"Error sending beacon to {uri}: {ex.Message}");
                status = Status.Retrying;
            }

            return status;
        }

        private void _UpdateState(Status status)
        {
            if (status == Status.Retrying)
            {
                delayPeriodMs *= 2;
                if (delayPeriodMs > delayPeriodMaxMs)
                {
                    // We'll break out of the loop.
                    status = Status.Failed;
                    _cts.Cancel();
                    Log.Write($"Exceeded maximum wait time. Cancelling post requests.");
                }
                else
                {
                    Log.Write($"Last call did not succeed. Waiting {delayPeriodMs}ms until next attempt");
                }
            }
            Log.SetStatus(status);
        }

        public void Stop() 
        {
            _cts?.Cancel();
            
            Log.Write("Stopped beacon service");
            Log.SetStatus(Status.Stopped);

            IsRunning = false;
        }

        private static HttpClientHandler _GetCertificateCheckingHttpHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                {
                    if (sslPolicyErrors == SslPolicyErrors.None)
                    {
                        Log.Write("Certicate is valid and trusted: " + certificate.Subject);
                        // Certificate is valid and the chain is trusted
                        return true;
                    }

                    // Allow self-signed certificates only if they are the sole issue
                    if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors &&
                        chain.ChainStatus.Length == 1 &&
                        chain.ChainStatus[0].Status == X509ChainStatusFlags.UntrustedRoot)
                    {
                        // Self-signed certs will trigger "UntrustedRoot" because there's no CA.
                        // Accepting for demo/testing purposes only.
                        Log.Write("Accepting self-signed certificate: " + certificate.Subject);
                        return true;
                    }

                    // Reject all other cert errors
                    Log.Write("Certificate error: " + sslPolicyErrors);
                    foreach (var status in chain.ChainStatus)
                    {
                        Log.Write($"Chain status: {status.Status} - {status.StatusInformation}");
                    }

                    return false;
                }
            };
        }
    }
}
