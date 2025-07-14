using System.ServiceProcess;
using System.Threading;

namespace SecureBeaconService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            new UiCommsServer(30341).AcceptClientsAsync(CancellationToken.None).GetAwaiter().GetResult();
#else
            var servicesToRun = new ServiceBase[]
            {
                new UiCommsServer(30341)
            };
            ServiceBase.Run(servicesToRun);
#endif
        }
    }
}
