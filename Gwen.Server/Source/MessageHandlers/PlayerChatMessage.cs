using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace Gwen.Server.MessageHandlers {
  public class PlayerChatMessageHandler : IMessageHandler
  {
    private readonly ILogger<PlayerChatMessageHandler> _logger;
    public string MessageId => "CHAT";

    public PlayerChatMessageHandler(ILogger<PlayerChatMessageHandler> logger) {
      this._logger = logger;
    }

    public void Handle(string data, TcpClient client, NetworkStream stream) {
      var endpoint = client.Client.RemoteEndPoint as IPEndPoint;

      this._logger.LogInformation("[{0}:{1}] Chat message: {2}", endpoint!.Address, endpoint!.Port, data.Substring(this.MessageId.Length + 1));
    }
  }
}