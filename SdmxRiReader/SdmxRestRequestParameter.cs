using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SdmxRiReader
{
    class SdmxRestRequestParameter
    {
        private string name = "";
        private string value = "";

        public SdmxRestRequestParameter(XmlNode requestParameterNode)
        {
            this.Name = requestParameterNode.SelectSingleNode("Name").InnerText.Trim();
            this.Value = requestParameterNode.SelectSingleNode("Value").InnerText.Trim();
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

    }
}
