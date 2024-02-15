using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.AirIQ.AirIQClass
{
    public class FlightResponse
    {
        public int code { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public List<Datum> data { get; set; }
    }

    public class Datum
    {
        public string ticket_id { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string airline { get; set; }
        public string departure_date { get; set; }
        public string departure_time { get; set; }
        public string arival_time { get; set; }
        public string arival_date { get; set; }
        public string flight_number { get; set; }
        public string flight_route { get; set; }
        public int pax { get; set; }
        public decimal price { get; set; }
        public decimal infant_price { get; set; }
    }
}
