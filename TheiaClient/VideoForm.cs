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
        Dictionary<string, VideoHandler> threadlists = new Dictionary<string, VideoHandler>();
        UDPSocket udpsocket = null;
        public VideoForm()
        {
            InitializeComponent();
            udpsocket = new UDPSocket(0);
            udpsocket.SOCKETEventArrive += udpsocket_SOCKETEventArrive;
            udpsocket.StartRecvThreadListener();
            this.FormClosed += VideoForm_FormClosed; 
            System.Threading.Timer ClientTimer = new System.Threading.Timer(OnClientSend, this, 0, 1000);
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
        void udpsocket_SOCKETEventArrive(System.Net.IPEndPoint endpoint, string str)
        {
            switch (Basic.JsonBase.GetMsgType(str))
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
                        if (!threadlists.ContainsKey(server.FileName))
                        {
                            threadlists.Add(server.FileName, handler);
                            handler.Start();
                        }
                        //server.FileName
                    }
                    break;
                case 205:
                    {
                        TimeTick.Server serv = Basic.JsonBase.FromJson<TimeTick.Server>(str);
                        Global.ServTick = serv.Tick - Environment.TickCount;
                    }
                    break;

                //for client
                case 103:
                    {
                        FileTrans.Client cli = Basic.JsonBase.FromJson<FileTrans.Client>(str);
                        using (FileStream fs = new FileStream("./tmp/" + cli.filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            fs.Seek(cli.trunk * Basic.Common.maxsize, SeekOrigin.Begin);
                            byte[] tmp2 = new byte[Basic.Common.maxsize];
                            int len = fs.Read(tmp2, 0, Basic.Common.maxsize);

                            byte[] tmp = new byte[len];
                            Array.Copy(tmp2, tmp, len);

                            FileTrans.Server serv = new FileTrans.Server(cli.filename, cli.trunk, tmp);
                            udpsocket.send(endpoint, serv.ToString());
                        }
                    }
                    break;
                case 104:
                    {
                        FileTrans.Server serv = Basic.JsonBase.FromJson<FileTrans.Server>(str);
                        File.WriteAllBytes("./swap/" + serv.filename + "." + serv.trunk, serv.data);
                    }
                    break;
                //这个基本上属于走错片场了
                case 105:
                    {
                        MessageBox.Show("Error Message!");
                    }
                    break;
                case 206:
                    {
                        WantsCall.Server serv = Basic.JsonBase.FromJson<WantsCall.Server>(str);
                        udpsocket.send(serv.ip,serv.port,serv.ToString());
                    }
                    break;
            }
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
        int index = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            SendTick();
            //udpsocket.DisConnection();
            foreach (var t in Global.Global_mu38lists)
            {
                listBox1.Items.Add(t);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.hlsplayer1.FileName = "test.ts";
            this.hlsplayer1.Play(index);
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
                downloader.StartDownload();
            }
            catch
            {
                MessageBox.Show("下载失败");
            }
        }
    }
}
