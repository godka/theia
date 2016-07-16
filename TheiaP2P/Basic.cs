using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Theia.P2P
{
    public class Basic
    {
        public class JsonBase
        {
            public virtual int MsgType { get; set; }
            public virtual void Generate()
            {

            }
            public virtual string ToJson()
            {
                string jsonText = JsonConvert.SerializeObject(this);
                return jsonText;
            }
            public static int GetMsgType(string str)
            {
                var ret = (JObject)FromJson(str);
                return (int)ret["MsgType"];
            }
            public static object FromJson(string str)
            {
                return JsonConvert.DeserializeObject(str);
            }
        }
    }
}
