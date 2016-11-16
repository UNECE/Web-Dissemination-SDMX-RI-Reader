using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SdmxRiReader
{
    class Geo
    {
        private string dimensionId                  = "";
        private string conceptId                    = "";
        private List<Territory> territories         = new List<Territory>();

        public string ConceptId
        {
            get { return conceptId; }
            set { conceptId = value; }
        }

        internal List<Territory> Territories
        {
            get { return territories; }
            set { territories = value; }
        }

        public string DimensionId
        {
            get { return dimensionId; }
            set { dimensionId = value; }
        }

        public Geo(XmlNode geoNode)
        {
            this.DimensionId = geoNode.SelectSingleNode("DimensionId").InnerText.Trim();
        }

        public Geo()
        {
        }

        public Boolean Found(Territory thisTerritory)
        {
            foreach (Territory myTerritory in Territories) {
                if (myTerritory.Code.Equals(thisTerritory.Code))
                {
                    // territory is found in the list
                    return true;
                }
            }

            // territory is not found in the list
            return false;
        }
    }
}

