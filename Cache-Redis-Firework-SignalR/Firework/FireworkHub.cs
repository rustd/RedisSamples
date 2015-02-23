using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Firework
{
    [HubName("fireworkHub")]
    public class FireworkHub : Hub
    {
        public void Add(int type, double x, double y, string color, int tail)
        {
            Clients.All.addFirework(
                new
                {
                    Type = type,
                    X = x,
                    Y = y,
                    Color = color,
                    TailType = tail
                }
                );
        }
        public FireworkHub()
        {

        }
    }
}