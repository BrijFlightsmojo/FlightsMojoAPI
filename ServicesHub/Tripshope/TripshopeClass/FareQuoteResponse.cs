using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tripshope.GetFareQuoteResponse
{
    public class Root
    {
        public NextraPricingResponseV4 NextraPricingResponseV4 { get; set; }
    }

    public class NextraPricingResponseV4
    {
        public int statuscode { get; set; }
        public string statusmessage { get; set; }
        public string taxInfoStrOw { get; set; }
        public string taxInfoStrTw { get; set; }
        public bool pricechanged { get; set; }
        public string bookingkey { get; set; }
        public bool holdsupported { get; set; }
        public bool isbelow24hrs { get; set; }
        public string allowedfreemealscount { get; set; }
        public string domint { get; set; }
        public double newflightfare { get; set; }
        public double apireschcharges { get; set; }
        public bool seatmapavailable { get; set; }
        public bool ssravailable { get; set; }
        public double oldamount { get; set; }
        public double newamount { get; set; }
        public double rescheduleamount { get; set; }
        public string reschedulefarestr { get; set; }
        public List<Flightjourney> flightjourneys { get; set; }
    }

    public class Flightjourney
    {
        public List<Flightoption> flightoptions { get; set; }
    }

    public class Flightoption
    {
        public List<Recommendedflight> recommendedflight { get; set; }
    }

    public class Recommendedflight
    {
        public string nextraflightkey { get; set; }
        public int totaljourneyduration { get; set; }
        public string flightdeeplinkurl { get; set; }
        public string nextracustomstr { get; set; }
        public List<Flightleg> flightlegs { get; set; }
        public Flightfare flightfare { get; set; }
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
        public List<object> othertaxesbreakup { get; set; }
        public List<object> paxothertaxesbreakup { get; set; }
        public List<object> amenities { get; set; }
        public string refundableinfo { get; set; }
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
        public string farebasiscode { get; set; }
        public string inpolicy { get; set; }
        public string issbt { get; set; }
        public string policyid { get; set; }
        public string bagweight { get; set; }
        public string bagunit { get; set; }
        public string mileage { get; set; }
        public string specialinfo { get; set; }
        public string operatedby { get; set; }
    }


}
