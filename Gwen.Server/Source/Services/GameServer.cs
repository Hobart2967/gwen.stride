using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gwen.Server.Services {
  public class GameServer {
    private readonly ILogger _logger;
    private readonly PlayerHandlerFactory _playerHandlerFactory;
    private readonly List<PlayerHandler> _handlers = new List<PlayerHandler>();
    private string? _host;
    private int? _port;

    public GameServer(
      ILogger<GameServer> logger,
      PlayerHandlerFactory playerHandlerFactory) {
      this._logger = logger;
      this._playerHandlerFactory = playerHandlerFactory;
    }

    public void Configure(string host, int port) {
      this._host = host;
      this._port = port;

      this._logger.LogInformation( "Configured GameServer to listen at {0}:{1}", this._host, this._port);
    }

    public void Listen() {

      var server = new TcpListener(IPAddress.Parse(this._host!), this._port!.Value);
      server.Start();
      this._logger.LogInformation("Listening at {0}:{1}", this._host, this._port);

      // Enter the listening loop.
      while (true) {
        using TcpClient client = server.AcceptTcpClient();

        var handler = this._playerHandlerFactory.CreateHandler(client, this);
        this._handlers.Add(handler);
        handler.Handle();

        Console.WriteLine("Connected!");
      }
    }
  }
}