using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Redis;
using System.Configuration;

[assembly: OwinStartup(typeof(Firework.Startup))]

namespace Firework
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.UseRedis(ConfigurationManager.AppSettings["mCacheUrl"], 6379, ConfigurationManager.AppSettings["mCachePassword"], "Fireworks");
            app.MapSignalR();
        }
    }
}
