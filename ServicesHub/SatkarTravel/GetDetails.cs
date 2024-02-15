using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.SatkarTravel.ST_GetDetails
{
    public class GetDetails
    {
        public int seriesId { get; set; }
        public int availSeat { get; set; }
        public double agencyMarkup { get; set; }
        public bool reschedule { get; set; }
        public string txid { get; set; }
        public string bookingstatus { get; set; }
        public int status { get; set; }
        public int Id { get; set; }
        public string triptype { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public decimal totalfare { get; set; }
        public int adultcount { get; set; }
        public int childcount { get; set; }
        public int infantcount { get; set; }
        public string bookingclass { get; set; }
        public double totalbasefare { get; set; }
        public long bookdate { get; set; }
        public double offeredFare { get; set; }
        public List<FlightBookingSecotorDetail> FlightBookingSecotorDetails { get; set; }
        public string bookingtype { get; set; }
        public bool showHideFare { get; set; }
        public bool showHideAgency { get; set; }
        public bool showHideLogo { get; set; }
        public bool isReschedule { get; set; }
        public int totalrecords { get; set; }
    }

    public class FlightBookingSecotorDetail
    {
        public int Id { get; set; }
        public string bookingtype { get; set; }
        public string carriertype { get; set; }
        public string airlines { get; set; }
        public string flightnumber { get; set; }
        public string departureairport { get; set; }
        public string arrivalairport { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string departuredate { get; set; }
        public string arrivaldate { get; set; }
        public string departuretime { get; set; }
        public string arrivaltime { get; set; }
        public string departureterminal { get; set; }
        public string arrivalterminal { get; set; }
        public string refundable { get; set; }
        public int duration { get; set; }
        public double offeredFare { get; set; }
        public double yqCommission { get; set; }
        public double additionalmarkup { get; set; }
        public double totalbasefare { get; set; }
        public double fuelcharges { get; set; }
        public double managementFeeGST { get; set; }
        public double commissionGST { get; set; }
        public double totaltax { get; set; }
        public double feeAndSurcharge { get; set; }
        public double totalcommission { get; set; }
        public double totalfare { get; set; }
        public string bookingstatus { get; set; }
        public string faretype { get; set; }
        public int totalBaggageCharges { get; set; }
        public int totalMealCharges { get; set; }
        public int totalSeatCharges { get; set; }
        public int totalSpecialServiceCharges { get; set; }
        public List<PassengerInfo> passengerInfo { get; set; }
        public List<BookingSegment> bookingSegment { get; set; }
        public double artPf { get; set; }
        public string airlinepnr { get; set; }
        public string gdspnr { get; set; }
        public int supplierId { get; set; }
        public bool select { get; set; }
        public bool showDB { get; set; }
        public bool isTimeChange { get; set; }
        public bool isPriceChange { get; set; }
        public string cabinClass { get; set; }
    }

    public class PassengerInfo
    {
        public int seriesId { get; set; }
        public string website { get; set; }
        public int Id { get; set; }
        public string salutation { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string passenger_type { get; set; }
        public string mobileNumber { get; set; }
        public string email { get; set; }
        public string ticketId { get; set; }
        public string airlinepnr { get; set; }
        public string gdspnr { get; set; }
        public string pnrstatus { get; set; }
        public bool selectpax { get; set; }
        public List<SsrFare> ssrFare { get; set; }
        public List<object> ssrMeal { get; set; }
        public List<object> ssrSeat { get; set; }
        public List<object> ssBaggage { get; set; }
        public string barCodeData { get; set; }
    }

    public class BookingSegment
    {
        public bool eticketEligible { get; set; }
        public int Id { get; set; }
        public string baggage { get; set; }
        public string cabinBaggage { get; set; }
        public int tripIndicator { get; set; }
        public int segmentIndicator { get; set; }
        public Airline airline { get; set; }
        public int NoOfSeatAvailable { get; set; }
        public Origin origin { get; set; }
        public Destination destination { get; set; }
        public int duration { get; set; }
        public int groundTime { get; set; }
        public int Mile { get; set; }
        public bool stopOver { get; set; }
        public string remark { get; set; }
        public bool isETicketEligible { get; set; }
        public string airlinePNR { get; set; }
    }

    public class SsrFare
    {
        public double manageMentFeeWithGST { get; set; }
        public int Id { get; set; }
        public double baseFare { get; set; }
        public double tax { get; set; }
        public int yQTax { get; set; }
        public int additionalTxnFeeOfrd { get; set; }
        public double additionalTxnFeePub { get; set; }
        public int pGCharge { get; set; }
        public int otherCharges { get; set; }
        public int discount { get; set; }
        public double publishedFare { get; set; }
        public int commissionEarned { get; set; }
        public int pLBEarned { get; set; }
        public int incentiveEarned { get; set; }
        public int offeredFare { get; set; }
        public int tdsOnCommission { get; set; }
        public int tdsOnPLB { get; set; }
        public int tdsOnIncentive { get; set; }
        public int serviceFee { get; set; }
        public int totalBaggageCharges { get; set; }
        public int totalMealCharges { get; set; }
        public int totalSeatCharges { get; set; }
        public int totalSpecialServiceCharges { get; set; }
        public double artPf { get; set; }
        public double instantDiscountOnFareSwapping { get; set; }
    }


    public class Airline
    {
        public int id { get; set; }
        public string airlineCode { get; set; }
        public string airlineName { get; set; }
        public string flightNumber { get; set; }
        public string fareClass { get; set; }
        public string operatingCarrier { get; set; }
    }



    public class Destination
    {
        public int id { get; set; }
        public string airportCode { get; set; }
        public string airportName { get; set; }
        public string terminal { get; set; }
        public string cityCode { get; set; }
        public string cityName { get; set; }
        public string countryCode { get; set; }
        public string arrivaldate { get; set; }
        public string arrivaltime { get; set; }
    }



    public class Origin
    {
        public int id { get; set; }
        public string airportCode { get; set; }
        public string airportName { get; set; }
        public string terminal { get; set; }
        public string cityCode { get; set; }
        public string cityName { get; set; }
        public string countryCode { get; set; }
        public string departuredate { get; set; }
        public string departuretime { get; set; }
    }

}
