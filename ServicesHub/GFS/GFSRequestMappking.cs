using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.GFS
{
    public class GFSRequestMappking
    {
        public string getFareQuoteRequest(Core.Flight.PriceVerificationRequest request, int i)
        {
            GFSFareQuoteRequestClass.FareQuoteRequest fareQuote = new GFSFareQuoteRequestClass.FareQuoteRequest()
            {
                currency = "INR",
                total_price = request.flightResult[i].Fare.NetFare,
                flight_keys=new List<string>(),
                query=new GFSFareQuoteRequestClass.Query()
        };
            fareQuote.flight_keys.Add(request.flightResult[i].Fare.GFS_FlightKey);
            fareQuote.query.nAdt = request.adults;
            fareQuote.query.nChd = request.child;
            fareQuote.query.nInf = request.infants;
            fareQuote.query.legs = new List<GFSFareQuoteRequestClass.Leg>();
            GFSFareQuoteRequestClass.Leg leg = new GFSFareQuoteRequestClass.Leg();
            leg.dst = request.origin.ToUpper();
            leg.src = request.destination.ToUpper();
            leg.dep = request.depDate.Replace("-", "/");
            fareQuote.query.legs.Add(leg);
            return Newtonsoft.Json.JsonConvert.SerializeObject(fareQuote);
        }

        public string getBookingRequest(Core.Flight.FlightBookingRequest request)
        {
            GFS_BookRequest.BookFlightRequest bookFlight = new GFS_BookRequest.BookFlightRequest()
            {
                query = new GFS_BookRequest.Query(),
                total_price = request.flightResult[0].Fare.NetFare,
                agent_reference = "",
                currency = "INR",
                flight_keys = new List<string>(),
                client_details = new GFS_BookRequest.ClientDetails(),
                paxes= new List<GFS_BookRequest.Paxis>()
            };
            bookFlight.flight_keys.Add(request.flightResult[0].Fare.GFS_FlightKey);
            
            bookFlight.client_details.phone = request.phoneNo;
            bookFlight.client_details.email = request.emailID;
            bookFlight.query.nAdt = request.adults;
            bookFlight.query.nChd = request.child;
            bookFlight.query.nInf = request.infants;
            bookFlight.query.legs = new List<GFS_BookRequest.Leg>();
            GFS_BookRequest.Leg leg = new GFS_BookRequest.Leg();
            leg.dst = request.flightResult[0].FlightSegments[0].Segments[0].Origin.ToUpper();
            leg.src = request.flightResult[0].FlightSegments[0].Segments[0].Destination.ToUpper();
            leg.dep = request.flightResult[0].FlightSegments[0].Segments[0].DepTime.ToString("dd/MM/yyyy");
            leg.dep = leg.dep.Replace("-", "/");
            bookFlight.query.legs.Add(leg);

            Random rnd = new Random();
            foreach (var pax in request.passengerDetails)
            {
                if (pax.passengerType == Core.PassengerType.Adult)
                {
                    int y = rnd.Next(0, 24);
                    int d = rnd.Next(0, 365);
                    pax.dateOfBirth = DateTime.Today.AddYears(-24).AddYears(-y).AddDays(d);
                    int age = (request.flightResult.LastOrDefault().FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime.Year - pax.dateOfBirth.Year);

                    GFS_BookRequest.Paxis _pax = new GFS_BookRequest.Paxis
                    {
                        title = pax.title,
                        first_name = pax.firstName,
                        last_name = pax.lastName,
                        type = "adult",
                        dob = pax.dateOfBirth.ToString("yyyy-MM-dd"),
                        nationality = "IN",
                        passport_num = pax.passportNumber
                    };
                    bookFlight.paxes.Add(_pax);
                }

                if (pax.passengerType == Core.PassengerType.Child)
                {
                    int age = (request.flightResult.LastOrDefault().FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime.Year - pax.dateOfBirth.Year);

                    GFS_BookRequest.Paxis _pax = new GFS_BookRequest.Paxis
                    {
                        title = pax.title  == "Master" ? "Mstr" : "Miss",
                        first_name = pax.firstName,
                        last_name = pax.lastName,
                        type = "child",
                        dob = pax.dateOfBirth.ToString("yyyy-MM-dd"),
                        nationality = "IN",
                        passport_num = pax.passportNumber
                    };
                    bookFlight.paxes.Add(_pax);
                }

                if (pax.passengerType == Core.PassengerType.Infant)
                {
                    int age = (request.flightResult.LastOrDefault().FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime.Year - pax.dateOfBirth.Year);

                    GFS_BookRequest.Paxis _pax = new GFS_BookRequest.Paxis
                    {
                        title = pax.title == "Master" ? "Mstr" : "Miss",
                        first_name = pax.firstName,
                        last_name = pax.lastName,
                        type = "infant",
                        dob = pax.dateOfBirth.ToString("yyyy-MM-dd"),
                        nationality = "IN",
                        passport_num = pax.passportNumber
                    };
                    bookFlight.paxes.Add(_pax);
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(bookFlight);
        }
    }
}

