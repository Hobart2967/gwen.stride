using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gwen.Server.Services;
using Gwen.Server.MessageHandlers;

namespace Gwen.Server.Configuration {
  public class ContainerConfiguration {
    public IHost Run() {
      var builder = Host.CreateDefaultBuilder(null);

      builder.ConfigureServices(services =>
        services
          .AddSingleton<GameServer>()
          .AddSingleton<PlayerHandlerFactory>()
          .AddSingleton<IMessageHandler, PlayerChatMessageHandler>()
        );

      var host = builder.Build();

      return host;
    }
  }
}