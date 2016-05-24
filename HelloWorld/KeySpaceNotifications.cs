using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorld
{
    public static class KeySpaceNotifications
    {
        /// <summary>
        /// NOTE: For this sample to work, you need to go to the Azure Portal and configure keyspace notifications with "Kxg" to
        /// turn on expiration notifications (x) and general command notices (g).
        /// IMPORTANT - MAKE SURE YOU UNDERSTAND THE PERFORMANCE IMPACT OF TURNING ON KEYSPACE NOTIFICATIONS BEFORE PROCEEDING
        /// See http://redis.io/topics/notifications for more details
        public static void Run()
        {
            IDatabase connection = Helper.Connection.GetDatabase();
            var subscriber = Helper.Connection.GetSubscriber();
            int db = 0; //what Redis DB do you want notifications on?
            string notificationChannel = "__keyspace@" + db + "__:*";

            //you only have to do this once, then your callback will be invoked.
            subscriber.Subscribe(notificationChannel, (channel, notificationType) =>
            {
                var key = GetKey(channel);
                switch (notificationType)
                {
                    case "expire":
                        Console.WriteLine("Expiration Set for Key: " + key);
                        break;
                    case "expired":
                        Console.WriteLine("Expiration hit for Key: " + key);
                        break;
                }
            });

            Console.WriteLine("Subscribed to notifications...");

            // Demo Setup
            DemoSetup(connection);            
        }

        private static void DemoSetup(IDatabase connection)
        {
            for (int i = 0; i < 10; i++)
            {
                connection.KeyDelete("foo" + i);
            }

            //add some keys that will expire to test the above callback that I configured.
            var random = new Random();
            for (int i = 0; i < 10; i++)
            {
                var expiry = TimeSpan.FromMilliseconds(random.Next(1, 5));
                connection.StringSet("foo" + i, "bar", expiry);

            }
        }

        private static string GetKey(string channel)
        {
            var index = channel.IndexOf(':');
            if (index >= 0 && index < channel.Length - 1)
                return channel.Substring(index + 1);

            //we didn't find the delimeter, so just return the whole thing
            return channel;
        }
    }
}
