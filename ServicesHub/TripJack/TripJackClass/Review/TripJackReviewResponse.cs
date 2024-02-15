using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.TripJack.TripJackClass.Review
{
   
    public class TripJackReviewResponse
    {
        public List<TripInfo> tripInfos { get; set; }
        public SearchQuery searchQuery { get; set; }
        public string bookingId { get; set; }
        public TotalPriceInfo totalPriceInfo { get; set; }
        public Status status { get; set; }
        public Conditions conditions { get; set; }
        public List<Error> errors { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
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

    public class ADULT
    {
        public FC fC { get; set; }
        public AfC afC { get; set; }
        public int sR { get; set; }
        public BI bI { get; set; }
        public int rT { get; set; }
        public string cc { get; set; }
        public string cB { get; set; }
        public string fB { get; set; }
        public bool mI { get; set; }
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

    public class BAGGAGE
    {
        public string code { get; set; }
        public decimal amount { get; set; }
        public string desc { get; set; }
    }

    public class BI
    {
        public string iB { get; set; }
        public string cB { get; set; }
    }

    public class CHILD
    {
        public FC fC { get; set; }
        public AfC afC { get; set; }
        public int sR { get; set; }
        public BI bI { get; set; }
        public int rT { get; set; }
        public string cc { get; set; }
        public string cB { get; set; }
        public string fB { get; set; }
        public bool mI { get; set; }
    }

    public class Conditions
    {
        public List<object> ffas { get; set; }
        public bool isa { get; set; }
        public Dob dob { get; set; }
        public bool iecr { get; set; }
        public Dc dc { get; set; }
        public bool isBA { get; set; }
        public int st { get; set; }
        public DateTime sct { get; set; }
        public Gst gst { get; set; }
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

    public class Dc
    {
        public bool ida { get; set; }
        public bool idm { get; set; }
    }

    public class Dob
    {
        public bool adobr { get; set; }
        public bool cdobr { get; set; }
        public bool idobr { get; set; }
    }

    public class FC
    {
        public decimal BF { get; set; }
        public decimal TAF { get; set; }
        public decimal NF { get; set; }
        public decimal TF { get; set; }
        public decimal NCM { get; set; }
    }

    public class FD
    {
        public AI aI { get; set; }
        public string fN { get; set; }
        public string eT { get; set; }
    }

    public class Fd2
    {
        public ADULT ADULT { get; set; }
        public CHILD CHILD { get; set; }
    }

    public class FromCityOrAirport
    {
        public string code { get; set; }
        public string name { get; set; }
        public string cityCode { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
    }

    public class Gst
    {
        public bool gstappl { get; set; }
        public bool igm { get; set; }
    }

    public class MEAL
    {
        public string code { get; set; }
        public decimal amount { get; set; }
        public string desc { get; set; }
    }

    public class PaxInfo
    {
        public int ADULT { get; set; }
        public int CHILD { get; set; }
        public int INFANT { get; set; }
    }

    public class Pc
    {
        public string code { get; set; }
        public string name { get; set; }
        public bool isLcc { get; set; }
    }

   

    public class RouteInfo
    {
        public FromCityOrAirport fromCityOrAirport { get; set; }
        public ToCityOrAirport toCityOrAirport { get; set; }
        public string travelDate { get; set; }
    }

    public class SearchModifiers
    {
        public bool isDirectFlight { get; set; }
        public bool isConnectingFlight { get; set; }
        public string pft { get; set; }
    }

    public class SearchQuery
    {
        public List<RouteInfo> routeInfos { get; set; }
        public string cabinClass { get; set; }
        public PaxInfo paxInfo { get; set; }
        public string searchType { get; set; }
        public SearchModifiers searchModifiers { get; set; }
        public bool isDomestic { get; set; }
    }

    public class SI
    {
        public string id { get; set; }
        public FD fD { get; set; }
        public int stops { get; set; }
        public List<object> so { get; set; }
        public int duration { get; set; }
        public int cT { get; set; }
        public Da da { get; set; }
        public Aa aa { get; set; }
        public string dt { get; set; }
        public string at { get; set; }
        public bool iand { get; set; }
        public bool isRs { get; set; }
        public int sN { get; set; }
        public SsrInfo ssrInfo { get; set; }
    }

    public class SsrInfo
    {
        public List<MEAL> MEAL { get; set; }
        public List<BAGGAGE> BAGGAGE { get; set; }
    }

    public class Status
    {
        public bool success { get; set; }
        public int httpStatus { get; set; }
    }

    public class TAF
    {
        public decimal YQ { get; set; }
        public decimal OT { get; set; }
        public decimal MF { get; set; }
        public decimal MFT { get; set; }
        public decimal AGST { get; set; }
    }

    public class ToCityOrAirport
    {
        public string code { get; set; }
        public string name { get; set; }
        public string cityCode { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
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

    public class TotalPriceList
    {
        public Fd2 fd { get; set; }
        public string fareIdentifier { get; set; }
        public string id { get; set; }
        public List<object> messages { get; set; }
        public Pc pc { get; set; }
    }

    public class TripInfo
    {
        public List<SI> sI { get; set; }
        public List<TotalPriceList> totalPriceList { get; set; }
    }





  

    public class Error
    {
        public string errCode { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public string id { get; set; }
    }



}
