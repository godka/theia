﻿using System;
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
            public int trunk;
            public Client(string _filename,int _trunk)
            {
                MsgType = 103;  //client 1问client2要东西
                filename = _filename;
                trunk = _trunk;
            }
        }

        public class Server : Basic.JsonBase
        {
            public string filename;
            public byte[] data;
            public int trunk;
            public Server(string _filename,int id,byte[] _data)
            {
                MsgType = 104;
                filename = _filename;
                data = _data;
                trunk = id;
            }
        }
    }
}
