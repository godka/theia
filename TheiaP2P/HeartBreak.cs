using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Theia.P2P
{
    public class HeartBreak
    {
        public class ClientFile
        {
            public List<int> ClientList;
            public string FileName;
            public ClientFile(string file)
            {
                FileName = file;
                ClientList = new List<int>();
            }
            public void Add(int id)
            {
                if (ClientList.Contains(id))
                    return;
                ClientList.Add(id);
            }
            public void Remove(int id)
            {
                if (ClientList.Contains(id))
                {
                    ClientList.Remove(id);
                }
            }
            public int[] ToArray()
            {
                return ClientList.ToArray();
            }
            public int Get(int index)
            {
                return ClientList[index];
            }
            public int Len()
            {
                return ClientList.Count;
            }
        }
        public class Client : Basic.JsonBase
        {
            public int ClientID;
            public List<ClientFile> files;
            public Client(int id = 0)
            {
                MsgType = 101;
                ClientID = id;
                files = new List<ClientFile>(); string tmpDir = "./tmp";
                files.Clear();
                if (Directory.Exists(tmpDir))
                {
                    var _files = Directory.GetFiles(tmpDir);
                    foreach (var file in _files)
                    {
                        var simplefile = new FileInfo(file);
                        files.Add(new ClientFile(simplefile.Name));
                    }
                }
            }
            public virtual void AddFile(string filename)
            {
                if (File.Exists("./tmp/" + filename))
                {
                    var simplefile = new FileInfo(filename);
                    foreach (var tmp in files)
                    {
                        if (tmp.FileName.Equals(simplefile.Name))
                            return;
                    }
                    files.Add(new ClientFile(simplefile.Name));
                }

            }
            public override void Generate()
            {

            }
        }
        public class Server : Basic.JsonBase
        {
            public int OK;
            public Server()
            {
                MsgType = 201;
                OK = 1;
            }
        }

    }
}
