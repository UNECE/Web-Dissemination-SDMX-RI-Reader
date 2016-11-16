using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SdmxRiReader
{
    class Language
    {
        private string code = "";
        private string name = "";
        private string longCode = "";
        private string defaultLanguage = "";

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        public string LongCode
        {
            get { return longCode; }
            set { longCode = value; }
        }

        public string DefaultLanguage
        {
            get { return defaultLanguage; }
            set { defaultLanguage = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Language(XmlNode languageNode)
        {
            this.Code = languageNode.SelectSingleNode("Code").InnerText.Trim();
            this.LongCode = languageNode.SelectSingleNode("LongCode").InnerText.Trim();
            this.DefaultLanguage = languageNode.SelectSingleNode("DefaultLanguage").InnerText.Trim();
        }

        public Language(string code, string name)
        {
            this.Code = code;
            this.name = name;
        }

    }
}
