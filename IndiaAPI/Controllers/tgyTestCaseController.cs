using Core;
using Core.Flight;
using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace IndiaAPI.Controllers
{
    [RoutePrefix("tgyTestCase")]
    public class tgyTestCaseController : ApiController
    {

        [HttpGet]
        [Route("Case1")]
        public HttpResponseMessage Case1(int d = 1)
        {
            FlightSearchRequest flightSearchReq = new FlightSearchRequest();
            //flightSearchReq.Sources = new List<string>();
            flightSearchReq.adults = 1;
            flightSearchReq.child = 0;
            flightSearchReq.infants = 0;
            flightSearchReq.tripType = Core.TripType.RoundTrip;
            flightSearchReq.searchDirectFlight = false;
            flightSearchReq.userIP = "180.151.226.194";
            //flightSearchReq.OneStopFlight = false;
            //flightSearchReq.PreferredAirlines = new List<string>();

            //flightSearchReq.PreferredAirlines.Add("All");
            flightSearchReq.segment = new List<SearchSegment>();
            flightSearchReq.segment.Add(new SearchSegment()
            {
                originAirport = "BOM",
                orgArp = Core.FlightUtility.GetAirport("BOM"),
                destinationAirport = "DXB",
                destArp = Core.FlightUtility.GetAirport("DXB"),
                travelDate = DateTime.Today.AddDays(61)//Convert.ToDateTime("2023-12-27") //
            });

            if (flightSearchReq.tripType != Core.TripType.OneWay)
            {
                flightSearchReq.segment.Add(new SearchSegment()
                {
                    originAirport = "DXB",
                    orgArp = Core.FlightUtility.GetAirport("DXB"),
                    destinationAirport = "BOM",
                    destArp = Core.FlightUtility.GetAirport("BOM"),
                    travelDate = DateTime.Today.AddDays(73)
                });
            }
            flightSearchReq.tgy_Request_id = DateTime.Now.ToString("ddMMyyyHHmmsss");
            flightSearchReq.travelType = new Core.FlightUtility().getTravelType(flightSearchReq.segment[0].orgArp.countryCode, flightSearchReq.segment[0].destArp.countryCode);
            flightSearchReq.siteId = SiteId.FlightsMojoIN;
            flightSearchReq.sourceMedia = "1000";
            flightSearchReq.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmmss");
            FlightSearchResponse response = new FlightSearchResponse(flightSearchReq);
            new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightResult(flightSearchReq, ref response); 

            PriceVerificationRequest pvReq = new PriceVerificationRequest()
            {
                adults = flightSearchReq.adults,
                child = flightSearchReq.child,
                infants = flightSearchReq.infants,
                infantsWs = 0,
                flightResult = new List<FlightResult>(),
                isFareQuote = true,
                isFareRule = false,
                isSSR = false,
                siteID = flightSearchReq.siteId,
                sourceMedia = flightSearchReq.sourceMedia,
                userIP = flightSearchReq.userIP,
                userSearchID = flightSearchReq.userSearchID,
                userSessionID = flightSearchReq.userSessionID,
                tgy_Search_Key = response.tgy_Search_Key,
                PhoneNo = "6464646464",
                tgy_Request_id = flightSearchReq.tgy_Request_id
            };

            pvReq.flightResult.Add(response.Results[0][0]);
            if (flightSearchReq.tripType == TripType.RoundTrip && response.Results.Count > 1)
            {
                pvReq.flightResult.Add(response.Results[1][0]);
            }
            var kk = new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightRePrice(pvReq);
            //return Request.CreateResponse(HttpStatusCode.OK, kk);
            FlightBookingRequest bookingReq = new FlightBookingRequest()
            {
                AdminID = 100,
                adults = flightSearchReq.adults,
                aircraftDetail = new List<AircraftDetail>(),
                airline = new List<Airline>(),
                airport = new List<Airport>(),
                bookingID = 0,
                BrowserDetails = "",
                child = flightSearchReq.child,
                infants = flightSearchReq.infants,
                currencyCode = "INR",
                deepLink = "",
                emailID = "kundan@flightsmojo.com",
                flightResult = pvReq.flightResult,
                infantsWs = 0,
                LastCheckInDate = DateTime.Today,
                mobileNo = "6464646464",
                passengerDetails = new List<PassengerDetails>(),
                paymentDetails = new PaymentDetails(),
                phoneNo = "6464646464",
                prodID = 1,
                siteID = Core.SiteId.FlightsMojoIN,
                sourceMedia = "1000",
                transactionID = 3131,
                updatedBookingAmount = 0,
                userIP = pvReq.userIP,
                userSearchID = flightSearchReq.userSearchID,
                userSessionID = flightSearchReq.userSessionID,
                TvoTraceId = response.TraceId,
                TjBookingID = kk.TjBookingID,
                tgy_Flight_Key = kk.tgy_Flight_Key,
                tgy_Search_Key = response.tgy_Search_Key,
                tgy_Request_id = flightSearchReq.tgy_Request_id
            };
            //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
            //{
            //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
            //}

            PassengerDetails objPassenger = new PassengerDetails
            {
                firstName = "KUNDAN",
                lastName = "KUMAR",
                passengerType = PassengerType.Adult,
                title = "Mr",
                gender = Gender.Male,
                dateOfBirth = DateTime.Today.AddYears(-25)
            };
            if (flightSearchReq.travelType == TravelType.International)
            {
                objPassenger.passportNumber = "KJHHJKHKJH";
                objPassenger.expiryDate = DateTime.Today.AddYears(5);
                objPassenger.nationality = "IN";
            }
            objPassenger.nationality = "IN";
            bookingReq.passengerDetails.Add(objPassenger);
            if (bookingReq.adults >= 2)
            {
                PassengerDetails objPassenger1 = new PassengerDetails
                {
                    firstName = "OOM",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Adult,
                    title = "Mr",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-21)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger1.passportNumber = "KJHHJKHKJK";
                    objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                }
                bookingReq.passengerDetails.Add(objPassenger1);
            }
            if (bookingReq.child > 0)
            {
                PassengerDetails objPassenger2 = new PassengerDetails
                {
                    firstName = "MOHON",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Child,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-8)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger2.passportNumber = "KJHHJKHKKK";
                    objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                }
                bookingReq.passengerDetails.Add(objPassenger2);
            }
            if (bookingReq.child >= 2)
            {
                PassengerDetails objPassenger1 = new PassengerDetails
                {
                    firstName = "SOHAN",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Child,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-6)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger1.passportNumber = "KKHHJKHKJK";
                    objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                }
                bookingReq.passengerDetails.Add(objPassenger1);
            }
            if (bookingReq.infants > 0)
            {
                PassengerDetails objPassenger3 = new PassengerDetails
                {
                    firstName = "GOPAL",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Infant,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-1)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger3.passportNumber = "KJHHJKKKKK";
                    objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                }
                bookingReq.passengerDetails.Add(objPassenger3);
            }
            //var response = new FlightMapper().saveBookingDetails(bookingReq);
            FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq);
            StringBuilder sbLogger = new StringBuilder();
            new ServicesHub.Travelogy.TravelogyServiceMapping().CreateBooking(bookingReq, ref BookResponse, ref sbLogger);


            return Request.CreateResponse(HttpStatusCode.OK, BookResponse);


        }

        [HttpGet]
        [Route("Case2")]
        public HttpResponseMessage Case2(int d = 2)
        {
            FlightSearchRequest flightSearchReq = new FlightSearchRequest();
            //flightSearchReq.Sources = new List<string>();
            flightSearchReq.adults = 2;
            flightSearchReq.child = 2;
            flightSearchReq.infants = 2;
            flightSearchReq.tripType = Core.TripType.RoundTrip;
            flightSearchReq.searchDirectFlight = false;
            flightSearchReq.userIP = "180.151.226.194";
            //flightSearchReq.OneStopFlight = false;
            //flightSearchReq.PreferredAirlines = new List<string>();

            //flightSearchReq.PreferredAirlines.Add("All");
            flightSearchReq.segment = new List<SearchSegment>();
            flightSearchReq.segment.Add(new SearchSegment()
            {
                originAirport = "DEL",
                orgArp = Core.FlightUtility.GetAirport("DEL"),
                destinationAirport = "PAT",
                destArp = Core.FlightUtility.GetAirport("PAT"),
                travelDate = DateTime.Today.AddDays(48)//Convert.ToDateTime("2023-12-27") //
            });

            if (flightSearchReq.tripType != Core.TripType.OneWay)
            {
                flightSearchReq.segment.Add(new SearchSegment()
                {
                    originAirport = "PAT",
                    orgArp = Core.FlightUtility.GetAirport("PAT"),
                    destinationAirport = "DEL",
                    destArp = Core.FlightUtility.GetAirport("DEL"),
                    travelDate = DateTime.Today.AddDays(58)
                });
            }
            flightSearchReq.tgy_Request_id = DateTime.Now.ToString("ddMMyyyHHmmsss");
            flightSearchReq.travelType = new Core.FlightUtility().getTravelType(flightSearchReq.segment[0].orgArp.countryCode, flightSearchReq.segment[0].destArp.countryCode);
            flightSearchReq.siteId = SiteId.FlightsMojoIN;
            flightSearchReq.sourceMedia = "1000";
            flightSearchReq.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmmss");
            FlightSearchResponse response = new FlightSearchResponse(flightSearchReq);
            new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightResult(flightSearchReq, ref response);
            //new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(flightSearchReq, ref response);
            //return Request.CreateResponse(HttpStatusCode.OK, response);

            //return SearchFlight("fl1asdfghasdftmoasdfjado2o", flightSearchReq);
            //var result = new FlightMapper().GetFlightResult(flightSearchReq);
            //return Request.CreateResponse(HttpStatusCode.OK, response);


            PriceVerificationRequest pvReq = new PriceVerificationRequest()
            {
                adults = flightSearchReq.adults,
                child = flightSearchReq.child,
                infants = flightSearchReq.infants,
                infantsWs = 0,
                flightResult = new List<FlightResult>(),
                isFareQuote = true,
                isFareRule = false,
                isSSR = false,
                siteID = flightSearchReq.siteId,
                sourceMedia = flightSearchReq.sourceMedia,
                userIP = flightSearchReq.userIP,
                userSearchID = flightSearchReq.userSearchID,
                userSessionID = flightSearchReq.userSessionID,
                tgy_Search_Key = response.tgy_Search_Key,
                PhoneNo = "9874563215",
                tgy_Request_id = flightSearchReq.tgy_Request_id
            };

            pvReq.flightResult.Add(response.Results[0].Where(k => k.FlightSegments[0].Segments[0].Airline == "G8").FirstOrDefault());
            if (flightSearchReq.tripType == TripType.RoundTrip && response.Results.Count > 1)
            {
                pvReq.flightResult.Add(response.Results[1].Where(k => k.FlightSegments[0].Segments[0].Airline == "G8").FirstOrDefault());
            }
            var kk = new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightRePrice(pvReq);
            //return Request.CreateResponse(HttpStatusCode.OK, kk);
            FlightBookingRequest bookingReq = new FlightBookingRequest()
            {
                AdminID = 100,
                adults = flightSearchReq.adults,
                aircraftDetail = new List<AircraftDetail>(),
                airline = new List<Airline>(),
                airport = new List<Airport>(),
                bookingID = 0,
                BrowserDetails = "",
                child = flightSearchReq.child,
                infants = flightSearchReq.infants,
                currencyCode = "INR",
                deepLink = "",
                emailID = "brij@flightsmojo.com",
                flightResult = pvReq.flightResult,
                infantsWs = 0,
                LastCheckInDate = DateTime.Today,
                mobileNo = "9874563215",
                passengerDetails = new List<PassengerDetails>(),
                paymentDetails = new PaymentDetails(),
                phoneNo = "9874563215",
                prodID = 1,
                siteID = Core.SiteId.FlightsMojoIN,
                sourceMedia = "1000",
                transactionID = 3131,
                updatedBookingAmount = 0,
                userIP = pvReq.userIP,
                userSearchID = flightSearchReq.userSearchID,
                userSessionID = flightSearchReq.userSessionID,
                TvoTraceId = response.TraceId,
                TjBookingID = kk.TjBookingID,
                tgy_Flight_Key = kk.tgy_Flight_Key,
                tgy_Search_Key = response.tgy_Search_Key,
                tgy_Request_id = flightSearchReq.tgy_Request_id,
                tgy_Block_Ticket_Allowed=kk.tgy_Block_Ticket_Allowed,
            };
            //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
            //{
            //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
            //}

            PassengerDetails objPassenger = new PassengerDetails
            {
                firstName = "KUNDAN",
                lastName = "KUMAR",
                passengerType = PassengerType.Adult,
                title = "Mr",
                gender = Gender.Male,
                dateOfBirth = DateTime.Today.AddYears(-25)
            };
            if (flightSearchReq.travelType == TravelType.International)
            {
                objPassenger.passportNumber = "KJHHJKHKJH";
                objPassenger.expiryDate = DateTime.Today.AddYears(5);
                objPassenger.nationality = "IN";
            }
            objPassenger.nationality = "IN";
            bookingReq.passengerDetails.Add(objPassenger);
            if (bookingReq.adults >= 2)
            {
                PassengerDetails objPassenger1 = new PassengerDetails
                {
                    firstName = "OOM",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Adult,
                    title = "Mr",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-21)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger1.passportNumber = "KJHHJKHKJK";
                    objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                }
                bookingReq.passengerDetails.Add(objPassenger1);
            }
            if (bookingReq.child > 0)
            {
                PassengerDetails objPassenger2 = new PassengerDetails
                {
                    firstName = "MOHON",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Child,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-8)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger2.passportNumber = "KJHHJKHKKK";
                    objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                }
                bookingReq.passengerDetails.Add(objPassenger2);
            }
            if (bookingReq.child >= 2)
            {
                PassengerDetails objPassenger1 = new PassengerDetails
                {
                    firstName = "SOHAN",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Child,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-6)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger1.passportNumber = "KKHHJKHKJK";
                    objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                }
                bookingReq.passengerDetails.Add(objPassenger1);
            }
            if (bookingReq.infants > 0)
            {
                PassengerDetails objPassenger3 = new PassengerDetails
                {
                    firstName = "GOPAL",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Infant,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-1)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger3.passportNumber = "KJHHJKKKKK";
                    objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                }
                bookingReq.passengerDetails.Add(objPassenger3);
            }
            if (bookingReq.infants >= 2)
            {
                PassengerDetails objPassenger3 = new PassengerDetails
                {
                    firstName = "GOPI",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Infant,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-1).AddMonths(2)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger3.passportNumber = "KJHHJKKKKK";
                    objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                }
                bookingReq.passengerDetails.Add(objPassenger3);
            }

            //var response = new FlightMapper().saveBookingDetails(bookingReq);
            FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq);
            StringBuilder sbLogger = new StringBuilder();
            new ServicesHub.Travelogy.TravelogyServiceMapping().CreateBooking(bookingReq, ref BookResponse, ref sbLogger);
            return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
        }
        [HttpGet]
        [Route("Case3")]
        public HttpResponseMessage Case3(int d = 2)
        {
            FlightSearchRequest flightSearchReq = new FlightSearchRequest();
            //flightSearchReq.Sources = new List<string>();
            flightSearchReq.adults = 1;
            flightSearchReq.child = 1;
            flightSearchReq.infants = 1;
            flightSearchReq.tripType = Core.TripType.RoundTrip;
            flightSearchReq.searchDirectFlight = false;
            flightSearchReq.userIP = "180.151.226.194";
            //flightSearchReq.OneStopFlight = false;
            //flightSearchReq.PreferredAirlines = new List<string>();

            //flightSearchReq.PreferredAirlines.Add("All");
            flightSearchReq.segment = new List<SearchSegment>();
            flightSearchReq.segment.Add(new SearchSegment()
            {
                originAirport = "PAT",
                orgArp = Core.FlightUtility.GetAirport("PAT"),
                destinationAirport = "GOI",
                destArp = Core.FlightUtility.GetAirport("GOI"),
                travelDate = DateTime.Today.AddDays(40)//Convert.ToDateTime("2023-12-27") //
            });

            if (flightSearchReq.tripType != Core.TripType.OneWay)
            {
                flightSearchReq.segment.Add(new SearchSegment()
                {
                    originAirport = "GOI",
                    orgArp = Core.FlightUtility.GetAirport("GOI"),
                    destinationAirport = "PAT",
                    destArp = Core.FlightUtility.GetAirport("PAT"),
                    travelDate = DateTime.Today.AddDays(50)
                });
            }
            flightSearchReq.tgy_Request_id = DateTime.Now.ToString("ddMMyyyHHmmsss");
            flightSearchReq.travelType = new Core.FlightUtility().getTravelType(flightSearchReq.segment[0].orgArp.countryCode, flightSearchReq.segment[0].destArp.countryCode);
            flightSearchReq.siteId = SiteId.FlightsMojoIN;
            flightSearchReq.sourceMedia = "1000";
            flightSearchReq.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmmss");
            FlightSearchResponse response = new FlightSearchResponse(flightSearchReq);
            new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightResult(flightSearchReq, ref response);
            //new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(flightSearchReq, ref response);
            //return Request.CreateResponse(HttpStatusCode.OK, response);

            //return SearchFlight("fl1asdfghasdftmoasdfjado2o", flightSearchReq);
            //var result = new FlightMapper().GetFlightResult(flightSearchReq);
            //return Request.CreateResponse(HttpStatusCode.OK, response);


            PriceVerificationRequest pvReq = new PriceVerificationRequest()
            {
                adults = flightSearchReq.adults,
                child = flightSearchReq.child,
                infants = flightSearchReq.infants,
                infantsWs = 0,
                flightResult = new List<FlightResult>(),
                isFareQuote = true,
                isFareRule = false,
                isSSR = false,
                siteID = flightSearchReq.siteId,
                sourceMedia = flightSearchReq.sourceMedia,
                userIP = flightSearchReq.userIP,
                userSearchID = flightSearchReq.userSearchID,
                userSessionID = flightSearchReq.userSessionID,
                tgy_Search_Key = response.tgy_Search_Key,
                PhoneNo = "9638527415",
                tgy_Request_id = flightSearchReq.tgy_Request_id
            };

            pvReq.flightResult.Add(response.Results[0].Where(k => k.FlightSegments[0].Segments[0].Airline == "G8" && k.FlightSegments[0].Segments.Count>1).FirstOrDefault());
            if (flightSearchReq.tripType == TripType.RoundTrip && response.Results.Count > 1)
            {
                pvReq.flightResult.Add(response.Results[1].Where(k => k.FlightSegments[0].Segments[0].Airline == "G8"&& k.FlightSegments[0].Segments.Count > 1).FirstOrDefault());
            }
            var kk = new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightRePrice(pvReq);
            //return Request.CreateResponse(HttpStatusCode.OK, kk);
            FlightBookingRequest bookingReq = new FlightBookingRequest()
            {
                AdminID = 100,
                adults = flightSearchReq.adults,
                aircraftDetail = new List<AircraftDetail>(),
                airline = new List<Airline>(),
                airport = new List<Airport>(),
                bookingID = 0,
                BrowserDetails = "",
                child = flightSearchReq.child,
                infants = flightSearchReq.infants,
                currencyCode = "INR",
                deepLink = "",
                emailID = "amrish@flightsmojo.com",
                flightResult = pvReq.flightResult,
                infantsWs = 0,
                LastCheckInDate = DateTime.Today,
                mobileNo = "9638527415",
                passengerDetails = new List<PassengerDetails>(),
                paymentDetails = new PaymentDetails(),
                phoneNo = "9638527415",
                prodID = 1,
                siteID = Core.SiteId.FlightsMojoIN,
                sourceMedia = "1000",
                transactionID = 3131,
                updatedBookingAmount = 0,
                userIP = pvReq.userIP,
                userSearchID = flightSearchReq.userSearchID,
                userSessionID = flightSearchReq.userSessionID,
                TvoTraceId = response.TraceId,
                TjBookingID = kk.TjBookingID,
                tgy_Flight_Key = kk.tgy_Flight_Key,
                tgy_Search_Key = response.tgy_Search_Key,
                tgy_Request_id = flightSearchReq.tgy_Request_id,
                tgy_Block_Ticket_Allowed = kk.tgy_Block_Ticket_Allowed,
            };
            //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
            //{
            //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
            //}

            PassengerDetails objPassenger = new PassengerDetails
            {
                firstName = "KUNDAN",
                lastName = "KUMAR",
                passengerType = PassengerType.Adult,
                title = "Mr",
                gender = Gender.Male,
                dateOfBirth = DateTime.Today.AddYears(-25)
            };
            if (flightSearchReq.travelType == TravelType.International)
            {
                objPassenger.passportNumber = "KJHHJKHKJH";
                objPassenger.expiryDate = DateTime.Today.AddYears(5);
                objPassenger.nationality = "IN";
            }
            objPassenger.nationality = "IN";
            bookingReq.passengerDetails.Add(objPassenger);
            if (bookingReq.adults >= 2)
            {
                PassengerDetails objPassenger1 = new PassengerDetails
                {
                    firstName = "OOM",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Adult,
                    title = "Mr",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-21)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger1.passportNumber = "KJHHJKHKJK";
                    objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                }
                bookingReq.passengerDetails.Add(objPassenger1);
            }
            if (bookingReq.child > 0)
            {
                PassengerDetails objPassenger2 = new PassengerDetails
                {
                    firstName = "MOHON",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Child,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-8)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger2.passportNumber = "KJHHJKHKKK";
                    objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                }
                bookingReq.passengerDetails.Add(objPassenger2);
            }
            if (bookingReq.child >= 2)
            {
                PassengerDetails objPassenger1 = new PassengerDetails
                {
                    firstName = "SOHAN",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Child,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-6)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger1.passportNumber = "KKHHJKHKJK";
                    objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                }
                bookingReq.passengerDetails.Add(objPassenger1);
            }
            if (bookingReq.infants > 0)
            {
                PassengerDetails objPassenger3 = new PassengerDetails
                {
                    firstName = "GOPAL",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Infant,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-1)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger3.passportNumber = "KJHHJKKKKK";
                    objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                }
                bookingReq.passengerDetails.Add(objPassenger3);
            }
            if (bookingReq.infants >= 2)
            {
                PassengerDetails objPassenger3 = new PassengerDetails
                {
                    firstName = "GOPI",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Infant,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-1).AddMonths(2)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger3.passportNumber = "KJHHJKKKKK";
                    objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                }
                bookingReq.passengerDetails.Add(objPassenger3);
            }

            //var response = new FlightMapper().saveBookingDetails(bookingReq);
            FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq);
            StringBuilder sbLogger = new StringBuilder();
            new ServicesHub.Travelogy.TravelogyServiceMapping().CreateBooking(bookingReq, ref BookResponse, ref sbLogger);
            return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
        }
        [HttpGet]
        [Route("Case4")]
        public HttpResponseMessage Case4(int d = 2)
        {
            FlightSearchRequest flightSearchReq = new FlightSearchRequest();
            //flightSearchReq.Sources = new List<string>();
            flightSearchReq.adults = 2;
            flightSearchReq.child = 1;
            flightSearchReq.infants = 0;
            flightSearchReq.tripType = Core.TripType.OneWay;
            flightSearchReq.searchDirectFlight = false;
            flightSearchReq.userIP = "127.0.0.1";
            //flightSearchReq.OneStopFlight = false;
            //flightSearchReq.PreferredAirlines = new List<string>();

            //flightSearchReq.PreferredAirlines.Add("All");
            flightSearchReq.segment = new List<SearchSegment>();
            flightSearchReq.segment.Add(new SearchSegment()
            {
                originAirport = "DEL",
                orgArp = Core.FlightUtility.GetAirport("DEL"),
                destinationAirport = "BOM",
                destArp = Core.FlightUtility.GetAirport("BOM"),
                travelDate = DateTime.Today.AddDays(51)//Convert.ToDateTime("2023-12-27") //
            });
          
            flightSearchReq.tgy_Request_id = DateTime.Now.ToString("ddMMyyyHHmmsss");
            flightSearchReq.travelType = new Core.FlightUtility().getTravelType(flightSearchReq.segment[0].orgArp.countryCode, flightSearchReq.segment[0].destArp.countryCode);
            flightSearchReq.siteId = SiteId.FlightsMojoIN;
            flightSearchReq.sourceMedia = "1000";
            flightSearchReq.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmmss");
            FlightSearchResponse response = new FlightSearchResponse(flightSearchReq);
            new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightResult(flightSearchReq, ref response);
            //new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(flightSearchReq, ref response);
            //return Request.CreateResponse(HttpStatusCode.OK, response);

            //return SearchFlight("fl1asdfghasdftmoasdfjado2o", flightSearchReq);
            //var result = new FlightMapper().GetFlightResult(flightSearchReq);
            //return Request.CreateResponse(HttpStatusCode.OK, response);


            PriceVerificationRequest pvReq = new PriceVerificationRequest()
            {
                adults = flightSearchReq.adults,
                child = flightSearchReq.child,
                infants = flightSearchReq.infants,
                infantsWs = 0,
                flightResult = new List<FlightResult>(),
                isFareQuote = true,
                isFareRule = false,
                isSSR = false,
                siteID = flightSearchReq.siteId,
                sourceMedia = flightSearchReq.sourceMedia,
                userIP = flightSearchReq.userIP,
                userSearchID = flightSearchReq.userSearchID,
                userSessionID = flightSearchReq.userSessionID,
                tgy_Search_Key = response.tgy_Search_Key,
                PhoneNo = "9638527415",
                tgy_Request_id = flightSearchReq.tgy_Request_id
            };

            pvReq.flightResult.Add(response.Results[0].Where(k => k.FlightSegments[0].Segments[0].Airline == "G8").FirstOrDefault());
            if (flightSearchReq.tripType == TripType.RoundTrip && response.Results.Count > 1)
            {
                pvReq.flightResult.Add(response.Results[1].Where(k => k.FlightSegments[0].Segments[0].Airline == "G8" && k.FlightSegments[0].Segments.Count > 1).FirstOrDefault());
            }
            var kk = new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightRePrice(pvReq);
            //return Request.CreateResponse(HttpStatusCode.OK, kk);
            FlightBookingRequest bookingReq = new FlightBookingRequest()
            {
                AdminID = 100,
                adults = flightSearchReq.adults,
                aircraftDetail = new List<AircraftDetail>(),
                airline = new List<Airline>(),
                airport = new List<Airport>(),
                bookingID = 0,
                BrowserDetails = "",
                child = flightSearchReq.child,
                infants = flightSearchReq.infants,
                currencyCode = "INR",
                deepLink = "",
                emailID = "amrish@flightsmojo.com",
                flightResult = pvReq.flightResult,
                infantsWs = 0,
                LastCheckInDate = DateTime.Today,
                mobileNo = "9638527415",
                passengerDetails = new List<PassengerDetails>(),
                paymentDetails = new PaymentDetails(),
                phoneNo = "9638527415",
                prodID = 1,
                siteID = Core.SiteId.FlightsMojoIN,
                sourceMedia = "1000",
                transactionID = 3131,
                updatedBookingAmount = 0,
                userIP = pvReq.userIP,
                userSearchID = flightSearchReq.userSearchID,
                userSessionID = flightSearchReq.userSessionID,
                TvoTraceId = response.TraceId,
                TjBookingID = kk.TjBookingID,
                tgy_Flight_Key = kk.tgy_Flight_Key,
                tgy_Search_Key = response.tgy_Search_Key,
                tgy_Request_id = flightSearchReq.tgy_Request_id,
                tgy_Block_Ticket_Allowed = kk.tgy_Block_Ticket_Allowed,
            };
            //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
            //{
            //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
            //}

            PassengerDetails objPassenger = new PassengerDetails
            {
                firstName = "KUNDAN",
                lastName = "KUMAR",
                passengerType = PassengerType.Adult,
                title = "Mr",
                gender = Gender.Male,
                dateOfBirth = DateTime.Today.AddYears(-25)
            };
            if (flightSearchReq.travelType == TravelType.International)
            {
                objPassenger.passportNumber = "KJHHJKHKJH";
                objPassenger.expiryDate = DateTime.Today.AddYears(5);
                objPassenger.nationality = "IN";
            }
            objPassenger.nationality = "IN";
            bookingReq.passengerDetails.Add(objPassenger);
            if (bookingReq.adults >= 2)
            {
                PassengerDetails objPassenger1 = new PassengerDetails
                {
                    firstName = "OOM",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Adult,
                    title = "Mr",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-21)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger1.passportNumber = "KJHHJKHKJK";
                    objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                }
                bookingReq.passengerDetails.Add(objPassenger1);
            }
            if (bookingReq.child > 0)
            {
                PassengerDetails objPassenger2 = new PassengerDetails
                {
                    firstName = "MOHON",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Child,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-8)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger2.passportNumber = "KJHHJKHKKK";
                    objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                }
                bookingReq.passengerDetails.Add(objPassenger2);
            }
            if (bookingReq.child >= 2)
            {
                PassengerDetails objPassenger1 = new PassengerDetails
                {
                    firstName = "SOHAN",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Child,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-6)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger1.passportNumber = "KKHHJKHKJK";
                    objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                }
                bookingReq.passengerDetails.Add(objPassenger1);
            }
            if (bookingReq.infants > 0)
            {
                PassengerDetails objPassenger3 = new PassengerDetails
                {
                    firstName = "GOPAL",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Infant,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-1)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger3.passportNumber = "KJHHJKKKKK";
                    objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                }
                bookingReq.passengerDetails.Add(objPassenger3);
            }
            if (bookingReq.infants >= 2)
            {
                PassengerDetails objPassenger3 = new PassengerDetails
                {
                    firstName = "GOPI",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Infant,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-1).AddMonths(2)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger3.passportNumber = "KJHHJKKKKK";
                    objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                }
                bookingReq.passengerDetails.Add(objPassenger3);
            }

            //var response = new FlightMapper().saveBookingDetails(bookingReq);
            FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq);
            StringBuilder sbLogger = new StringBuilder();
            new ServicesHub.Travelogy.TravelogyServiceMapping().CreateBooking(bookingReq, ref BookResponse, ref sbLogger);
            return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
        }
        [HttpGet]
        [Route("Case5")]
        public HttpResponseMessage Case5(int d = 2)
        {
            FlightSearchRequest flightSearchReq = new FlightSearchRequest();
            //flightSearchReq.Sources = new List<string>();
            flightSearchReq.adults = 2;
            flightSearchReq.child = 1;
            flightSearchReq.infants = 1;
            flightSearchReq.tripType = Core.TripType.OneWay;
            flightSearchReq.searchDirectFlight = false;
            flightSearchReq.userIP = "127.0.0.1";
            //flightSearchReq.OneStopFlight = false;
            //flightSearchReq.PreferredAirlines = new List<string>();

            //flightSearchReq.PreferredAirlines.Add("All");
            flightSearchReq.segment = new List<SearchSegment>();
            flightSearchReq.segment.Add(new SearchSegment()
            {
                originAirport = "DEL",
                orgArp = Core.FlightUtility.GetAirport("DEL"),
                destinationAirport = "BOM",
                destArp = Core.FlightUtility.GetAirport("BOM"),
                travelDate = DateTime.Today.AddDays(55)//Convert.ToDateTime("2023-12-27") //
            });

            flightSearchReq.tgy_Request_id = DateTime.Now.ToString("ddMMyyyHHmmsss");
            flightSearchReq.travelType = new Core.FlightUtility().getTravelType(flightSearchReq.segment[0].orgArp.countryCode, flightSearchReq.segment[0].destArp.countryCode);
            flightSearchReq.siteId = SiteId.FlightsMojoIN;
            flightSearchReq.sourceMedia = "1000";
            flightSearchReq.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmmss");
            FlightSearchResponse response = new FlightSearchResponse(flightSearchReq);
            new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightResult(flightSearchReq, ref response);
            //new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(flightSearchReq, ref response);
            //return Request.CreateResponse(HttpStatusCode.OK, response);

            //return SearchFlight("fl1asdfghasdftmoasdfjado2o", flightSearchReq);
            //var result = new FlightMapper().GetFlightResult(flightSearchReq);
            //return Request.CreateResponse(HttpStatusCode.OK, response);


            PriceVerificationRequest pvReq = new PriceVerificationRequest()
            {
                adults = flightSearchReq.adults,
                child = flightSearchReq.child,
                infants = flightSearchReq.infants,
                infantsWs = 0,
                flightResult = new List<FlightResult>(),
                isFareQuote = true,
                isFareRule = false,
                isSSR = false,
                siteID = flightSearchReq.siteId,
                sourceMedia = flightSearchReq.sourceMedia,
                userIP = flightSearchReq.userIP,
                userSearchID = flightSearchReq.userSearchID,
                userSessionID = flightSearchReq.userSessionID,
                tgy_Search_Key = response.tgy_Search_Key,
                PhoneNo = "9638527415",
                tgy_Request_id = flightSearchReq.tgy_Request_id
            };

            pvReq.flightResult.Add(response.Results[0].Where(k => k.FlightSegments[0].Segments[0].Airline == "G8").FirstOrDefault());
            if (flightSearchReq.tripType == TripType.RoundTrip && response.Results.Count > 1)
            {
                pvReq.flightResult.Add(response.Results[1].Where(k => k.FlightSegments[0].Segments[0].Airline == "G8" && k.FlightSegments[0].Segments.Count > 1).FirstOrDefault());
            }
            var kk = new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightRePrice(pvReq);
            //return Request.CreateResponse(HttpStatusCode.OK, kk);
            FlightBookingRequest bookingReq = new FlightBookingRequest()
            {
                AdminID = 100,
                adults = flightSearchReq.adults,
                aircraftDetail = new List<AircraftDetail>(),
                airline = new List<Airline>(),
                airport = new List<Airport>(),
                bookingID = 0,
                BrowserDetails = "",
                child = flightSearchReq.child,
                infants = flightSearchReq.infants,
                currencyCode = "INR",
                deepLink = "",
                emailID = "amrish@flightsmojo.com",
                flightResult = pvReq.flightResult,
                infantsWs = 0,
                LastCheckInDate = DateTime.Today,
                mobileNo = "9638527415",
                passengerDetails = new List<PassengerDetails>(),
                paymentDetails = new PaymentDetails(),
                phoneNo = "9638527415",
                prodID = 1,
                siteID = Core.SiteId.FlightsMojoIN,
                sourceMedia = "1000",
                transactionID = 3131,
                updatedBookingAmount = 0,
                userIP = pvReq.userIP,
                userSearchID = flightSearchReq.userSearchID,
                userSessionID = flightSearchReq.userSessionID,
                TvoTraceId = response.TraceId,
                TjBookingID = kk.TjBookingID,
                tgy_Flight_Key = kk.tgy_Flight_Key,
                tgy_Search_Key = response.tgy_Search_Key,
                tgy_Request_id = flightSearchReq.tgy_Request_id,
                tgy_Block_Ticket_Allowed = kk.tgy_Block_Ticket_Allowed,
                GSTCompany= "FLIGHTS MOJO BOOKINGS PRIVATE LIMITED",
                GSTNo= "07AACCF6706H1ZQ", 
                GSTAddress= "Plot No 83 Sector-28 Gurugram Haryana 122001",
                isGST=true,
                
            };
            //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
            //{
            //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
            //}

            PassengerDetails objPassenger = new PassengerDetails
            {
                firstName = "KUNDAN",
                lastName = "KUMAR",
                passengerType = PassengerType.Adult,
                title = "Mr",
                gender = Gender.Male,
                dateOfBirth = DateTime.Today.AddYears(-25)
            };
            if (flightSearchReq.travelType == TravelType.International)
            {
                objPassenger.passportNumber = "KJHHJKHKJH";
                objPassenger.expiryDate = DateTime.Today.AddYears(5);
                objPassenger.nationality = "IN";
            }
            objPassenger.nationality = "IN";
            bookingReq.passengerDetails.Add(objPassenger);
            if (bookingReq.adults >= 2)
            {
                PassengerDetails objPassenger1 = new PassengerDetails
                {
                    firstName = "OOM",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Adult,
                    title = "Mr",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-21)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger1.passportNumber = "KJHHJKHKJK";
                    objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                }
                bookingReq.passengerDetails.Add(objPassenger1);
            }
            if (bookingReq.child > 0)
            {
                PassengerDetails objPassenger2 = new PassengerDetails
                {
                    firstName = "MOHON",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Child,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-8)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger2.passportNumber = "KJHHJKHKKK";
                    objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                }
                bookingReq.passengerDetails.Add(objPassenger2);
            }
            if (bookingReq.child >= 2)
            {
                PassengerDetails objPassenger1 = new PassengerDetails
                {
                    firstName = "SOHAN",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Child,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-6)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger1.passportNumber = "KKHHJKHKJK";
                    objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                }
                bookingReq.passengerDetails.Add(objPassenger1);
            }
            if (bookingReq.infants > 0)
            {
                PassengerDetails objPassenger3 = new PassengerDetails
                {
                    firstName = "GOPAL",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Infant,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-1)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger3.passportNumber = "KJHHJKKKKK";
                    objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                }
                bookingReq.passengerDetails.Add(objPassenger3);
            }
            if (bookingReq.infants >= 2)
            {
                PassengerDetails objPassenger3 = new PassengerDetails
                {
                    firstName = "GOPI",
                    lastName = "KUMAR",
                    passengerType = PassengerType.Infant,
                    title = "MSTR",
                    gender = Gender.Male,
                    dateOfBirth = DateTime.Today.AddYears(-1).AddMonths(2)
                };
                if (flightSearchReq.travelType == TravelType.International)
                {
                    objPassenger3.passportNumber = "KJHHJKKKKK";
                    objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                }
                bookingReq.passengerDetails.Add(objPassenger3);
            }

            //var response = new FlightMapper().saveBookingDetails(bookingReq);
            FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq);
            StringBuilder sbLogger = new StringBuilder();
            new ServicesHub.Travelogy.TravelogyServiceMapping().CreateBooking(bookingReq, ref BookResponse, ref sbLogger);
            return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
        }
        [HttpGet]
        [Route("Case6")]
        public HttpResponseMessage Case6(int d = 2)
        {
            StringBuilder sbLogger = new StringBuilder();
            string req = "{\"Auth_Header\":{\"UserId\":\"travelogyuat\",\"Password\":\"46B98418DB0908EA37DFC32765CD73491439F030\",\"IP_Address\":\"127.0.0.1\",\"Request_Id\":\"17032023194528\",\"IMEI_Number\":\"2232323232323\"},\"AirTicketCancelDetails\":[{\"FlightId\":\"4864499531030308419\",\"PassengerId\":\"1\",\"SegmentId\":\"1\"}],\"Airline_PNR\":\"TKIL9N\",\"RefNo\":\"TBB62G1Q\",\"CancelCode\":\"015\",\"ReqRemarks\":\"I cancelled the ticket directly with Airline\",\"CancellationType\":0}";
            new ServicesHub.Travelogy.TravelogyServiceMapping().CancelBooking(req, ref sbLogger);
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }
    }
}