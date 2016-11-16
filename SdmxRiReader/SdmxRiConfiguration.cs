using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace SdmxRiReader
{
    public class SdmxRiConfiguration
    {

        private const int UPDATE_ID_LENGTH = 7;
        private const string PREFIX_FILE_NAME = "a";

        private const string SUFFIX_FILE_NAME_UPDATED = "_updated";
        private const string SUFFIX_FILE_NAME_TERRITORIES = "_territories_";
        private const string SUFFIX_FILE_NAME_DOMAINS = "_domains_";
        private const string SUFFIX_FILE_NAME_INDICATORS = "_indicators_";

        private const string SUFFIX_FILE_NAME_TIMESERIES = "_timeseries.txt";
        private const string SUFFIX_FILE_NAME_TS_INDEX = "_tsindex.txt";

        private const string SUFFIX_FILE_NAME_DATASETS = "_datasets.txt";
        private const string SUFFIX_FILE_NAME_DS_INDEX = "_dsindex.txt";

        private const string SUFFIX_FILE_NAME_WEBSITE_MESSAGE = "_websitemessage_";
        private const string SUFFIX_FILE_NAME_TERRITORY_PROFILES = "_territoryprofiles_";

        private const string SUFFIX_FILE_NAME_TEMP_TIMESERIES = "__qst_timeseries.csv";
        private const string SUFFIX_FILE_NAME_TEMP_DATASETS = "__qst_datasets.csv";

        private string configFilePath = "";
        private string validationFilePath = "";
        private string temporaryDataFilesFolder = "";
        private string outputDataFilesFolder = "";

        private string updateId = "";

        private Geo geo = new Geo();

        private WebsiteMessage websiteMessage = null;
        private TerritoryProfiles territoryProfiles = null;

        private List<Domain> domainsList = new List<Domain>();
        private List<Language> languagesList = new List<Language>();
        private List<string> timeSeriesIndexList = new List<string>();
        private List<string> datasetIndexList = new List<string>();

        private string tempCsvTimeSeriesFilePath = "";
        private string tempCsvDatasetsFilePath = "";


        public List<string> DatasetIndexList
        {
            get { return datasetIndexList; }
            set { datasetIndexList = value; }
        }

        internal TerritoryProfiles TerritoryProfiles
        {
            get { return territoryProfiles; }
            set { territoryProfiles = value; }
        }

        internal WebsiteMessage WebsiteMessage
        {
            get { return websiteMessage; }
            set { websiteMessage = value; }
        }

        public string TempCsvDatasetsFilePath
        {
            get { return tempCsvDatasetsFilePath; }
            set { tempCsvDatasetsFilePath = value; }
        }

        public List<string> TimeSeriesIndexList
        {
            get { return timeSeriesIndexList; }
            set { timeSeriesIndexList = value; }
        }

        public string TempCsvTimeSeriesFilePath
        {
            get { return tempCsvTimeSeriesFilePath; }
            set { tempCsvTimeSeriesFilePath = value; }
        }

        internal Geo Geo
        {
            get { return geo; }
            set { geo = value; }
        }

        public string ConfigFilePath
        {
            get { return configFilePath; }
            set { configFilePath = value; }
        }

        public string ValidationFilePath
        {
            get { return validationFilePath; }
            set { validationFilePath = value; }
        }

        public string OutputDataFilesFolder
        {
            get { return outputDataFilesFolder; }
            set { outputDataFilesFolder = value; }
        }

        public string TemporaryDataFilesFolder
        {
            get { return temporaryDataFilesFolder; }
            set { temporaryDataFilesFolder = value; }
        }

        internal List<Domain> DomainsList
        {
            get { return domainsList; }
            set { domainsList = value; }
        }

        internal List<Language> LanguagesList
        {
            get { return languagesList; }
            set { languagesList = value; }
        }

        public string UpdateId
        {
            get { return updateId; }
            set { updateId = value; }
        }

        public SdmxRiConfiguration(string strConfigFilePath, string strValidationFilePath)
        {
            this.UpdateId = DateTime.Now.ToString("yyyyMMdd_hhmm") + "_" + System.Guid.NewGuid().ToString().Replace("-", "").Replace("_", "");
            this.ConfigFilePath = strConfigFilePath;
            this.ValidationFilePath = strValidationFilePath;
        }

        public Boolean Validate()
        {
            if (this.ValidationFilePath.Trim().Equals(""))
            {
                App.AppendRecordToLog("Validation is skipped because the XSD file path is not specified");
                return true;
            }

            bool errors = false;
            try
            {
                // validate config file with XSD schema
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add("", this.ValidationFilePath);

                App.AppendRecordToLog("Validating config file with XSD schema at " + this.ValidationFilePath);
                XDocument custOrdDoc = XDocument.Load(this.ConfigFilePath);

                custOrdDoc.Validate(schemas, (o, e) =>
                {
                    Console.WriteLine("{0}", e.Message);
                    errors = true;
                });

            }
            catch (Exception e)
            {
                App.AppendRecordToLog(e.Message);
            }

            return !errors;
        }

        public Boolean Parse()
        {
            XmlDocument doc = null;

            try
            {
                doc = new XmlDocument();
                doc.Load(this.ConfigFilePath);

                // get folder for output data files
                OutputDataFilesFolder = doc.DocumentElement.SelectSingleNode("OutputDataFilesFolder").InnerText.Trim();
                if (!(OutputDataFilesFolder.EndsWith("\\") || OutputDataFilesFolder.EndsWith("//")))
                {
                    OutputDataFilesFolder = OutputDataFilesFolder + "\\";
                }

                // get folder for temporary data files
                TemporaryDataFilesFolder = doc.DocumentElement.SelectSingleNode("TemporaryDataFilesFolder").InnerText.Trim();
                if (!(TemporaryDataFilesFolder.EndsWith("\\") || TemporaryDataFilesFolder.EndsWith("//")))
                {
                    TemporaryDataFilesFolder = TemporaryDataFilesFolder + "\\";
                }

                // get language codes
                XmlNodeList languagesList = doc.DocumentElement.SelectNodes("/Configuration/ActiveLanguages/Language");
                foreach (XmlNode languageNode in languagesList)
                {
                    Language thisLanguage = new Language(languageNode);
                    this.LanguagesList.Add(thisLanguage);
                }

                // get domains
                XmlNodeList domainsList = doc.DocumentElement.SelectNodes("/Configuration/Domains/Domain");
                foreach (XmlNode domainNode in domainsList)
                {
                    Domain thisDomain = new Domain(domainNode);
                    this.DomainsList.Add(thisDomain);
                }

                // get website message
                XmlNode nodeWebsiteMessage = doc.DocumentElement.SelectSingleNode("/Configuration/WebsiteMessage");
                this.WebsiteMessage = new WebsiteMessage(nodeWebsiteMessage);

                // get territory profiles
                XmlNode nodeTerritoryProfiles = doc.DocumentElement.SelectSingleNode("/Configuration/TerritoryProfiles");
                this.TerritoryProfiles = new TerritoryProfiles(nodeTerritoryProfiles);

                return true;
            }
            catch (Exception e)
            {
                App.AppendRecordToLog(e.Message);
            }

            return false;
        }

        public Boolean DownloadSdmxFiles()
        {
            // download data files
            foreach (Domain thisDomain in this.domainsList)
            {
                Boolean answer = thisDomain.DownloadSdmxFiles();
                if (answer == false) { return false; }
            }

            return true;
        }

        public Boolean ProcessSdmxFiles()
        {
            // process data files
            foreach (Domain thisDomain in this.domainsList)
            {
                Boolean answer = thisDomain.ProcessSdmxFiles();
                if (answer == false) { return false; }
            }

            return true;
        }

        public Boolean AggregateData()
        {
            Boolean answer;

            answer = AggregateGeo();
            if (answer == false) { return false; }

            answer = AggregateTimeSeries();
            if (answer == false) { return false; }

            answer = AggregateTimeSeriesIndex();
            if (answer == false) { return false; }

            answer = AggregateDatasets();
            if (answer == false) { return false; }

            answer = AggregateDatasetIndex();
            if (answer == false) { return false; }

            return true;
        }


        public Boolean AggregateDatasetIndex()
        {
            string strPreviousKey = "";
            int counter = 0;
            string thisLine = "";

            System.IO.StreamReader file = new System.IO.StreamReader(this.TempCsvDatasetsFilePath);
            while ((thisLine = file.ReadLine()) != null)
            {
                string[] keyValues = thisLine.Split(',');
                string strCurerntKey = keyValues[0] + "," + keyValues[1];

                if (thisLine.Equals(strPreviousKey) == false)
                {
                    // key has changed, update the index file
                    string strNewLine = strCurerntKey + "," + counter.ToString();
                    this.DatasetIndexList.Add(strNewLine);

                    strPreviousKey = strCurerntKey;
                }

                counter = counter + thisLine.Length + 2;
            }

            file.Close();

            return true;
        }

        public Boolean AggregateTimeSeriesIndex()
        {
            string strPreviousKey = "";
            int counter = 0;
            string thisLine = "";

            System.IO.StreamReader file = new System.IO.StreamReader(TempCsvTimeSeriesFilePath);
            while ((thisLine = file.ReadLine()) != null)
            {
                string[] keyValues = thisLine.Split(',');
                string strCurerntKey = keyValues[0] + "," + keyValues[1];

                if (thisLine.Equals(strPreviousKey) == false)
                {
                    // key has changed, update the index file
                    string strNewLine = strCurerntKey + "," + counter.ToString();
                    this.TimeSeriesIndexList.Add(strNewLine);

                    strPreviousKey = strCurerntKey;
                }

                counter = counter + thisLine.Length + 2;
            }

            file.Close();

            return true;
        }


        public Boolean AggregateDatasets()
        {
            TempCsvDatasetsFilePath = TemporaryDataFilesFolder + "A_" + this.UpdateId + SUFFIX_FILE_NAME_TEMP_DATASETS;

            using (var output = File.Create(TempCsvDatasetsFilePath))
            {
                foreach (Domain thisDomain in this.domainsList)
                {
                    foreach (QuickStatsItem qsItem in thisDomain.QuickStatsItemsList)
                    {
                        using (var input = File.OpenRead(qsItem.TempDatasetCsvFile))
                        {
                            input.CopyTo(output);
                        }
                    }
                }
            }

            return true;
        }

        public Boolean AggregateTimeSeries()
        {
            TempCsvTimeSeriesFilePath = TemporaryDataFilesFolder + "A_" + this.UpdateId + SUFFIX_FILE_NAME_TEMP_TIMESERIES;

            using (var output = File.Create(TempCsvTimeSeriesFilePath))
            {
                foreach (Domain thisDomain in this.domainsList)
                {
                    foreach (QuickStatsItem qsItem in thisDomain.QuickStatsItemsList)
                    {
                        using (var input = File.OpenRead(qsItem.TempTimeSeriesCsvFile))
                        {
                            input.CopyTo(output);
                        }
                    }
                }
            }

            return true;
        }

        public Boolean AggregateGeo()
        {
            foreach (Domain thisDomain in this.domainsList)
            {
                foreach (QuickStatsItem qsItem in thisDomain.QuickStatsItemsList)
                {
                    foreach (Territory thisTerritory in qsItem.Geo.Territories)
                    {
                        if (this.Geo.Found(thisTerritory) == false)
                        {
                            // territory is not found, add it to the global list
                            this.Geo.Territories.Add(thisTerritory);
                        }
                    }
                }
            }

            return true;
        }


        public string getPresentUpdateId()
        {
            string strUpdateId = "";
            if (File.Exists(getFileNameUpdated()))
            {
                Updated thisUpdate = JsonConvert.DeserializeObject<Updated>(File.ReadAllText(getFileNameUpdated()));

                strUpdateId = thisUpdate.Id;
            }

            return strUpdateId;
        }


        public string getNewUpdateId()
        {
            string strNewUpdateIdWithZeros = (new String('0', UPDATE_ID_LENGTH - 1)) + "1";
            if (File.Exists(getFileNameUpdated()))
            {
                Updated thisUpdate = JsonConvert.DeserializeObject<Updated>(File.ReadAllText(getFileNameUpdated()));

                int updateId = Int32.Parse(thisUpdate.Id);
                string strNewUpdateId = (updateId + 1).ToString().Trim();

                strNewUpdateIdWithZeros = (new String('0', UPDATE_ID_LENGTH - strNewUpdateId.Length)) + strNewUpdateId;
            }

            return strNewUpdateIdWithZeros;
        }

        public Boolean ValidateUpdate()
        {
            Updated thisUpdate = new Updated();
            thisUpdate.Id = getNewUpdateId();
            thisUpdate.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            foreach (Language thisLanguage in this.LanguagesList)
            {
                JsonLanguage jsonLang = new JsonLanguage();
                jsonLang.Code = thisLanguage.Code;
                jsonLang.LongCode = thisLanguage.LongCode;
                jsonLang.DefaultLanguage = thisLanguage.DefaultLanguage;

                thisUpdate.Languages.Add(jsonLang);
            }

            File.WriteAllText(this.getFileNameUpdated(), JsonConvert.SerializeObject(thisUpdate));

            return true;
        }

        public Boolean UpdateIsDifferent()
        {
            string strPresentUpdateId = this.getPresentUpdateId();
            string strNewUpdateId = this.getNewUpdateId();

            if (strPresentUpdateId.Equals(""))
            {
                // this is the first update, there is nothing to compare with
                return true;
            }

            //---------------------------------------------------------------------------------------------------------------------------------------
            string strFile1 = getFileNameTimeSeriesNewUpdate();
            string strFile2 = getFileNameTimeSeriesPresentUpdate();

            if (App.FilesAreDifferent(strFile1, strFile2))
            {
                return false;
            }

            //---------------------------------------------------------------------------------------------------------------------------------------
            strFile1 = getFileNameTsIndexNewUpdate();
            strFile2 = getFileNameTsIndexPresentUpdate();

            if (App.FilesAreDifferent(strFile1, strFile2))
            {
                return false;
            }

            //---------------------------------------------------------------------------------------------------------------------------------------

            strFile1 = getFileNameDsIndexNewUpdate();
            strFile2 = getFileNameDsIndexPresentUpdate();

            if (App.FilesAreDifferent(strFile1, strFile2))
            {
                return false;
            }

            //---------------------------------------------------------------------------------------------------------------------------------------

            strFile1 = getFileNameDatasetsNewUpdate();
            strFile2 = getFileNameDatasetsPresentUpdate();

            if (App.FilesAreDifferent(strFile1, strFile2))
            {
                return false;
            }

            foreach (Language thisLanguage in this.LanguagesList)
            {
                string languageCode = thisLanguage.Code;

                //---------------------------------------------------------------------------------------------------------------------------------------

                strFile1 = getFileNameTerritoriesJavascriptNewUpdate(languageCode);
                strFile2 = getFileNameTerritoriesJavascriptPresentUpdate(languageCode);

                if (App.FilesAreDifferent(strFile1, strFile2))
                {
                    return false;
                }

                //---------------------------------------------------------------------------------------------------------------------------------------

                strFile1 = getFileNameTerritoriesJsonNewUpdate(languageCode);
                strFile2 = getFileNameTerritoriesJsonPresentUpdate(languageCode);

                if (App.FilesAreDifferent(strFile1, strFile2))
                {
                    return false;
                }

                //---------------------------------------------------------------------------------------------------------------------------------------

                strFile1 = getFileNameDomainsNewUpdate(languageCode);
                strFile2 = getFileNameDomainsPresentUpdate(languageCode);

                if (App.FilesAreDifferent(strFile1, strFile2))
                {
                    return false;
                }

                //---------------------------------------------------------------------------------------------------------------------------------------

                strFile1 = getFileNameIndicatorsNewUpdate(languageCode);
                strFile2 = getFileNameIndicatorsPresentUpdate(languageCode);

                if (App.FilesAreDifferent(strFile1, strFile2))
                {
                    return false;
                }

                //---------------------------------------------------------------------------------------------------------------------------------------

                strFile1 = getFileNameWebsiteMessageNewUpdate(languageCode);
                strFile2 = getFileNameWebsiteMessagePresentUpdate(languageCode);

                if (App.FilesAreDifferent(strFile1, strFile2))
                {
                    return false;
                }

                //---------------------------------------------------------------------------------------------------------------------------------------

                strFile1 = getFileNameTerritoryProfilesNewUpdate(languageCode);
                strFile2 = getFileNameTerritoryProfilesPresentUpdate(languageCode);

                if (App.FilesAreDifferent(strFile1, strFile2))
                {
                    return false;
                }

            }

            return true;
        }

        public Boolean UpdateOutputFiles()
        {
            Boolean answer;

            answer = UpdateDomainFile();
            if (answer == false) { return false; }

            answer = UpdateIndicatorFile();
            if (answer == false) { return false; }

            answer = UpdateGeoFileJson();
            if (answer == false) { return false; }

            answer = UpdateGeoFileJavascipt();
            if (answer == false) { return false; }

            answer = UpdateTimeSeriesFile();
            if (answer == false) { return false; }

            answer = UpdateTsIndexFile();
            if (answer == false) { return false; }

            answer = UpdateDatasetsFile();
            if (answer == false) { return false; }

            answer = UpdateDsIndexFile();
            if (answer == false) { return false; }

            answer = UpdateWebsiteMessageFile();
            if (answer == false) { return false; }

            answer = UpdateTerritoryProfiles();
            if (answer == false) { return false; }

            return true;
        }

        public Boolean UpdateWebsiteMessageFile()
        {
            FileStream fs = null;
            StreamWriter sw = null;

            foreach (Language thisLanguage in this.LanguagesList)
            {
                string languageCode = thisLanguage.Code;

                string strExpireDate = this.WebsiteMessage.ExpireDate;
                string strWebLink = this.WebsiteMessage.getWebLink(languageCode);
                string strMessageText = this.WebsiteMessage.getMessageText(languageCode);

                try
                {
                    fs = File.Create(getFileNameWebsiteMessageNewUpdate(languageCode));
                    sw = new StreamWriter(fs);

                    String strWebsiteMessage = 
                        " {" + 
                            "\"ExpireDate\":\"" + strExpireDate + "\", " + 
                             "\"WebLink\": \"" + strWebLink + "\", " + 
                             "\"MessageText\": \"" + strMessageText  + "\"" + 
                        "}";
                    
                    sw.WriteLine(strWebsiteMessage);
                    
                }
                catch (Exception e)
                {
                    App.AppendRecordToLog("Unable to create the data file with web message. See message: " + e.Message);
                    return false;
                }
                finally
                {
                    sw.Close();
                    fs.Close();
                }
            }

            return true;
        }

        public Boolean UpdateDatasetsFile()
        {
            File.Copy(this.TempCsvDatasetsFilePath, this.getFileNameDatasetsNewUpdate());
            return true;
        }

        public Boolean UpdateTsIndexFile()
        {
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                fs = File.Create(this.getFileNameTsIndexNewUpdate());
                sw = new StreamWriter(fs);

                foreach (string thisLine in this.TimeSeriesIndexList)
                {
                    sw.WriteLine(thisLine);
                }
            }
            catch (Exception e)
            {
                App.AppendRecordToLog("Unable to create the time series index file. See message: " + e.Message);
                return false;
            }
            finally
            {
                sw.Close();
                fs.Close();
            }

            return true;
        }

        public Boolean UpdateDsIndexFile()
        {
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {
                fs = File.Create(this.getFileNameDsIndexNewUpdate());
                sw = new StreamWriter(fs);

                foreach (string thisLine in this.datasetIndexList)
                {
                    sw.WriteLine(thisLine);
                }
            }
            catch (Exception e)
            {
                App.AppendRecordToLog("Unable to create the dataset index file. See message: " + e.Message);
                return false;
            }
            finally
            {
                sw.Close();
                fs.Close();
            }

            return true;
        }

        public Boolean UpdateTimeSeriesFile()
        {
            File.Copy(this.TempCsvTimeSeriesFilePath, this.getFileNameTimeSeriesNewUpdate());
            return true;
        }

        public Boolean UpdateIndicatorFile()
        {
            foreach (Language thisLanguage in this.LanguagesList)
            {
                string languageCode = thisLanguage.Code;

                List<Indicator> theseIndicators = new List<Indicator>();

                foreach (Domain thisDomain in this.domainsList)
                {
                    foreach (QuickStatsItem qsItem in thisDomain.QuickStatsItemsList)
                    {
                        Indicator thisIndicator = new Indicator();

                        thisIndicator.Code = qsItem.UniqueId;
                        thisIndicator.Domain_Id = thisDomain.UniqueId;
                        thisIndicator.Name = qsItem.getName(languageCode);
                        thisIndicator.SourceWebLink = qsItem.SourceWebLink;
                        thisIndicator.ColorScale = qsItem.ColorScale;
                        thisIndicator.Measure = qsItem.getMeasure(languageCode);
                        thisIndicator.Note = qsItem.getNote(languageCode) + qsItem.getImputedTerritoriesNote(languageCode);

                        // get grade values
                        string strResult = "";
                        Boolean firstTime = true;
                        foreach (string thisGradeValue in qsItem.GradeValues)
                        {
                            if (firstTime)
                            {
                                strResult = thisGradeValue;
                            }
                            else
                            {
                                strResult = strResult + ", " + thisGradeValue;
                            }

                            firstTime = false;
                        }

                        thisIndicator.GradeValues = strResult;

                        // get grade colors
                        strResult = "";
                        firstTime = true;
                        foreach (string thisGradeColor in qsItem.GradeColors)
                        {
                            if (firstTime)
                            {
                                strResult = "'" + thisGradeColor + "'";
                            }
                            else
                            {
                                strResult = strResult + ", " + "'" + thisGradeColor + "'";
                            }

                            firstTime = false;
                        }

                        thisIndicator.GradeColors = strResult;
                        theseIndicators.Add(thisIndicator);
                    }
                }

                File.WriteAllText(this.getFileNameIndicatorsNewUpdate(languageCode), JsonConvert.SerializeObject(theseIndicators));
            }

            return true;
        }

        public Boolean UpdateGeoFileJson()
        {
            FileStream fs = null;
            StreamWriter sw = null;

            foreach (Language thisLanguage in this.LanguagesList)
            {
                string languageCode = thisLanguage.Code;

                try
                {
                    fs = File.Create(getFileNameTerritoriesJsonNewUpdate(languageCode));
                    sw = new StreamWriter(fs);

                    sw.WriteLine("[");

                    Boolean firstTime = true;
                    foreach (Territory thisTerritory in this.Geo.Territories)
                    {
                        String strTerritory = " {\"Name\":\"" + thisTerritory.getName(languageCode) + "\", \"Code\": \"" + thisTerritory.Code + "\"}";
                        if (firstTime)
                        {
                            sw.WriteLine(strTerritory);
                        }
                        else
                        {
                            sw.WriteLine(", " + strTerritory);
                        }

                        firstTime = false;
                    }

                    sw.WriteLine("]");
                }
                catch (Exception e)
                {
                    App.AppendRecordToLog("Unable to create the data file with territories. See message: " + e.Message);
                    return false;
                }
                finally
                {
                    sw.Close();
                    fs.Close();
                }
            }

            return true;
        }

        public Boolean UpdateGeoFileJavascipt()
        {
            FileStream fs = null;
            StreamWriter sw = null;

            foreach (Language thisLanguage in this.LanguagesList)
            {
                string languageCode = thisLanguage.Code;

                try
                {
                    fs = File.Create(getFileNameTerritoriesJavascriptNewUpdate(languageCode));
                    sw = new StreamWriter(fs);

                    sw.WriteLine("var statesNames = {\"titles\": [");

                    Boolean firstTime = true;
                    foreach (Territory thisTerritory in this.Geo.Territories)
                    {
                        String strTerritory = 
                            " { " + 
                                "\"code\": \"" + thisTerritory.Code + "\"" + ", " + 
                                "\"name\":\"" + thisTerritory.getName(languageCode) + "\"" + 
                            " }";
                        if (firstTime)
                        {
                            sw.WriteLine(strTerritory);
                        }
                        else
                        {
                            sw.WriteLine(", " + strTerritory);
                        }

                        firstTime = false;
                    }

                    sw.WriteLine("]}");
                }
                catch (Exception e)
                {
                    App.AppendRecordToLog("Unable to create the data file with territories. See message: " + e.Message);
                    return false;
                }
                finally
                {
                    sw.Close();
                    fs.Close();
                }
            }

            return true;
        }

        public Boolean UpdateDomainFile()
        {
            FileStream fs = null;
            StreamWriter sw = null;

            foreach (Language thisLanguage in this.LanguagesList)
            {
                string languageCode = thisLanguage.Code;

                try
                {
                    fs = File.Create(getFileNameDomainsNewUpdate(languageCode));
                    sw = new StreamWriter(fs);

                    sw.WriteLine("[");

                    Boolean firstTime = true;
                    foreach (Domain thisDomain in this.DomainsList)
                    {
                        String strDomain = " {\"Name\":\"" + thisDomain.getName(languageCode) + "\", \"Code\": \"" + thisDomain.UniqueId + "\"}";
                        if (firstTime)
                        {
                            sw.WriteLine(strDomain);
                        }
                        else
                        {
                            sw.WriteLine(", " + strDomain);
                        }

                        firstTime = false;
                    }

                    sw.WriteLine("]");
                }
                catch (Exception e)
                {
                    App.AppendRecordToLog("Unable to create the data file with domains. See message: " + e.Message);
                    return false;
                }
                finally
                {
                    sw.Close();
                    fs.Close();
                }
            }

            return true;
        }


        public Boolean UpdateTerritoryProfiles()
        {
            FileStream fs = null;
            StreamWriter sw = null;

            foreach (Language thisLanguage in this.LanguagesList)
            {
                string languageCode = thisLanguage.Code;

                try
                {
                    fs = File.Create(this.getFileNameTerritoryProfilesNewUpdate(languageCode));
                    sw = new StreamWriter(fs);

                    sw.WriteLine("[");

                    Boolean firstTime = true;
                    foreach (Territory thisTerritory in this.TerritoryProfiles.Territories)
                    {
                        string strTerritoryCode = thisTerritory.Code;
                        string strTerritoryName = thisTerritory.getName(languageCode);
                        string strWebLink = thisTerritory.getWebLink(languageCode);

                        String strTerritoryProfile = 
                            "{" +
                                "\"Code\": \"" + strTerritoryCode + "\", " + 
                                "\"Name\":\"" + strTerritoryName + "\", " +
                                "\"WebLink\":\"" + strWebLink + "\" " +
                            "}";

                        if (firstTime)
                        {
                            sw.WriteLine(strTerritoryProfile);
                        }
                        else
                        {
                            sw.WriteLine(", " + strTerritoryProfile);
                        }

                        firstTime = false;
                    }

                    sw.WriteLine("]");
                }
                catch (Exception e)
                {
                    App.AppendRecordToLog("Unable to create the data file with territory profiles. See message: " + e.Message);
                    return false;
                }
                finally
                {
                    sw.Close();
                    fs.Close();
                }
            }

            return true;
        }

        public string getFileNameUpdated()
        {
            return OutputDataFilesFolder + PREFIX_FILE_NAME + SUFFIX_FILE_NAME_UPDATED + ".json";
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        public string getFileNameTerritoriesJavascriptNewUpdate(string languageCode)
        {
            return getFileNameTerritoriesJavascript(languageCode, getNewUpdateId());
        }

        public string getFileNameTerritoriesJavascriptPresentUpdate(string languageCode)
        {
            return getFileNameTerritoriesJavascript(languageCode, getPresentUpdateId());
        }

        public string getFileNameTerritoriesJavascript(string languageCode, string updateId)
        {
            return OutputDataFilesFolder + PREFIX_FILE_NAME + updateId + SUFFIX_FILE_NAME_TERRITORIES + languageCode + ".js";
        }

        //---------------------------------------------------------------------------------------------------------------------------------------

        public string getFileNameTerritoriesJsonNewUpdate(string languageCode)
        {
            return getFileNameTerritoriesJson(languageCode, getNewUpdateId());
        }

        public string getFileNameTerritoriesJsonPresentUpdate(string languageCode)
        {
            return getFileNameTerritoriesJson(languageCode, getPresentUpdateId());
        }

        public string getFileNameTerritoriesJson(string languageCode, string updateId)
        {
            return OutputDataFilesFolder + PREFIX_FILE_NAME + updateId + SUFFIX_FILE_NAME_TERRITORIES + languageCode + ".json";
        }

        //---------------------------------------------------------------------------------------------------------------------------------------

        public string getFileNameDomainsNewUpdate(string languageCode)
        {
            return getFileNameDomains(languageCode, getNewUpdateId());
        }

        public string getFileNameDomainsPresentUpdate(string languageCode)
        {
            return getFileNameDomains(languageCode, getPresentUpdateId());
        }

        public string getFileNameDomains(string languageCode, string updateId)
        {
            return OutputDataFilesFolder + PREFIX_FILE_NAME + updateId + SUFFIX_FILE_NAME_DOMAINS + languageCode + ".json";
        }

        //---------------------------------------------------------------------------------------------------------------------------------------

        public string getFileNameIndicatorsNewUpdate(string languageCode)
        {
            return getFileNameIndicators(languageCode, getNewUpdateId());
        }

        public string getFileNameIndicatorsPresentUpdate(string languageCode)
        {
            return getFileNameIndicators(languageCode, getPresentUpdateId());
        }

        public string getFileNameIndicators(string languageCode, string updateId)
        {
            return OutputDataFilesFolder + PREFIX_FILE_NAME + updateId + SUFFIX_FILE_NAME_INDICATORS + languageCode + ".json";
        }

        //---------------------------------------------------------------------------------------------------------------------------------------

        public string getFileNameTimeSeriesNewUpdate()
        {
            return getFileNameTimeSeries(getNewUpdateId());
        }

        public string getFileNameTimeSeriesPresentUpdate()
        {
            return getFileNameTimeSeries(getPresentUpdateId());
        }

        public string getFileNameTimeSeries(string updateId)
        {
            return OutputDataFilesFolder + PREFIX_FILE_NAME + updateId + SUFFIX_FILE_NAME_TIMESERIES;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------

        public string getFileNameTsIndexNewUpdate()
        {
            return getFileNameTsIndex(getNewUpdateId());
        }

        public string getFileNameTsIndexPresentUpdate()
        {
            return getFileNameTsIndex(getPresentUpdateId());
        }

        public string getFileNameTsIndex(string updateId)
        {
            return OutputDataFilesFolder + PREFIX_FILE_NAME + updateId + SUFFIX_FILE_NAME_TS_INDEX;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------

        public string getFileNameDsIndexNewUpdate()
        {
            return getFileNameDsIndexNewUpdate(getNewUpdateId());
        }

        public string getFileNameDsIndexPresentUpdate()
        {
            return getFileNameDsIndexNewUpdate(getPresentUpdateId());
        }

        public string getFileNameDsIndexNewUpdate(string updateId)
        {
            return OutputDataFilesFolder + PREFIX_FILE_NAME + updateId + SUFFIX_FILE_NAME_DS_INDEX;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------

        public string getFileNameDatasetsNewUpdate()
        {
            return getFileNameDatasets(getNewUpdateId());
        }

        public string getFileNameDatasetsPresentUpdate()
        {
            return getFileNameDatasets(getPresentUpdateId());
        }

        public string getFileNameDatasets(string updateId)
        {
            return OutputDataFilesFolder + PREFIX_FILE_NAME + updateId + SUFFIX_FILE_NAME_DATASETS;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------

        public string getFileNameWebsiteMessageNewUpdate(string languageCode)
        {
            return getFileNameWebsiteMessage(languageCode, getNewUpdateId());
        }

        public string getFileNameWebsiteMessagePresentUpdate(string languageCode)
        {
            return getFileNameWebsiteMessage(languageCode, getPresentUpdateId());
        }

        public string getFileNameWebsiteMessage(string languageCode, string updateId)
        {
            return OutputDataFilesFolder + PREFIX_FILE_NAME + updateId + SUFFIX_FILE_NAME_WEBSITE_MESSAGE + languageCode + ".json";
        }

        //---------------------------------------------------------------------------------------------------------------------------------------

        public string getFileNameTerritoryProfilesNewUpdate(string languageCode)
        {
            return getFileNameTerritoryProfiles(languageCode, getNewUpdateId());
        }

        public string getFileNameTerritoryProfilesPresentUpdate(string languageCode)
        {
            return getFileNameTerritoryProfiles(languageCode, getPresentUpdateId());
        }

        public string getFileNameTerritoryProfiles(string languageCode, string updateId)
        {
            return OutputDataFilesFolder + PREFIX_FILE_NAME + updateId + SUFFIX_FILE_NAME_TERRITORY_PROFILES + languageCode + ".json";
        }

        //---------------------------------------------------------------------------------------------------------------------------------------

    }
}

