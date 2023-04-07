using System.Net.Sockets;

namespace Gwen.Server.MessageHandlers {
  public interface IMessageHandler {
    public string MessageId { get; }

    void Handle(string data, TcpClient client, NetworkStream stream);
  }
}