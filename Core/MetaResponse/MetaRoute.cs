using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.MetaResponse
{
    [DataContract]
    public class MetaRoute
    {
        [DataMember]
        public List<string> locale { get; set; }
        [DataMember]
        public List<Route> routes { get; set; }
    }

    [DataContract]
    public class Route
    {
        [DataMember]
        public string OriginAirportCode { get; set; }
        [DataMember]
        public string DestinationtAirportCode { get; set; }
    }
    [DataContract]
    public class RouteSyc
    {
        [DataMember]
        public string origin { get; set; }
        [DataMember]
        public string days_of_week { get; set; }
        [DataMember]
        public string destination { get; set; }
        [DataMember]
        public string effective_start { get; set; }
        [DataMember]
        public string effective_end { get; set; }
    }
}
