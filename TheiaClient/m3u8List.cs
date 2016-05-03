using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheiaClient
{
    /// <summary>
    /// m3u8list,include m3u8file's version,targetmaxtime,and m3u8detail[] with filename,timespan,bandwidth.
    /// </summary>
    public class m3u8List
    {
        public class m3u8Detail
        {
            private string _tsfile;
            private double _timespan;
            private int _bandwidth;
            public string file
            {
                get
                {
                    return _tsfile;
                }
            }
            public double timespan
            {
                get
                {
                    return _timespan;
                }
            }
            public int bandwidth
            {
                get
                {
                    return _bandwidth;
                }
            }
            public m3u8Detail(string file, double timespan,int bandwidth)
            {
                _tsfile = file;
                _timespan = timespan;
                _bandwidth = bandwidth;
            }
        }
        public int VERSION
        {
            get;
            set;
        }
        public int TARGETDURATION
        {
            get;
            set;
        }
        public List<m3u8Detail> Detail;
        public m3u8List()
        {
            Detail = new List<m3u8Detail>();
        }
    }
}
