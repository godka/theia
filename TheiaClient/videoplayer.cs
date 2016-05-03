using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace TheiaClient
{
    /*
     Private Declare Function BigPotInit Lib "BigPot.dll" (ByVal handle As Long) As Long
Private Declare Function BigPotInputVideo Lib "BigPot.dll" (ByVal handle As Long, ByVal name As String) As Long
Private Declare Function BigPotSeek Lib "BigPot.dll" (ByVal handle As Long, ByVal seektime As Long) As Long
Private Declare Function BigPotClose Lib "BigPot.dll" (ByVal handle As Long) As Long

     */
    public partial class videoplayer : UserControl
    {
        [DllImport("BigPot.dll")]
        private extern static int BigPotInit(IntPtr handle);
        [DllImport("BigPot.dll")]
        private extern static int BigPotInputVideo(int handle,char[] filename,int seektime);
        [DllImport("BigPot.dll")]
        private extern static int BigPotSeek(int handle, int seektime);
        [DllImport("BigPot.dll")]
        private extern static int BigPotClose(int handle, char[] filename);
        bool isplaying;
        public delegate void OnVideoTimeChanged(object sender, int videotime, int totaltime);
        public delegate void OnVideoStop(object sender);
        char[] _char_filename;
        string _filename;
        int handle;
        public string FileName
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;
                _char_filename = ConvertToCppChar(_filename);
            }
        }
        public int VideoTime
        {
            get
            {
                return _videotime;
            }
            set
            {
                _videotime = value;
                SetSeekTime(_videotime);
            }
        }
        int _videotime;
        public int TotalTime
        {
            get;
            set;
        }
        public videoplayer()
        {
            InitializeComponent();
        }
        public void Play(int seektime = 0)
        {
            if(handle == 0)
                handle = BigPotInit(this.Handle);

            if (isplaying)
                Stop();
            isplaying = true;
            if (FileName.Equals(string.Empty))
                return;
            BigPotInputVideo(handle, _char_filename, seektime);
        }

        public void Stop()
        {
            BigPotClose(handle,_char_filename);
            isplaying = false;
        }
        private void SetSeekTime(int seektime)
        {

        }
        private void hlsplayer_Load(object sender, EventArgs e)
        {
            handle = 0;
            isplaying = false;
        }
        public char[] ConvertToCppChar(string str)
        {
            if (str == null) { return null; }
            char[] tmpchar = str.ToCharArray();
            byte[] buf = System.Text.Encoding.Default.GetBytes(tmpchar);
            char[] aC = System.Text.Encoding.Default.GetChars(buf);
            return aC;
        }
    }
}
