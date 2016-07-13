using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
namespace Theia.P2P
{
    public class HeartBreak
    {
        public class Client : JsonBase
        {
            public Client()
            {

            }
            public override void Generate()
            {
                //File
                //base.Generate();
            }

        }
        public class JsonBase
        {
            StringWriter sw = null;
            JsonWriter writer = null;
            public virtual void Generate()
            {

            }
            public JsonBase()
            {
                sw = new StringWriter();
                writer = new JsonTextWriter(sw);
                writer.WriteStartObject();
            }
            public void WriteKeyValue(string key, string value)
            {
                writer.WritePropertyName(key);
                writer.WriteValue(value);
            }
            public void WriteKeyValue(string key, int value)
            {
                writer.WritePropertyName(key);
                writer.WriteValue(value);
            }
            public virtual string ToJson()
            {
                writer.WriteEndObject();
                string jsonText = sw.GetStringBuilder().ToString();
                return jsonText;
            }
        }
    }
}
