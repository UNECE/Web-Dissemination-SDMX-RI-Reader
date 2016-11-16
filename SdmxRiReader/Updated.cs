using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdmxRiReader
{
    class Updated
    {
        private string id = "";
        private string date = "";
        private List<JsonLanguage> languages = new List<JsonLanguage>();

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        public List<JsonLanguage> Languages
        {
            get { return languages; }
            set { languages = value; }
        }
    }
}
