using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Theia.P2P
{
    public class Basic
    {
        public class JsonBase
        {
            public virtual void Generate()
            {

            }
            public JsonBase()
            {
            }
            public virtual string ToJson()
            {
                string jsonText = JsonConvert.SerializeObject(this);
                return jsonText;
            }
        }
    }
}
