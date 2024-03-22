using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Ease2Fly
{
    public class Ease2FlyRequestMappking
    {
        string UserName = ConfigurationManager.AppSettings["E2F_UserName"].ToString();
        string PWD = ConfigurationManager.AppSettings["E2F_PWD"].ToString();
        string ApiKey = ConfigurationManager.AppSettings["E2F_ApiKey"].ToString();
        public string getSearchToken()
        {
            Ease2FlyClass.Token token = new Ease2FlyClass.Token()
            {
                email = UserName,
                pwd = PWD,
                efly_api_key = ApiKey

            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(token);
        }


        public string getBookingRequest(Core.Flight.FlightBookingRequest request, int ctr)
        {
            Ease2FlyClass.book_flight bookFlight = new Ease2FlyClass.book_flight()
            {
                adults = request.adults,
                child = request.child,
                infant = request.infants,
                sector_id = request.flightResult[0].Fare.E2F_id,
                fare = request.flightResult[0].Fare.PublishedFare,
                phone = request.phoneNo,
                email = request.emailID,
                adult_info = new List<Ease2FlyClass.AdultInfo>(),
                child_info = new List<Ease2FlyClass.ChildInfo>(),
                infant_info = new List<Ease2FlyClass.InfantInfo>()
            };
            Random rnd = new Random();
            foreach (var pax in request.passengerDetails)
            {
                if (pax.passengerType == Core.PassengerType.Adult)
                {

                    Ease2FlyClass.AdultInfo _pax = new Ease2FlyClass.AdultInfo
                    {
                        ttl = pax.title + ".",
                        first_name = pax.firstName,
                        last_name = pax.lastName,
                        whlchr = "false",
                        passport_no = "",
                        passport_nationality = "",
                        passport_dob = "",
                        passport_exp = ""
                    };
                    bookFlight.adult_info.Add(_pax);
                }
                else if (pax.passengerType == Core.PassengerType.Child)
                {
                    int age = (request.flightResult.LastOrDefault().FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime.Year - pax.dateOfBirth.Year);
                    Ease2FlyClass.ChildInfo _pax = new Ease2FlyClass.ChildInfo
                    {
                        ttl =  pax.gender == Core.Gender.Male ? "MASTER" : "MISS",
                        first_name = pax.firstName,
                        last_name = pax.lastName,
                        whlchr = "false",
                        passport_no = "",
                        passport_nationality = "",
                        passport_dob = "",
                        passport_exp = "",
                        //age = age == 0 ? 1 : age
                        age =pax.dateOfBirth.ToString("yyyy-MM-dd")
                    };
                    bookFlight.child_info.Add(_pax);
                }

                else if (pax.passengerType == Core.PassengerType.Infant)
                {
                    int age = (request.flightResult.LastOrDefault().FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime.Year - pax.dateOfBirth.Year);
                    Ease2FlyClass.InfantInfo _pax = new Ease2FlyClass.InfantInfo
                    {
                        ttl = pax.gender == Core.Gender.Male ? "Male" : "Female",
                        first_name = pax.firstName,
                        last_name = pax.lastName,
                        whlchr = "false",
                        passport_no = "",
                        passport_nationality = "",
                        passport_dob = "",
                        passport_exp = "",
                        // age = age == 0 ? 0 : 1
                        age = pax.dateOfBirth.ToString("yyyy-MM-dd")
                    };
                    bookFlight.infant_info.Add(_pax);
                }

            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(bookFlight);
        }
    }
}
