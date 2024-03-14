using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Ease2Fly.Ease2FlyClass
{
    public class FlightResponse
    {
        public string jsonrpc { get; set; }
        public string refresh_token { get; set; }
        public bool status { get; set; }
        public List<FlightResult> result { get; set; }
    }

    public class FlightResult
    {
        public int id { get; set; }
        public string sector { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public int total_fare { get; set; }
        public string flight_no { get; set; }
        public string pnr { get; set; }
        public string d_owner { get; set; }
        public string flight_key { get; set; }
        public string departure_date { get; set; }
        public string departure_time { get; set; }
        public string arrival_date { get; set; }
        public string arrival_time { get; set; }
        public string airline_name { get; set; }
        public int seat { get; set; }
        public int infant_charge { get; set; }
    }
}
