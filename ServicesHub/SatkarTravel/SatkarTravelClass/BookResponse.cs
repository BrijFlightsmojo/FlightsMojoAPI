using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.SatkarTravel.ST_BookFlight
{
    public class BookResponse
    {
        public int reqId { get; set; }
        public string txid { get; set; }
        public string bookingstatus { get; set; }
        public int status { get; set; }
        public string errorMsg { get; set; }
    }
}
