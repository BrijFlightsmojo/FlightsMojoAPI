using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Ease2Fly.Ease2FlyClass
{
    public class Ease2FlyRequest
    {

    }

    public class Token
    {
        public string email { get; set; }
        public string pwd { get; set; }
        public string efly_api_key { get; set; }
    }


    public class book_flight
    {
        public List<AdultInfo> adult_info { get; set; }
        public List<ChildInfo> child_info { get; set; }
        public List<InfantInfo> infant_info { get; set; }
        public int adults { get; set; }
        public int child { get; set; }
        public int infant { get; set; }
        public int sector_id { get; set; }
        public decimal fare { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
    }


    public class AdultInfo
    {
        public string ttl { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string whlchr { get; set; }
        public string passport_no { get; set; }
        public string passport_nationality { get; set; }
        public string passport_dob { get; set; }
        public string passport_exp { get; set; }
    }

    public class ChildInfo
    {
        public string ttl { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int age { get; set; }
        public string whlchr { get; set; }
        public string passport_no { get; set; }
        public string passport_nationality { get; set; }
        public string passport_dob { get; set; }
        public string passport_exp { get; set; }
    }

    public class InfantInfo
    {
        public string ttl { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int age { get; set; }
        public string whlchr { get; set; }
        public string passport_no { get; set; }
        public string passport_nationality { get; set; }
        public string passport_dob { get; set; }
        public string passport_exp { get; set; }
    }
}
