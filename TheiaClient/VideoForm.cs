using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TheiaClient
{
    public partial class VideoForm : Form
    {
        public VideoForm()
        {
            InitializeComponent();
        }
        int index = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            Theia.P2P.HeartBreak.Client cli = new Theia.P2P.HeartBreak.Client();
            cli.Generate();
            MessageBox.Show(cli.ToJson());
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

            index += 1000;
            this.hlsplayer1.VideoTime = index;
        }
    }
}
