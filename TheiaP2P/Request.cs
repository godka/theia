using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
namespace Theia.P2P
{
    public class Request
    {
        public class Client:Basic.JsonBase
        {
            //0 - login;1 - logout;2 - request
            public string RequestFileName;
            public Client(string _request = "")
            {
                MsgType = 102;  //client向tracker server请求文件
                RequestFileName = _request;
            }
        }
        public class ServerFile
        {
            public IPAddress ip;
            public int port;
            public int from;
            public int to;
            public ServerFile(IPAddress _ip,int _port,int _from,int _to){
                ip = _ip;
                port = _port;
                from = _from;
                to = _to;
            }
        }
        public class Server : Basic.JsonBase
        {
            public string FileName;
            public List<ServerFile> FileList;
            public Server()
            {
                MsgType = 202;//Tracker server向client给文件列表
                FileList = new List<ServerFile>();
            }
            public void Add(IPAddress _ip, int _port, int _from, int _to)
            {
                FileList.Add(new ServerFile(_ip, _port, _from, _to));
            }
            public int Len()
            {
                return FileList.Count;
            }
            public ServerFile Get(int index)
            {
                return FileList[index];
            }
        }
    }
}
