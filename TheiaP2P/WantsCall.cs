using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Theia.P2P
{
    public class WantsCall
    {
        public class Client:Basic.JsonBase
        {
            public string ip;
            public int port;
            public Client(string _ip,int _port)
            {
                MsgType = 106;
                ip = _ip;
                port = _port;
            }
        }
        public class Server : Basic.JsonBase
        {
            public string ip;
            public int port;
            public Server(string _ip, int _port)
            {
                MsgType = 206;
                ip = _ip;
                port = _port;
            }
        }
    }
}
