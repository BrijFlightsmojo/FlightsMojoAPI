using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Ease2Fly.Ease2FlyClass
{
    public class TokenResponse
    {
        public string jsonrpc { get; set; }
        public bool status { get; set; }
        public Result result { get; set; }
    }

    public class Result
    {
        public string email { get; set; }
        public string name { get; set; }
        public string contact_number { get; set; }
        public string address { get; set; }
        public string contact_name { get; set; }
        public int balance { get; set; }
        public string token { get; set; }
    }
}
