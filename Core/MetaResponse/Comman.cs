using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Meta.Comman
{
    [DataContract]
    public class Flights
    {
        [DataMember]
        public IList<Flight> flights { get; set; }
    }
    [DataContract]
    public class Flight
    {
        [DataMember]
        public string price { get; set; }
        [DataMember]
        public List<PaymentFee> paymentFees { get;set;}
        [DataMember]
        public string currency { get; set; }
        [DataMember]
        public string Class { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public string marketingAirline { get; set; }
        [DataMember]
        public List<Segment> segment { get; set; }
    }
   
    [DataContract]
    public class Segment
    {
        [DataMember]
        public List<Leg> leg { get; set; }
    }

    [DataContract]
    public class Leg
    {
        [DataMember]
        public string airline { get; set; }
        [DataMember]
        public string operatingAirline { get; set; }
        [DataMember]
        public string flightNumber { get; set; }
        [DataMember]
        public string origin { get; set; }
        [DataMember]
        public string departureDate { get; set; }
        [DataMember]
        public string departureTime { get; set; }
        [DataMember]
        public string destination { get; set; }
        [DataMember]
        public string arrivalDate { get; set; }
        [DataMember]
        public string arrivalTime { get; set; }
        [DataMember]
        public string equipment { get; set; }
        [DataMember]
        public string cabin { get; set; }
    }
   
    
}
