using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.FareBoutique.FareBoutiqueClass
{
    public class departure_city_list
    {
        public string trip_type { get; set; }
        public string end_user_ip { get; set; }
        public string token { get; set; }
    }
    public class arrival_city_list
    {
        public string trip_type { get; set; }
        public string departure_city_code { get; set; }
        public string end_user_ip { get; set; }
        public string token { get; set; }
    }
    public class sector
    {
        public string trip_type { get; set; }
        public string end_user_ip { get; set; }
        public string token { get; set; }
    }
    public class onward_dates
    {       
        public string trip_type { get; set; }
        public string departure_city_code { get; set; }
        public string arrival_city_code { get; set; }
        public string end_user_ip { get; set; }
        public string token { get; set; }
    }
    public class return_dates
    {
        public string trip_type { get; set; }
        public string departure_city_code { get; set; }
        public string arrival_city_code { get; set; }
        public string departure_date { get; set; }
        public string end_user_ip { get; set; }
        public string token { get; set; }
    }
    public class search
    {
        public string end_user_ip { get; set; }
        public string token { get; set; }
        public string departure_city_code { get; set; }
        public string arrival_city_code { get; set; }
        public string trip_type { get; set; }
        public string departure_date { get; set; }
        public string return_date { get; set; }
        public int adult { get; set; }
        public int children { get; set; }
        public int infant { get; set; }
        public bool with_aiq { get; set; }
    }
    public class fare_quote
    {
        public int flight_id { get; set; }
        public string departure_date { get; set; }
        public int adult_children { get; set; }
        public int infant { get; set; }
        public string end_user_ip { get; set; }
        public string token { get; set; }
        public string @static { get; set; }
    }
    public class book_flight
    {
        public int flight_id { get; set; }
        public string departure_date { get; set; }
        public int adult { get; set; }
        public int children { get; set; }
        public int infant { get; set; }
        public int total_book_seats { get; set; }
        public decimal total_amount { get; set; }
        public string contact_name { get; set; }
        public string contact_email { get; set; }
        public string contact_number { get; set; }
        public string return_date { get; set; }
        public List<FlightTravellerDetail> flight_traveller_details { get; set; }
        public string booking_token_id { get; set; }
        public string end_user_ip { get; set; }
        public string token { get; set; }
        public string @static { get; set; }
    }
    public class FlightTravellerDetail
    {
        public string gender { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public int age { get; set; }
        public string passport_no { get; set; }
        public string passport_expire_date { get; set; }
        public string dob { get; set; }
    }
    public class booking_details
    {
        public string reference_id { get; set; }
        public string end_user_ip { get; set; }
        public string token { get; set; }
    }
}
