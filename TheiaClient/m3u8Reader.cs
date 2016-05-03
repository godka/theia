using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace TheiaClient
{
    public class m3u8Reader
    {
        string _filename;
        public m3u8Reader(string filename)
        {
            _filename = filename;
        }
        /// <summary>
        /// parse m3u8 to m3u8list
        /// </summary>
        /// <returns>simple m3u8list</returns>
        public m3u8List Parse()
        {
            var allStr = File.ReadAllLines(_filename, Encoding.Default);
            string lastread = string.Empty;
            m3u8List retlist = new m3u8List();
            int tmp = 0;
            for (int i = 0; i < allStr.Length; i++)
            {
                var singlestr = allStr[i];
                var splitstr = singlestr.Split(':');
                if (splitstr.Length > 1)
                {
                    switch (splitstr[0])
                    {
                        case "#EXT-X-VERSION":
                            tmp = 0;
                            int.TryParse(splitstr[1], out tmp);
                            retlist.VERSION = tmp;
                            break;
                        case "#EXT-X-TARGETDURATION":
                            tmp = 0;
                            int.TryParse(splitstr[1], out tmp);
                            retlist.TARGETDURATION = tmp;
                            break;
                        case "#EXTINF":
                            double timetmp = 0.0f;
                            tmp = 0;
                            //double.TryParse(splitstr[1].Replace(",",""), out timetmp);
                            var timespit = splitstr[1].Split(',');
                            double.TryParse(timespit[0], out timetmp);
                            int.TryParse(timespit[1], out tmp);
                            string detailfile = string.Empty;
                            //read next line if necessary
                            if (i < allStr.Length - 1)
                            {
                                detailfile = allStr[++i];
                            }

                            m3u8List.m3u8Detail singledetail =
                                new m3u8List.m3u8Detail(detailfile, timetmp,tmp);
                            retlist.Detail.Add(singledetail);
                            break;
                    }
                }
            }
            return retlist;
        }
        public void SingleStep()
        {

        }
    }
}
