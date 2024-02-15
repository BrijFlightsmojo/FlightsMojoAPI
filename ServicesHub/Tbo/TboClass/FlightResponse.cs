using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{
    [DataContract]
    public class FlightResponse
    {
        [DataMember]
        public Response Response { get; set; }
    }
    [DataContract]
    public class Response
    {
        [DataMember]
        public int ResponseStatus { get; set; }
        [DataMember]
        public Error Error { get; set; }
        [DataMember]
        public string TraceId { get; set; }
        [DataMember]
        public string Origin { get; set; }
        [DataMember]
        public string Destination { get; set; }
        [DataMember]
        public List<List<Itinerary>> Results { get; set; }
    }
    [DataContract]
    public class Itinerary
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
        public DateTime? LastTicketDate { get; set; }
        [DataMember]
        public string TicketAdvisory { get; set; }
        [DataMember]
        public List<FareRule> FareRules { get; set; }
        [DataMember]
        public string AirlineCode { get; set; }
        [DataMember]
        public string ValidatingAirline { get; set; }
        [DataMember]
        public fareClassification FareClassification { get; set; }
    }
    [DataContract]
    public class fareClassification
    {
        [DataMember]
        public string Color { get; set; }
        [DataMember]
        public string Type { get; set; }
    }
    [DataContract]
    public class ChargeBU
    {
        [DataMember]
        public string key { get; set; }
        [DataMember]
        public decimal value { get; set; }
    }

    [DataContract]
    public class Fare
    {
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public decimal BaseFare { get; set; }
        [DataMember]
        public decimal Tax { get; set; }
        [DataMember]
        public decimal YQTax { get; set; }
        [DataMember]
        public decimal AdditionalTxnFeeOfrd { get; set; }
        [DataMember]
        public decimal AdditionalTxnFeePub { get; set; }
        [DataMember]
        public decimal OtherCharges { get; set; }
        [DataMember]
        public List<ChargeBU> ChargeBU { get; set; }
        [DataMember]
        public decimal Discount { get; set; }
        [DataMember]
        public decimal PublishedFare { get; set; }
        [DataMember]
        public decimal CommissionEarned { get; set; }
        [DataMember]
        public decimal PLBEarned { get; set; }
        [DataMember]
        public decimal IncentiveEarned { get; set; }
        [DataMember]
        public decimal OfferedFare { get; set; }
        [DataMember]
        public decimal TdsOnCommission { get; set; }
        [DataMember]
        public decimal TdsOnPLB { get; set; }
        [DataMember]
        public decimal TdsOnIncentive { get; set; }
        [DataMember]
        public decimal ServiceFee { get; set; }
    }

    [DataContract]
    public class FareBreakdown
    {
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public string PassengerType { get; set; }
        [DataMember]
        public int PassengerCount { get; set; }
        [DataMember]
        public decimal BaseFare { get; set; }
        [DataMember]
        public decimal Tax { get; set; }
        [DataMember]
        public decimal YQTax { get; set; }
        [DataMember]
        public decimal AdditionalTxnFeeOfrd { get; set; }
        [DataMember]
        public decimal AdditionalTxnFeePub { get; set; }
        [DataMember]
        public decimal AdditionalTxnFee { get; set; }
        [DataMember]
        public decimal PGCharge { get; set; }
    }

    [DataContract]
    public class FareRule
    {
        [DataMember]
        public string Origin { get; set; }
        [DataMember]
        public string Destination { get; set; }
        [DataMember]
        public string Airline { get; set; }
        [DataMember]
        public string FareBasisCode { get; set; }
        [DataMember]
        public string FareRuleDetail { get; set; }
        [DataMember]
        public string FareRestriction { get; set; }
    }
    [DataContract]
    public class FlightSegment
    {
        [DataMember]
        public int TripIndicator { get; set; }
        [DataMember]
        public int SegmentIndicator { get; set; }
        [DataMember]
        public Airline Airline { get; set; }
        [DataMember]
        public Origin Origin { get; set; }
        [DataMember]
        public Destination Destination { get; set; }
        [DataMember]
        public int Duration { get; set; }
        [DataMember]
        public int GroundTime { get; set; }
        [DataMember]
        public int Mile { get; set; }
        [DataMember]
        public bool StopOver { get; set; }
        [DataMember]
        public string StopPoint { get; set; }
        [DataMember]
        public string StopPointArrivalTime { get; set; }
        [DataMember]
        public string StopPointDepartureTime { get; set; }
        [DataMember]
        public string Craft { get; set; }
        [DataMember]
        public bool IsETicketEligible { get; set; }
        [DataMember]
        public string FlightStatus { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public int NoOfSeatAvailable { get; set; }
    }
    [DataContract]
    public class Airline
    {
        [DataMember]
        public string AirlineCode { get; set; }
        [DataMember]
        public string AirlineName { get; set; }
        [DataMember]
        public string FlightNumber { get; set; }
        [DataMember]
        public string FareClass { get; set; }
        [DataMember]
        public string OperatingCarrier { get; set; }
    }

    [DataContract]
    public class Airport
    {
        [DataMember]
        public string AirportCode { get; set; }
        [DataMember]
        public string AirportName { get; set; }
        [DataMember]
        public string Terminal { get; set; }
        [DataMember]
        public string CityCode { get; set; }
        [DataMember]
        public string CityName { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string CountryName { get; set; }
    }

    [DataContract]
    public class Origin
    {
        [DataMember]
        public Airport Airport { get; set; }
        [DataMember]
        public DateTime DepTime { get; set; }
    }
    public class Destination
    {
        [DataMember]
        public Airport Airport { get; set; }
        [DataMember]
        public DateTime ArrTime { get; set; }
    }
}
