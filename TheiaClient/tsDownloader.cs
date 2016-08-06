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
        UDPSocket _udpsocket;
        bool IsOK = false;
        public bool DownloadComplete()
        {
            return IsOK;
        }
        public string GetFileName()
        {
            if (_req != null)
                return _req.FileName;
            else
                return string.Empty;
        }
        public VideoHandler(Request.Server req, UDPSocket udpsocket)
        {
            _udpsocket = udpsocket;
            _req = req;
        }
        public void RefreshRequest()
        {
            foreach (var t in _req.FileList)
            {
                if (bytelist[t.trunk] == null)
                {
                    if (_udpsocket != null)
                    {
                        //resend request
                        FileTrans.Client cli = new FileTrans.Client(t.filename, t.trunk);
                        _udpsocket.send(t.ip, t.port, cli.ToJson());
                        if (!Global.iphashset.Contains(t.ip + ":" + t.port.ToString()))
                        {
                            WantsCall.Client wantscli = new WantsCall.Client(t.ip, t.port);
                            _udpsocket.send(Global.trackerip, Global.trackerport, wantscli.ToString());
                        }
                    }
                }
            }
        }
        private void ThreadMethod(Object obj)
        {
            if (_req.Len() == 0)
            {
                //start http downloader
                string filename = _req.FileName;

                Uri uri = new Uri(string.Format("http://{0}:{1}/{2}", Global.serverip, Global.serverport, filename));
                WebClient myWebClient = new WebClient();
                try
                {
                    myWebClient.DownloadFile(uri, "./tmp/" + filename);
                    IsOK = true;
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
                if (_udpsocket != null)
                {
                    FileTrans.Client cli = new FileTrans.Client(t.filename, t.trunk);
                    _udpsocket.send(t.ip, t.port, cli.ToJson());
                    if (!Global.iphashset.Contains(t.ip + ":" + t.port.ToString()))
                    {
                        WantsCall.Client wantscli = new WantsCall.Client(t.ip, t.port);
                        _udpsocket.send(Global.trackerip, Global.trackerport, wantscli.ToString());
                    }
                }
            }
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(ThreadMethod);
            return;
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
            IsOK = true;
        }

        public void Add(int index,byte[] data,int size)
        {
            if (bytelist[index] == null)
            {
                byte[] tmp = new byte[size];
                Array.Copy(data,tmp,size);
                bytelist[index] = tmp;
            }

            bool allok = true;
            foreach (var t in _req.FileList)
            {
                if (bytelist[t.trunk] == null)
                {
                    if (_udpsocket != null)
                    {
                        allok = false;
                        break;
                        //FileTrans.Client cli = new FileTrans.Client(t.filename, t.trunk);
                        //_udpsocket.send(t.ip, t.port, cli.ToJson());
                        //if (!Global.iphashset.Contains(t.ip + ":" + t.port.ToString()))
                        //{
                        //    WantsCall.Client wantscli = new WantsCall.Client(t.ip, t.port);
                        //    _udpsocket.send(Global.trackerip, Global.trackerport, wantscli.ToString());
                        //}
                    }
                }
            }
            if (allok && _req.FileList.Count > 0)
            {
                CombineFiles(_req.FileName);
            }
        }
    }
    public class m3u8DownloadContainer
    {
        Dictionary<string, m3u8Downloader> DownLoadList = new Dictionary<string, m3u8Downloader>();
        public void AddHandler(VideoHandler videohandler)
        {
            foreach (var t in DownLoadList)
            {
                if (t.Value.AddHandler(videohandler))
                {
                    break;
                }
            }
        }
        public void AddFileTrans(FileTrans.Server serv)
        {
            foreach (var t in DownLoadList)
            {
                var handler = GetHandlerByName(serv.filename);
                if (handler != null)
                {
                    handler.Add(serv.trunk, serv.data, serv.len);
                    //break;
                }
            }
        }
        private VideoHandler GetHandlerByName(string name)
        {
            foreach (var t in DownLoadList)
            {
                var ret = t.Value.GetHandler(name);
                if (ret != null)
                {
                    return ret;
                }
            }
            return null;
        }
        public void AddDownloader(m3u8Downloader downloader)
        {
            if (!DownLoadList.ContainsKey(downloader._m3u8file))
            {
                DownLoadList.Add(downloader._m3u8file, downloader);
                downloader.StartDownload();
            }
        }
    }
    public class m3u8Downloader
    {

        /// <summary> 
        /// 定义委托 
        /// </summary> 
        public delegate void m3u8DownloaderComplete(string m3u8filename,string filename);



        /// <summary> 
        /// 定义一个消息接收事件 
        /// </summary> 
        public event m3u8DownloaderComplete _m3u8DownloaderComplete;
        //private VideoHandler _videohandler = null;
        public string _m3u8file;
        m3u8List m3u8list;
        UDPSocket _udpsocket;
        string _request_file;
        public m3u8Downloader(string m3u8file, UDPSocket udpsocket)
        {
            _udpsocket = udpsocket;
            
            _m3u8file = m3u8file;
            var reader = new m3u8Reader(_m3u8file);
            m3u8list = reader.Parse();
        }
        public VideoHandler GetHandler(string filename)
        {
            VideoHandler _handler = null;
            foreach (var t in m3u8list.Detail)
            {
                if (t.file.Equals(filename))
                {
                    _handler = t.handler;
                    break;
                }
            }
            return _handler;
        }
        private void LoopWhileDone(string filename)
        {
            VideoHandler _handler = null;
            var time = Environment.TickCount;
            for (; ; )
            {
                if (_handler != null)
                {
                    if (_handler.DownloadComplete())
                    {
                        if (_m3u8DownloaderComplete != null)
                        {
                            _m3u8DownloaderComplete(_m3u8file,filename);
                        }
                        break;
                    }
                    else
                    {
                        var realtime = Environment.TickCount;
                        if (realtime - time >= 1000)
                        {
                            _handler.RefreshRequest();
                            time = Environment.TickCount;
                        }
                        Thread.Sleep(1);
                    }
                }
                else
                {
                    foreach (var t in m3u8list.Detail)
                    {
                        if (t.file.Equals(filename))
                        {
                            _handler = t.handler;
                            break;
                        }
                    }
                    Thread.Sleep(1);
                }
            }
        }

        public bool AddHandler(VideoHandler _handler)
        {
            if (_handler == null)
                return false;
            foreach (var t in m3u8list.Detail)
            {
                if(t.file.Equals(_handler.GetFileName()))
                {
                    if (t.handler == null)
                    {
                        t.handler = _handler;
                        t.handler.Start();
                    }
                    return true;
                }
            }
            return false;

        }

        private void ThreadMethod(object obj)
        {
            foreach (var t in m3u8list.Detail)
            {
                if (_udpsocket != null)
                {
                    //_videohandler = null;
                    Request.Client cli = new Request.Client(t.file);
                    _request_file = t.file;
                    _udpsocket.send(Global.trackerip, Global.trackerport, cli.ToString());
                    LoopWhileDone(t.file);
                }
            }
        }

        public void StartDownload()
        {
            ThreadPool.QueueUserWorkItem(ThreadMethod);

        }
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
