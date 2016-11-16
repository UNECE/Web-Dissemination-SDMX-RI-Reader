using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SdmxRiReader
{
    class FileDownloader
    {
        public Boolean DownloadFile(SdmxRestRequest sdmxRestRequest, string tempSdmxFilePath)
        {
            string strBaseUrl = sdmxRestRequest.BaseUrl;
            string strRequestString = sdmxRestRequest.RequestString;

            // start downloading
            try
            {
                using (var writer = File.OpenWrite(tempSdmxFilePath))
                {
                    var client = new RestClient(strBaseUrl);
                    var restRequest = new RestRequest(strRequestString, Method.GET);

                    // add parameters
                    foreach (SdmxRestRequestParameter requestParameter in sdmxRestRequest.RequestParameters)
                    {
                        restRequest.AddParameter(requestParameter.Name, requestParameter.Value, ParameterType.UrlSegment);
                    }

                    // set timeout
                    restRequest.Timeout = 1800 * 1000;

                    // flush data into the file
                    restRequest.ResponseWriter = (responseStream) => responseStream.CopyTo(writer);

                    // execute restful request
                    var response = client.Execute(restRequest);

                    var httpStatusCode = response.StatusCode;
                    var responseStatus = response.ResponseStatus;
                    var responseUri = response.ResponseUri;

                    // we need to check the status if it is OK or not. No exceptions are thrown.
                    if (httpStatusCode == HttpStatusCode.NotFound || httpStatusCode == HttpStatusCode.Unauthorized)
                    {
                        App.AppendRecordToLog("Not found");
                        return false;
                    }
                    if (httpStatusCode != HttpStatusCode.OK)
                    {
                        App.AppendRecordToLog(response.StatusDescription);
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                App.AppendRecordToLog(ex.Message);
                return false;
            }

            return true;
        }
    }
}
