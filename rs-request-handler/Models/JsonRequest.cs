using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using System.Net.Http.Json;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace rs_request_handler.Models {
    public class JsonRequest {
        public class DocumentRequest {
            public Header Header { get; set; }
        }

        public class Header {
            public string RequestType { get; set; }
            public string LetterDate { get; set; }
            public string OutputFileName { get; set; }
            public Keys Keys { get; set; }
        }

        public class Keys {
            public string value_date { get; set; }
            public string investor_name { get; set; }
            public string rep_ccy { get; set; }
            public string int_party { get; set; }
        }

        public class Root {
            public DocumentRequest DocumentRequest { get; set; }
        }

        public XmlDocument convertToXml(string jsonIn) {
            XmlDocument xml = JsonConvert.DeserializeXmlNode(jsonIn);
            return xml;

        }

        private string toJson() {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
