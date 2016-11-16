using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SdmxRiReader
{
    class Domain
    {
        private string uniqueId = "";

        private List<QuickStatsItem> quickStatsItemsList = new List<QuickStatsItem>();
        private List<Language> translationsList = new List<Language>();

        public string UniqueId
        {
            get { return uniqueId; }
            set { uniqueId = value; }
        }

        internal List<QuickStatsItem> QuickStatsItemsList
        {
            get { return quickStatsItemsList; }
            set { quickStatsItemsList = value; }
        }

        public string getName(string languageCode)
        {
            foreach (Language thisLanguage in translationsList)
            {
                if (thisLanguage.Code.ToUpper().Equals(languageCode.ToUpper()))
                {
                    return thisLanguage.Name;
                }
            }

            return "";
        }

        public void setName(string languageCode, string name)
        {
            foreach (Language thisLanguage in translationsList)
            {
                if (thisLanguage.Code.ToUpper().Equals(languageCode.ToUpper()))
                {
                    // language is found, set its name
                    thisLanguage.Name = name;
                    return;
                }
            }

            // language is not found, add it to the list
            Language newLanguage = new Language(languageCode, name);
            translationsList.Add(newLanguage);
        }

        public Domain(XmlNode domainNode)
        {
            // add unique Id
            this.UniqueId = domainNode.SelectSingleNode("UniqueId").InnerText.Trim();

            // add translations
            XmlNode domainNames = domainNode.SelectSingleNode("Name");

            foreach (Language thisLanguage in App.getConfiguration().LanguagesList)
            {
                string code = thisLanguage.Code;
                XmlNode domainName = domainNames.SelectSingleNode(code);

                if (domainName == null)
                {
                    throw new Exception("The translation for the language code " + code + " is not specified in the domain name with ID = " + this.UniqueId + ".");
                }
                else
                {
                    this.setName(code, domainName.InnerText.Trim());
                }

            }

            // get quick stats items
            XmlNodeList quickStatsItemNodes = domainNode.SelectSingleNode("QuickStatsList").SelectNodes("QuickStatsItem");
            foreach (XmlNode quickStatsItemNode in quickStatsItemNodes)
            {
                QuickStatsItem qsItem = new QuickStatsItem(quickStatsItemNode);
                this.QuickStatsItemsList.Add(qsItem);
            }
        }

        public Boolean DownloadSdmxFiles()
        {
            foreach (QuickStatsItem thisItem in this.QuickStatsItemsList)
            {
                Boolean answer = thisItem.DownloadSdmxDataFile();
                if (answer == false) { return false; }

                answer = thisItem.DownloadSdmxDsdFile();
                if (answer == false) { return false; }
            }

            return true;
        }

        public Boolean ProcessSdmxFiles()
        {
            foreach (QuickStatsItem thisItem in this.QuickStatsItemsList)
            {
                Boolean answer;

                answer = thisItem.ProcessSdmxDsdFile();
                if (answer == false) { return false; }

                answer = thisItem.ProcessSdmxDataFile();
                if (answer == false) { return false; }
            }

            return true;
        }
    }
}
