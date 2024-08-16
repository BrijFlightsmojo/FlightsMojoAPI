using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tripshope
{
   public class BookResponse
    {
        public ApiStatus ApiStatus { get; set; }
    }

    public class ApiStatus
    {
        public string message { get; set; }
        public string operation { get; set; }
        public string result { get; set; }
        public string resultFormat { get; set; }
        public string status { get; set; }
        public string statusCode { get; set; }
        public string transactionStatus { get; set; }
    }
}
