using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.FareBoutique.FB_BookFlight
{
    public class BookResponse
    {
        public string errorMessage { get; set; }
        public int errorCode { get; set; }
        public string replyCode { get; set; }
        public string replyMsg { get; set; }
        public Data data { get; set; }
    }
    public class Data
    {
        public string reference_id { get; set; }
    }    
}
