using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SvcResponse {

    public Boolean OK { get; set; } = true;
    public string Type {get; set;} = "LetterId";
    public string Id { get; set; } = "";
    public string ExceptionMessage { get; set; } = "";

    public SvcResponse() {

    }

    public SvcResponse(Boolean pOK, string type, string id, string pExceptionMessage) { 
        OK = pOK;
        Type = type;
        Id = id;
        ExceptionMessage = pExceptionMessage;
    }
}

