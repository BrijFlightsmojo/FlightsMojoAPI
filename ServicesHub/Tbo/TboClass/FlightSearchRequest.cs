using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{    
    [DataContract]
    public class FlightSearchRequest
    {
        [DataMember]
        public string EndUserIp { get; set; }
        [DataMember]
        public string TokenId { get; set; }
        [DataMember]
        public int AdultCount { get; set; }
        [DataMember]
        public int ChildCount { get; set; }
        [DataMember]
        public int InfantCount { get; set; }
        [DataMember]
        public bool DirectFlight { get; set; }
        [DataMember]
        public bool OneStopFlight { get; set; }
        [DataMember]
        public JourneyType JourneyType { get; set; }
        [DataMember]
        public List<string> PreferredAirlines { get; set; }
        [DataMember]
        public List<Segment> Segments { get; set; }
        [DataMember]
        public List<string> Sources { get; set; }
    }
    [DataContract]
    public class Segment
    {
        [DataMember]
        public string Origin { get; set; }
        [DataMember]
        public string Destination { get; set; }
        [DataMember]
        public FlightCabinClass FlightCabinClass { get; set; }
        [DataMember]
        public DateTime PreferredDepartureTime { get; set; }
        [DataMember]
        public DateTime PreferredArrivalTime { get; set; }
    }
    public enum JourneyType : int
    {
        OneWay = 1,
        Return = 2,
        MultiStop = 3,
        AdvanceSearch = 4,
        SpecialReturn = 5
    }
    public enum FlightCabinClass : int
    {
        All = 1,
        Economy = 2,
        PremiumEconomy = 3,
        Business = 4,
        PremiumBusiness = 5,
        First = 6
    }
}
