using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Theia.P2P
{
    public class FileTrans
    {
        public class Client : Basic.JsonBase
        {
            public string filename;
            public int from;
            public int to;
            public Client(string _filename,int _from,int _to)
            {
                MsgType = 103;  //client 1问client2要东西
                filename = _filename;
                from = _from;
                to = _to;
            }
        }

        public class Server : Basic.JsonBase
        {
            public string filename;
            public byte[] data;
            public Server(string _filename,byte[] _data)
            {
                MsgType = 104;
                filename = _filename;
                data = _data;
            }
        }
    }
}
