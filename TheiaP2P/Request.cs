using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Theia.P2P
{
    public class Request
    {
        public class Client:Basic.JsonBase
        {
            //0 - login;1 - logout;2 - request
            public int MsgType;
            public string RequestFileName;
            public Client(int _msgtype,string _request = "")
            {
                MsgType = _msgtype;
                RequestFileName = _request;
            }
        }
    }
}
