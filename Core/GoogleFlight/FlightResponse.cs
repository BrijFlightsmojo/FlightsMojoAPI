using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Core.GoogleFlight
{   
    [DataContract]
    public class FlightResponse
    {
        //[DataMember(EmitDefaultValue = false)]
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public ErrorResponse errorResponse { get; set; }
        [DataMember(EmitDefaultValue = false)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Itinerary> itineraries { get; set; }
        [DataMember(EmitDefaultValue = false)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> warnings { get; set; }

    }
    [DataContract]
    public class Itinerary
    {
        [DataMember]
        public Outbound outbound { get; set; }
        [DataMember(EmitDefaultValue = false)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Inbound inbound { get; set; }
        [DataMember]
        public Price price { get; set; }
        [DataMember]
        public string booking_url { get; set; }
        [DataMember]
        public string validity_seconds { get; set; }
        [DataMember]
        public string virtual_interline_type { get; set; }
    }
    [DataContract]
    public class Outbound
    {
        [DataMember]
        public List<Segment> segments { get; set; }
    }
   [DataContract]
    public class Inbound
    {
        [DataMember]
        public List<Segment> segments { get; set; }
    }
    [DataContract]
    public class Price
    {
        [DataMember]
        public string total_decimal { get; set; }
        [DataMember]
        public string currency_code { get; set; }
    }
    [DataContract]
    public class Segment
    {
        [DataMember]
        public List<Leg> legs { get; set; }
        [DataMember]
        public string cabin_code { get; set; }
        [DataMember]
        public BaggageAllowance baggage_allowance { get; set; }
    }
    [DataContract]
    public class Leg
    {
        [DataMember]
        public string departure_airport { get; set; }
        [DataMember]
        public DateTime departure_date_time { get; set; }
        [DataMember]
        public string arrival_airport { get; set; }
        [DataMember]
        public DateTime arrival_date_time { get; set; }
        [DataMember]
        public string carrier { get; set; }
        [DataMember]
        public string flight_number { get; set; }
    }
    [DataContract]
    public class BaggageAllowance
    {
        [DataMember]
        public CarryOnBaggage carry_on_baggage { get; set; }
        [DataMember]
        public CheckedBaggage checked_baggage { get; set; }
    }
    [DataContract]
    public class CarryOnBaggage
    {
        [DataMember]
        public int max_weight { get; set; }
        [DataMember]
        public int count { get; set; }
    }
    [DataContract]
    public class CheckedBaggage
    {
        [DataMember]
        public int max_weight { get; set; }
        [DataMember]
        public int count { get; set; }
    }

    //[DataContract]
    //public class ResponseStatus
    //{
    //    [DataMember]
    //    public TransactionStatus status { get; set; }
    //    [DataMember]
    //    public string message { get; set; }
    //    public ResponseStatus()
    //    {
    //        status = TransactionStatus.Success;
    //        message = "Success";
    //    }

    //}
    [DataContract]
    public class Error
    {
        [DataMember]
        public string code { get; set; }
        [DataMember]
        public string description { get; set; }
    }
    [DataContract]
    public class ErrorResponse
    {
       [DataMember]
        public List<Error> errors { get; set; }
    }
}
