using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{
    [DataContract]
    public class FareLTRes
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
    public class BaggageLTRes
    {
        [DataMember]
        public int WayType { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public int Description { get; set; }
        [DataMember]
        public int Weight { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public double Price { get; set; }
        [DataMember]
        public string Origin { get; set; }
        [DataMember]
        public string Destination { get; set; }
    }

    [DataContract]
    public class MealDynamicLTRes
    {
        [DataMember]
        public int WayType { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public int Description { get; set; }
        [DataMember]
        public string AirlineDescription { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public double Price { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public string Origin { get; set; }
        [DataMember]
        public string Destination { get; set; }
    }

    [DataContract]
    public class TicketLTRes
    {
        [DataMember]
        public int TicketId { get; set; }
        [DataMember]
        public string TicketNumber { get; set; }
        [DataMember]
        public string IssueDate { get; set; }
        [DataMember]
        public string ValidatingAirline { get; set; }
        [DataMember]
        public string Remarks { get; set; }
        [DataMember]
        public string ServiceFeeDisplayType { get; set; }
        [DataMember]
        public string Status { get; set; }
    }

    [DataContract]
    public class SegmentAdditionalInfoLTRes
    {
        [DataMember]
        public string FareBasis { get; set; }
        [DataMember]
        public DateTime? NVA { get; set; }
        [DataMember]
        public DateTime? NVB { get; set; }
        [DataMember]
        public string Baggage { get; set; }
        [DataMember]
        public string Meal { get; set; }
    }

    [DataContract]
    public class PassengerLTRes
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
        public FareLTRes Fare { get; set; }
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
        public string FFAirlineCode { get; set; }
        [DataMember]
        public string FFNumber { get; set; }
        [DataMember]
        public List<BaggageLTRes> Baggage { get; set; }
        [DataMember]
        public List<MealDynamicLTRes> MealDynamic { get; set; }
        [DataMember]
        public TicketLTRes Ticket { get; set; }
        [DataMember]
        public List<SegmentAdditionalInfoLTRes> SegmentAdditionalInfo { get; set; }
    }

    [DataContract]
    public class AirlineLTRes
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
    public class AirportLTRes
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
    public class OriginLTRes
    {
        [DataMember]
        public AirportLTRes Airport { get; set; }
        [DataMember]
        public string DepTime { get; set; }
    }

    [DataContract]
    public class DestinationLTRes
    {
        [DataMember]
        public AirportLTRes Airport { get; set; }
        [DataMember]
        public string ArrTime { get; set; }
    }

    [DataContract]
    public class SegmentLTRes
    {
        [DataMember]
        public int TripIndicator { get; set; }
        [DataMember]
        public int SegmentIndicator { get; set; }
        [DataMember]
        public AirlineLTRes Airline { get; set; }
        [DataMember]
        public string AirlinePNR { get; set; }
        [DataMember]
        public OriginLTRes Origin { get; set; }
        [DataMember]
        public DestinationLTRes Destination { get; set; }
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
    }

    [DataContract]
    public class FareRuleLTRes
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
    public class FlightItineraryLTRes
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
        public string AirlineTollFreeNo { get; set; }
        [DataMember]
        public bool IsLCC { get; set; }
        [DataMember]
        public bool NonRefundable { get; set; }
        [DataMember]
        public string FareType { get; set; }
        [DataMember]
        public FareLTRes Fare { get; set; }
        [DataMember]
        public List<PassengerLTRes> Passenger { get; set; }
        [DataMember]
        public List<SegmentLTRes> Segments { get; set; }
        [DataMember]
        public List<FareRuleLTRes> FareRules { get; set; }
        [DataMember]
        public int Status { get; set; }
        [DataMember]
        public decimal InvoiceAmount { get; set; }
        [DataMember]
        public string InvoiceNo { get; set; }
        [DataMember]
        public string InvoiceCreatedOn { get; set; }
        [DataMember]
        public int TicketStatus { get; set; }
        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public class ResponseLTRes2
    {
        [DataMember]
        public string PNR { get; set; }
        [DataMember]
        public int BookingId { get; set; }
        [DataMember]
        public bool SSRDenied { get; set; }
        [DataMember]
        public string SSRMessage { get; set; }
        [DataMember]
        public bool IsPriceChanged { get; set; }
        [DataMember]
        public bool IsTimeChanged { get; set; }
        [DataMember]
        public FlightItineraryLTRes FlightItinerary { get; set; }
        [DataMember]
        public int ResponseStatus { get; set; }
        [DataMember]
        public string TraceId { get; set; }

        [DataMember]
        public string InvoiceNo { get; set; }
    }

    [DataContract]
    public class ResponseLTRes
    {
        [DataMember]
        public Error Error { get; set; }
        [DataMember]
        public string ResponseStatus { get; set; }
        [DataMember]
        public ResponseLTRes2 Response { get; set; }
    }

    [DataContract]
    public class LccTicketingResponse
    {
        [DataMember]
        public ResponseLTRes Response { get; set; }
    }

}
