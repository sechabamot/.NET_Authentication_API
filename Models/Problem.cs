using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthenticationApp.Models
{
    public class Problem
    {
        public Problem(string controllerName, string action, string stackTrace, string message)
        {
            Id = Guid.NewGuid().ToString();
            ControllerName = controllerName;
            Action = action;
            StackTrace = stackTrace;
            Message = message;
            DateTime = DateTime.UtcNow;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("controllerName")]
        public string ControllerName { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("stackTrace")]
        public string StackTrace { get; set; }

        [JsonPropertyName("dateTime")]
        public DateTime DateTime { get; set; }
    }
}
