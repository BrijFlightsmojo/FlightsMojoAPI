using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.FareBoutique.FB_FareQuote
{
    public class FareQuoteResponse
    {
        public int errorCode { get; set; }
        public Data data { get; set; }
    }
    public class Data
    {
        public int flight_id { get; set; }
        public int available_seats { get; set; }
        public int total_amount { get; set; }
    }
}
