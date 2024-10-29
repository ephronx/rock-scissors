# Rock-Scissors

**Rock-Scissors** is a group of applications used to request and merge data (formatted as XML) with a Word document template to produce high fidelity PDF reports, letters etc. For purposes of simplicity, this solution uses a SQL Server db as the repository and queue for the document requests.

Included in this solution are:
- the **db schema** to spin up the database
- the **request handler** which is a .net core 6 API to add and query a request as well as add a batch to wrap a set of requests as a group of requests
- an **event handler** - in this case just a simple Azure timer function but you could use any scheduler to execute the merge
- the **merge api** - when exectuted the Merge api checks the queue for an unprocessed request and processes it
