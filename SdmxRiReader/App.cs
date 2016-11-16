using Org.Sdmxsource.Sdmx.Api.Constants;
using Org.Sdmxsource.Sdmx.Api.Model.Format;
using Org.Sdmxsource.Sdmx.SdmxObjects.Model;
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
    public static class App
    {
        public static string APP_VERSION = "1.2";
        public static SdmxRiConfiguration thisConfiguration = null;

        public static SdmxRiConfiguration getConfiguration()
        {
            return thisConfiguration;
        }

        public static void setConfiguration(SdmxRiConfiguration myConfiguration)
        {
            App.thisConfiguration = myConfiguration;
        }

        static void Main(string[] args)
        {
            string strConfigFilePath = "";
            string strValidationFilePath = "";

            
            if (args.Length == 2)
            {
                strConfigFilePath = args[0];
                strValidationFilePath = args[1];
            }
            else
            {
                // just console, impossible to add a message to the log because parsing is not done yet
                Console.WriteLine("Configuration and validation files are not specified. The update is quitted.");
                return;
            }

            // make sure that configuration and validation files exist
            if (!File.Exists(strConfigFilePath))
            {
                // just console, impossible to add a message to the log because parsing is not done yet
                Console.WriteLine("Configuration file is not found. The update is quitted.");
                return;
            }

            if (!File.Exists(strValidationFilePath))
            {
                // just console, impossible to add a message to the log because parsing is not done yet
                Console.WriteLine("Validation file is not found. The update is quitted.");
                return;
            }

            // launch update
            SdmxRiConfiguration mgr = new SdmxRiConfiguration(strConfigFilePath, strValidationFilePath);
            App.setConfiguration(mgr);

            if (mgr.Validate())
            {
                // just console, impossible to add a message to the log because parsing is not done yet
                Console.WriteLine("Parsing configuiration file at " + strConfigFilePath);
                if (mgr.Parse())
                {
                    App.AppendRecordToLog("**********************************************************************");
                    App.AppendRecordToLog("Downloading SDMX files..");
                    if (mgr.DownloadSdmxFiles())
                    {
                        App.AppendRecordToLog("Processing SDMX files..");
                        if (mgr.ProcessSdmxFiles())
                        {
                            App.AppendRecordToLog("Aggregating data..");
                            if (mgr.AggregateData())
                            {
                                App.AppendRecordToLog("Updating output files..");
                                if (mgr.UpdateOutputFiles())
                                {
                                    if (mgr.UpdateIsDifferent())
                                    {
                                        if (mgr.ValidateUpdate())
                                        {
                                            App.AppendRecordToLog("Update is successfully completed.");
                                        }
                                        else
                                        {
                                            App.AppendRecordToLog("This update cannot be made valid.");
                                        }
                                    }
                                    else
                                    {
                                        App.AppendRecordToLog("This update is not different from the present one. Update is skipped");
                                    }
                                }
                                else
                                {
                                    App.AppendRecordToLog("Unable to update output files.");
                                }
                            }
                            else
                            {
                                App.AppendRecordToLog("Unable to aggregate data.");
                            }
                        }
                        else
                        {
                            App.AppendRecordToLog("Unable to process SDMX files.");
                        }
                    }
                    else
                    {
                        App.AppendRecordToLog("Unable to download SDMX files.");
                    }
                }
                else
                {
                    // just console, impossible to add a message to the log because of parsing error, folder cannot be retrieved
                    Console.WriteLine("Unable to parse the configuration file.");
                }
            }
            else
            {
                // just console, impossible to add a message to the log because of parsing error, folder cannot be retrieved
                Console.WriteLine("Unable to validate the configuration file.");
            }

        }


        public static void AppendRecordToLog(string strLine)
        {
            Console.WriteLine(strLine);

            string path = App.getConfiguration().OutputDataFilesFolder + "log.txt";
            string strDateAndTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            string strMessage = strDateAndTime + ", " + strLine;

            if (File.Exists(path))
            {
                // append text
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(strMessage);
                }
            }
            else
            {
                // Create a log file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(strMessage);
                }
            }
        }

        public static Boolean FilesAreDifferent(string strFilePath1, string strFilePath2)
        {
            FileInfo firstFile = new FileInfo(strFilePath1);
            FileInfo secondFile = new FileInfo(strFilePath2);

            Boolean answer = App.Compare(firstFile, secondFile);

            return !answer;
        }


        public static bool Compare(FileInfo firstFile, FileInfo secondFile)
        {
            if (!firstFile.Exists)
            {
                string message = "File '" + firstFile.FullName + "' does not exist";
                throw new FileNotFoundException(message);
            }

            if (!secondFile.Exists)
            {
                string message = "File '" + secondFile.FullName + "' does not exist";
                throw new FileNotFoundException(message);
            }


            // Check Each byte
            FileStream fs1 = new FileStream(firstFile.FullName, FileMode.Open, FileAccess.Read);
            FileStream fs2 = new FileStream(secondFile.FullName, FileMode.Open, FileAccess.Read);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            int file1byte;
            int file2byte;

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);

        }

    }
}
