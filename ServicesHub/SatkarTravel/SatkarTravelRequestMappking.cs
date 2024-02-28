using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.SatkarTravel
{
    public class SatkarTravelRequestMappking
    {
        public string getFlightSearchRequest(Core.Flight.FlightSearchRequest fsr)
        {
            SatkarTravelClass.search flightSearchRequest = new SatkarTravelClass.search()
            {
                onwarddate = fsr.segment[0].travelDate.ToString("yyyy-MM-dd"),
                returndate = "",
                resultCategory = "1",
                prefclass = "2",
                segments = new List<ServicesHub.SatkarTravel.SatkarTravelClass.Segment>(),
                adultCount = fsr.adults,
                childCount = fsr.child,
                infantCount = fsr.infants,
                endUserIp = fsr.userIP,
                journeyType = (fsr.segment.Count > 1 ? (fsr.segment.Count - 1).ToString() : "1"),
                domIntFlag = fsr.travelType == Core.TravelType.Domestic ? "D" : "I"
            };

            foreach (Core.Flight.SearchSegment ss in fsr.segment)
            {
                ServicesHub.SatkarTravel.SatkarTravelClass.Segment seg = new ServicesHub.SatkarTravel.SatkarTravelClass.Segment();
                seg.origin = ss.originAirport;
                seg.destination = ss.destinationAirport;
                seg.flightCabinClass = "2";
                seg.preferredDepartureTime = ss.travelDate;
                flightSearchRequest.segments.Add(seg);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(flightSearchRequest);
        }
        public string getFareQuoteRequest(Core.Flight.PriceVerificationRequest request)
        {
            SatkarTravelClass.fare_quote fareQuote = new SatkarTravelClass.fare_quote()
            {
                resultSessionId = new List<string>()
            };
            fareQuote.resultSessionId.Add(request.flightResult.First().Fare.ST_ResultSessionID);

            return Newtonsoft.Json.JsonConvert.SerializeObject(fareQuote);
        }
        public string getBookingRequest(Core.Flight.FlightBookingRequest request, int ctr)
        {
            SatkarTravelClass.book_flight bookFlight = new SatkarTravelClass.book_flight()
            {
                // resultSessionId = request.flightResult.First().Fare.ST_ResultSessionID,
                resultSessionId = request.STSessionID,
                lcc = false,
                contactNo = request.phoneNo,
                email = "agency@flightsmojo.in",
                gSTDetailsRequired = false,
                passengers = new List<SatkarTravelClass.Passenger>()
            };
            Random rnd = new Random();
            foreach (var pax in request.passengerDetails)
            {
                if (pax.passengerType == Core.PassengerType.Adult)
                {
                    int y = rnd.Next(0, 24);
                    int d = rnd.Next(0, 365);
                    pax.passportIssueDate = DateTime.Today.AddYears(-12).AddYears(-y).AddDays(d);

                    SatkarTravelClass.Passenger _pax = new SatkarTravelClass.Passenger
                    {
                        paxType = 1,
                        title = pax.title,
                        firstName = pax.firstName,
                        lastName = pax.lastName,
                        addressLine1 = "",
                        city = "",
                        countryCode = "IN",
                        nationality = "IN",
                        postalCode = 110011,
                        dateOfBirth = pax.dateOfBirth.ToString("yyyy-MM-dd"),
                        passportNo = pax.passportNumber,
                        passportExpiry = pax.expiryDate.Value.ToString("yyyy-MM-dd"),
                        passportIssueDate = pax.passportIssueDate.Value.ToString("yyyy-MM-dd"),
                        gender = pax.gender == Core.Gender.Male ? 1 : 2,
                        paxId = 0,
                        isLeadPax = true
                    };
                    bookFlight.passengers.Add(_pax);
                }
                else if (pax.passengerType == Core.PassengerType.Child)
                {
                    //int age = (request.flightResult.LastOrDefault().FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime.Year - pax.dateOfBirth.Year);
                    pax.passportIssueDate = pax.dateOfBirth.AddYears(1);
                    SatkarTravelClass.Passenger _pax = new SatkarTravelClass.Passenger
                    {
                        paxType = 2,
                        title = pax.title,
                        firstName = pax.firstName,
                        lastName = pax.lastName,
                        addressLine1 = "",
                        city = "",
                        countryCode = "IN",
                        nationality = "IN",
                        postalCode = 110011,
                        dateOfBirth = pax.dateOfBirth.ToString("yyyy-MM-dd"),
                        passportNo = pax.passportNumber,
                        passportExpiry = pax.expiryDate.Value.ToString("yyyy-MM-dd"),
                        passportIssueDate = pax.passportIssueDate.Value.ToString("yyyy-MM-dd"),
                        gender = pax.gender == Core.Gender.Male ? 1 : 2,
                        paxId = 0,
                        isLeadPax = true
                    };
                    bookFlight.passengers.Add(_pax);
                }

                else if (pax.passengerType == Core.PassengerType.Infant)
                {
                    // int age = (request.flightResult.LastOrDefault().FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime.Year - pax.dateOfBirth.Year);
                    TimeSpan ts = DateTime.Today - pax.dateOfBirth;
                    pax.passportIssueDate = pax.dateOfBirth.AddDays(ts.TotalDays / 2);
                    SatkarTravelClass.Passenger _pax = new SatkarTravelClass.Passenger
                    {
                        paxType = 3,
                        title = pax.title,
                        firstName = pax.firstName,
                        lastName = pax.lastName,
                        addressLine1 = "",
                        city = "",
                        countryCode = "IN",
                        nationality = "IN",
                        postalCode = 110011,
                        dateOfBirth = pax.dateOfBirth.ToString("yyyy-MM-dd"),
                        passportNo = pax.passportNumber,
                        passportExpiry = pax.expiryDate.Value.ToString("yyyy-MM-dd"),
                        passportIssueDate = pax.passportIssueDate.Value.ToString("yyyy-MM-dd"),
                        gender = pax.gender == Core.Gender.Male ? 1 : 2,
                        paxId = 0,
                        isLeadPax = true
                    };
                    bookFlight.passengers.Add(_pax);
                }

            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(bookFlight);
        }

        string ST_URL = ConfigurationManager.AppSettings["ST_Url"].ToString();
        string ST_UserName = ConfigurationManager.AppSettings["ST_UserName"].ToString();
        string ST_PWD = ConfigurationManager.AppSettings["ST_PWD"].ToString();
        public string getSearchToken()
        {
            SatkarTravelClass.Token token = new SatkarTravelClass.Token()
            {
                auth_channel = ST_URL,
                username = ST_UserName,
                password = ST_PWD
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(token);
        }


    


    }
}
