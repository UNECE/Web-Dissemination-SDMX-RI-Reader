using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdmxRiReader
{
    class Observation
    {
        private string indicator = "";
        private string territory = "";
        private string timespan = "";
        private string value = "";

        public string Indicator
        {
            get { return indicator; }
            set { indicator = value; }
        }

        public string Territory
        {
            get { return territory; }
            set { territory = value; }
        }

        public string Timespan
        {
            get { return timespan; }
            set { timespan = value; }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

    }
}
