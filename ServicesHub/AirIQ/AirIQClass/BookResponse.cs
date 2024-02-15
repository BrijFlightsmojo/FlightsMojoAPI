using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.AirIQ.AirIQClass
{
    public class BookResponse
    {
        public string code { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string booking_id { get; set; }
        public string airline_code { get; set; }
    }
}
