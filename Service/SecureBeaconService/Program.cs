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
            new Service().RunAsConsole();
#else
            var servicesToRun = new ServiceBase[]
            {
                new Service()
            };
            ServiceBase.Run(servicesToRun);
#endif
        }
    }
}
