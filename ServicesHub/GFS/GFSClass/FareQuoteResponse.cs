using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.GFS.GFSFareQuoteResponse
{
    public class FareQuoteResponse
    {
        public bool success { get; set; }
        public string response_ref { get; set; }
        public Data _data { get; set; }
    }

    public class Data
    {
        public Flight flight { get; set; }
        public Query query { get; set; }
        public InputRequirements input_requirements { get; set; }
    }

    public class Flight
    {
        public decimal infant_price { get; set; }
        public decimal child_price { get; set; }
        public decimal total_price { get; set; }
        public string checkin_baggage { get; set; }
        public decimal adult_price { get; set; }
        public string currency { get; set; }
        public bool non_refundable { get; set; }
        public int seats_available { get; set; }
        public string key { get; set; }
        public List<Segment> segments { get; set; }
    }

    public class Query
    {
        public int nAdt { get; set; }
        public int nInf { get; set; }
        public int nChd { get; set; }
        public List<Leg> legs { get; set; }
    }

    public class InputRequirements
    {
        public Dob dob { get; set; }
    }

    public class Segment
    {
        public int duration { get; set; }
        public List<Leg> legs { get; set; }
    }

    public class Dob
    {
        public string @for { get; set; }
        public bool required { get; set; }
    }

    public class Leg
    {
        public int duration { get; set; }
        public string arrival_time { get; set; }
        public string flight_number { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string airline { get; set; }
        public string departure_time { get; set; }
        public string dst { get; set; }
        public string src { get; set; }
        public string dep { get; set; }
    }


}
