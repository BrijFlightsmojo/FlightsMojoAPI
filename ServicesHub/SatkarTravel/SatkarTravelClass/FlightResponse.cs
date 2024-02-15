using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.SatkarTravel.SatkarTravelClass
{
    public class FlightResponse
    {
        public string errorCode { get; set; }
        public int responseStatus { get; set; }
        public Error error { get; set; }
        public List<List<ResultsItem>> results { get; set; }
    }
    public class Error
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
    }
    public class Fare
    {
        public decimal cgstax { get; set; }
        public decimal igstax { get; set; }
        public decimal sgstax { get; set; }
        public decimal baseFare { get; set; }
        public decimal tax { get; set; }
        public decimal flat { get; set; }
        public decimal yQTax { get; set; }
        public decimal additionalTxnFeeOfrd { get; set; }
        public decimal additionalTxnFeePub { get; set; }
        public decimal pGCharge { get; set; }
        public decimal artGST { get; set; }
        public decimal artGSTOnMFee { get; set; }
        public decimal artTDS { get; set; }
        public decimal otherCharges { get; set; }
        public decimal discount { get; set; }
        public decimal publishedFare { get; set; }
        public decimal commissionEarned { get; set; }
        public decimal pLBEarned { get; set; }
        public decimal incentiveEarned { get; set; }
        public decimal artIncentive { get; set; }
        public decimal offeredFare { get; set; }
        public decimal tdsOnCommission { get; set; }
        public decimal tdsOnPLB { get; set; }
        public decimal tdsOnIncentive { get; set; }
        public decimal serviceFee { get; set; }
        public decimal totalBaggageCharges { get; set; }
        public decimal totalMealCharges { get; set; }
        public decimal totalSeatCharges { get; set; }
        public decimal totalSpecialServiceCharges { get; set; }
        public decimal transactionFee { get; set; }
        public decimal managementFee { get; set; }
        public decimal cGSTax { get; set; }
        public decimal sGSTax { get; set; }
        public decimal iGSTax { get; set; }
        public decimal feeSurcharges { get; set; }
        public decimal instantDiscontOnFare { get; set; }
        public decimal publishedFareSoldInSwapping { get; set; }
        public decimal taxSoldInSwapping { get; set; }
        public decimal baseFareSoldInSwapping { get; set; }
        public decimal instantDiscountSoldInSwapping { get; set; }
        public decimal yqSoldInSwapping { get; set; }
        public string isFareSwapped { get; set; }
    }
    public class FareBreakdownItem
    {
        public int passengerType { get; set; }
        public int passengerCount { get; set; }
        public int baseFare { get; set; }
        public decimal tax { get; set; }
        public decimal yQTax { get; set; }
        public decimal additionalTxnFeeOfrd { get; set; }
        public decimal additionalTxnFeePub { get; set; }
        public decimal pGCharge { get; set; }
        public decimal discount { get; set; }
    }
    public class Airline
    {
        public string airlineCode { get; set; }
        public string airlineName { get; set; }
        public string flightNumber { get; set; }
        public string fareClass { get; set; }
        public string operatingCarrier { get; set; }
    }
    public class Airport
    {
        public string airportCode { get; set; }
        public string airportName { get; set; }
        public string terminal { get; set; }
        public string cityCode { get; set; }
        public string cityName { get; set; }
    }
    public class Origin
    {
        public Airport airport { get; set; }
        public string depTime { get; set; }
        public int depTimeMnt { get; set; }
    }
    //public class Airport
    //{
    //    public string airportCode { get; set; }
    //    public string airportName { get; set; }
    //    public string terminal { get; set; }
    //    public string cityCode { get; set; }
    //}
    public class Destination
    {
        public Airport airport { get; set; }
        public string arrTime { get; set; }
        public int arrTimeMnt { get; set; }
    }
    public class SegmentsItem
    {
        public string baggage { get; set; }
        public string cabinBaggage { get; set; }
        public int tripIndicator { get; set; }
        public int segmentIndicator { get; set; }
        public Airline airline { get; set; }
        public int noOfSeatAvailable { get; set; }
        public Origin origin { get; set; }
        public Destination destination { get; set; }
        public int duration { get; set; }
        public int groundTime { get; set; }
        public int mile { get; set; }
        public string stopOver { get; set; }
        public string remark { get; set; }
        public string remisETicketEligibleark { get; set; }
        public string airlinePNR { get; set; }
        public string refundable { get; set; }
        public string lcc { get; set; }
    }
    public class ResultsItem
    {
        public string pnr { get; set; }
        public string showFareIdentifier { get; set; }
        public string lcc { get; set; }
        public string freeMeal { get; set; }
        public string passportRequiredAtBook { get; set; }
        public string passportRequiredAtTicket { get; set; }
        public string resultSessionId { get; set; }
        public string isFreeMeal { get; set; }
        public int source { get; set; }
        public string isLCC { get; set; }
        public string refundable { get; set; }
        public string dobAirAshiya { get; set; }
        public string isHoldAllowedWithSSR { get; set; }
        public string isUpsellAllowed { get; set; }
        public string isCouponAppilcable { get; set; }
        public string gSTAllowed { get; set; }
        public string isGSTMandatory { get; set; }
        public string IsPassportRequiredAtBook { get; set; }
        public string IsPassportRequiredAtTicket { get; set; }
        public string airlineRemark { get; set; }
        public Fare fare { get; set; }
        public List<FareBreakdownItem> fareBreakdown { get; set; }
        public List<List<SegmentsItem>> segments { get; set; }
        public string airlineCode { get; set; }
        public string displayFareGroup { get; set; }
        public string isPID { get; set; }
    }

}
