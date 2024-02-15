using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{
    public class FareQuoteRequest
    {
        [DataMember]
        public string EndUserIp { get; set; }
        [DataMember]
        public string TokenId { get; set; }
        [DataMember]
        public string TraceId { get; set; }
        [DataMember]
        public string ResultIndex { get; set; }
    }

    [DataContract]
    public class FareQuoteResponse
    {
        [DataMember]
        public FQResponse Response { get; set; }
    }
    [DataContract]
    public class FQResponse
    {
        [DataMember]
        public Error Error { get; set; }
        [DataMember]
        public bool IsPriceChanged { get; set; }
        [DataMember]
        public decimal fareIncreaseAmount { get; set; }
        [DataMember]
        public int ResponseStatus { get; set; }
        [DataMember]
        public Results Results { get; set; }
        [DataMember]
        public string TraceId { get; set; }
    }
    [DataContract]
    public class Results
    {
        [DataMember]
        public string ResultIndex { get; set; }
        [DataMember]
        public int Source { get; set; }
        [DataMember]
        public bool IsLCC { get; set; }
        [DataMember]
        public bool IsRefundable { get; set; }
        [DataMember]
        public string AirlineRemark { get; set; }
        [DataMember]
        public Fare Fare { get; set; }
        [DataMember]
        public List<FareBreakdown> FareBreakdown { get; set; }
        [DataMember]
        public List<List<FlightSegment>> Segments { get; set; }
        [DataMember]
        public object LastTicketDate { get; set; }
        [DataMember]
        public object TicketAdvisory { get; set; }
        [DataMember]
        public List<FareRule> FareRules { get; set; }
        [DataMember]
        public string AirlineCode { get; set; }
        [DataMember]
        public string ValidatingAirline { get; set; }
        [DataMember]
        public bool IsGSTMandatory { get; set; }
    }
}
