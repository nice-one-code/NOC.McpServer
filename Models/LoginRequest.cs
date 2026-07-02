using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NOC.McpServer.Models
{
    public class LoginRequest
    {
        [JsonPropertyName("UserId")]
        public string UserId { get; set; } = "";

        [JsonPropertyName("Password")]
        public string Password { get; set; } = "";
    }
}
