﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheiaClient
{
    public static class Global
    {
        public static List<string> Global_mu38lists = new List<string>();
        public static string serverip;
        public static int serverport;

        public static string trackerip;
        public static int trackerport;
        public static int ServTick = int.MaxValue;

        public static HashSet<string> iphashset = new HashSet<string>();
    }
}
