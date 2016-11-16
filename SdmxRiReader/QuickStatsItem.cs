using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Engine;
using Org.Sdmxsource.Sdmx.Api.Manager.Parse;
using Org.Sdmxsource.Sdmx.Api.Manager.Retrieval;
using Org.Sdmxsource.Sdmx.Api.Model;
using Org.Sdmxsource.Sdmx.Api.Model.Data;
using Org.Sdmxsource.Sdmx.Api.Model.Objects;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Base;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.Codelist;
using Org.Sdmxsource.Sdmx.Api.Model.Objects.DataStructure;
using Org.Sdmxsource.Sdmx.Api.Util;
using Org.Sdmxsource.Sdmx.DataParser.Manager;
using Org.Sdmxsource.Sdmx.Structureparser.Manager.Parsing;
using Org.Sdmxsource.Sdmx.StructureRetrieval.Manager;
using Org.Sdmxsource.Util.Io;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SdmxRiReader
{
    class QuickStatsItem
    {
        private const string PREFIX_FILE_NAME_SDMX_DATA         = "___qst_" + "data";
        private const string PREFIX_FILE_NAME_SDMX_DSD          = "___qst_" + "dsd";
        private const string PREFIX_FILE_NAME_TIMESERIES_CSV    = "___qst_" + "timeseries";
        private const string PREFIX_FILE_NAME_DATASET_CSV       = "___qst_" + "dataset";
        
        private string uniqueId                                 = "";
        private string strName                                  = "";

        private SdmxRestRequest dataSdmxRestRequest             = null;
        private SdmxRestRequest dsdSdmxRestRequest              = null;

        private string tempDataSdmxFile                         = "";
        private string tempDsdSdmxFile                          = "";
        private string tempTimeSeriesCsvFile                    = "";
        private string tempDatasetCsvFile                       = "";

        private List<Language> nameTranslationsList             = new List<Language>();
        private List<Language> measureTranslationsList          = new List<Language>();
        private List<Language> noteTranslationsList             = new List<Language>();
        private List<Language> noteImputedTranslationsList      = new List<Language>();

        private List<string> gradeValues                        = new List<string>();
        private List<string> gradeColors                        = new List<string>();

        private List<string> imputedTerritoriesList             = new List<string>();

        private Geo geo                                         = null;
        private Coverage coverage                               = null;

        private string sourceWebLink                            = "";
        private string colorScale                               = "";

        private int latestTimePeriod                            = -1;
        private int decimalsNumber                              = -1;

        public int DecimalsNumber
        {
            get { return decimalsNumber; }
            set { decimalsNumber = value; }
        }

        public string SourceWebLink
        {
            get { return sourceWebLink; }
            set { sourceWebLink = value; }
        }

        public string ColorScale
        {
            get { return colorScale; }
            set { colorScale = value; }
        }

        internal List<Language> NoteImputedTranslationsList
        {
            get { return noteImputedTranslationsList; }
            set { noteImputedTranslationsList = value; }
        }

        public List<string> ImputedTerritoriesList
        {
            get { return imputedTerritoriesList; }
            set { imputedTerritoriesList = value; }
        }

        public string TempDatasetCsvFile
        {
            get { return tempDatasetCsvFile; }
            set { tempDatasetCsvFile = value; }
        }

        public int LatestTimePeriod
        {
            get { return latestTimePeriod; }
            set { latestTimePeriod = value; }
        }

        internal Coverage Coverage
        {
            get { return coverage; }
            set { coverage = value; }
        }

        public string TempTimeSeriesCsvFile
        {
            get { return tempTimeSeriesCsvFile; }
            set { tempTimeSeriesCsvFile = value; }
        }

        public List<string> GradeColors
        {
            get { return gradeColors; }
            set { gradeColors = value; }
        }

        public List<string> GradeValues
        {
            get { return gradeValues; }
            set { gradeValues = value; }
        }

        internal Geo Geo
        {
            get { return geo; }
            set { geo = value; }
        }

        public string getName(string code)
        {
            foreach (Language thisLanguage in nameTranslationsList)
            {
                if (thisLanguage.Code.ToUpper().Equals(code.ToUpper()))
                {
                    return thisLanguage.Name;
                }
            }

            return "";
        }

        public void setName(string code, string name)
        {
            foreach (Language thisLanguage in nameTranslationsList)
            {
                if (thisLanguage.Code.ToUpper().Equals(code.ToUpper()))
                {
                    // language is found, set its name
                    thisLanguage.Name = name;
                    return;
                }
            }

            // language is not found, add it to the list
            Language newLanguage = new Language(code, name);
            nameTranslationsList.Add(newLanguage);
        }

        public string getMeasure(string code)
        {
            foreach (Language thisLanguage in measureTranslationsList)
            {
                if (thisLanguage.Code.ToUpper().Equals(code.ToUpper()))
                {
                    return thisLanguage.Name;
                }
            }

            return "";
        }

        public void setMeasure(string code, string name)
        {
            foreach (Language thisLanguage in measureTranslationsList)
            {
                if (thisLanguage.Code.ToUpper().Equals(code.ToUpper()))
                {
                    // language is found, set its name
                    thisLanguage.Name = name;
                    return;
                }
            }

            // language is not found, add it to the list
            Language newLanguage = new Language(code, name);
            measureTranslationsList.Add(newLanguage);
        }
        
        public string getNote(string code)
        {
            string resultNote = "";

            foreach (Language thisLanguage in noteTranslationsList)
            {
                if (thisLanguage.Code.ToUpper().Equals(code.ToUpper()))
                {
                    resultNote = thisLanguage.Name;
                    break;
                }
            }

            return resultNote;
        }
        
        public void setNote(string code, string name)
        {
            foreach (Language thisLanguage in noteTranslationsList)
            {
                if (thisLanguage.Code.ToUpper().Equals(code.ToUpper()))
                {
                    // language is found, set its name
                    thisLanguage.Name = name;
                    return;
                }
            }

            // language is not found, add it to the list
            Language newLanguage = new Language(code, name);
            noteTranslationsList.Add(newLanguage);
        }

        public string getImputedTerritoriesNote(string code)
        {
            string resultNote = "";

            // get note about imputed territories
            string strImputedTerritories = "";
            foreach (string strTerritoryCode in this.ImputedTerritoriesList)
            {
                Territory thisTerritory = this.getTerritoryByCode(strTerritoryCode);

                if (thisTerritory != null)
                {
                    // imputed territory is found 
                    string strTerritoryName = thisTerritory.getName(code);

                    if (strImputedTerritories.Equals(""))
                    {
                        strImputedTerritories = strTerritoryName;
                    }
                    else
                    {
                        strImputedTerritories = strImputedTerritories + ", " + strTerritoryName;
                    }
                }
            }

            if (strImputedTerritories.Equals("") == false)
            {
                foreach (Language thisLanguage in NoteImputedTranslationsList)
                {
                    if (thisLanguage.Code.ToUpper().Equals(code.ToUpper()))
                    {
                        resultNote = thisLanguage.Name;
                        break;
                    }
                }

                resultNote = "<BR/>" + resultNote.Trim() + " " + strImputedTerritories.Trim() + ".";
            }

            return resultNote;
        }

        public void setImputedTerritoriesNote(string code, string name)
        {
            foreach (Language thisLanguage in this.NoteImputedTranslationsList)
            {
                if (thisLanguage.Code.ToUpper().Equals(code.ToUpper()))
                {
                    // language is found, set its name
                    thisLanguage.Name = name;
                    return;
                }
            }

            // language is not found, add it to the list
            Language newLanguage = new Language(code, name);
            this.NoteImputedTranslationsList.Add(newLanguage);
        }

        public QuickStatsItem(XmlNode quickStatsItemNode)
        {
            // get indicator Id
            this.UniqueId = quickStatsItemNode.SelectSingleNode("UniqueId").InnerText.Trim();

            // get decimals number
            this.DecimalsNumber = Int32.Parse(quickStatsItemNode.SelectSingleNode("DecimalsNumber").InnerText.Trim());

            // get source web link
            this.SourceWebLink = quickStatsItemNode.SelectSingleNode("SourceWebLink").InnerText.Trim();

            // get color scale
            this.ColorScale = quickStatsItemNode.SelectSingleNode("ColorScale").InnerText.Trim();

            // get coverage
            XmlNode coverageNode = quickStatsItemNode.SelectSingleNode("Coverage");
            this.Coverage = new Coverage(coverageNode);

            // add translations
            XmlNode quickStatsNames = quickStatsItemNode.SelectSingleNode("Name");

            foreach (Language thisLanguage in App.getConfiguration().LanguagesList)
            {
                // initialize quick stats translations
                string code = thisLanguage.Code;

                XmlNode quickStatsName = quickStatsNames.SelectSingleNode(code);

                if (quickStatsName == null)
                {
                    throw new Exception("The translation for the language code " + code + " is not specified in the quick stats name with ID = " + this.UniqueId + ".");
                }
                else
                {
                    this.setName(code, quickStatsName.InnerText.Trim());
                }
            }

            // get grade values
            XmlNode gradeValuesNode = quickStatsItemNode.SelectSingleNode("GradeValues");

            foreach (XmlNode thisgradeValueNode in gradeValuesNode.SelectNodes("GradeValue"))
            {
                this.GradeValues.Add(thisgradeValueNode.InnerText.Trim());
            }

            // get grade colors
            XmlNode gradeColorsNode = quickStatsItemNode.SelectSingleNode("GradeColors");

            foreach (XmlNode thisgradeColorNode in gradeColorsNode.SelectNodes("GradeColor"))
            {
                this.GradeColors.Add(thisgradeColorNode.InnerText.Trim());
            }

            // get measures
            XmlNode quickStatsMeasures = quickStatsItemNode.SelectSingleNode("Measure");

            foreach (Language thisLanguage in App.getConfiguration().LanguagesList)
            {
                string code = thisLanguage.Code;
                XmlNode quickStatsMeasure = quickStatsMeasures.SelectSingleNode(code);

                if (quickStatsMeasure == null)
                {
                    throw new Exception("The translation for the language code " + code + " is not specified in the quick stats measure with ID = " + this.UniqueId + ".");
                }
                else
                {
                    this.setMeasure(code, quickStatsMeasure.InnerText.Trim());
                }
            }

            // get notes
            XmlNode quickStatsNotes = quickStatsItemNode.SelectSingleNode("Note");

            foreach (Language thisLanguage in App.getConfiguration().LanguagesList)
            {
                string code = thisLanguage.Code;
                XmlNode quickStatsNote = quickStatsNotes.SelectSingleNode(code);

                if (quickStatsNote == null)
                {
                    throw new Exception("The translation for the language code " + code + " is not specified in the quick stats note with ID = " + this.UniqueId + ".");
                }
                else
                {
                    this.setNote(code, quickStatsNote.InnerText.Trim());
                }
            }


            // get imputed notes
            XmlNode quickStatsImputedNotes = quickStatsItemNode.SelectSingleNode("ImputedTerritoriesNote");

            foreach (Language thisLanguage in App.getConfiguration().LanguagesList)
            {
                string code = thisLanguage.Code;
                XmlNode quickStatsImputedNote = quickStatsImputedNotes.SelectSingleNode(code);

                if (quickStatsImputedNote == null)
                {
                    throw new Exception("The translation for the language code " + code + " is not specified in the quick stats imputed note with ID = " + this.UniqueId + ".");
                }
                else
                {
                    this.setImputedTerritoriesNote(code, quickStatsImputedNote.InnerText.Trim());
                }
            }



            // set restful web service parameters
            XmlNode restfuleWebService = quickStatsItemNode.SelectSingleNode("RestfulWebService");

            // get data sdmx node
            XmlNode dataRestfulWebService = restfuleWebService.SelectSingleNode("Data");
            this.DataSdmxRestRequest = new SdmxRestRequest(dataRestfulWebService);

            // get dsd sdmx node
            XmlNode dsdRestfulWebService = restfuleWebService.SelectSingleNode("Dsd");
            this.DsdSdmxRestRequest = new SdmxRestRequest(dsdRestfulWebService);

            // get Geo
            XmlNode geoNode = dsdRestfulWebService.SelectSingleNode("Geo");
            this.Geo = new Geo(geoNode);

            // get update id
            string strUpdateId = App.getConfiguration().UpdateId;

            // get folder path for temp files
            string strTemporaryDataFilesFolder = App.getConfiguration().TemporaryDataFilesFolder;

            // set temp data file path
            this.TempDataSdmxFile = strTemporaryDataFilesFolder + "A_" + strUpdateId + PREFIX_FILE_NAME_SDMX_DATA + "_" + this.UniqueId + ".xml";

            // set temp data file path
            this.TempDsdSdmxFile = strTemporaryDataFilesFolder + "A_" + strUpdateId + PREFIX_FILE_NAME_SDMX_DSD + "_" + this.UniqueId + ".xml";

            // set temp csv time series file path
            this.tempTimeSeriesCsvFile = strTemporaryDataFilesFolder + "A_" + strUpdateId + PREFIX_FILE_NAME_TIMESERIES_CSV + "_" + this.UniqueId + ".txt";

            // set temp csv dataset file path
            this.tempDatasetCsvFile = strTemporaryDataFilesFolder + "A_" + strUpdateId + PREFIX_FILE_NAME_DATASET_CSV + "_" + this.UniqueId + ".txt";
        }

        public string Name
        {
            get { return strName; }
            set { strName = value; }
        }

        internal SdmxRestRequest DataSdmxRestRequest
        {
            get { return dataSdmxRestRequest; }
            set { dataSdmxRestRequest = value; }
        }

        internal SdmxRestRequest DsdSdmxRestRequest
        {
            get { return dsdSdmxRestRequest; }
            set { dsdSdmxRestRequest = value; }
        }

        public string UniqueId
        {
            get { return uniqueId; }
            set { uniqueId = value; }
        }

        public string TempDataSdmxFile
        {
            get { return tempDataSdmxFile; }
            set { tempDataSdmxFile = value; }
        }

        public string TempDsdSdmxFile
        {
            get { return tempDsdSdmxFile; }
            set { tempDsdSdmxFile = value; }
        }

        public Boolean DownloadSdmxDataFile()
        {
            // get Sdmx request
            SdmxRestRequest sdmxRestRequest = this.DataSdmxRestRequest;

            // download data file
            FileDownloader fd = new FileDownloader();

            return fd.DownloadFile(sdmxRestRequest, this.TempDataSdmxFile);
        }

        public Boolean DownloadSdmxDsdFile()
        {
            // get Sdmx request
            SdmxRestRequest sdmxRestRequest = this.DsdSdmxRestRequest;

            // download data file
            FileDownloader fd = new FileDownloader();

            return fd.DownloadFile(sdmxRestRequest, this.TempDsdSdmxFile);
        }

        public Boolean ProcessSdmxDataFile()
        {
            // initialize list of records
            List<Observation> lines = new List<Observation>();

            // browse dsd and data files
            IReadableDataLocation rdl = new FileReadableDataLocation(this.TempDsdSdmxFile);
            IStructureParsingManager spm = new StructureParsingManager();

            IStructureWorkspace workspace = spm.ParseStructures(rdl);
            ISdmxObjects sdmxObjects = workspace.GetStructureObjects(false);

            ISdmxObjectRetrievalManager retrievalManager = new InMemoryRetrievalManager(sdmxObjects, spm);

            IReadableDataLocation dataLocation = new FileReadableDataLocation(this.TempDataSdmxFile);
            IDataReaderManager dataReaderManager = new DataReaderManager();
            IDataReaderEngine dre = dataReaderManager.GetDataReaderEngine(dataLocation, retrievalManager);

            while (dre.MoveNextDataset())
            {
                IDataStructureObject dsd = dre.DataStructure;

                while (dre.MoveNextKeyable())
                {
                    IKeyable currentKey = dre.CurrentKey;

                    while (dre.MoveNextObservation())
                    {
                        IObservation obs = dre.CurrentObservation;

                        string thisDate = obs.ObsTime;
                        string thisValue = obs.ObservationValue;

                        if (thisValue.Equals("NaN") == false)
                        {
                            // make sure that you get valid values only
                            foreach (IKeyValue thisKey in currentKey.Key)
                            {
                                string keyCode = thisKey.Code;
                                string keyConcept = thisKey.Concept;

                                if (keyConcept.Equals(this.Geo.ConceptId))
                                {
                                    Observation thisObservation = new Observation();
                                    
                                    // get needed decimals number
                                    string strValue = Math.Round(Convert.ToDouble(obs.ObservationValue), this.DecimalsNumber).ToString("0." + new String('0', this.DecimalsNumber));

                                    // set observation fields
                                    thisObservation.Indicator = this.UniqueId;
                                    thisObservation.Territory = keyCode;
                                    thisObservation.Timespan = obs.ObsTime;
                                    thisObservation.Value = strValue;

                                    // get latest time period needed to check coverage
                                    int intTimePeriod = Int32.Parse(obs.ObsTime);

                                    if (intTimePeriod > this.LatestTimePeriod)
                                    {
                                        this.LatestTimePeriod = intTimePeriod;
                                    }
                                    

                                    lines.Add(thisObservation);
                                }
                            }
                        }
                    }
                }
            }


            // make sure that the latest time period has been found
            if (this.LatestTimePeriod == -1)
            {
                App.AppendRecordToLog("Unable to find the latest time period for the quick stats # " + this.UniqueId + ".");
                return false;
            }

            // Sort observations by territory name
            // Skipped for the time being as it is not clear if we need files to be sorted by indicator and territory code

            // create time series file
            using (System.IO.StreamWriter fileTimeSeries = new System.IO.StreamWriter(this.TempTimeSeriesCsvFile))
            {
                foreach (Observation thisObservation in lines)
                {
                    // fileTimeSeries.WriteLine(thisObservation.Indicator + "," + thisObservation.Territory + "," + thisObservation.Timespan + "," + thisObservation.Value + ";");
                    fileTimeSeries.WriteLine(thisObservation.Indicator + "," + thisObservation.Territory + "," + thisObservation.Timespan + "," + thisObservation.Value);
                }
            }


            // check coverage
            Boolean answer;

            answer = this.DatasetIsGeoAndTimeDimensional(lines);
            if (answer == false) { return false; }

            answer = this.TerritoryCoverageIsCorrect(lines);
            if (answer == false) { return false; }

            answer = this.TimespanCoverageIsCorrect(lines);
            if (answer == false) { return false; }


            // get datasets for the best year
            List<Observation> datasets = new List<Observation>();
            List<string> bestYearTerritoriesList = new List<string>();

            foreach (Observation thisObservation in lines)
            {
                if (thisObservation.Timespan.Equals(this.Coverage.BestTimePeriod))
                {
                    datasets.Add(thisObservation);
                    bestYearTerritoriesList.Add(thisObservation.Territory);
                }
            }

            // get datasets for the previous year
            //TO DO HERE
            string strPreviousYear = (Int32.Parse(this.Coverage.BestTimePeriod) - 3).ToString();

            foreach (Observation thisObservation in lines)
            {
                if (thisObservation.Timespan.Equals(strPreviousYear))
                {
                    if (bestYearTerritoriesList.Contains(thisObservation.Territory) == false)
                    {
                        // this territory is not in the best years list but present in the previous year
                        datasets.Add(thisObservation);

                        // add this territory to the list of imputed territories
                        this.ImputedTerritoriesList.Add(thisObservation.Territory);                        
                    }
                }
            }

            // save datasets into the file
            using (System.IO.StreamWriter fileDataset = new System.IO.StreamWriter(this.TempDatasetCsvFile))
            {
                foreach (Observation thisObservation in datasets)
                {
                    if (thisObservation.Timespan.Equals(this.Coverage.BestTimePeriod))
                    {
                        // fileDataset.WriteLine(thisObservation.Indicator + "," + thisObservation.Territory + "," + thisObservation.Timespan + "," + thisObservation.Value + ";");
                        fileDataset.WriteLine(thisObservation.Indicator + "," + thisObservation.Territory + "," + thisObservation.Timespan + "," + thisObservation.Value);
                    }
                }
            }

            return true;
        }

        public Boolean DatasetIsGeoAndTimeDimensional(List<Observation> lines)
        {
            string firstIndicator = "";

            List<string> timeSeries = new List<string>();

            foreach(Observation thisLine in lines) {
                // get combination of territory and time period
                string strCombination = thisLine.Territory + "." + thisLine.Timespan;

                if (firstIndicator.Equals(""))
                {
                    // get language from the first record
                    firstIndicator = thisLine.Indicator;
                }
                else
                {
                    // make sure that all subsequent indicators are the same as the first one
                    if (thisLine.Indicator.Equals(firstIndicator) == false)
                    {
                        App.AppendRecordToLog("Combination " + strCombination + " is changed its indicator code for the quick stats #" + this.UniqueId + ".");
                        return false;
                    }
                }
                
                // analyze the combination
                if (timeSeries.Contains(strCombination))
                {
                    App.AppendRecordToLog("Combination " + strCombination + " is met more than one in the dataset for the quick stats #" + this.UniqueId + ".");
                    return false;
                }
                else
                {
                    timeSeries.Add(strCombination);
                }
            }

            return true;
        }

        public Boolean TerritoryCoverageIsCorrect(List<Observation> lines)
        {
            List<string> territoriesList = new List<string>();

            foreach (Observation thisObservation in lines)
            {
                string thisTerritoryCode = thisObservation.Territory;

                if (territoriesList.Contains(thisTerritoryCode) == false)
                {
                    territoriesList.Add(thisTerritoryCode);
                }
            }

            int intLowerLimit = this.Coverage.TerritoryLimit;
            if (territoriesList.Count < intLowerLimit)
            {
                App.AppendRecordToLog("Number of territories for the quick stats #" + this.UniqueId + " is higher than allowed lower limit specified in the config and equal to " + intLowerLimit.ToString() + ".");
                return false;
            }
            else
            {
                this.Coverage.NumberOfTerritories = territoriesList.Count;
            }

            return true;
        }

        public Boolean TimespanCoverageIsCorrect(List<Observation> lines)
        {
            Boolean BestTimePeriodIsFound = false;
            string strBestTimePeriod = "";

            int intTimespanLimit = this.Coverage.TimespanLimit;

            for (int thisPeriod = this.LatestTimePeriod; thisPeriod > this.LatestTimePeriod - intTimespanLimit; thisPeriod--)
            {
                // get number of countries for the current time period
                int counter = 0;

                foreach (Observation thisLine in lines)
                {
                    if (thisLine.Timespan.Equals(thisPeriod.ToString()))
                    {
                        counter++;
                    }
                }

                // check if this time period is suitable because data coverage is enough
                if (counter >= this.Coverage.TerritoryLimit)
                {
                    strBestTimePeriod = thisPeriod.ToString();
                    BestTimePeriodIsFound = true;
                    break;
                }
            }

            if (BestTimePeriodIsFound)
            {
                this.Coverage.BestTimePeriod = strBestTimePeriod;
                return true;
            }
            else
            {
                App.AppendRecordToLog("The best time period with requested coverage is not found for the quick stats #" + this.UniqueId + ".");
                return false;
            }

        }

        public Boolean ProcessSdmxDsdFile()
        {
            IReadableDataLocation rdl = new FileReadableDataLocation(this.TempDsdSdmxFile);
            IStructureParsingManager spm = new StructureParsingManager();

            IStructureWorkspace workspace = spm.ParseStructures(rdl);
            ISdmxObjects sdmxObjects = workspace.GetStructureObjects(true);

            // get codelist for Geo 
            foreach (IDataStructureObject dataStruct in sdmxObjects.DataStructures)
            {
                // get Geo code list Id
                IDimension geoDimension = dataStruct.GetDimension(this.Geo.DimensionId);
                IRepresentation thisRepresentation = geoDimension.Representation;
                string geoCodelistId = thisRepresentation.Representation.MaintainableId;

                this.Geo.ConceptId = geoDimension.ConceptRef.FullId;

                // get items name in the Geo code list
                foreach (ICodelistObject codelist in sdmxObjects.Codelists)
                {
                    if (codelist.Id.Equals(geoCodelistId))
                    {
                        foreach (ICode thiscode in codelist.Items)
                        {
                            // add territory to the territories list
                            Territory thisTerritory = new Territory();

                            string territoryId = thiscode.Id;
                            thisTerritory.Code = territoryId;

                            this.Geo.Territories.Add(thisTerritory);

                            // add translation to the territory                            
                            foreach (ITextTypeWrapper thisName in thiscode.Names)
                            {
                                string languageCode = thisName.Locale;
                                string territoryName = thisName.Value;

                                Language thisLanguage = new Language(languageCode, territoryName);

                                thisTerritory.TranslationsList.Add(thisLanguage);
                            }
                        }
                    }
                }
            }

            return true;
        }

        public Territory getTerritoryByCode(string code)
        {
            Territory resultTerritory = null;
            foreach (Territory thisTerritory in this.Geo.Territories)
            {
                if (thisTerritory.Code.Equals(code))
                {
                    resultTerritory = thisTerritory;
                    break;
                }
            }

            return resultTerritory;
        }
    }
}

