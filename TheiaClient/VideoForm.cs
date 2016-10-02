using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Udp;
using System.Threading;
using System.IO;
using Theia.P2P;
namespace TheiaClient
{
    public partial class VideoForm : Form
    {
        public class workclass
        {
            public System.Net.IPEndPoint endpoint;
            public string str;
        }
        List<workclass> worklist = new List<workclass>();
        m3u8DownloadContainer container = new m3u8DownloadContainer();
        UDPSocket udpsocket = null;
        private object writeobj = new object();
        public VideoForm()
        {
            if (!Directory.Exists("./tmp"))
            {
                Directory.CreateDirectory("./tmp");
            }
            if (!Directory.Exists("./swap"))
            {
                Directory.CreateDirectory("./swap");
            }
            InitializeComponent();
            udpsocket = new UDPSocket(0);
            udpsocket.SOCKETEventArrive += udpsocket_SOCKETEventArrive;
            udpsocket.SOCKETEventSend += udpsocket_SOCKETEventSend;
            udpsocket.StartRecvThreadListener();
            this.FormClosed += VideoForm_FormClosed;
            ThreadPool.QueueUserWorkItem(this.WorkThread);
            //System.Threading.Timer ClientTimer = new System.Threading.Timer(OnClientSend, this, 0, 1000);
        }

        void udpsocket_SOCKETEventSend(System.Net.IPEndPoint endpoint, string str)
        {
            var msgtype = Basic.JsonBase.GetMsgType(str);
            if (msgtype != 101)
            {
                //listBox2.Items.Add("Send " + endpoint.Address.ToString() + ":" + endpoint.Port.ToString() + " - " + str);
                // this.textBox1.Text += "Send " + endpoint.Address.ToString() + ":" + endpoint.Port.ToString() + " - " + str + "\r\n";
                SimpleDebug("Send " + endpoint.Address.ToString() + ":" + endpoint.Port.ToString() + " - " + str);
                //this.listBox2.Items.Add("Send " + endpoint.Address.ToString() + ":" + endpoint.Port.ToString() + " - " + str);
            }
            //throw new NotImplementedException();
        }
        private void OnClientSend(object obj)
        {
            if (Global.ServTick == int.MaxValue)
            {
                SendTick();
                return;
            } 
            int ticks = Environment.TickCount + Global.ServTick;
            HeartBreak.Client cli = new HeartBreak.Client(ticks);
            udpsocket.send(Global.trackerip, Global.trackerport, cli.ToJson());
        }
        private void SimpleDebug(string str)
        {
            //lock (writeobj)
            //{
            //    StreamWriter sw = new StreamWriter("./debug.txt", true);
            //    sw.WriteLine(DateTime.Now.ToString() + "-" + str);
            //    sw.Close();
            //}
        }
        private void WorkThread(object obj)
        {
            for (; ; )
            {
                if(worklist.Count == 0)
                {
                    Thread.Sleep(1);
                    continue;
                }
                var tmps = worklist[0];
                var endpoint = tmps.endpoint;
                var str = tmps.str;
                if (str == "")
                {
                    MessageBox.Show("1");
                }
                var msgtype = Basic.JsonBase.GetMsgType(str);
                if (msgtype != 201)
                {
                    //listBox2.Items.Add("From " + endpoint.Address.ToString() + ":" + endpoint.Port.ToString() + " - " + str);
                    SimpleDebug("From " + endpoint.Address.ToString() + ":" + endpoint.Port.ToString() + " - " + str);
                }
                //this.listBox2.Items.Add("From " + endpoint.Address.ToString() + ":" + endpoint.Port.ToString() + " - " + str);
                switch (msgtype)
                {
                    //for server
                    case 201:
                        {
                            //nop
                        }
                        break;
                    case 202:
                        {
                            Theia.P2P.Request.Server server = Basic.JsonBase.FromJson<Request.Server>(str);
                            VideoHandler handler = new VideoHandler(server, udpsocket);
                            container.AddHandler(handler);
                            //threadlists.Add(server.FileName, handler);

                            //server.FileName
                        }
                        break;
                    case 205:
                        {
                            TimeTick.Server serv = Basic.JsonBase.FromJson<TimeTick.Server>(str);
                            Global.ServTick = serv.Tick - Environment.TickCount;
                        }
                        break;

                    case 103:
                        {
                            FileTrans.Client cli = Basic.JsonBase.FromJson<FileTrans.Client>(str);
                            using (FileStream fs = new FileStream("./tmp/" + cli.filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                fs.Seek(cli.trunk * Basic.Common.maxsize, SeekOrigin.Begin);
                                //long len = fs.Length;
                                byte[] tmp = new byte[Basic.Common.maxsize];
                                var len = fs.Read(tmp, 0, Basic.Common.maxsize);
                                FileTrans.Server serv = new FileTrans.Server(cli.filename, cli.trunk, tmp, (int)len);
                                udpsocket.send(endpoint, serv.ToString());
                            }
                        }
                        break;
                    case 104:
                        {
                            //for client
                            FileTrans.Server serv = Basic.JsonBase.FromJson<FileTrans.Server>(str);
                            container.AddFileTrans(serv);
                            //if (threadlists.ContainsKey(serv.filename))
                            //{
                            //    threadlists[serv.filename].Add(serv.trunk,serv.data,serv.len);
                            //}
                            //using (FileStream fs = new FileStream("./swap/" + serv.filename + "." + serv.trunk, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                            //{
                            //    fs.Write(serv.data, 0, serv.len);
                            //}

                            //File.WriteAllBytes("./swap/" + serv.filename + "." + serv.trunk, serv.data,serv.len);
                        }
                        break;
                    //这个基本上属于走错片场了
                    case 105:
                        {
                            MessageBox.Show("Error Message!");
                        }
                        break;
                    case 106:
                        {
                            WantsCall.Client cli = Basic.JsonBase.FromJson<WantsCall.Client>(str);
                            if (!Global.iphashset.Contains(cli.ip + ":" + cli.port.ToString()))
                            {
                                Global.iphashset.Add(cli.ip + ":" + cli.port.ToString());
                            }
                        }
                        break;
                    case 206:
                        {
                            WantsCall.Server serv = Basic.JsonBase.FromJson<WantsCall.Server>(str);
                            if (!Global.iphashset.Contains(serv.ip + ":" + serv.port.ToString()))
                            {
                                WantsCall.Client cli = new WantsCall.Client(endpoint.Address.ToString(), endpoint.Port);
                                udpsocket.send(serv.ip, serv.port, cli.ToString());
                                Global.iphashset.Add(serv.ip + ":" + serv.port.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
                worklist.RemoveAt(0);
                
            }
        }

        void udpsocket_SOCKETEventArrive(System.Net.IPEndPoint endpoint, string str)
        {
            //WorkThread(ref endpoint, str);
            workclass t = new workclass();
            t.endpoint = endpoint;
            t.str = str;
            worklist.Add(t);
            //MessageBox.Show(str);
            //throw new NotImplementedException();
        }
        
        void VideoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
            //throw new NotImplementedException();
        }
        private void SendTick()
        {
            TimeTick.Client cli = new TimeTick.Client();
            udpsocket.send(Global.trackerip, Global.trackerport, cli.ToJson());
        }
        //int index = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            SendTick();
            //udpsocket.DisConnection();
            foreach (var t in Global.Global_mu38lists)
            {
                listBox1.Items.Add(t);
            }
            timer1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void systemToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var cli = new TimeTick.Server();
            var str = cli.ToJson();
            var ret = (TimeTick.Server)TimeTick.Server.FromJson(str);
            MessageBox.Show(ret.ToString());
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = this.listBox1.SelectedIndex;
            if (index < 0) return;
            string filename = this.listBox1.Items[index].ToString(); 
            Uri uri = new Uri(string.Format("http://{0}:{1}/{2}", Global.serverip, Global.serverport, filename));
            System.Net.WebClient myWebClient = new System.Net.WebClient();
            /*
            myWebClient.DownloadProgressChanged += myWebClient_DownloadProgressChanged;
            myWebClient.DownloadDataCompleted += myWebClient_DownloadDataCompleted;
            myWebClient.DownloadDataAsync(uri, this);
             */
            try
            {
                if (!Directory.Exists("./tmp/"))
                {
                    Directory.CreateDirectory("./tmp/");
                }
                myWebClient.DownloadFile(uri, "./tmp/" + filename);
                m3u8Downloader downloader = new m3u8Downloader("./tmp/" + filename, udpsocket);
                downloader._m3u8DownloaderComplete += downloader__m3u8DownloaderComplete;
                container.AddDownloader(downloader);
                //downloader.StartDownload();
            }
            catch
            {
                MessageBox.Show("下载失败");
            }
        }

        void downloader__m3u8DownloaderComplete(string m3u8filename,string filename)
        {
            if (!this.videoplayer1.isplaying)
            {
                this.videoplayer1.FileName = m3u8filename;
                this.videoplayer1.Play();
            }
            //throw new NotImplementedException();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            OnClientSend(this);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            this.videoplayer1.FileName = "./tmp/lipreading_640_360.m3u8";
            this.videoplayer1.Play();
        }
    }
}
