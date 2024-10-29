# Rock-Scissors

_Rock-Scissors_ is a group of applications used to request and merge data (formatted as XML) with a Word document template to produce high fidelity PDF reports, letters etc. For purposes of simplicity, this solution uses a SQL Server db as the repository and queue for the document requests.

Included in this solution are:
- the _db schema_ to spin up the database
- the _request handler_ which is a .net core 6 API to add and query a request as well as add a batch to wrap a set of requests as a group of requests
- an _event handler_ - in this case just a simple Azure timer function but you could use any scheduler to execute the merge
- the _merge api_ - when exectuted the Merge api checks the queue for an unprocessed request and processes it

