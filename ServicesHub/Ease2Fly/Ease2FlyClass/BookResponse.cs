using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Ease2Fly.Ease2FlyClass
{
   public class BookResponse
    {
        public string jsonrpc { get; set; }
        public result result { get; set; }
        public bool status { get; set; }
        public string error { get; set; }
    }

    public class result
    {
        public string airline { get; set; }
        public string pnr_no { get; set; }
        public string flight_no { get; set; }
        public string booking_date { get; set; }
        public string departure_date { get; set; }
        public string arrival_date { get; set; }
        public string departure_time { get; set; }
        public string arrival_time { get; set; }
        public int booking_id { get; set; }
        public string booking_status { get; set; }
        public List<PassengerDetail> passenger_details { get; set; }
        public ContactDetails contact_details { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string via { get; set; }
    }

    public class ContactDetails
    {
        public string cntEml { get; set; }
        public string cntPhone { get; set; }
    }

    public class PassengerDetail
    {
        public string ttl { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string whlchr { get; set; }
        public string passport_no { get; set; }
        public string passport_nationality { get; set; }
        public string passport_dob { get; set; }
        public string passport_exp { get; set; }
        public string age { get; set; }
    }
}
