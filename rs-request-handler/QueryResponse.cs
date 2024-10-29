using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class QueryResponse {

    public Boolean OK { get; set; } = true;
    public string LetterId { get; set; } = "";
    public string Status { get; set; }
    public string ExceptionMessage { get; set; } = "";

    public QueryResponse() {

    }

    public QueryResponse(Boolean pOK, string pLetterId, string pExceptionMessage, string pStatus = "") {
        OK = pOK;
        LetterId = pLetterId;
        Status = pStatus;
        ExceptionMessage = pExceptionMessage;
    }
}

