using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SdmxRiReader
{
    class Territory
    {
        private string code                             = "";
        private List<Language> translationsList         = new List<Language>();
        private List<Language> webLink                  = new List<Language>();

        internal List<Language> WebLink
        {
            get { return webLink; }
            set { webLink = value; }
        }

        public string Code
        {
            get { return code; }
            set { code = value; }
        }

        internal List<Language> TranslationsList
        {
            get { return translationsList; }
            set { translationsList = value; }
        }

        public Territory()
        {
        }

        public Territory(XmlNode thisTerritoryNode)
        {
            // get code
            this.Code = thisTerritoryNode.SelectSingleNode("Code").InnerText.Trim();

            // get name
            XmlNode thisTerritoryNameNode = thisTerritoryNode.SelectSingleNode("Name");

            foreach (Language thisLanguage in App.getConfiguration().LanguagesList) {
                string thisLanguageCode = thisLanguage.Code;

                XmlNode thisTerritoryLanguageNode = thisTerritoryNameNode.SelectSingleNode(thisLanguageCode);

                if (thisTerritoryLanguageNode == null)
                {
                    App.AppendRecordToLog("The translation for the language code " + code + " is not specified in the territory name, TerritoryProfiles.");
                    throw new Exception("The translation for the language code " + code + " is not specified in the territory name, TerritoryProfiles.");
                }
                else
                {
                    string strName = thisTerritoryLanguageNode.InnerText.Trim();
                    this.setName(thisLanguageCode, strName);
                }
            }

            // get web link
            XmlNode thisWebLinkNode = thisTerritoryNode.SelectSingleNode("WebLink");

            foreach (Language thisLanguage in App.getConfiguration().LanguagesList)
            {
                string thisLanguageCode = thisLanguage.Code;

                XmlNode thisWebLinkLanguageNode = thisWebLinkNode.SelectSingleNode(thisLanguageCode);

                if (thisWebLinkLanguageNode == null)
                {
                    App.AppendRecordToLog("The translation for the language code " + code + " is not specified in the web link, TerritoryProfiles.");
                    throw new Exception("The translation for the language code " + code + " is not specified in the web link, TerritoryProfiles.");
                }
                else
                {
                    string strWebLink = thisWebLinkLanguageNode.InnerText.Trim();
                    this.setWebLink(thisLanguageCode, strWebLink);
                }
            }
        }

        public string getName(string languageCode)
        {
            foreach (Language thisLanguage in this.TranslationsList)
            {
                if (thisLanguage.Code.ToUpper().Equals(languageCode.ToUpper()))
                {
                    return thisLanguage.Name;
                }
            }

            return "";
        }

        public void setName(string languageCode, string strName)
        {
            foreach (Language thisLanguage in TranslationsList)
            {
                if (thisLanguage.Code.ToUpper().Equals(languageCode.ToUpper()))
                {
                    // language is found, set its name
                    thisLanguage.Name = strName;
                    return;
                }
            }

            // language is not found, add it to the list
            Language newLanguage = new Language(languageCode, strName);
            TranslationsList.Add(newLanguage);
        }


        public string getWebLink(string languageCode)
        {
            foreach (Language thisLanguage in this.WebLink)
            {
                if (thisLanguage.Code.ToUpper().Equals(languageCode.ToUpper()))
                {
                    return thisLanguage.Name;
                }
            }

            return "";
        }

        public void setWebLink(string languageCode, string strName)
        {
            foreach (Language thisLanguage in WebLink)
            {
                if (thisLanguage.Code.ToUpper().Equals(languageCode.ToUpper()))
                {
                    // language is found, set its name
                    thisLanguage.Name = strName;
                    return;
                }
            }

            // language is not found, add it to the list
            Language newLanguage = new Language(languageCode, strName);
            WebLink.Add(newLanguage);
        }
    }
}


