using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Theia.P2P;
using System.Net.Udp;
using System.IO;
using System.Net;
namespace TheiaClient
{
    public class VideoHandler
    {
        List<byte[]> bytelist = new List<byte[]>();
        Request.Server _req;
        Thread CheckThread;
        bool isrunning = false;
        UDPSocket _udpsocket;
        public VideoHandler(Request.Server req, UDPSocket udpsocket)
        {
            _udpsocket = udpsocket;
            _req = req;
        }

        public void Start()
        {
            isrunning = true;
            CheckThread = new Thread(new ParameterizedThreadStart(LoopThread));
            CheckThread.Start();
        }
        private bool CheckFile(string filename)
        {
            if (File.Exists("./swap/" + filename))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void CombineFiles(string filename)
        {
            List<byte> tmpwrite = new List<byte>();
            foreach (var t in bytelist)
            {
                if(t != null)
                    tmpwrite.AddRange(t);
            }
            File.WriteAllBytes("./tmp/" + filename, tmpwrite.ToArray());
        }
        public void Add(int index,byte[] data,int size)
        {
            if (bytelist[index] == null)
            {
                byte[] tmp = new byte[size];
                Array.Copy(data,tmp,size);
                bytelist[index] = tmp;
            }
        }
        private void LoopThread(object obj)
        {
            if (_req.Len() == 0)
            {
                //start http downloader
                string filename = _req.FileName;

                Uri uri = new Uri(string.Format("http://{0}:{1}/{2}", Global.serverip,Global.serverport,filename));
                WebClient myWebClient = new WebClient();
                /*
                myWebClient.DownloadProgressChanged += myWebClient_DownloadProgressChanged;
                myWebClient.DownloadDataCompleted += myWebClient_DownloadDataCompleted;
                myWebClient.DownloadDataAsync(uri, this);
                 */
                try
                {
                    myWebClient.DownloadFile(uri, "./tmp/" + filename);
                    return;

                }
                catch
                {
                    return;
                }
            }
            foreach (var t in _req.FileList)
            {
                bytelist.Add(null);
            }
            while (isrunning)
            {
                bool allok = true;
                foreach (var t in _req.FileList)
                {
                    if (bytelist[t.trunk] == null)
                    {
                        if (_udpsocket != null)
                        {
                            allok = false;
                            FileTrans.Client cli = new FileTrans.Client(t.filename, t.trunk);
                            _udpsocket.send(t.ip, t.port, cli.ToJson());
                            WantsCall.Client wantscli = new WantsCall.Client(t.ip, t.port);
                            _udpsocket.send(Global.trackerip, Global.trackerport, wantscli.ToString());
                        }
                    }
                }
                if (allok)
                {
                    CombineFiles(_req.FileName);
                    break;
                }
                Thread.Sleep(5000);
            }
        }
    }
    public class m3u8Downloader
    {
        string _m3u8file;
        m3u8List m3u8list;
        UDPSocket _udpsocket;
        public m3u8Downloader(string m3u8file, UDPSocket udpsocket)
        {
            _udpsocket = udpsocket;
            _m3u8file = m3u8file;
            var reader = new m3u8Reader(_m3u8file);
            m3u8list = reader.Parse();
        }
        public void StartDownload()
        {
            foreach (var t in m3u8list.Detail)
            {
                if (_udpsocket != null)
                {
                    Request.Client cli = new Request.Client(t.file);
                    _udpsocket.send(Global.trackerip, Global.trackerport, cli.ToString());
                    
                }
            }
            //isrunning = true;
            //CheckThread = new Thread(new ParameterizedThreadStart(LoopThread));
            //CheckThread.Start();
        }
        /*
        private void LoopThread(object obj)
        {
            while (isrunning)
            {
                Thread.Sleep(1000);
            }
        }
         */
    }
    public class tsDownloader
    {
        private string _tsfile;
        public tsDownloader(string tsfile)
        {
            _tsfile = tsfile;
        }

    }
}
