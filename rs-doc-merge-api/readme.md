## Merge Api
The Merge Api is where all the magic happens. When called by the Event Handler, it checks the SQL queue to check if there are any documents to be processed. If it finds a letter then it loads the Xml request, loads the respective template and calls the Docentric merge, the resultant stream is then passed to Aspose and the resultant Pdf is saved to an Azure File Share or alternatively a file share on - for those using this project on-prem