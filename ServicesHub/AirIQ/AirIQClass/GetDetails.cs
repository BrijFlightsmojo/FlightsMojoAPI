using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.AirIQ.AirIQ_GetDetails
{
    public class GetDetails
    {
        public string code { get; set; }
        public string status { get; set; }
        public Data data { get; set; }
    }

    public class Adult
    {
        public string Name { get; set; }
    }

    public class Data
    {
        public string agency_name { get; set; }
        public string booking_id { get; set; }
        public string booking_date { get; set; }
        public string sector { get; set; }
        public string pnr { get; set; }
        public string airline { get; set; }
        public string flight_no { get; set; }
        public string departure_date { get; set; }
        public string departure_time { get; set; }
        public string arrival_date { get; set; }
        public string arrival_time { get; set; }
        public decimal total_amount { get; set; }
        public PassengerDetails passenger_details { get; set; }
    }

    public class Infant
    {
        public string Name { get; set; }
        public string Dob { get; set; }
        public string Travel_with { get; set; }
    }

    public class PassengerDetails
    {
        public List<Adult> Adult { get; set; }
        public List<Infant> Infant { get; set; }
    }
}
