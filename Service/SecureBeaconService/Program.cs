using SecureBeaconService;
using System.ServiceProcess;

namespace SecureBeaconService2
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            new BeaconService().Run().GetAwaiter().GetResult();
#else
            var servicesToRun = new ServiceBase[]
            {
                new BeaconService()
            };
            ServiceBase.Run(servicesToRun);
#endif
        }
    }
}
