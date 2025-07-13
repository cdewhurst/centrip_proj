using Newtonsoft.Json;

namespace SecureBeaconService
{
    internal enum CommandType
    {
        Start,
        Stop
    }

    internal class UiCommand
    {
        [JsonProperty(Required = Required.Always)]
        public CommandType Command { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }
    }
}
