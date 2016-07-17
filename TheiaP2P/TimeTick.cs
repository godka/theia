using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Theia.P2P
{
    public class TimeTick
    {
        public class Client : Basic.JsonBase
        {
            public Client()
            {
                MsgType = 105;
            }
        }
        public class Server : Basic.JsonBase
        {
            public int Tick{get;set;}
            public Server()
            {
                MsgType = 205;
                Tick = Environment.TickCount;
            }
            public static Server FromJson(string str)
            {
                return (Server)Newtonsoft.Json.JsonConvert.DeserializeObject(str);
            }
        }
    }
}
