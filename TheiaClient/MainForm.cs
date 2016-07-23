using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Threading;
using imzopr;
namespace TheiaClient
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }
        public void DownLoadListFromInternet(string remoteUri)
        {
            Uri uri = new Uri(remoteUri);
            WebClient myWebClient = new WebClient();
            /*
            myWebClient.DownloadProgressChanged += myWebClient_DownloadProgressChanged;
            myWebClient.DownloadDataCompleted += myWebClient_DownloadDataCompleted;
            myWebClient.DownloadDataAsync(uri, this);
             */
            try
            {
                var tmp = myWebClient.DownloadData(uri);
                var reader = new inireader(new MemoryStream(tmp));
                var num = reader.ReadIniInt("list", "num");
                for (int i = 0; i < num; i++)
                {
                    var file = reader.ReadIniString("list", "list" + i.ToString());
                    Global.Global_mu38lists.Add(file);
                    //frm.listBox1.Items.Add(file);
                    //frm.DownLoadm3u8FromInternet(file);
                }
            }
            catch
            {
                MessageBox.Show("连接失败！");
            }
        }
        //public void DownLoadm3u8FromInternet(string m3u8file)
        //{
        //    if (!Directory.Exists("tmp"))
        //        Directory.CreateDirectory("tmp");
        //    var simple_ip = this.textBox1.Text;
        //    var simple_port = this.textBox2.Text;
        //    var remoteUri = string.Format("http://{0}:{1}/{2}", simple_ip, simple_port, m3u8file);
        //    Uri uri = new Uri(remoteUri);
        //    WebClient myWebClient = new WebClient();
        //    myWebClient.DownloadProgressChanged += myWebClient_DownloadProgressChanged;
        //    myWebClient.DownloadDataCompleted += myWebClient_DownloadFileCompleted;
        //    myWebClient.DownloadFileAsync(uri, "tmp/" + m3u8file,this);
        //}

        //void myWebClient_DownloadFileCompleted(object sender, DownloadDataCompletedEventArgs e)
        //{
        //    MainForm frm = (MainForm)e.UserState;
        //    Application.DoEvents();
        //}
        static void myWebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            MainForm frm = (MainForm)e.UserState;
            if (e.Error == null && e.Cancelled == false)
            {
                Application.DoEvents();
                byte[] tmp = e.Result;
                var reader = new inireader(new MemoryStream(tmp));
                var num = reader.ReadIniInt("list", "num");
                for (int i = 0; i < num; i++)
                {
                    var file = reader.ReadIniString("list", "list" + i.ToString());
                    Global.Global_mu38lists.Add(file);
                    //frm.listBox1.Items.Add(file);
                    //frm.DownLoadm3u8FromInternet(file);
                }
            }
            else
            {
                MessageBox.Show(e.Error.ToString());
            }
            frm.button1.Enabled = true;
        }
        static void myWebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            MainForm frm = (MainForm)e.UserState;
            //frm.progressBar1.Maximum = 100;
            //frm.progressBar1.Value = e.ProgressPercentage;
           // frm.label3.Text = string.Format("downloaded {0} of {1} bytes. {2} % complete...",
           //     e.BytesReceived,
           //     e.TotalBytesToReceive,
           //     e.ProgressPercentage);
            Application.DoEvents();
        }
        void startDownLoad()
        {
            var simple_ip = this.textBox1.Text;
            var simple_port = this.textBox2.Text;
            var simple_url = string.Format("http://{0}:{1}/index.txt", simple_ip, simple_port);
            Global.serverip = simple_ip;
            Global.serverport = int.Parse(simple_port);
            Global.trackerip = this.textBox3.Text;
            Global.trackerport = int.Parse(this.textBox4.Text);
            DownLoadListFromInternet(simple_url);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            //listBox1.Items.Clear();
            button1.Enabled = false;
            //START DOWNLOAD
            startDownLoad();
            //thread = new Thread(startDownLoad);
            //thread.Start();
            VideoForm videoform = new VideoForm();
            videoform.Show();
            Hide();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }
    }
}
