using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.AirIQ
{
    public class AirIQRequestMappking
    {
        string UserName = ConfigurationManager.AppSettings["AIQ_UserName"].ToString();
        string PWD = ConfigurationManager.AppSettings["AIQ_PWD"].ToString();
        public string getSearchToken()
        {
            AirIQClass.Token token = new AirIQClass.Token()
            {
                username = UserName,
                password = PWD
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(token);
        }



        public string getFlightSearchRequest(Core.Flight.FlightSearchRequest fsr)
        {
            ServicesHub.AirIQ.AirIQClass.search flightSearchRequest = new AirIQClass.search()
            {
                origin = fsr.segment[0].originAirport,
                destination = fsr.segment[0].destinationAirport,
                departure_date = fsr.segment[0].travelDate.ToString("yyyy-MM-dd"),
                adult = fsr.adults,
                child = fsr.child,
                infant = fsr.infants,
                airline_code = ""
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(flightSearchRequest);
        }


        //public string getFareQuoteRequest(Core.Flight.PriceVerificationRequest request)
        //{
        //    AirIQClass.fare_quote fareQuote = new AirIQClass.fare_quote()
        //    {
        //        resultSessionId = new List<string>()
        //    };
        //    fareQuote.resultSessionId.Add(request.ST_ResultSessionID);

        //    return Newtonsoft.Json.JsonConvert.SerializeObject(fareQuote);
        //}


        public string getBookingRequest(Core.Flight.FlightBookingRequest request, int ctr)
        {
            AirIQClass.book_flight bookFlight = new AirIQClass.book_flight()
            {
                ticket_id = request.flightResult[0].Fare.AQ_ticket_id,
                total_pax = request.passengerDetails.Count,
                adult = request.adults,
                child = request.child,
                infant = request.infants,
                adult_info = new List<AirIQClass.AdultInfo>(),
                child_info= new List<AirIQClass.ChildInfo>(),
                infant_info = new List<AirIQClass.InfantInfo>()
            };
            Random rnd = new Random();
            foreach (var pax in request.passengerDetails)
            {
                if (pax.passengerType == Core.PassengerType.Adult)
                {
                    //int y = rnd.Next(0, 24);
                    //int d = rnd.Next(0, 365);
                    //pax.passportIssueDate = DateTime.Today.AddYears(-12).AddYears(-y).AddDays(d);

                    AirIQClass.AdultInfo _pax = new AirIQClass.AdultInfo
                    {
                        title =pax.title +".",
                        first_name=pax.firstName,
                        last_name=pax.lastName
                    };
                    bookFlight.adult_info.Add(_pax);
                }
                else if (pax.passengerType == Core.PassengerType.Child)
                {
                    //int age = (request.flightResult.LastOrDefault().FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime.Year - pax.dateOfBirth.Year);
                  //  pax.passportIssueDate = pax.dateOfBirth.AddYears(1);
                    AirIQClass.ChildInfo _pax = new AirIQClass.ChildInfo
                    {
                        title = pax.title == "Master" ? "Mstr." : "Miss",
                        first_name = pax.firstName,
                        last_name = pax.lastName
                    };
                    bookFlight.child_info.Add(_pax);
                }

                else if (pax.passengerType == Core.PassengerType.Infant)
                {
                    // int age = (request.flightResult.LastOrDefault().FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime.Year - pax.dateOfBirth.Year);
                   // TimeSpan ts = DateTime.Today - pax.dateOfBirth;
                  //  pax.passportIssueDate = pax.dateOfBirth.AddDays(ts.TotalDays / 2);
                    AirIQClass.InfantInfo _pax = new AirIQClass.InfantInfo
                    {
                        title = pax.title == "Master"? "Mstr.": "Miss",
                        first_name = pax.firstName,
                        last_name = pax.lastName,
                        dob=pax.dateOfBirth.ToString("yyyy/MM/dd"),
                        travel_with="1"
                    };
                    bookFlight.infant_info.Add(_pax);
                }

            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(bookFlight);
        }

    }
}
