using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Markup
{
    public class FlightMarkup
    {
        public int Id { get; set; }
        //public List<int> FareType { get; set; }
        public List<int> Stops { get; set; }
        public List<string> Airline { get; set; }
        public AirlineMatchType airlineMatchType { get; set; }
        public List<string> AirlineClass { get; set; }
        public AirlineClassMatchType airlineClassMatchType { get; set; }
        public decimal Amount { get; set; }
        public int AmountType { get; set; }
        public int CalculateOn { get; set; }
        public int RuleType { get; set; }
        public string RuleName { get; set; }
        public int GdsType { get; set; }
        public CheckOperatedBy checkOperatedBy { get; set; }
    }

    public class FlightMarkupNew
    {      
        public List<string> Airline { get; set; }
        public List<string> AirlineNot { get; set; }
        
        public List<MojoFareType> FmFareType { get; set; }
        public decimal Amount { get; set; }
        public int AmountType { get; set; }
        public string RuleName { get; set; }
        public int GdsType { get; set; }   
        public List<SubProvider> SubProvider { get; set; }
    }
    public class skyScannerMetaRankData
    {
        public string Airline { get; set; }
        public string flightNo { get; set; }      
        public decimal Amount { get; set; }
      
    }
}
