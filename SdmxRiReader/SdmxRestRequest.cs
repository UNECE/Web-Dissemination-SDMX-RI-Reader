using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SdmxRiReader
{
    class SdmxRestRequest
    {
        private string baseUrl                                      = "";
        private string requestString                                = "";
        private List<SdmxRestRequestParameter> requestParameters    = new List<SdmxRestRequestParameter>();

        public SdmxRestRequest(XmlNode restfulWebService)
        {
            this.BaseUrl = restfulWebService.SelectSingleNode("BaseUrl").InnerText.Trim();
            this.RequestString = restfulWebService.SelectSingleNode("RequestString").InnerText.Trim();

            XmlNodeList RequestParameterNodes = restfulWebService.SelectSingleNode("RequestParameters").SelectNodes("RequestParameter");

            foreach (XmlNode requestParameterNode in RequestParameterNodes)
            {
                SdmxRestRequestParameter requestParameter = new SdmxRestRequestParameter(requestParameterNode);
                requestParameters.Add(requestParameter);
            }
        }

        public string BaseUrl
        {
            get { return baseUrl; }
            set { baseUrl = value; }
        }

        public string RequestString
        {
            get { return requestString; }
            set { requestString = value; }
        }

        internal List<SdmxRestRequestParameter> RequestParameters
        {
            get { return requestParameters; }
            set { requestParameters = value; }
        }
    }
}


