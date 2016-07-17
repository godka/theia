using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Udp;
using System.Net;
using Theia.P2P;
namespace TheiaServer
{
    public partial class Form1 : Form
    {
        UDPSocket udpsocket = null;
        Dictionary<string, HeartBreak.Client> clientlist;
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            clientlist = new Dictionary<string, HeartBreak.Client>();
            System.Threading.Timer ClientCloseTimer = new System.Threading.Timer(OnClientClose, this, 0, 1000);
            //IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
            //MessageBox.Show(ip.ToString());
        }
        private void OnRefreshListbox()
        {
            foreach (var t in clientlist)
            {
                if (!listBox1.Items.Contains(t.Key))
                {
                    listBox1.Items.Add(t.Key);
                }
            }
            foreach (var t in listBox1.Items)
            {
                //IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse)
                if (!clientlist.ContainsKey(t.ToString()))
                {
                    listBox1.Items.Remove(t.ToString());
                }
            }
        }
        private void OnClientClose(object obj)
        {
            //get max value
            var max = int.MinValue;
            foreach (var t in clientlist.Values)
            {
                if (t.TickCount > max)
                {
                    max = t.TickCount;
                }
            }

            //shut down the false positive client

            foreach (var t in clientlist)
            {
                var value = t.Value;
                if (value.TickCount < (max - 5000))//小于5秒的基本算失联了
                {
                    clientlist.Remove(t.Key);
                }
            }
            OnRefreshListbox();
        }
        void udpsocket_SOCKETEventArrive(System.Net.IPEndPoint endpoint, string str)
            {
            //throw new NotImplementedException();
            switch (Basic.JsonBase.GetMsgType(str))
            {
                case 101:
                    {
                        //heartbreak;
                        HeartBreak.Client cli = Basic.JsonBase.FromJson<HeartBreak.Client>(str);
                        if (clientlist.ContainsKey(endpoint.ToString()))
                        {
                            clientlist[endpoint.ToString()] = cli;
                        }
                        else
                        {
                            clientlist.Add(endpoint.ToString(), cli);
                        }
                        //if(clientlist.)
                    }
                    break;
                case 102:
                    {

                    }
                    break;
                case 105:
                    {
                        TimeTick.Server server = new TimeTick.Server();
                        udpsocket.send(endpoint, server.ToJson());
                    }
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            this.button2.Enabled = true;
            udpsocket = new UDPSocket(23583);
            udpsocket.SOCKETEventArrive += udpsocket_SOCKETEventArrive;
            udpsocket.StartRecvThreadListener();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.button1.Enabled = true;
            this.button2.Enabled = false;
            if (udpsocket != null)
            {
                udpsocket.DisConnection();
            }
        }
    }
}
