using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.AirIQ.AirIQClass
{
    public class AirIQRequest
    {
       
    }

    public class search
    {
        public string origin { get; set; }
        public string destination { get; set; }
        public string departure_date { get; set; }
        public int adult { get; set; }
        public int child { get; set; }
        public int infant { get; set; }
        public string airline_code { get; set; }
    }


    public class Token
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class fare_quote
    {
        public List<string> resultSessionId { get; set; }
    }


    public class book_flight
    {
        public string ticket_id { get; set; }
        public int total_pax { get; set; }
        public int adult { get; set; }
        public int child { get; set; }
        public int infant { get; set; }
        public List<AdultInfo> adult_info { get; set; }
        public List<ChildInfo> child_info { get; set; }
        public List<InfantInfo> infant_info { get; set; }
    }

    public class AdultInfo
    {
        public string title { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

    public class ChildInfo
    {
        public string title { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }

    public class InfantInfo
    {
        public string title { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string dob { get; set; }
        public string travel_with { get; set; }
    }
}
