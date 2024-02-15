using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.FareBoutique.FareBoutiqueClass
{
    public class FlightResponse
    {
        public int errorCode { get; set; }
        public string replyCode { get; set; }
        public List<Datum> data { get; set; }
        public string booking_token_id { get; set; }
        public string imgBasePath { get; set; }
        public string errorMessage { get; set; }
        
    }
   
    public class Datum
    {
        public int flight_id { get; set; }
        public string flight_number { get; set; }
        public string airline_name { get; set; }
        public string airlines_logo { get; set; }
        public string airline_code { get; set; }
        public string departure_city_name { get; set; }
        public string departure_city_code { get; set; }
        public string departure_airport_name { get; set; }
        public string departure_airport_code { get; set; }
        public string departure_terminal_no { get; set; }
        public string departure_date { get; set; }
        public string departure_time { get; set; }
        public string arrival_city_name { get; set; }
        public string arrival_city_code { get; set; }
        public string arrival_airport_name { get; set; }
        public string arrival_airport_code { get; set; }
        public string arrival_terminal_no { get; set; }
        public string arrival_date { get; set; }
        public string arrival_time { get; set; }
        public string trip_type { get; set; }
        public int international_flight_staus { get; set; }
        public string check_in_baggage_adult { get; set; }
        public string check_in_baggage_children { get; set; }
        public string check_in_baggage_infant { get; set; }
        public string cabin_baggage_adult { get; set; }
        public string cabin_baggage_children { get; set; }
        public string cabin_baggage_infant { get; set; }
        public int total_available_seats { get; set; }
        public decimal total_payable_price { get; set; }
        public decimal per_adult_child_price { get; set; }
        public decimal per_infant_price { get; set; }
        public FlightFeeTaxes flight_fee_taxes { get; set; }
        public InventoryFrom inventory_from { get; set; }
        public int no_of_stop { get; set; }
        public List<object> stop_data { get; set; }
        public List<object> return_stop_data { get; set; }
        public string @static { get; set; }
    }

    public class FlightFeeTaxes
    {
        public string base_fare { get; set; }
        public string fee_taxes { get; set; }
        public int infant_base_price { get; set; }
    }

    public class InventoryFrom
    {
        public string name { get; set; }
        public string company_name { get; set; }
        public string website { get; set; }
        public string email_id { get; set; }
        public string mobile { get; set; }
        public string share_id { get; set; }
        public int balance { get; set; }
        public string logo { get; set; }
    }
}
