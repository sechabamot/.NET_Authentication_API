using AuthenticationApp.Interfaces;
using AuthenticationApp.Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace CoTripRest.Services.Implimentations
{
    public class RecordProblems : IRecordProblems
    {
        private string JsonDataFileLocation { get;}
        private List<Problem> ExceptionsLog { get; set; }
        public IHostingEnvironment HostingEnvironment { get; }

        public RecordProblems(IHostingEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
            JsonDataFileLocation = Path.Combine(HostingEnvironment.ContentRootPath, "Data/CustomExceptionsLog.json");

            ExceptionsLog = JsonConvert.DeserializeObject<List<Problem>>(File.ReadAllText(JsonDataFileLocation));

        }

        private void UpdateJsonDataFile()
        {
            File.WriteAllText(JsonDataFileLocation, JsonConvert.SerializeObject(ExceptionsLog));
        }

        public void RecordProblem(string controllerName, string action, Exception exception)
        {
            //HostingEnvironment.IsProduction()
            if (true)
            {
                Problem cException = new Problem(controllerName, action, exception.StackTrace, exception.Message);
                ExceptionsLog.Add(cException);

                UpdateJsonDataFile();
            }
        }

        public List<Problem> ReturnListOfProblems()
        {
            return ExceptionsLog;
        }

        public void ClearListOfProblems()
        {
            ExceptionsLog.Clear();
            UpdateJsonDataFile();
        }
    }
}
