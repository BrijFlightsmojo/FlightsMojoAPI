using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.TripJack
{
    public class TripJackRequestMapping
    {
        public string getFlightSearchRequest(Core.Flight.FlightSearchRequest fsr)
        {
            TripJack.TripJackClass.TripJackFlightSearchRequest request = new TripJackClass.TripJackFlightSearchRequest()
            {
                searchQuery = new TripJackClass.SearchQuery()
                {
                    cabinClass = getCabinType(fsr.cabinType),
                    paxInfo = new TripJackClass.PaxInfo()
                    {
                        ADULT = fsr.adults.ToString(),
                        CHILD = fsr.child.ToString(),
                        INFANT = fsr.infants.ToString(),
                    },

                    searchModifiers = new TripJackClass.SearchModifiers()
                    {
                        isConnectingFlight = true,
                        isDirectFlight = true
                    },
                    routeInfos = new List<TripJackClass.RouteInfo>()
                }                
            };
            if (fsr.searchDirectFlight)
            {
                request.searchQuery.searchModifiers.isConnectingFlight = false;              
            }
            foreach (Core.Flight.SearchSegment seg in fsr.segment)
            {
                request.searchQuery.routeInfos.Add(new TripJackClass.RouteInfo()
                {
                    fromCityOrAirport = new TripJackClass.CityOrAirport() { code = seg.originAirport },
                    toCityOrAirport = new TripJackClass.CityOrAirport() { code = seg.destinationAirport },
                    travelDate = seg.travelDate.ToString("yyy-MM-dd")
                });
            }
            if (fsr.searchDirectFlight)
            {

            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(request);
        }

        public string getFlightReviewRequest(Core.Flight.PriceVerificationRequest fbr)
        {
            TripJack.TripJackClass.Review.TripJackReviewRequest request = new TripJackClass.Review.TripJackReviewRequest()
            {
                priceIds = fbr.PriceID
            };


            return Newtonsoft.Json.JsonConvert.SerializeObject(request);
        }
        public string getFlightBookRequest(Core.Flight.FlightBookingRequest fbr)
        {
            TripJack.TripJackClass.BookingRequest.TripJackFlightBookingRequest request = new TripJackClass.BookingRequest.TripJackFlightBookingRequest()
            {
                bookingId = fbr.TjBookingID,
                deliveryInfo = new TripJackClass.BookingRequest.DeliveryInfo() { contacts = new List<string>(), emails = new List<string>() },
                gstInfo = new TripJackClass.BookingRequest.GstInfo(),// { gstNumber = "07AACCF6706H1ZQ", registeredName = "FLIGHTS MOJO BOOKINGS PRIVATE LIMITED", address = "Plot No 83 Sector-28 Gurugram Haryana 122001", email = "SUPPORT@FLIGHTSMOJO.IN", mobile = "9876543265" },
                paymentInfos = new List<TripJackClass.BookingRequest.PaymentInfo>(),
                travellerInfo = new List<TripJackClass.BookingRequest.TravellerInfo>()
            };
            if (!string.IsNullOrEmpty(fbr.GSTNo) && !string.IsNullOrEmpty(fbr.GSTCompany))
            {
                request.gstInfo.gstNumber = fbr.GSTNo;
                request.gstInfo.registeredName = fbr.GSTCompany;
                request.gstInfo.address = fbr.GSTCompany;
                request.gstInfo.mobile = fbr.phoneNo;
                request.gstInfo.email = fbr.emailID;
            }

            //request.deliveryInfo.emails.Add(fbr.emailID);
            request.deliveryInfo.emails.Add("flightsmojollc@gmail.com");
            request.deliveryInfo.contacts.Add("+91" + fbr.phoneNo);
            TripJackClass.BookingRequest.PaymentInfo pi = new TripJackClass.BookingRequest.PaymentInfo() { amount=fbr.VerifiedTotalPrice};
            request.paymentInfos.Add(pi);
            foreach (Core.PassengerDetails pax in fbr.passengerDetails)
            {
                TripJackClass.BookingRequest.TravellerInfo tinfo = new TripJackClass.BookingRequest.TravellerInfo { dob = pax.dateOfBirth.ToString("yyyy-MM-dd"), fN = pax.firstName, lN = pax.lastName, ti = pax.title, pt = getPaxType(pax.passengerType) };
                request.travellerInfo.Add(tinfo);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(request);
        }

        public string getFlightBookingDetailsRequest(Core.Flight.FlightBookingRequest fbr)
        {
            TripJack.TripJackClass.BookingDetails.TripJackBookingDetailsRequest request = new TripJackClass.BookingDetails.TripJackBookingDetailsRequest()
            {
                bookingId = fbr.TjBookingID,
             };
          
            return Newtonsoft.Json.JsonConvert.SerializeObject(request);
        }
        public string getCabinType(Core.CabinType ct)
        {
            string CabinName = string.Empty;
            if (ct == Core.CabinType.Economy)
            {
                CabinName = "ECONOMY";
            }
            else if (ct == Core.CabinType.PremiumEconomy)
            {
                CabinName = "PREMIUM_ECONOMY";
            }
            else if (ct == Core.CabinType.Business)
            {
                CabinName = "BUSINESS ";
            }
            else if (ct == Core.CabinType.First)
            {
                CabinName = "FIRST";
            }
            else
            {
                CabinName = "ECONOMY";
            }
            return CabinName;
        }
        public string getPaxType(Core.PassengerType ct)
        {
            string paxtype = string.Empty;
            if (ct == Core.PassengerType.Adult)
            {
                paxtype = "ADULT";
            }
            else if (ct == Core.PassengerType.Child)
            {
                paxtype = "CHILD";
            }
            else if (ct == Core.PassengerType.Infant)
            {
                paxtype = "INFANT";
            }

            return paxtype;
        }
    }
}
