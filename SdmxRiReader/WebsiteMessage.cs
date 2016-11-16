using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SdmxRiReader
{
    class WebsiteMessage
    {
        private List<Language> webLink              = new List<Language>();
        private List<Language> messageText          = new List<Language>();
        private string expireDate                   = "";

        internal List<Language> WebLink
        {
            get { return webLink; }
            set { webLink = value; }
        }

        internal List<Language> MessageText
        {
            get { return messageText; }
            set { messageText = value; }
        }

        public string ExpireDate
        {
            get { return expireDate; }
            set { expireDate = value; }
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

        public string getMessageText(string languageCode)
        {
            foreach (Language thisLanguage in this.MessageText)
            {
                if (thisLanguage.Code.ToUpper().Equals(languageCode.ToUpper()))
                {
                    return thisLanguage.Name;
                }
            }

            return "";
        }

        public void setWebLink(string languageCode, string strWebLink) 
        {
            foreach (Language thisLanguage in WebLink)
            {
                if (thisLanguage.Code.ToUpper().Equals(languageCode.ToUpper()))
                {
                    // language is found, set its name
                    thisLanguage.Name = strWebLink;
                    return;
                }
            }

            // language is not found, add it to the list
            Language newLanguage = new Language(languageCode, strWebLink);
            WebLink.Add(newLanguage);
        }

        public void setMessageText(string languageCode, string strMessageText)
        {
            foreach (Language thisLanguage in MessageText)
            {
                if (thisLanguage.Code.ToUpper().Equals(languageCode.ToUpper()))
                {
                    // language is found, set its name
                    thisLanguage.Name = strMessageText;
                    return;
                }
            }

            // language is not found, add it to the list
            Language newLanguage = new Language(languageCode, strMessageText);
            MessageText.Add(newLanguage);
        }

        public WebsiteMessage(XmlNode nodeWebsiteMessage)
        {
            // get expire date
            this.ExpireDate = nodeWebsiteMessage.SelectSingleNode("ExpireDate").InnerText.Trim();

            // get web link
            XmlNode nodeWebLink = nodeWebsiteMessage.SelectSingleNode("WebLink");

            foreach (Language thisLanguage in App.getConfiguration().LanguagesList)
            {
                string code = thisLanguage.Code;
                XmlNode thisWeblink = nodeWebLink.SelectSingleNode(code);

                if (thisWeblink == null)
                {
                    App.AppendRecordToLog("The translation for the language code " + code + " is not specified in the web message link.");
                    throw new Exception("The translation for the language code " + code + " is not specified in the web message link.");
                }
                else
                {
                    this.setWebLink(code, thisWeblink.InnerText.Trim());
                }
            }

            // get message text
            XmlNode nodeMessageText = nodeWebsiteMessage.SelectSingleNode("MessageText");

            foreach (Language thisLanguage in App.getConfiguration().LanguagesList)
            {
                string code = thisLanguage.Code;
                XmlNode thisMessageLink = nodeMessageText.SelectSingleNode(code);

                if (thisMessageLink == null)
                {
                    App.AppendRecordToLog("The translation for the language code " + code + " is not specified in the web message text.");
                    throw new Exception("The translation for the language code " + code + " is not specified in the web message text.");
                }
                else
                {
                    this.setMessageText(code, thisMessageLink.InnerText.Trim());
                }
            }

            return;
        }
    }
}
