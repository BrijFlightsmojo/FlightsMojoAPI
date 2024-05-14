using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.FareBoutique.FB_Booking_Details
{   
    public class Booking_Details
    {
        public string replyCode { get; set; }
        public string replyMsg { get; set; }
        public Data data { get; set; }
        public string imgBasePath { get; set; }
    }
    public class Baggage
    {
        public string checkin_baggages_adult { get; set; }
        public string checkin_baggages_children { get; set; }
        public string checkin_baggages_infant { get; set; }
        public string cabin_baggages_adult { get; set; }
        public string cabin_baggages_children { get; set; }
        public string cabin_baggages_infant { get; set; }
        public string disclaimer { get; set; }
    }

    public class Data
    {
        public string reference_id { get; set; }
        public string departure_date { get; set; }
        public string return_date { get; set; }
        public int adult { get; set; }
        public int children { get; set; }
        public int infant { get; set; }
        public int total_book_seats { get; set; }
        public bool seat_book_status { get; set; }
        public bool payment_status { get; set; }
        public string special_Information { get; set; }
        public string booking_date { get; set; }
        public decimal total_amount { get; set; }
        public int return_flight { get; set; }
        public decimal infant_price { get; set; }
        public string flight_pnrs { get; set; }
        public string contact_name { get; set; }
        public string contact_email { get; set; }
        public string contact_number { get; set; }
        public Onward onward { get; set; }
        //public List<object> @return { get; set; }
        //public List<object> price_breakup { get; set; }
        public Baggage baggage { get; set; }
        public List<Traveller> travellers { get; set; }
        public SellerInfo seller_info { get; set; }
    }

    public class Onward
    {
        public string depeparture_city_name { get; set; }
        public string depeparture_city_code { get; set; }
        public string arrival_city_name { get; set; }
        public string arrival_city_code { get; set; }
        public string departure_date { get; set; }
        public string departure_time { get; set; }
        public string departure_terminal_no_id { get; set; }
        public string arrival_date { get; set; }
        public string arrival_time { get; set; }
        public string arrival_terminal_no_id { get; set; }
        public int stop_count { get; set; }
        public string airline_name { get; set; }
        public string airline_code { get; set; }
        public string flight_number { get; set; }
        public string airline_logo { get; set; }
    }

   

    public class SellerInfo
    {
        public string contact_name { get; set; }
        public string company_name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public int share_id { get; set; }
    }

    public class Traveller
    {
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public decimal ticket_price { get; set; }
        public string gender { get; set; }
        public string dob { get; set; }
        public bool status { get; set; }
        public string ticket_cancel_at { get; set; }
        public int age { get; set; }
        public string passport_no { get; set; }
        public string passport_expire_date { get; set; }
    }


}
