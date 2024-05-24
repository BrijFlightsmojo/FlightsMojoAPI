using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelopedia.TravelopediaClass.Ticket_Invoice
{
    public class TravelopediaTicketInvoiceRequest
    {
        public AuthHeader Auth_Header { get; set; }
        public string Booking_RefNo { get; set; }
        public string Airline_PNR { get; set; }
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
