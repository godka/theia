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
        public static class Common
        {
            public static int maxsize = 1024;//一个块最大30k
        }
        public class JsonBase
        {
            public virtual int MsgType { get; set; }
            public virtual void Generate()
            {

            }
            public override string ToString()
            {
                return ToJson();
                //return base.ToString();
            }
            public virtual string ToJson()
            {
                string jsonText = JsonConvert.SerializeObject(this);
                return jsonText;
            }
            public static int GetMsgType(string str)
            {
                var ret = FromJson<JObject>(str);
                return (int)ret["MsgType"];
            }
            public static T FromJson<T>(string str)
            {
                return JsonConvert.DeserializeObject<T>(str);
            }

        }
    }
}
