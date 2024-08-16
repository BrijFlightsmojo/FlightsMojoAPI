using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.GoogleFlight
{
    [DataContract]
    public class FlightSearchRequest
    {
        [DataMember]
        public string currencyCode { get; set; }
        [DataMember]
        public string languageCode { get; set; }
        [DataMember]
        public string countryCode { get; set; }
        [DataMember]
        public int adults { get; set; }
        [DataMember]
        public int children { get; set; }
        [DataMember]
        public int infants { get; set; }
        [DataMember]
        public CabinType intendedCabin { get; set; }
        [DataMember]
        public TripSpec tripSpec { get; set; }
        [DataMember]
        public int version { get; set; }
    }

    [DataContract]
    public class TripSpec
    {
        [DataMember]
        public List<string> departureAirports { get; set; }
        [DataMember]
        public List<string> arrivalAirports { get; set; }
        [DataMember]
        public string departureDate { get; set; }
        [DataMember]
        public string returnDate { get; set; }
    }
}
