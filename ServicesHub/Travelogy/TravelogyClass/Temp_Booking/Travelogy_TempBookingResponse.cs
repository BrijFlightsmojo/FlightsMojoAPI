using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelogy.TravelogyClass.TempBookingResponse
{
    public class Travelogy_TempBookingResponse
    {
        public string Booking_RefNo { get; set; }
        public ResponseHeader Response_Header { get; set; }
    }
    public class ResponseHeader
    {
        public string Error_Code { get; set; }
        public string Error_Desc { get; set; }
        public string Error_InnerException { get; set; }
        public string Request_Id { get; set; }
        public string Status_Id { get; set; }
    }   
}
