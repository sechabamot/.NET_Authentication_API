using AuthenticationApp.Models;
using System;
using System.Collections.Generic;

namespace AuthenticationApp.Interfaces
{
    public interface IRecordProblems
    {
        void RecordProblem(string controllerName, string action, Exception exception);
        List<Problem> ReturnListOfProblems();
        void ClearListOfProblems();
    }
}
