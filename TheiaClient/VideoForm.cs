﻿using System;
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
                case 101:
                    {

                    }
                    break;
                case 205:
                    {
                        TimeTick.Server serv = Basic.JsonBase.FromJson<TimeTick.Server>(str);
                        Global.ServTick = serv.Tick - Environment.TickCount;
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
    }
}
