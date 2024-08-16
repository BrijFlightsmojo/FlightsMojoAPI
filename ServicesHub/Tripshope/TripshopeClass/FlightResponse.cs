using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tripshope.TripshopeClass
{
   
    public class SearchResponse
    {
        public Flightsearchresponse flightsearchresponse { get; set; }
    }
    public class Flightsearchresponse
    {
        public int totalresults { get; set; }
        public int numpages { get; set; }
        public int currentpagenumber { get; set; }
        public string statuscode { get; set; }
        public string statusmessage { get; set; }
        public string nextrasearchkey { get; set; }
        public string deeplinkurl { get; set; }
        public string searchresulttype { get; set; }
        public string domint { get; set; }
        public Flightsearchrequest searchrequest { get; set; }
        public List<Flightjourney> flightjourneys { get; set; }
        //public List<object> faretypeinclusions { get; set; }
        public string origincountrycode { get; set; }
        public string destinationcountrycode { get; set; }
        public string farebreakupformat { get; set; }
    }
    public class Flightfare
    {
        public decimal totalbasefare { get; set; }
        public string currency { get; set; }
        public decimal totaltax { get; set; }
        public decimal totalyq { get; set; }
        public decimal totalnet { get; set; }
        public decimal discount { get; set; }
        public decimal servicetax { get; set; }
        public decimal tds { get; set; }
        public decimal servicefee { get; set; }
        public decimal markup { get; set; }
        public decimal plb { get; set; }
        public decimal transactionfee { get; set; }
        public decimal adultbasefare { get; set; }
        public decimal adulttax { get; set; }
        public decimal adultyq { get; set; }
        public decimal childbasefare { get; set; }
        public decimal childtax { get; set; }
        public decimal childyq { get; set; }
        public decimal infantbasefare { get; set; }
        public decimal infanttax { get; set; }
        public decimal infantyq { get; set; }
        public string faretype { get; set; }
        public decimal totalfq { get; set; }
        public decimal adultinvoice { get; set; }
        public decimal childinvoice { get; set; }
        public decimal infantinvoice { get; set; }
        public decimal handlingcharges { get; set; }
        public decimal adtreissuecharge { get; set; }
        public decimal chdreissuecharge { get; set; }
        public decimal inftreissuecharge { get; set; }
        public string cancelable { get; set; }
        public string changable { get; set; }
        public string ticketbydate { get; set; }
        public string cabinbaggage { get; set; }
        public decimal originalbasefare { get; set; }
        public decimal originaltotalnet { get; set; }
        public decimal originaltotalfq { get; set; }
        public string faretype_supplier { get; set; }
        public string issplrtfare { get; set; }
        public List<Othertaxesbreakup> othertaxesbreakup { get; set; }
        public List<object> paxothertaxesbreakup { get; set; }
        public List<object> amenities { get; set; }
        public string refundableinfo { get; set; }
    }

    public class Flightjourney
    {
        public List<Flightoption> flightoptions { get; set; }
    }

    public class Flightleg
    {
        public string origin { get; set; }
        public string destination { get; set; }
        public string carrier { get; set; }
        public string validatingcarrier { get; set; }
        public string validatingcarriername { get; set; }
        public string carriername { get; set; }
        public string flightnumber { get; set; }
        public string bookingclass { get; set; }
        public string deptime { get; set; }
        public string arrtime { get; set; }
        public string depdate { get; set; }
        public string arrdate { get; set; }
        public int journeyduration { get; set; }
        public string cabinclass { get; set; }
        public string origin_name { get; set; }
        public string destination_name { get; set; }
        public string ontimeinfo { get; set; }
        public string equipment { get; set; }
        public string weather_origin { get; set; }
        public string weather_destination { get; set; }
        public string stopover { get; set; }
        public string stopoverinfo { get; set; }
        public string depterminal { get; set; }
        public string arrterminal { get; set; }
        public int numseatsavailable { get; set; }
        public string earlierbookingcount { get; set; }
        public string transitvisainfo { get; set; }
        public string farebasiscode { get; set; }
        public string issbt { get; set; }
        public string inpolicy { get; set; }
        public string policyid { get; set; }
        public string bagweight { get; set; }
        public string bagunit { get; set; }
        public string mileage { get; set; }
        public string specialinfo { get; set; }
        public int totalduration { get; set; }
        public string checkinbagweight { get; set; }
        public string checkinbagunit { get; set; }
        public string baggagetext { get; set; }
        public string operatedby { get; set; }
    }

    public class Flightoption
    {
        public List<Recommendedflight> recommendedflight { get; set; }
    }

    //public class Flightsearchrequest
    //{
    //    public string origin { get; set; }
    //    public string destination { get; set; }
    //    public string onwarddate { get; set; }
    //    public string returndate { get; set; }
    //    public int numadults { get; set; }
    //    public int numchildren { get; set; }
    //    public int numinfants { get; set; }
    //    public string journeytype { get; set; }
    //    public string requestformat { get; set; }
    //    public string searchmode { get; set; }
    //    public string issbt { get; set; }
    //    public string profileid { get; set; }
    //    public bool debug { get; set; }
    //    public string isapi { get; set; }
    //    public string groupcategory { get; set; }
    //    public string currency { get; set; }
    //    public string isdirectflightonly { get; set; }
    //    public string preddeptimewindow { get; set; }
    //    public string prefarrtimewindow { get; set; }
    //    public string resultformat { get; set; }
    //    public string prefclass { get; set; }
    //    public string prefcarrier { get; set; }
    //    public string excludecarriers { get; set; }
    //    public string searchtype { get; set; }
    //    public string promocode { get; set; }
    //    public int numresults { get; set; }
    //    public string sortkey { get; set; }
    //    public string actionname { get; set; }
    //    public string refundtype { get; set; }
    //    public string layovertime { get; set; }
    //    public string flightnumber { get; set; }
    //}



    public class Othertaxesbreakup
    {
        public string taxcode { get; set; }
        public decimal taxamount { get; set; }
        public string paxtype { get; set; }
        public string description { get; set; }
    }

    public class Recommendedflight
    {
        public string nextraflightkey { get; set; }
        public int totaljourneyduration { get; set; }
        public string flightdeeplinkurl { get; set; }
        public string nextracustomstr { get; set; }
        public string ssravailable { get; set; }
        public string seatmapavailable { get; set; }
        public string autoissuance { get; set; }
        public bool islcc { get; set; }
        public string supplier { get; set; }
        public string ismorefaresavailable { get; set; }
        public string isnegotiationsavailable { get; set; }
        public string stopoverinfo { get; set; }
        public List<Flightleg> flightlegs { get; set; }
        public Flightfare flightfare { get; set; }
    }

   


}
