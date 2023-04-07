using System.Net;
using System.Net.Sockets;
using Gwen.Server.MessageHandlers;
using Microsoft.Extensions.Logging;

namespace Gwen.Server.Services {
  public class PlayerHandler {
    private readonly ILogger<PlayerHandler> _logger;
    private readonly IMessageHandler[] _messageHandlers;
    private readonly TcpClient _client;

    private readonly GameServer _server;

    private List<byte> _messageContent = new List<byte>();
    private IPEndPoint _endpoint;

    public PlayerHandler(
      IMessageHandler[] messageHandlers,
      ILogger<PlayerHandler> logger,
      TcpClient client,
      GameServer server) {
      this._messageHandlers = messageHandlers;
      this._client = client;
      this._logger = logger;
      this._server = server;
    }

    public void Handle() {
      var endpoint = this._client.Client.RemoteEndPoint as IPEndPoint;
      if (endpoint == null) {
        this._logger.LogError("Error getting client endpoint, disposing client");
        //this._server.DisposeClient(this);
        return;
      }

      this._endpoint = endpoint;
      this._logger.LogInformation("Accepted client from {0}:{1}", endpoint.Address, endpoint.Port);

      this.StartLoop();
    }

    private void StartLoop() {
      Byte[] bytes = new Byte[256];
      String data = null;

      data = null;

      var messageEndToken = "#";
      // Get a stream object for reading and writing
      NetworkStream stream = this._client.GetStream();

      int i;
      // Loop to receive all the data sent by the client.
      while((i = stream.Read(bytes, 0, bytes.Length)) != 0)
      {
        // Translate data bytes to a ASCII string.
        data += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
        if (!data.Contains(messageEndToken)) {
          continue;
        }

        var remaining = data.Substring(data.IndexOf(messageEndToken) + 1);
        data = data.Substring(0, data.IndexOf(messageEndToken));

        var messageHandler = this._messageHandlers
          .Where(x => data.StartsWith(x.MessageId))
          .FirstOrDefault();

        if (messageHandler == null) {
          this._logger.LogWarning("[{0}:{1}] Unknown message received: {2}", this._endpoint.Address, this._endpoint.Port, data);
        } else {
          messageHandler.Handle(data, this._client, stream);
        }

        data = remaining;
      }
    }
  }
}