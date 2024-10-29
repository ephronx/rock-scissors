# Rock-Scissors

_Rock-Scissors_ is a group of applications used to request and merge data (formatted as XML) with a Word document template to produce high fidelity PDF reports, letters etc. For purposes of simplicity, this solution uses a SQL Server db as the repository and queue for the document requests.

Included in this solution are:
- the _db schema_ to spin up the database
- the _request handler_ which is a .net core 6 API to add and query a request as well as add a batch to wrap a set of requests as a group of requests
- an _event handler_ - in this case just a simple Azure timer function but you could use any scheduler to execute the merge
- the _merge api_ - when exectuted the Merge api checks the queue for an unprocessed request and processes it

## History of this project
In 1997 I developed an administration system to replace an existing, ageing "Clipper/Dbase3" system that was not Y2k compliant. Part of the solution was to generate client statements as well as letters to the clients confirming transactions etc. The system was used to track clients investments. At that time I was already frustrated with the quality of correspondence generated by most Report writers e.g. Crystal Reports. I decided to investigate whether it was possible to automate MS Word. Although clunky, it worked, with a discrete template for each type of correspondence and using find and replace of bookmarks I was able to get it working.

In 2003, on another major administration system development, I again had a need to automate correspondence. This time a lot more sophisticated requirements producing all correspondence and much more complex client statements at volume (120k per quarter), so I needed an engine that could scale out, running on multiple nodes etc. Again I turned to automating Word, but this time using XML as a data source which meant I could do tables, and conditions to show or hide sections as needed. Still using Find and Replace and even though it was slow and clunky we ran that engine for 10 years.

When Microsoft launched OpenXml document format I started researching injecting Xml directly into the Word document template without the need for Word actually being installed which would have meant a significant improvement in speed. In the early days, the Word document specification was OK, but really left much to be desired. I made some progress but eventually work and other priorities just got in the way.

By 2015 I was on another project and another correspondence requirement. And so I dusted off the, by now C#, engine code. This time I was asked to use QorusDocs (a SharePoint based templating engine for MS Word) as the merge engine. This project reinvigorated my search for a solution to injecting Xml directly into the customxml parts of a word document. This time around I came across the Docentric Toolkit (https://www.docentric.com/) and my life was complete...

And again other work and priorities got in the way. I moved countries, changed jobs etc etc.

In 2023 I dusted off Rock-Scissors again. A new project and a new correspondence requirement.

## Taking Rock-Scissors OSS
I will never take Rock-Scissors commercial. Long time collaborators have tried to encourage me to do something with this project. But I toyed with the idea in the past of taking the project open source. It may just help some project team make a deadline or solve a long standing issue with report writers that never quite give you a rich, predictable output quality.

This project is licensed under the Apache v2 License. There are closed source components in use in this project: Aspose.Words and Docentric Toolkit.



## Architecture
Programming Language: C# .net core 6
Additional Assemblies:
- Docentric Toolkit - latest from Nuget
- Aspose.Words for .net 
- Microsoft.Data.SqlClient - for Azure SQL db and local SQL server
Development Pattern: MVC
