using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.TripJack.TripJackClass.BookingResponse
{
    public class TripJackFlightBookingResponse
    {
        public Order order { get; set; }
        public ItemInfos itemInfos { get; set; }
        public GstInfo gstInfo { get; set; }
        public Status status { get; set; }
    }

    public class Aa
    {
        public string code { get; set; }
        public string name { get; set; }
        public string cityCode { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string terminal { get; set; }
    }

    public class AfC
    {
        public TAF TAF { get; set; }
    }

    public class AI
    {
        public string code { get; set; }
        public string name { get; set; }
        public bool isLcc { get; set; }
    }

    public class AIR
    {
        public List<TripInfo> tripInfos { get; set; }
        public List<TravellerInfo> travellerInfos { get; set; }
        public TotalPriceInfo totalPriceInfo { get; set; }
    }

    public class Da
    {
        public string code { get; set; }
        public string name { get; set; }
        public string cityCode { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string terminal { get; set; }
    }

    public class DeliveryInfo
    {
        public List<string> emails { get; set; }
        public List<string> contacts { get; set; }
    }

    public class FC
    {
        public double TF { get; set; }
        public double IGST { get; set; }
        public double TAF { get; set; }
        public double NF { get; set; }
        public double BF { get; set; }
    }

    public class FD
    {
        public AI aI { get; set; }
        public string fN { get; set; }
        public string eT { get; set; }
    }

    public class GstInfo
    {
        public string gstNumber { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }
        public string registeredName { get; set; }
        public string bookingId { get; set; }
        public string bookingUserId { get; set; }
        public int id { get; set; }
    }

    public class ItemInfos
    {
        public AIR AIR { get; set; }
    }

    public class Order
    {
        public string bookingId { get; set; }
        public double amount { get; set; }
        public double markup { get; set; }
        public DeliveryInfo deliveryInfo { get; set; }
        public string status { get; set; }
        public DateTime createdOn { get; set; }
    }

    public class PnrDetails
    {
        //[JsonProperty("DEL-CCU")]
        public object DELCCU { get; set; }

        //[JsonProperty("CCU-BOM")]
        public string CCUBOM { get; set; }

        [JsonProperty("BOM-DEL")]
        public string BOMDEL { get; set; }
    }

    public class Root
    {

    }

    public class SI
    {
        public string id { get; set; }
        public FD fD { get; set; }
        public int stops { get; set; }
        public int duration { get; set; }
        public int cT { get; set; }
        public Da da { get; set; }
        public Aa aa { get; set; }
        public string dt { get; set; }
        public string at { get; set; }
        public bool iand { get; set; }
        public bool isRs { get; set; }
        public int sN { get; set; }
    }

    public class Status
    {
        public bool success { get; set; }
        public int httpStatus { get; set; }
    }

    public class TAF
    {
        public double MF { get; set; }
        public double YQ { get; set; }
        public double AGST { get; set; }
        public double MFT { get; set; }
        public double OT { get; set; }
    }

    public class TotalFareDetail
    {
        public FC fC { get; set; }
        public AfC afC { get; set; }
    }

    public class TotalPriceInfo
    {
        public TotalFareDetail totalFareDetail { get; set; }
    }

    public class TravellerInfo
    {
        public PnrDetails pnrDetails { get; set; }
        public string ti { get; set; }
        public string pt { get; set; }
        public string fN { get; set; }
        public string lN { get; set; }
        public string dob { get; set; }
    }

    public class TripInfo
    {
        public List<SI> sI { get; set; }
    }


}
