using System.Net.Sockets;
using Gwen.Server.MessageHandlers;
using Microsoft.Extensions.Logging;

namespace Gwen.Server.Services {
  public class PlayerHandlerFactory {
    private readonly ILogger<PlayerHandler> _logger;
    private readonly IMessageHandler[] _messageHandlers;
    private List<byte> _messageContent = new List<byte>();

    public PlayerHandlerFactory(
      IMessageHandler messageHandlers,
      ILogger<PlayerHandler> logger) {
      this._logger = logger;
      this._messageHandlers = new IMessageHandler[]{ messageHandlers };
    }

    public PlayerHandler CreateHandler(TcpClient client, GameServer server) {
      return new PlayerHandler(
      this._messageHandlers,
      this._logger, client, server);
    }

  }
}