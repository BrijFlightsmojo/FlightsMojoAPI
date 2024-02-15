using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelogy.TravelogyClass.RePrice
{
    public class TravelogyRePriceRequest
    {
        public AuthHeader Auth_Header { get; set; }
        public string Search_Key { get; set; }
        public List<AirRepriceRequest> AirRepriceRequests { get; set; }
        public string Customer_Mobile { get; set; }
        public bool GST_Input { get; set; }
        public bool SinglePricing { get; set; }
    }
    public class AirRepriceRequest
    {
        public string Flight_Key { get; set; }
        public string Fare_Id { get; set; }
    }

    public class AuthHeader
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string IP_Address { get; set; }
        public string Request_Id { get; set; }
        public string IMEI_Number { get; set; }
    }
}
