using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Udp;
using Theia.P2P;
namespace TheiaClient
{
    public partial class VideoForm : Form
    {
        UDPSocket udpsocket = null;
        public VideoForm()
        {
            InitializeComponent();
            udpsocket = new UDPSocket(34718);
            udpsocket.SOCKETEventArrive += udpsocket_SOCKETEventArrive;
            udpsocket.StartRecvThreadListener();
            this.FormClosed += VideoForm_FormClosed;
        }

        void udpsocket_SOCKETEventArrive(System.Net.IPEndPoint endpoint, string str)
        {
            switch (Basic.JsonBase.GetMsgType(str))
            {
                case 101:
                    {

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
        int index = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            
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
            Theia.P2P.HeartBreak.Client cli = new Theia.P2P.HeartBreak.Client();
            var str = cli.ToJson();
            var ret = Theia.P2P.Basic.JsonBase.GetMsgType(str);
            MessageBox.Show(ret.ToString());
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
