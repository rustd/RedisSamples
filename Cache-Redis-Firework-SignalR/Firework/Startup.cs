using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Redis;

[assembly: OwinStartup(typeof(Firework.Startup))]

namespace Firework
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.UseRedis("fireworksdemo.redis.cache.windows.net", 6379, "Dp3Ecu9gJqvyyJ8A8XGGH0z2UfphHMEH375zBm1p1Os=", "Fireworks");
            app.MapSignalR();
        }
    }
}
