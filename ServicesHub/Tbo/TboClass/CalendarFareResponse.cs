using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{
    [DataContract]
    public class CalendarFareResponse
    {
        [DataMember]
        public ResponseCFR Response { get; set; }
    }
    [DataContract]
    public class ResponseCFR
    {
        [DataMember]
        public int ResponseStatus { get; set; }
        [DataMember]
        public ErrorCFR Error { get; set; }
        [DataMember]
        public string TraceId { get; set; }
        [DataMember]
        public string Origin { get; set; }
        [DataMember]
        public string Destination { get; set; }
        [DataMember]
        public List<SearchResultCFR> SearchResults { get; set; }
    }
    [DataContract]
    public class ErrorCFR
    {
        [DataMember]
        public int ErrorCode { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
    }

    [DataContract]
    public class SearchResultCFR
    {
        [DataMember]
        public string AirlineCode { get; set; }
        [DataMember]
        public string AirlineName { get; set; }
        [DataMember]
        public DateTime DepartureDate { get; set; }
        [DataMember]
        public bool IsLowestFareOfMonth { get; set; }
        [DataMember]
        public decimal Fare { get; set; }
        [DataMember]
        public decimal BaseFare { get; set; }
        [DataMember]
        public decimal Tax { get; set; }
        [DataMember]
        public decimal OtherCharges { get; set; }
        [DataMember]
        public decimal FuelSurcharge { get; set; }
    }
}
