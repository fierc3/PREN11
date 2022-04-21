using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketIOClient;

namespace PiController
{
    public class ServerCommunicator
    {
        SocketIO client;
        public ServerCommunicator(string url)
        {
            var initTask = init(url);
            initTask.Wait();
        }

        private async Task init(string url)
        {
            Console.WriteLine("Setting up SocketIO Client");
            client = new SocketIO(url);
            client.OnConnected += async (sender, e) =>
            {
                Console.WriteLine("Connected To Server");
            };
            await client.ConnectAsync();
        }

        public void SendStart()
        {
            SendEvent("START");
        }

        public void SendEnd()
        {
            SendEvent("END");
        }

        private void SendEvent(string sendEvent)
        {
            Console.WriteLine("Sending " + sendEvent + " Event to Server");
            client.EmitAsync("RobotInput", response =>
                {
                // You can print the returned data first to decide what to do next.
                // output: [{"result":true,"message":"Prometheus - server"}]
                Console.WriteLine(response);
                }
                    , "{\"event_type\":\"" + sendEvent + "\"}").Wait();
        }

        public void SendPlant(string plantName)
        {
            Console.WriteLine("Sending Plant" + plantName + " to Server");
            client.EmitAsync("RobotInput", response =>
            {
                // You can print the returned data first to decide what to do next.
                // output: [{"result":true,"message":"Prometheus - server"}]
                Console.WriteLine(response);
            }
                    , "{\"event_type\":\"PLANT\", \"event_value\":{\"plantName\":\""+plantName+"\"}}").Wait();
        }
    }
}
