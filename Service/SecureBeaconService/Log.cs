using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace SecureBeaconService
{
    public enum Status
    {
        Stopped,
        StartRequested,
        LastCallGood,
        Retrying,
        Failed,
    }

    internal static class Log
    {
        public static UiCommsServer UiCommsServer { get; set; } = null;
        public static string _logDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "SecureBeaconService");
        public static string _logPath = Path.Combine(_logDir, "service.log");

        public static void Write(string message)
        {
            // Add the time at the start of each message.
            message = $"{DateTimeOffset.Now:O} - {message}";

            if (UiCommsServer != null)
            {
                try
                {
                    UiCommsServer.SendMessageToClients(JObject.FromObject(new
                    {
                        Type = "Log",
                        Message = message,
                    }));
                }
                catch (Exception ex)
                {
                    // If we can't send the message, just write it to the console.
                    Console.WriteLine($"[Log] Error sending log message to UI: {ex.Message}");
                    Console.WriteLine(message);
                }
            }

            try
            {
                if (!Directory.Exists(_logDir))
                {
                    // Should create the entire dir hierarchy.
                    Directory.CreateDirectory(_logDir);
                }

                File.AppendAllText(_logPath, $"{message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Log] Error sending log message to file: {ex.Message}");
                Console.WriteLine(message);
            }
        }

        public static void SetStatus(Status status)
        {
            if (UiCommsServer != null)
            {
                try
                {
                    UiCommsServer.SendMessageToClients(JObject.FromObject(new
                    {
                        Type = "Status",
                        Status = status.ToString(),
                    }));
                }
                catch (Exception ex)
                {
                    // If we can't send the status, just write it to the console.
                    Console.WriteLine($"[Log] Error setting status on UI: {status}");
                }
            }
        }
    }
}
