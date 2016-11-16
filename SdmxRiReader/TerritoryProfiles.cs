using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SdmxRiReader
{
    class TerritoryProfiles
    {
        private List<Territory> territories = new List<Territory>();

        internal List<Territory> Territories
        {
            get { return territories; }
            set { territories = value; }
        }

        public TerritoryProfiles(XmlNode nodeTerritoryProfiles)
        {
            XmlNodeList nodeTerritoryProfilesList = nodeTerritoryProfiles.SelectNodes("Territory");

            foreach(XmlNode nodeTerritoryProfile in nodeTerritoryProfilesList) {
                Territory thisTerritory = new Territory(nodeTerritoryProfile);
                territories.Add(thisTerritory);
            }
        }
    }
}
