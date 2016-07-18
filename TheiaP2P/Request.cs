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
            public string ip;
            public int port;
            public int trunk;
            public string filename;
            public ServerFile(string _ip,int _port,int _trunk){
                ip = _ip;
                port = _port;
                trunk = _trunk;
            }
        }
        public class Server : Basic.JsonBase
        {
            public string FileName;
            public long Filelen;
            public List<ServerFile> FileList;
            public Server()
            {
                MsgType = 202;//Tracker server向client给文件列表
                FileList = new List<ServerFile>();
            }
            public void Add(string _ip, int _port,int _trunk)
            {
                FileList.Add(new ServerFile(_ip, _port, _trunk));
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
