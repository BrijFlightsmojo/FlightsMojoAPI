using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.FareBoutique
{
    public class FareBoutiqueRequestMappking
    {
        public string getFlightSearchRequest(Core.Flight.FlightSearchRequest fsr, string TokenId, string FB_Ip, int i)
        {
            ServicesHub.FareBoutique.FareBoutiqueClass.search flightSearchRequest = new FareBoutiqueClass.search()
            {
                end_user_ip = FB_Ip,//\\fsr.userIP,
                token = TokenId,
                departure_city_code = fsr.segment[i].originAirport,
                arrival_city_code = fsr.segment[i].destinationAirport,
                trip_type = "0",//(fsr.segment.Count - 1).ToString(),
                departure_date = fsr.segment[i].travelDate.ToString("yyyy-MM-dd"),
                return_date = "",//(fsr.segment.Count > 1 ? fsr.segment[0].travelDate.ToString("yyyy-MM-dd") : ""),
                adult = fsr.adults,
                children = fsr.child,
                infant = fsr.infants,
                with_aiq = !fsr.isMetaRequest
            };
            //if (isAirIQ)
            //{
            flightSearchRequest.with_aiq = false;
            //}
            return Newtonsoft.Json.JsonConvert.SerializeObject(flightSearchRequest);
        }
        public string getFareQuoteRequest(Core.Flight.PriceVerificationRequest request, string TokenId, string FB_Ip)
        {
            FareBoutiqueClass.fare_quote fareQuote = new FareBoutiqueClass.fare_quote()
            {
                adult_children = request.adults + request.child,
                infant = request.infants,
                departure_date = request.flightResult[0].FlightSegments.FirstOrDefault().Segments.FirstOrDefault().DepTime.ToString("yyyy-MM-dd"),
                end_user_ip = FB_Ip,
                flight_id = request.flightResult[0].Fare.FB_flight_id,
                token = TokenId,
                @static = request.flightResult[0].Fare.FB_static

            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(fareQuote);
        }
        public string getBookingRequest(Core.Flight.FlightBookingRequest request, int ctr, string TokenId, string FB_Ip)
        {
            FareBoutiqueClass.book_flight bookFlight = new FareBoutiqueClass.book_flight()
            {
                total_book_seats = request.adults + request.child + request.infants,
                infant = request.infants,
                departure_date = request.flightResult[0].FlightSegments.FirstOrDefault().Segments.FirstOrDefault().DepTime.ToString("yyyy-MM-dd"),
                end_user_ip = FB_Ip,//request.userIP,
                flight_id = request.flightResult[0].Fare.FB_flight_id,
                token = TokenId,
                adult = request.adults,
                children = request.child,
                booking_token_id = request.FB_booking_token_id,
                contact_email = request.emailID,
                contact_name = request.passengerDetails[0].firstName + " " + request.passengerDetails[0].lastName,
                contact_number = request.phoneNo,
                return_date = "",
                total_amount = (int)request.flightResult[ctr].Fare.PublishedFare,
                flight_traveller_details = new List<FareBoutiqueClass.FlightTravellerDetail>(),
                @static = request.flightResult[ctr].Fare.FB_static
            };
            Random rnd = new Random();
            foreach (var pax in request.passengerDetails)
            {
                if (pax.passengerType == Core.PassengerType.Adult)
                {

                    int y = rnd.Next(0, 24);
                    int d = rnd.Next(0, 365);
                    pax.dateOfBirth = DateTime.Today.AddYears(-24).AddYears(-y).AddDays(d);
                    int age = (request.flightResult.LastOrDefault().FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime.Year - pax.dateOfBirth.Year);

                    FareBoutiqueClass.FlightTravellerDetail _pax = new FareBoutiqueClass.FlightTravellerDetail
                    {
                        gender = pax.title,
                        first_name = pax.firstName,
                        middle_name = "",
                        last_name = pax.lastName,
                        dob = pax.dateOfBirth.ToString("yyyy-MM-dd"),
                        age = age,
                        passport_expire_date = pax.expiryDate.Value.ToString("yyyy-MM-dd"),
                        passport_no = pax.passportNumber
                    };
                    bookFlight.flight_traveller_details.Add(_pax);
                }
                else if (pax.passengerType == Core.PassengerType.Child)
                {
                    int age = (request.flightResult.LastOrDefault().FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime.Year - pax.dateOfBirth.Year);

                    FareBoutiqueClass.FlightTravellerDetail _pax = new FareBoutiqueClass.FlightTravellerDetail
                    {
                        gender = pax.title,
                        first_name = pax.firstName,
                        middle_name = "",
                        last_name = pax.lastName,
                        dob = pax.dateOfBirth.ToString("yyyy-MM-dd"),
                        age = age == 0 ? 1 : age,
                        passport_expire_date = pax.expiryDate.Value.ToString("yyyy-MM-dd"),
                        passport_no = pax.passportNumber
                    };
                    bookFlight.flight_traveller_details.Add(_pax);
                }

                else if (pax.passengerType == Core.PassengerType.Infant)
                {
                    int age = (request.flightResult.LastOrDefault().FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime.Year - pax.dateOfBirth.Year);

                    FareBoutiqueClass.FlightTravellerDetail _pax = new FareBoutiqueClass.FlightTravellerDetail
                    {
                        gender = pax.title,
                        first_name = pax.firstName,
                        middle_name = "",
                        last_name = pax.lastName,
                        dob = pax.dateOfBirth.ToString("yyyy-MM-dd"),
                        age = age >= 2 ? 1 : age,
                        passport_expire_date = pax.expiryDate.Value.ToString("yyyy-MM-dd"),
                        passport_no = pax.passportNumber
                    };
                    bookFlight.flight_traveller_details.Add(_pax);
                }

            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(bookFlight);
        }
        public string getCheckBookingRequest(Core.Flight.FlightBookingRequest request, int ctr, string TokenId, string reference_id, string FB_Ip)
        {
            FareBoutiqueClass.booking_details bookFlight = new FareBoutiqueClass.booking_details()
            {
                end_user_ip = FB_Ip,
                token = TokenId,
                reference_id = reference_id
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(bookFlight);
        }

        public string getRouteRequest(string TokenId, string FB_Ip)
        {
            FareBoutiqueClass.RouteRequest routeRequest = new FareBoutiqueClass.RouteRequest()
            {
                end_user_ip = FB_Ip,

                token = TokenId,
                trip_type = "oneway",
                with_aiq = 0

            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(routeRequest);
        }
        public string getRouteDateRequest(string TokenId, string FB_Ip,string Org,string dest)
        {
            FareBoutiqueClass.RouteDateRequest routeDateRequest = new FareBoutiqueClass.RouteDateRequest()
            {
                end_user_ip = FB_Ip,
                token = TokenId,
                trip_type = "oneway",
               departure_city_code=Org,
               arrival_city_code=dest

            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(routeDateRequest);
        }
    }
}
