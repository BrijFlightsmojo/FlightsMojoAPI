using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{
    [DataContract]
    public class CalendarFareRequest
    {
        [DataMember]
        public ServicesHub.Tbo.TboClass.JourneyType JourneyType { get; set; }
        [DataMember]
        public string EndUserIp { get; set; }
        [DataMember]
        public string TokenId { get; set; }
        [DataMember]
        public List<string> PreferredAirlines { get; set; }
        [DataMember]
        public List<SegmentCFR> Segments { get; set; }
        [DataMember]
        public List<string> Sources { get; set; }
    }
    public class SegmentCFR
    {
        [DataMember]
        public string Origin { get; set; }
        [DataMember]
        public string Destination { get; set; }
        [DataMember]
        public FlightCabinClass FlightCabinClass { get; set; }
        [DataMember]
        public DateTime  PreferredDepartureTime { get; set; }
    }

   

    public class CalendarFareUpdateRequest
    {
        public ServicesHub.Tbo.TboClass.JourneyType JourneyType { get; set; }
        public string EndUserIp { get; set; }
        public string TokenId { get; set; }
        public List<string> PreferredAirlines { get; set; }
        public List<SegmentCFR> Segments { get; set; }
        public List<string> Sources { get; set; }
    }
}
