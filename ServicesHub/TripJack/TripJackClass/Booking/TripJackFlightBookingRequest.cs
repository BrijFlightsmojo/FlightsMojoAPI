using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.TripJack.TripJackClass.BookingRequest
{
  
    public class TripJackFlightBookingRequest
    {
        public string bookingId { get; set; }
        public List<PaymentInfo> paymentInfos { get; set; }
        public List<TravellerInfo> travellerInfo { get; set; }
        public GstInfo gstInfo { get; set; }
        public DeliveryInfo deliveryInfo { get; set; }
    }

    public class DeliveryInfo
    {
        public List<string> emails { get; set; }
        public List<string> contacts { get; set; }
    }

    public class GstInfo
    {
        public string gstNumber { get; set; }
        public string email { get; set; }
        public string registeredName { get; set; }
        public string mobile { get; set; }
        public string address { get; set; }
    }

    public class PaymentInfo
    {
        public decimal amount { get; set; }
    }

   
    public class TravellerInfo
    {
        public string ti { get; set; }
        public string fN { get; set; }
        public string lN { get; set; }
        public string pt { get; set; }
        public string dob { get; set; }
        public string pNum { get; set; }
        public string eD { get; set; }
        public string pid { get; set; }
        public string pNat { get; set; }

    }
}
