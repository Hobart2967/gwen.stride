using Gwen.Server.Configuration;
using Gwen.Server.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Gwen.Server {
  public class Application {
    public static void Main(string[] args) {
      var configuration = new ContainerConfiguration();
      var host = configuration.Run();
      var task = host.RunAsync();

      var server = host.Services.GetService<GameServer>()!;
      server.Configure("0.0.0.0", 4981);
      server.Listen();

      task.Wait();
    }
  }
}