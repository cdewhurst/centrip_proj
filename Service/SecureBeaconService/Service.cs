using System.ServiceProcess;
using System.Threading.Tasks;

namespace SecureBeaconService
{
    internal class Service : ServiceBase
    {
        private UiCommsServer _uiCommsServer = new UiCommsServer(30341);
        private HttpsCaller _httpsCaller = new HttpsCaller();

        protected override void OnStart(string[] args)
        {
            Log.Write("SecureBeaconService starting...");
            _uiCommsServer.OnMessageReceived += _OnUiCommand;
            _uiCommsServer.Start();
        }

        protected override void OnStop()
        {
            Log.Write("SecureBeaconService stopping...");
            _uiCommsServer.Stop();

            if (_httpsCaller.IsRunning)
            {
                // Make sure we call this after _uiCommsServer is stopped so that there's no chance
                // of a "start" coming in after we stop the HTTPS caller.
                _httpsCaller.Stop();
            }
        }

        public void RunAsConsole()
        {
            OnStart(null);
            // Run for 24 hours - plenty enough to debug, or until it's killed.
            Task.Delay(new System.TimeSpan(24,0,0)).Wait();
        }

        private void _OnUiCommand(UiCommand cmd)
        {
            switch (cmd.Command)
            {
                case CommandType.Start:
                    _ = _httpsCaller.Start(cmd.Address, cmd.Port);
                    break;

                case CommandType.Stop:
                    _httpsCaller.Stop();
                    break;

                default:
                    Log.Write($"Unknown command received: {cmd.Command}");
                    break;
            }
        }
    }
}
