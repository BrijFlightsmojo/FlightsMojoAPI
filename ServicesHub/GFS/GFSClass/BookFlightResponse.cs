using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.GFS.GFS_BookFlightResponse
{
   public class BookFlightResponse
    {
        public bool success { get; set; }
        public string response_ref { get; set; }
        public Data _data { get; set; }

        public string error_msg { get; set; }
        public int error_code { get; set; }
    }

    public class Data
    {
        public string agent_reference { get; set; }
        public ClientDetails client_details { get; set; }
        public string product_type { get; set; }
        public List<BookingItem> booking_items { get; set; }
        public List<PaxDetail> pax_details { get; set; }
        public string booking_date { get; set; }
        public PriceDetails price_details { get; set; }
        public string booking_reference { get; set; }
        public string travel_date { get; set; }
        public string status { get; set; }
    }

    public class BookingItem
    {
        public Flight flight { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string type { get; set; }
        public List<Confirmation> confirmations { get; set; }
        public string status { get; set; }
    }

    public class ClientDetails
    {
        public string pax_name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
    }

    public class Confirmation
    {
        public string pax_type { get; set; }
        public int pax_id { get; set; }
        public string status { get; set; }
    }

    public class Flight
    {
        public int duration { get; set; }
        public string checkin_baggage { get; set; }
        public List<Leg> legs { get; set; }
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
    }

    public class PaxDetail
    {
        public string last_name { get; set; }
        public int id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public string first_name { get; set; }
    }

    public class PriceDetails
    {
        public decimal total_amount { get; set; }
        public string currency { get; set; }
    }
}
