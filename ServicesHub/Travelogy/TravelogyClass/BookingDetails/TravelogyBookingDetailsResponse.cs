using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelogy.TravelogyClass.BookingDetails
{
    public class TravelogyBookingDetailsResponse
    {
        public List<AirlinePNRDetail> AirlinePNRDetails { get; set; }
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
    public class AirlinePNR
    {
        public string Airline_Code { get; set; }
        public string Airline_PNR { get; set; }
        public object CRS_Code { get; set; }
        public object CRS_PNR { get; set; }
        public object Record_Locator { get; set; }
        public string Supplier_RefNo { get; set; }
    }

    public class AirlinePNRDetail
    {
        public List<AirlinePNR> AirlinePNRs { get; set; }
        public object Failure_Remark { get; set; }
        public string Flight_Id { get; set; }
        public string Hold_Validity { get; set; }
        public string Status_Id { get; set; }
    }
}
