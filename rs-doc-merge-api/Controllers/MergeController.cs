using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Diagnostics.Tracing;
using System.Diagnostics;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using rs_doc_merge_api.Models;

namespace rs_doc_merge_api.Controllers {
    [ApiController]
    [Route("api/v1/[action]")]
    public class MergeController : ControllerBase {

        private IConfiguration _configuration;
        private readonly ILogger<MergeController> _logger;
        private readonly ILogger<LetterMerge> _loggerLetter;

        public MergeController(IConfiguration configuration, ILogger<MergeController> logger, ILogger<LetterMerge> logger1) {
            _configuration = configuration;
            _logger = logger;
            _loggerLetter = logger1;
        }

        [HttpGet]
        public ActionResult<string> Index() {
            string outputlocation = _configuration.GetValue<string>("outputLocation");
            _logger.Log(LogLevel.Information, "Connecting to db");
            try {
                Models.DBConnection letterDb = new Models.DBConnection(_configuration, "db_connection");
                if (letterDb.IsOpen()) {
                    _logger.Log(LogLevel.Information, "Db Is Open");
                }
            } catch (Exception ex) {
                _logger.Log(LogLevel.Error, ex.Message);
            }
            return outputlocation;
        }

        [HttpGet]
        public ActionResult<string> GoMerge() {
            string _errors = "";
            Guid requestId = Guid.NewGuid();

            _logger.Log(LogLevel.Information, "In Merge Api");

            string outputlocation = _configuration.GetValue<string>("outputLocation");
            Models.DBConnection letterDb = new Models.DBConnection(_configuration, "db_connection");

            try {
                string _sSql = "";
                _sSql = @"select top 1 requestid from dbo.letterqueue where processing = 0 and status = 'new' order by priority, createddate desc";

                SqlCommand myCommand = new SqlCommand(_sSql, letterDb.GetDBConnection());
                SqlDataReader mySqlRS = myCommand.ExecuteReader();

                if (mySqlRS.HasRows) {
                    mySqlRS.Read();
                    Trace.WriteLine("Found a letter!!!");
                    requestId = new Guid(mySqlRS["requestId"].ToString());
                } else {
                    Trace.WriteLine("No letter found!!!");
                };
                mySqlRS.Close();
                myCommand.Dispose();
            } catch (Exception err) {
                _logger.Log(LogLevel.Error, err.Message);
                _errors += err.Message;
                return _errors;
            }
            try {
                using (var command = new SqlCommand("update letterqueue set processing=1, startdate=getdate() where requestid = @requestid", letterDb.GetDBConnection())) {
                    command.Parameters.AddWithValue("@requestid", requestId);
                    command.ExecuteNonQuery();
                }
                Models.LetterMerge letterMerge = new Models.LetterMerge(_configuration, requestId, outputlocation, _loggerLetter, "NO");
                _errors = letterMerge._errors;
            } catch (Exception ex) {
                _logger.Log(LogLevel.Error, ex.Message);
                _errors = ex.Message.ToString();
            } finally {
                letterDb.Close();
            }
            return _errors;
        }
    }
}