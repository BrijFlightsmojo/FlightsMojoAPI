using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelogy.TravelogyClass.Air_TicketCancellation
{
    public class Air_TicketCancellationRequest
    {
        public AuthHeader Auth_Header { get; set; }
        public List<AirTicketCancelDetail> AirTicketCancelDetails { get; set; }
        public string Airline_PNR { get; set; }
        public string RefNo { get; set; }
        public string CancelCode { get; set; }
        public string ReqRemarks { get; set; }
        public int CancellationType { get; set; }
    }
    public class AirTicketCancelDetail
    {
        public string FlightId { get; set; }
        public string PassengerId { get; set; }
        public string SegmentId { get; set; }
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
