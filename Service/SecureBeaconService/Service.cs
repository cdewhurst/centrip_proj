using System.ServiceProcess;

namespace SecureBeaconService
{
    internal class Service : ServiceBase
    {
        private UiCommsServer _uiCommsServer = new UiCommsServer(30341);
        private HttpsCaller _httpsCaller = new HttpsCaller();

        protected override void OnStart(string[] args)
        {
            _uiCommsServer.OnMessageReceived += _OnUiCommand;
            _uiCommsServer.Start();
        }

        protected override void OnStop()
        {
            _uiCommsServer.Stop();

            if (_httpsCaller.IsRunning)
            {
                // Make sure we call this after _uiCommsServer is stopped so that there's no chance
                // of a "start" coming in after we stop the HTTPS caller.
                _httpsCaller.Stop();
            }
        }

        private void _OnUiCommand(UiCommand cmd)
        {
            switch (cmd.Command)
            {
                case CommandType.Start:
                    _ = _httpsCaller.Start();
                    break;

                case CommandType.Stop:
                    _httpsCaller.Stop();
                    break;

                default:
                    //Log($"Unknown command received: {cmd.Command}");
                    break;
            }
        }
    }
}
