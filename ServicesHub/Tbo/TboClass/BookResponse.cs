using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{
    [DataContract]
    public class BookResponse
    {
        [DataMember]
        public ResponseBR Response { get; set; }
    }
    [DataContract]
    public class ResponseBR
    {
        [DataMember]
        public Error Error { get; set; }
        [DataMember]
        public string ResponseStatus { get; set; }
        [DataMember]
        public string TraceId { get; set; }
        [DataMember]
        public ResponseBR2 Response { get; set; }
    }
    [DataContract]
    public class ResponseBR2
    {
        [DataMember]
        public string PNR { get; set; }
        [DataMember]
        public int BookingId { get; set; }
        [DataMember]
        public bool SSRDenied { get; set; }
        [DataMember]
        public object SSRMessage { get; set; }
        [DataMember]
        public int Status { get; set; }
        [DataMember]
        public bool IsPriceChanged { get; set; }
        [DataMember]
        public bool IsTimeChanged { get; set; }
        [DataMember]
        public FlightItineraryBR FlightItinerary { get; set; }
    }
    [DataContract]
    public class FlightItineraryBR
    {
        [DataMember]
        public int BookingId { get; set; }
        [DataMember]
        public string PNR { get; set; }
        [DataMember]
        public bool IsDomestic { get; set; }
        [DataMember]
        public int Source { get; set; }
        [DataMember]
        public string Origin { get; set; }
        [DataMember]
        public string Destination { get; set; }
        [DataMember]
        public string AirlineCode { get; set; }
        [DataMember]
        public string ValidatingAirlineCode { get; set; }
        [DataMember]
        public string AirlineRemark { get; set; }
        [DataMember]
        public bool IsLCC { get; set; }
        [DataMember]
        public bool NonRefundable { get; set; }
        [DataMember]
        public string FareType { get; set; }
        [DataMember]
        public FareBR Fare { get; set; }
        [DataMember]
        public List<PassengerBR> Passenger { get; set; }
        [DataMember]
        public List<SegmentBR> Segments { get; set; }
        [DataMember]
        public List<FareRuleBR> FareRules { get; set; }
        [DataMember]
        public int Status { get; set; }
        [DataMember]
        public int ResponseStatus { get; set; }
        [DataMember]
        public string TraceId { get; set; }
    }
    [DataContract]
    public class FareBR
    {
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public double BaseFare { get; set; }
        [DataMember]
        public double Tax { get; set; }
        [DataMember]
        public double YQTax { get; set; }
        [DataMember]
        public double AdditionalTxnFeeOfrd { get; set; }
        [DataMember]
        public double AdditionalTxnFeePub { get; set; }
        [DataMember]
        public double OtherCharges { get; set; }
        [DataMember]
        public List<ChargeBU> ChargeBU { get; set; }
        [DataMember]
        public double Discount { get; set; }
        [DataMember]
        public double PublishedFare { get; set; }
        [DataMember]
        public double CommissionEarned { get; set; }
        [DataMember]
        public double PLBEarned { get; set; }
        [DataMember]
        public double IncentiveEarned { get; set; }
        [DataMember]
        public double OfferedFare { get; set; }
        [DataMember]
        public double TdsOnCommission { get; set; }
        [DataMember]
        public double TdsOnPLB { get; set; }
        [DataMember]
        public double TdsOnIncentive { get; set; }
        [DataMember]
        public double ServiceFee { get; set; }
    }
    [DataContract]
    public class PassengerBR
    {
        [DataMember]
        public int PaxId { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public int PaxType { get; set; }
        [DataMember]
        public string DateOfBirth { get; set; }
        [DataMember]
        public int Gender { get; set; }
        [DataMember]
        public string PassportNo { get; set; }
        [DataMember]
        public string PassportExpiry { get; set; }
        [DataMember]
        public string AddressLine1 { get; set; }
        [DataMember]
        public string AddressLine2 { get; set; }
        [DataMember]
        public FareBR Fare { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string CountryName { get; set; }
        [DataMember]
        public string Nationality { get; set; }
        [DataMember]
        public string ContactNo { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public bool IsLeadPax { get; set; }
        [DataMember]
        public object FFAirlineCode { get; set; }
        [DataMember]
        public string FFNumber { get; set; }
        [DataMember]
        public Meal Meal { get; set; }
        [DataMember]
        public Seat Seat { get; set; }
    }

    [DataContract]
    public class AirlineBR
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
    public class AirportBR
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
    public class OriginBR
    {
        [DataMember]
        public AirportBR Airport { get; set; }
        [DataMember]
        public string DepTime { get; set; }
    }
    [DataContract]
    public class DestinationBR
    {
        [DataMember]
        public AirportBR Airport { get; set; }
        [DataMember]
        public string ArrTime { get; set; }
    }

    [DataContract]
    public class SegmentBR
    {
        [DataMember]
        public int TripIndicator { get; set; }
        [DataMember]
        public int SegmentIndicator { get; set; }
        [DataMember]
        public AirlineBR Airline { get; set; }
        [DataMember]
        public string AirlinePNR { get; set; }
        [DataMember]
        public OriginBR Origin { get; set; }
        [DataMember]
        public DestinationBR Destination { get; set; }
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
        public int? AccumulatedDuration { get; set; }
    }

    [DataContract]
    public class FareRuleBR
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
        public string FareRestriction { get; set; }
        [DataMember]
        public string FareRuleDetail { get; set; }
    }
}
