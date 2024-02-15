using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{
    [DataContract]
    public class FareRuleResponse
    {
        [DataMember]
        public ResponseFR Response { get; set; }
    }
    [DataContract]
    public class FareRuleFR
    {
        [DataMember]
        public string Airline { get; set; }
        [DataMember]
        public string Destination { get; set; }
        [DataMember]
        public string FareBasisCode { get; set; }
        [DataMember]
        public string FareRestriction { get; set; }
        [DataMember]
        public string FareRuleDetail { get; set; }
        [DataMember]
        public string Origin { get; set; }
    }
    [DataContract]
    public class ResponseFR
    {
        [DataMember]
        public Error Error { get; set; }
        [DataMember]
        public List<FareRuleFR> FareRules { get; set; }
        [DataMember]
        public int ResponseStatus { get; set; }
        [DataMember]
        public string TraceId { get; set; }
    }
}
