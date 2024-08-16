using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tripshope
{
    [DataContract]
    public class SearchRequest
    {
        [DataMember]
        public Flightsearchrequest flightsearchrequest { get; set; }
    }
    [DataContract]
    public class Flightsearchrequest
    {
        [DataMember]
        public Credentials credentials { get; set; }
        [DataMember]
        public string destination { get; set; }
        [DataMember]
        public string origin { get; set; }
        [DataMember]
        public string journeytype { get; set; }
        [DataMember]
        public int numadults { get; set; }
        [DataMember]
        public int numchildren { get; set; }
        [DataMember]
        public int numinfants { get; set; }
        [DataMember]
        public int numresults { get; set; }
        [DataMember]
        public string onwarddate { get; set; }
        [DataMember]
        public string returndate { get; set; }
        [DataMember]
        public string prefcarrier { get; set; }
        [DataMember]
        public string prefclass { get; set; }
        [DataMember]
        public string requestformat { get; set; }
        [DataMember]
        public string resultformat { get; set; }
        [DataMember]
        public string searchmode { get; set; }
        [DataMember]
        public string searchtype { get; set; }
        [DataMember]
        public string sortkey { get; set; }
        [DataMember]
        public string promocode { get; set; }
        [DataMember]
        public string actionname { get; set; }
    }

    [DataContract]
    public class Credentials
    {
        [DataMember]
        public string officeid { get; set; }
        [DataMember]
        public string password { get; set; }
        [DataMember]
        public string username { get; set; }
    }

}
