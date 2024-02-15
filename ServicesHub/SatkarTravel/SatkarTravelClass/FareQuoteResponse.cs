using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.SatkarTravel.ST_FareQuote
{
    public class FareQuoteResponse
    {
        public bool isPriceChanged { get; set; }
        public int responseStatus { get; set; }
        public Results results { get; set; }
        public ArtSSRResponse artSSRResponse { get; set; }
        public bool creditLocked { get; set; }
    }

    public class ArtSSRResponse
    {
        public SsrInnerResponse ssrInnerResponse { get; set; }
    }
    public class Fare
    {
        public double cgstax { get; set; }
        public double igstax { get; set; }
        public double sgstax { get; set; }
        public double baseFare { get; set; }
        public double tax { get; set; }
        public double flat { get; set; }
        public double yQTax { get; set; }
        public double additionalTxnFeeOfrd { get; set; }
        public double additionalTxnFeePub { get; set; }
        public double pGCharge { get; set; }
        public double artGST { get; set; }
        public double artGSTOnMFee { get; set; }
        public double artTDS { get; set; }
        public double otherCharges { get; set; }
        public double discount { get; set; }
        public decimal publishedFare { get; set; }
        public decimal commissionEarned { get; set; }
        public double pLBEarned { get; set; }
        public double incentiveEarned { get; set; }
        public double artIncentive { get; set; }
        public double offeredFare { get; set; }
        public double tdsOnCommission { get; set; }
        public double tdsOnPLB { get; set; }
        public double tdsOnIncentive { get; set; }
        public double serviceFee { get; set; }
        public double totalBaggageCharges { get; set; }
        public double totalMealCharges { get; set; }
        public double totalSeatCharges { get; set; }
        public double totalSpecialServiceCharges { get; set; }
        public double transactionFee { get; set; }
        public double managementFee { get; set; }
        public double cGSTax { get; set; }
        public double sGSTax { get; set; }
        public double iGSTax { get; set; }
        public double feeSurcharges { get; set; }
        public double actualSupplierFare { get; set; }
        public string fareIdentifire { get; set; }
        public int instantDiscontOnFare { get; set; }
        public double publishedFareSoldInSwapping { get; set; }
        public double taxSoldInSwapping { get; set; }
        public double baseFareSoldInSwapping { get; set; }
        public int instantDiscountSoldInSwapping { get; set; }
        public double yqSoldInSwapping { get; set; }
        public bool isFareSwapped { get; set; }
    }
    public class FareBreakdown
    {
        public int passengerType { get; set; }
        public int passengerCount { get; set; }
        public int baseFare { get; set; }
        public double tax { get; set; }
        public int yQTax { get; set; }
        public int additionalTxnFeeOfrd { get; set; }
        public double additionalTxnFeePub { get; set; }
        public int pGCharge { get; set; }
        public int discount { get; set; }
    }
    public class Results
    {
        public string pnr { get; set; }
        public bool showFareIdentifier { get; set; }
        public bool lcc { get; set; }
        public bool freeMeal { get; set; }
        public bool passportRequiredAtBook { get; set; }
        public bool passportRequiredAtTicket { get; set; }
        public string resultSessionId { get; set; }
        public bool isFreeMeal { get; set; }
        public int source { get; set; }
        public bool isLCC { get; set; }
        public string refundable { get; set; }
        public bool dobAirAshiya { get; set; }
        public bool isHoldAllowedWithSSR { get; set; }
        public bool isUpsellAllowed { get; set; }
        public bool isCouponAppilcable { get; set; }
        public bool gSTAllowed { get; set; }
        public bool isGSTMandatory { get; set; }
        public bool IsPassportRequiredAtBook { get; set; }
        public bool IsPassportRequiredAtTicket { get; set; }
        public string airlineRemark { get; set; }
        public Fare fare { get; set; }
        public List<FareBreakdown> fareBreakdown { get; set; }
        //public List<List<>> segments { get; set; }
        public string airlineCode { get; set; }
        public string displayFareGroup { get; set; }
        public bool isPID { get; set; }
    }
    public class SsrInnerResponse
    {
        public List<object> meal { get; set; }
        public List<object> seatPreference { get; set; }
        public double responseStatus { get; set; }
        public List<object> baggage { get; set; }
        public List<object> mealDynamic { get; set; }
        public List<object> seatDynamic { get; set; }
        public List<object> specialServices { get; set; }
    }
}
