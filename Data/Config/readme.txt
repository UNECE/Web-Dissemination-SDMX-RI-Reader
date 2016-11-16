

1. Introduction

Sdmx-Ri Reader is a console application that retrieves Sdmx data and metadata files
from Restful Web service and converts them into the internal format. Sdmx-Ri is one such example.
The output files are used for dissemination through the Quick Stats web application. 

2. Configuring Sdmx-Ri Reader.

Configuration is set in the configuration file at Data/Config/config.xml 
There is also the XSD schema validation file at Data/Config/validate.xml 

3. Editing config file

You can use XML Notepad 2007 on Microsoft website to edit and validate the config file.

4. Launching Sdmx-Ri Reader

Use a_run.bat file to launch Sdmx-Ri Reader. 

5. Changing folder

The config files are set up for an easy run when the console files 
are in C:\MyProjects\WebDesign\Deploy\SdmxRiReader\ folder, e.g.

C:\MyProjects\WebDesign\Deploy\SdmxRiReader\sdmxrireader.exe
C:\MyProjects\WebDesign\Deploy\SdmxRiReader\Data\Config\config.xml
C:\MyProjects\WebDesign\Deploy\SdmxRiReader\Data\Config\validate.xml

etc.

Should you need to move the C:\MyProjects\WebDesign\Deploy\SdmxRiReader\ folder 
into another place, you need to change config.xml and a_run.bat files.

6. More information

You can find more information on the website describing Sdmx-Ri Reader application.
Or you can send a message to support.stat@unece.org.