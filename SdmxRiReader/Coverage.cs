using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SdmxRiReader
{
    class Coverage
    {
        int numberOfTerritories = 0;
        int territoryLimit = 0;
        int timespanLimit = 0;

        string bestTimePeriod = "";

        public string BestTimePeriod
        {
            get { return bestTimePeriod; }
            set { bestTimePeriod = value; }
        }

        public int NumberOfTerritories
        {
            get { return numberOfTerritories; }
            set { numberOfTerritories = value; }
        }

        public int TerritoryLimit
        {
            get { return territoryLimit; }
            set { territoryLimit = value; }
        }

        public int TimespanLimit
        {
            get { return timespanLimit; }
            set { timespanLimit = value; }
        }

        public Coverage(XmlNode coverageNode)
        {
            TerritoryLimit = Int32.Parse(coverageNode.SelectSingleNode("TerritoryLowerLimit").InnerText.Trim());
            TimespanLimit = Int32.Parse(coverageNode.SelectSingleNode("Timespan").InnerText.Trim());
        }
    }
}
