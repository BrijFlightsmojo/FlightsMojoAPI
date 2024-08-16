using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tripshope.TripshopeClass
{
    public class FareRuleRequest
    {
        public Farerulerequest farerulerequest { get; set; }
    }
    public class Farerulerequest
    {
        public Credentials credentials { get; set; }
        public string selectedflight { get; set; }
        public string selectedflighttw { get; set; }
        public string wsmode { get; set; }
    }
    public class Credentials
    {
        public string officeid { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public string agentid { get; set; }
    }

   
}
