For more details go to the page at http://www1.unece.org/stat/platform/display/pcaxis/Dissemination+Software

1. Introduction
Sdmx-Ri Reader is a console application that retrieves Sdmx data and metadata files from Sdmx-RI 
(SDMX Reference Infrastructure developed by Eurostat) and populates data files in Quick Stats File Format. 
The output files are used for dissemination through the Quick Stats web application.
Sdmx-Ri is a restful web service. You can use other restful web services returning data in SDMX format 
alothough this wasn't tested yet. Sdmx-Ri Reader uses a configuration file that is describing 
the parameters needed to read data from the restulf web service and process data accordingly. 
You should be able to find the executable files in C:\MyProjects\WebDesign\Deploy\SdmxRiReader\ folder.
 
2. Configuring Sdmx-Ri Reader.
Configuration is set in the configuration file at Data/Config/config.xml 
There is also the XSD schema validation file at Data/Config/validate.xml
 
3. Editing config file
You can use XML Notepad 2007 on Microsoft website to edit and validate the config file. 
Alternatively, you can develop your own GUI to edit the config file in a more user-friendly way. 
Use the manual on Sdmx-Ri Reader Configuration File Format describing how to control data flow in the application.
 
4. Launching Sdmx-Ri Reader
Use a_run.bat file to launch Sdmx-Ri Reader. You can launch it directly in the command line by 
typing the following,
 
cd C:\MyProjects\WebDesign\Deploy\SdmxRiReader\
SdmxRiReader.exe C:\MyProjects\WebDesign\Deploy\SdmxRiReader\Data\Config\config.xml C:\MyProjects\WebDesign\Deploy\SdmxRiReader\Data\Config\validate.xsd

Here the executable file SdmxRiReader.exe takes two arguments, paths to config and validation files
 
5. Changing folder
The config files are set up for an easy run when the console files are in C:\MyProjects\WebDesign\Deploy\SdmxRiReader\ folder, e.g.
C:\MyProjects\WebDesign\Deploy\SdmxRiReader\sdmxrireader.exe
C:\MyProjects\WebDesign\Deploy\SdmxRiReader\Data\Config\config.xml
C:\MyProjects\WebDesign\Deploy\SdmxRiReader\Data\Config\validate.xml
etc.
Should you need to move the C:\MyProjects\WebDesign\Deploy\SdmxRiReader\ folder into another place, 
you need to change config.xml and a_run.bat files.
 
6. More information
You can find more information on the website describing Sdmx-Ri Reader application.
Or you can send a message to support.stat@unece.org.


