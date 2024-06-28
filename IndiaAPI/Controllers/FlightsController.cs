using Core;
using Core.Flight;
using DAL;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace IndiaAPI.Controllers
{
    [RoutePrefix("Flights")]
    public class FlightsController : ApiController
    {
        private static string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static Random random = new Random();
        public static string SecurityCode = "fl1asdfghasdftmoasdfjado2o";
        public static bool authorizeRequest(string securityCodeGet)
        {
            return SecurityCode == securityCodeGet;
        }


        [HttpGet]
        [Route("test")]
        public HttpResponseMessage Test()
        {

            StringBuilder sbLogger = new StringBuilder();
            FlightSearchRequest flightSearchReq = new FlightSearchRequest();

            flightSearchReq.adults = 1;
            flightSearchReq.child = 0;
            flightSearchReq.infants = 1;
            flightSearchReq.tripType = Core.TripType.OneWay;
            flightSearchReq.searchDirectFlight = false;
            flightSearchReq.userIP = "183.83.43.117";
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
                travelDate = Convert.ToDateTime("2024-07-13") //DateTime.Today.AddDays(61)//
            });

            if (flightSearchReq.tripType != Core.TripType.OneWay)
            {
                flightSearchReq.segment.Add(new SearchSegment()
                {
                    originAirport = "BOM",
                    orgArp = Core.FlightUtility.GetAirport("BOM"),
                    destinationAirport = "PAT",
                    destArp = Core.FlightUtility.GetAirport("PAT"),
                    travelDate = Convert.ToDateTime("2024-04-23") //DateTime.Today.AddDays(73)
                });
            }
            flightSearchReq.cabinType = CabinType.Economy;
            flightSearchReq.tgy_Request_id = DateTime.Now.ToString("ddMMyyyHHmmsss");
            flightSearchReq.travelType = new Core.FlightUtility().getTravelType(flightSearchReq.segment[0].orgArp.countryCode, flightSearchReq.segment[0].destArp.countryCode);
            flightSearchReq.siteId = SiteId.FlightsMojoIN;
            flightSearchReq.sourceMedia = "1015";
            flightSearchReq.userSearchID = getSearchID();

            // return SearchFlightCRM("fl1asdfghasdftmoasdfjado2o", flightSearchReq);
            return SearchFlight("fl1asdfghasdftmoasdfjado2o", flightSearchReq);
            //   var kkdd = new ServicesHub.Travelopedia.TravelopediaServiceMapping().GetFlightResults(flightSearchReq, true, false);
            //       var kkdd = new ServicesHub.FareBoutique.FareBoutiqueServiceMapping().GetFlightResults(flightSearchReq);
            var kkdd = new ServicesHub.GFS.GFSServiceMapping().GetFlightResults(flightSearchReq, true, false);
            //   return Request.CreateResponse(HttpStatusCode.OK, kkdd);
            //  return SearchFlight("fl1asdfghasdftmoasdfjado2o", flightSearchReq);
            // var kkdd = new ServicesHub.SatkarTravel.SatkarTravelServiceMapping().GetFlightResults(flightSearchReq,true,true);
            //   var kkdd = new ServicesHub.Ease2Fly.Ease2FlyServiceMapping().GetFlightResults(flightSearchReq);
            //     var kkdd = new ServicesHub.FareBoutique.FareBoutiqueServiceMapping().GetFlightResults(flightSearchReq, true, false);
            //   var kkdd = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResults(flightSearchReq);
            //var kkdd = new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightResults(flightSearchReq);
            //return SearchFlight("fl1asdfghasdftmoasdfjado2o", flightSearchReq);
            //ServicesHub.LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\Error\\", "Test.txt");
            FlightSearchResponse response = new FlightSearchResponse(flightSearchReq);

            kkdd.Results[0][0].Fare = kkdd.Results[0][0].FareList[0];

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

            pvReq.flightResult.Add(kkdd.Results[0][0]);
            if (flightSearchReq.tripType == TripType.RoundTrip && response.Results.Count > 1)
            {
                pvReq.flightResult.Add(kkdd.Results[1][0]);
            }
            //    var kk = new ServicesHub.FareBoutique.FareBoutiqueServiceMapping().GetFareQuote(pvReq);
            var kk = new ServicesHub.AirIQ.AirIQServiceMapping().GetFareQuote(pvReq);
            return Request.CreateResponse(HttpStatusCode.OK, kk);
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
                tgy_Request_id = flightSearchReq.tgy_Request_id,
                FB_booking_token_id = kkdd.FB_booking_token_id
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
                    title = "Master",
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
                    title = "Master",
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
                    title = "Master",
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


            //  var response = new FlightMapper().saveBookingDetails(bookingReq);
            FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq);
            //  StringBuilder sbLogger = new StringBuilder();
            //new ServicesHub.Travelogy.TravelogyServiceMapping().BookFlight(bookingReq, ref BookResponse);
            //  new ServicesHub.SatkarTravel.SatkarTravelServiceMapping().BookFlight(bookingReq, ref BookResponse);
            return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
        }
        [HttpGet]
        [Route("TestTbo")]
        public HttpResponseMessage TestTbo()
        {
            //bool ky = CheckIfTimeIsBetweenShift(DateTime.Now);

            StringBuilder sbLogger = new StringBuilder();
            FlightSearchRequest flightSearchReq = new FlightSearchRequest();
            //flightSearchReq.Sources = new List<string>();
            flightSearchReq.adults = 2;
            flightSearchReq.child = 0;
            flightSearchReq.infants = 0;
            flightSearchReq.tripType = Core.TripType.RoundTrip;
            flightSearchReq.searchDirectFlight = false;
            flightSearchReq.userIP = "103.160.243.202";
            //flightSearchReq.OneStopFlight = false;
            //flightSearchReq.PreferredAirlines = new List<string>();

            //flightSearchReq.PreferredAirlines.Add("All");
            flightSearchReq.segment = new List<SearchSegment>();
            flightSearchReq.segment.Add(new SearchSegment()
            {
                originAirport = "IXD",
                orgArp = Core.FlightUtility.GetAirport("IXD"),
                destinationAirport = "BOM",
                destArp = Core.FlightUtility.GetAirport("BOM"),
                travelDate = Convert.ToDateTime("2024-06-16") //DateTime.Today.AddDays(61)//
            });

            if (flightSearchReq.tripType != Core.TripType.OneWay)
            {
                flightSearchReq.segment.Add(new SearchSegment()
                {
                    originAirport = "PAT",
                    orgArp = Core.FlightUtility.GetAirport("PAT"),
                    destinationAirport = "BOM",
                    destArp = Core.FlightUtility.GetAirport("BOM"),
                    travelDate = DateTime.Today.AddDays(73)
                });
            }
            flightSearchReq.cabinType = CabinType.Economy;
            flightSearchReq.tgy_Request_id = DateTime.Now.ToString("ddMMyyyHHmmsss");
            flightSearchReq.travelType = new Core.FlightUtility().getTravelType(flightSearchReq.segment[0].orgArp.countryCode, flightSearchReq.segment[0].destArp.countryCode);
            flightSearchReq.siteId = SiteId.FlightsMojoIN;
            flightSearchReq.sourceMedia = "1000";
            flightSearchReq.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmmss");
            var kkdd = new ServicesHub.Tbo.TboServiceMapping().GetFlightResults(flightSearchReq);

            string strJ = JsonConvert.SerializeObject(kkdd);
            //return Request.CreateResponse(HttpStatusCode.OK, kkdd);
            //var kkdd = new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightResults(flightSearchReq);
            //return SearchFlight("fl1asdfghasdftmoasdfjado2o", flightSearchReq);
            ServicesHub.LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\Error\\", "Test.txt");
            FlightSearchResponse response = new FlightSearchResponse(flightSearchReq);
            //new FlightMapper().DistinctFlightResult(ref response, new FlightSearchResponseShort(), new FlightSearchResponseShort(), kkdd);


            PriceVerificationRequest pvReq = new PriceVerificationRequest()
            {
                adults = flightSearchReq.adults,
                child = flightSearchReq.child,
                infants = flightSearchReq.infants,
                infantsWs = 0,
                flightResult = new List<FlightResult>(),
                isFareQuote = true,
                isFareRule = true,
                isSSR = false,
                siteID = flightSearchReq.siteId,
                sourceMedia = flightSearchReq.sourceMedia,
                userIP = flightSearchReq.userIP,
                userSearchID = flightSearchReq.userSearchID,
                userSessionID = flightSearchReq.userSessionID,
                tgy_Search_Key = response.tgy_Search_Key,
                PhoneNo = "6464646464",
                tgy_Request_id = flightSearchReq.tgy_Request_id,
                TvoTraceId = response.TraceId
            };

            pvReq.flightResult.Add(response.Results[0].FirstOrDefault());
            if (flightSearchReq.tripType == TripType.RoundTrip && response.Results.Count > 1)
            {
                pvReq.flightResult.Add(response.Results[1].FirstOrDefault());
            }
            var kk = new FlightMapper().TboVerifyThePrice(pvReq);
            return Request.CreateResponse(HttpStatusCode.OK, kk);
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
                //TjBookingID = kk.TjBookingID,
                //tgy_Flight_Key = kk.tgy_Flight_Key,
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
                    title = "Master",
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
                    title = "Master",
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
                    title = "Master",
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
            //StringBuilder sbLogger = new StringBuilder();
            new ServicesHub.Tbo.TboServiceMapping().BookFlight(bookingReq, ref BookResponse);
            return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
        }
        [HttpGet]
        [Route("testJson")]
        public HttpResponseMessage testJson()
        {
            FlightSearchRequest flightSearchReq = new FlightSearchRequest();
            //flightSearchReq.Sources = new List<string>();
            flightSearchReq.adults = 2;
            flightSearchReq.child = 1;
            flightSearchReq.infants = 0;
            flightSearchReq.tripType = Core.TripType.OneWay;
            flightSearchReq.searchDirectFlight = false;
            flightSearchReq.userIP = "103.160.243.202";
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
                travelDate = DateTime.Today.AddDays(60)//Convert.ToDateTime("2023-12-27") //
            });

            if (flightSearchReq.tripType != Core.TripType.OneWay)
            {
                flightSearchReq.segment.Add(new SearchSegment()
                {
                    originAirport = "BOM",
                    orgArp = Core.FlightUtility.GetAirport("BOM"),
                    destinationAirport = "DEL",
                    destArp = Core.FlightUtility.GetAirport("DEL"),
                    travelDate = DateTime.Today.AddDays(72)
                });
            }

            flightSearchReq.travelType = new Core.FlightUtility().getTravelType(flightSearchReq.segment[0].orgArp.countryCode, flightSearchReq.segment[0].destArp.countryCode);
            flightSearchReq.siteId = SiteId.FlightsMojoIN;
            flightSearchReq.sourceMedia = "1000";
            flightSearchReq.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmmss");
            StringBuilder sbLogger = new StringBuilder();
            string path = System.IO.Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "DomResponse.json");
            string data = "";
            using (System.IO.StreamReader r = new System.IO.StreamReader(path))
            {
                data = r.ReadToEnd();
            }


            //ServicesHub.Travelogy.TravelogyClass.TravelogyFlightSearchResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<ServicesHub.Travelogy.TravelogyClass.TravelogyFlightSearchResponse>(data);

            new ServicesHub.FareBoutique.FareBoutiqueServiceMapping().testJson(data);

            return Request.CreateResponse(HttpStatusCode.OK, true);


        }

        [HttpGet]
        [Route("testBalence")]
        private HttpResponseMessage testBalence()
        {
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }
        [HttpPost]
        [Route("SearchFlightDirect")]
        public HttpResponseMessage SearchFlightDirect(string authcode, FlightSearchRequest fsr)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            try
            {
                if (FlightUtility.isWriteLogSearch)
                {
                    new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(fsr), fsr.userSearchID, "Search", "Original Request");
                }
                FlightSearchResponse result = null;
                bool isFreshSearch = true;

                if (!string.IsNullOrEmpty(fsr.sID) && !string.IsNullOrEmpty(fsr.rID) && fsr.isGetLiveFare == false)
                {
                    int ctr = 0;
                    DAL.DALFlightCache objDal = new DAL.DALFlightCache();
                    Research:
                    //string str = objDal.getMetaSearchDetails(fsr.sID);
                    string str = new CacheRedis().getResult(fsr.sID);
                    str = StringHelper.DecompressString(str);

                    if (!string.IsNullOrEmpty(str))
                    {
                        result = JsonConvert.DeserializeObject<FlightSearchResponse>(StringHelper.DecompressString(str));
                        result.isCacheFare = true;
                        new ResponseMapper().checkResultIsExist(ref result, fsr.rID);
                        isFreshSearch = false;
                        ctr = 5;
                    }
                    else
                    {
                        if (ctr <= 1)
                        {
                            ctr++;
                            System.Threading.Thread.Sleep(3000);
                            goto Research;
                        }
                    }
                }
                if (isFreshSearch) /*&& (fsr.segment[0].originAirport == "DEL" && fsr.segment[0].destinationAirport == "BOM" && fsr.segment[0].travelDate > DateTime.Today.AddDays(1))*/
                {
                    fsr.fareCachingKey = ((int)fsr.siteId).ToString() + "_" + fsr.sourceMedia;
                    foreach (var item in fsr.segment)
                    {
                        fsr.fareCachingKey += (item.originAirport + item.destinationAirport + item.travelDate.ToString("ddMMyy"));
                    }
                    fsr.fareCachingKey += (fsr.adults.ToString() + fsr.child.ToString() + fsr.infants.ToString() +
                        ((int)fsr.cabinType).ToString());



                    if (isFreshSearch) /*&& (fsr.segment[0].originAirport == "DEL" && fsr.segment[0].destinationAirport == "BOM" && fsr.segment[0].travelDate > DateTime.Today.AddDays(1))*/
                    {
                        fsr.isMetaRequest = false;
                        result = new FlightMapper().GetFlightResultMultiGDS(fsr);
                        new CacheRedis().setResult(fsr.fareCachingKey, StringHelper.CompressString(JsonConvert.SerializeObject(result)));

                        if (result != null && result.Results != null && result.Results.Count() > 0 && result.Results[0].Count > 0 && result.Results.LastOrDefault().Count > 0 && result.isCacheFare == false)
                        {
                            saveTopFare(fsr, result.Results);
                        }
                    }
                    saveSearchList(fsr, false, result.Results[0].Count);
                }
                else
                {
                    saveSearchList(fsr, true, result.Results[0].Count);
                }
                if (result != null && result.Results != null && result.Results.Count() > 0 && result.Results[0].Count > 0 && result.Results.LastOrDefault().Count > 0 && result.isCacheFare == false)
                {
                    saveTopFare(fsr, result.Results);
                }
                new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(result), fsr.userSearchID, "Search", "Original Response");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                new ServicesHub.LogWriter_New(ex.ToString(), fsr.userSearchID, "Exeption", "SearchFlight Exeption");
                FlightSearchResponse flightSearchResponse = new FlightSearchResponse(fsr) { response = new Core.ResponseStatus() { status = Core.TransactionStatus.Error, message = ex.ToString() } };
                return Request.CreateResponse(HttpStatusCode.OK, flightSearchResponse);
            }
        }
        [HttpPost]
        [Route("SearchFlightTboRowData")]
        public HttpResponseMessage SearchFlightTboRowData(string authcode, FlightSearchRequest fsr)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }


            return Request.CreateResponse(HttpStatusCode.OK, new ServicesHub.Tbo.TboServiceMapping().GetFlightResultsRowData(fsr));

        }
        [HttpPost]
        [Route("SearchFlight")]
        public HttpResponseMessage SearchFlight(string authcode, FlightSearchRequest fsr)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            try
            {
                StringBuilder sbLogger = new StringBuilder();
                //bookingLog(ref sbLogger, "Search1 ",DateTime.Now.ToString());
                new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(fsr), fsr.userSearchID, "Search", "Original Request");
                FlightSearchResponse result = null;
                bool isFreshSearch = true;

                if (!string.IsNullOrEmpty(fsr.sID) && !string.IsNullOrEmpty(fsr.rID) && fsr.isGetLiveFare == false)
                {

                    int ctr = 0;
                    DAL.DALFlightCache objDal = new DAL.DALFlightCache();
                    System.Threading.Thread.Sleep(1000);
                    Research:
                    string str = objDal.getMetaSearchDetails(fsr.sID);
                    //string str = new CacheRedis().getResult(fsr.sID);
                    str = StringHelper.DecompressString(str);

                    if (!string.IsNullOrEmpty(str))
                    {
                        result = JsonConvert.DeserializeObject<FlightSearchResponse>(StringHelper.DecompressString(str));
                        result.isCacheFare = true;
                        new ResponseMapper().checkResultIsExist(ref result, fsr.rID);
                        isFreshSearch = false;
                        ctr = 5;
                    }
                    else
                    {
                        if (ctr <= 1)
                        {
                            ctr++;
                            System.Threading.Thread.Sleep(3000);
                            goto Research;
                        }
                    }
                }

                if (isFreshSearch)
                {
                    //bookingLog(ref sbLogger, "Search2 ", DateTime.Now.ToString());
                    fsr.fareCachingKey = ((int)fsr.siteId).ToString() + "_" + fsr.sourceMedia;
                    foreach (var item in fsr.segment)
                    {
                        fsr.fareCachingKey += (item.originAirport + item.destinationAirport + item.travelDate.ToString("ddMMyy"));
                    }
                    fsr.fareCachingKey += (fsr.adults.ToString() + fsr.child.ToString() + fsr.infants.ToString() +
                        ((int)fsr.cabinType).ToString());
                    //if (GlobalData.isUseCaching)/* && (fsr.segment[0].originAirport == "DEL" && fsr.segment[0].destinationAirport == "BOM" && fsr.segment[0].travelDate > DateTime.Today.AddDays(1))*/
                    //{
                    //    //   bookingLog(ref sbLogger, "Search3 ", DateTime.Now.ToString());
                    //    string str = new CacheRedis().getResult(fsr.fareCachingKey);
                    //    str = StringHelper.DecompressString(str);
                    //    //  bookingLog(ref sbLogger, "Search4 ", DateTime.Now.ToString());
                    //    if (!string.IsNullOrEmpty(str))
                    //    {
                    //        string ss = StringHelper.DecompressString(str);
                    //        //    bookingLog(ref sbLogger, "Search5 " , DateTime.Now.ToString());
                    //        result = JsonConvert.DeserializeObject<FlightSearchResponse>(ss);
                    //        //     bookingLog(ref sbLogger, "Search6 " , DateTime.Now.ToString());
                    //        isFreshSearch = false;
                    //        //   bookingLog(ref sbLogger, "Search7 ", DateTime.Now.ToString());
                    //    }
                    //}
                    if (isFreshSearch)
                    {
                        fsr.isMetaRequest = false;
                        result = new FlightMapper().GetFlightResultMultiGDS(fsr);
                        //if (GlobalData.isUseCaching)/*&& (fsr.segment[0].originAirport == "DEL" && fsr.segment[0].destinationAirport == "BOM" && fsr.segment[0].travelDate > DateTime.Today.AddDays(1))*/
                        //{
                        //    new CacheRedis().setResult(fsr.fareCachingKey, StringHelper.CompressString(JsonConvert.SerializeObject(result)));
                        //}
                        if (result != null && result.Results != null && result.Results.Count() > 0 && result.Results[0].Count > 0 && result.Results.LastOrDefault().Count > 0 && result.isCacheFare == false)
                        {
                            saveTopFare(fsr, result.Results);
                        }
                    }
                    saveSearchList(fsr, false, result.Results[0].Count);
                    //     bookingLog(ref sbLogger, "Search8 ", DateTime.Now.ToString());
                }
                else
                {
                    saveSearchList(fsr, true, result.Results[0].Count);
                }

                //new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(result), fsr.userSearchID, "Search", "Original Response");
                //   bookingLog(ref sbLogger, "Search9 ", DateTime.Now.ToString());
                //   new ServicesHub.LogWriter_New(sbLogger.ToString(), fsr.userSearchID, "Cache");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                new ServicesHub.LogWriter_New(ex.ToString(), fsr.userSearchID, "Exeption", "SearchFlight Exeption");
                FlightSearchResponse flightSearchResponse = new FlightSearchResponse(fsr) { response = new Core.ResponseStatus() { status = Core.TransactionStatus.Error, message = ex.ToString() } };
                return Request.CreateResponse(HttpStatusCode.OK, flightSearchResponse);
            }
        }
        [HttpPost]
        [Route("SearchFlightCRM")]
        public HttpResponseMessage SearchFlightCRM(string authcode, FlightSearchRequest fsr)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            try
            {

                FlightSearchResponse result = new FlightMapper().GetFlightResultALLGDS(fsr);



                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {

                FlightSearchResponse flightSearchResponse = new FlightSearchResponse(fsr) { response = new Core.ResponseStatus() { status = Core.TransactionStatus.Error, message = ex.ToString() } };
                return Request.CreateResponse(HttpStatusCode.OK, flightSearchResponse);
            }
        }

        [Route("SearchFlightGF")]
        public HttpResponseMessage SearchFlightGF(string authcode, FlightSearchRequest fsr)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            try
            {
                new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(fsr), fsr.userSearchID, "Search", "Original Request_GF");
                FlightSearchResponse result = null;

                result = new FlightMapper().GetFlightResultMultiGDSGF(fsr);
                if (GlobalData.isUseCaching)
                {
                    new CacheRedis().setResult(fsr.fareCachingKey, StringHelper.CompressString(JsonConvert.SerializeObject(result)));
                }
                if (result != null && result.Results != null && result.Results.Count() > 0 && result.Results[0].Count > 0 && result.Results.LastOrDefault().Count > 0 && result.isCacheFare == false)
                {
                    saveTopFare(fsr, result.Results);
                }

                saveSearchList(fsr, false, result.Results[0].Count);


                new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(result), fsr.userSearchID, "Search", "Original Response");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                new ServicesHub.LogWriter_New(ex.ToString(), fsr.userSearchID, "Exeption", "SearchFlight Exeption");
                FlightSearchResponse flightSearchResponse = new FlightSearchResponse(fsr) { response = new Core.ResponseStatus() { status = Core.TransactionStatus.Error, message = ex.ToString() } };
                return Request.CreateResponse(HttpStatusCode.OK, flightSearchResponse);
            }
        }

        [HttpPost]
        [Route("SaveBookingDetails")]
        public HttpResponseMessage SaveBookingDetails(string authcode, FlightBookingRequest flightBookingRequest)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            try
            {
                var response = new FlightMapper().saveBookingDetails(flightBookingRequest);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                FlightBookingResponse flightSearchResponse = new FlightBookingResponse() { responseStatus = new Core.ResponseStatus() { status = Core.TransactionStatus.Error, message = ex.ToString() } };
                return Request.CreateResponse(HttpStatusCode.OK, flightSearchResponse);
            }
        }

        [Route("MetaSearchFlights")]
        [HttpGet]
        public HttpResponseMessage MetaSearchFlights(string sec1, string adults, string cabin, string siteid, string campain, string pwd,
         string sec2 = "", string sec3 = "", string sec4 = "", string child = "", string children = "", string infants = "", string airline = "",
         string currency = "", string userip = "", string device = "")
        {
            //Flights/MetaSearchFlights?sec1=GOX|BOM|2023-12-13&sec2=&sec3=&sec4=&adults=1&child=0&infants=0&cabin=1&airline=all&siteid=1&campain=1015&pwd=Mojoindiaflights321&currency=INR 
            StringBuilder sbLogger = new StringBuilder();
            siteid = (siteid == "1" ? "2" : siteid);
            string ip = GetIpAddress(); //System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (CheckCredential(siteid, campain, pwd, ip) == false)
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            //if(ip.Equals("127.0.0.1"))
            //{ return Request.CreateResponse(HttpStatusCode.Forbidden); }
            #region make Request
            FlightSearchRequest fsr = new FlightSearchRequest();
            fsr.SearchURL = HttpContext.Current.Request.Url.AbsoluteUri;
            fsr.adults = string.IsNullOrEmpty(adults) ? 1 : Convert.ToInt32(adults);
            fsr.child = string.IsNullOrEmpty(child) ? 0 : Convert.ToInt32(child);
            int OptChild = string.IsNullOrEmpty(children) ? 0 : Convert.ToInt32(children);
            if (OptChild > 0)
            {
                fsr.child = OptChild;
                child = children;
            }
            fsr.infants = string.IsNullOrEmpty(infants) ? 0 : Convert.ToInt32(infants);


            fsr.segment = new List<SearchSegment>();
            if (!string.IsNullOrEmpty(sec1))
            {
                SearchSegment sseg = new SearchSegment();
                string[] arrStr = sec1.Split('|');
                if (arrStr.Length >= 3)
                {
                    sseg.originAirport = arrStr[0].ToUpper();
                    sseg.orgArp = FlightUtility.GetAirport(sseg.originAirport);
                    sseg.destinationAirport = arrStr[1].ToUpper();
                    sseg.destArp = FlightUtility.GetAirport(sseg.destinationAirport);
                    sseg.travelDate = Convert.ToDateTime(arrStr[2]);// DateTime.Today.AddDays(150);
                    fsr.segment.Add(sseg);
                    fsr.tripType = TripType.OneWay;
                }
            }


            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);

            if (!string.IsNullOrEmpty(sec1) && !string.IsNullOrEmpty(sec2))
            {
                SearchSegment sseg = new SearchSegment();
                string[] arrStr = sec2.Split('|');
                if (arrStr.Length >= 3)
                {
                    sseg.originAirport = arrStr[0].ToUpper();
                    sseg.destinationAirport = arrStr[1].ToUpper();
                    sseg.travelDate = Convert.ToDateTime(arrStr[2]);// DateTime.Today.AddDays(150);
                    fsr.segment.Add(sseg);

                    if (fsr.segment[0].originAirport == fsr.segment[1].destinationAirport && fsr.segment[0].destinationAirport == fsr.segment[1].originAirport)
                        fsr.tripType = TripType.RoundTrip;
                    else
                        fsr.tripType = TripType.MultiCity;
                }
            }
            if (!string.IsNullOrEmpty(sec1) && !string.IsNullOrEmpty(sec2) && !string.IsNullOrEmpty(sec3))
            {
                SearchSegment sseg = new SearchSegment();
                string[] arrStr = sec3.Split('|');
                if (arrStr.Length >= 3)
                {
                    sseg.originAirport = arrStr[0].ToUpper();
                    sseg.destinationAirport = arrStr[1].ToUpper();
                    sseg.travelDate = Convert.ToDateTime(arrStr[2]);// DateTime.Today.AddDays(150);
                    fsr.segment.Add(sseg);
                    fsr.tripType = TripType.MultiCity;
                }
            }
            if (!string.IsNullOrEmpty(sec1) && !string.IsNullOrEmpty(sec2) && !string.IsNullOrEmpty(sec3) && !string.IsNullOrEmpty(sec4))
            {
                SearchSegment sseg = new SearchSegment();
                string[] arrStr = sec4.Split('|');
                if (arrStr.Length >= 3)
                {
                    sseg.originAirport = arrStr[0].ToUpper();
                    sseg.destinationAirport = arrStr[1].ToUpper();
                    sseg.travelDate = Convert.ToDateTime(arrStr[2]);// DateTime.Today.AddDays(150);
                    fsr.segment.Add(sseg);
                }
            }
            fsr.siteId = (SiteId)Convert.ToInt32(siteid);
            fsr.cabinType = (CabinType)Convert.ToInt32(string.IsNullOrEmpty(cabin) ? "1" : cabin);
            fsr.airline = "";// airline.ToUpper();
            fsr.searchDirectFlight = false;
            //fsr.userIP = string.IsNullOrEmpty(userip) ? "182.72.103.98" : userip;//userip
            fsr.userIP = ip;
            fsr.sourceMedia = campain;
            fsr.userSearchID = getSearchID();
            //fsr.ProductType = ProductType.Flight;
            fsr.currencyCode = string.IsNullOrEmpty(currency) ? "INR" : currency.ToUpper();
            fsr.isMetaRequest = true;
            fsr.tgy_Request_id = DateTime.Now.ToString("ddMMyyyyHHmmsss");
            fsr.device = string.IsNullOrEmpty(device) ? Device.Desktop : (device.Equals("mobile", StringComparison.OrdinalIgnoreCase) ? Device.Mobile : Device.Desktop);
            #endregion

            FlightSearchResponse SearchRes = null;
            //string SearchID = Guid.NewGuid().ToString();
            string SearchID = fsr.userSearchID;
            string strProvider = "";
            bool isFreshSearch = true;

            if (FlightUtility.isWriteLogSearch)
            {
                new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(fsr), fsr.userSearchID, "Search", "Original Request");
            }
            //if (fsr.sourceMedia != "1010")
            //{
            if (isFreshSearch)
            {
                fsr.fareCachingKey = ((int)fsr.siteId).ToString() + "_" + fsr.sourceMedia;
                bookingLog(ref sbLogger, "Search1 ", DateTime.Now.ToString());
                foreach (var item in fsr.segment)
                {
                    fsr.fareCachingKey += (item.originAirport + item.destinationAirport + item.travelDate.ToString("ddMMyy"));
                }
                fsr.fareCachingKey += (fsr.adults.ToString() + fsr.child.ToString() + fsr.infants.ToString() +
                    ((int)fsr.cabinType).ToString());

                //    SearchRes = new FlightMapper().GetFlightResultMultiGDS(fsr);
                //if (GlobalData.isUseCaching && (fsr.segment[0].originAirport == "DEL" && fsr.segment[0].destinationAirport == "BOM" && fsr.segment[0].travelDate > DateTime.Today.AddDays(1)))/*&& fsr.segment[0].travelDate > DateTime.Today.AddDays(4)*/ /*&& (fsr.segment[0].originAirport == "DEL" && fsr.segment[0].destinationAirport == "BOM" && fsr.segment[0].travelDate > DateTime.Today.AddDays(1))*/
                //{
                //    bookingLog(ref sbLogger, "Search2 ", DateTime.Now.ToString());
                //    string str = new CacheRedis().getResult(fsr.fareCachingKey);
                //    str = StringHelper.DecompressString(str);
                //    if (!string.IsNullOrEmpty(str))
                //    {
                //        bookingLog(ref sbLogger, "Search3 ", DateTime.Now.ToString());
                //        string ss = StringHelper.DecompressString(str);
                //        SearchRes = JsonConvert.DeserializeObject<FlightSearchResponse>(ss);
                //        isFreshSearch = false;
                //    }

                //}

                if (isFreshSearch)
                {
                    bookingLog(ref sbLogger, "Search4 ", DateTime.Now.ToString());
                    fsr.isMetaRequest = false;
                    SearchRes = new FlightMapper().GetFlightResultMultiGDS(fsr);
                    //if (GlobalData.isUseCaching && (fsr.segment[0].originAirport == "DEL" && fsr.segment[0].destinationAirport == "BOM" && fsr.segment[0].travelDate > DateTime.Today.AddDays(1))) /*&& (fsr.segment[0].originAirport == "DEL" && fsr.segment[0].destinationAirport == "BOM" && fsr.segment[0].travelDate > DateTime.Today.AddDays(1))*/
                    //{
                    //    bookingLog(ref sbLogger, "Search5 ", DateTime.Now.ToString());
                    //    new CacheRedis().setResult(fsr.fareCachingKey, StringHelper.CompressString(JsonConvert.SerializeObject(SearchRes)));
                    //}
                    //if (SearchRes != null && SearchRes.Results != null && SearchRes.Results.Count() > 0 && SearchRes.Results[0].Count > 0 && SearchRes.Results.LastOrDefault().Count > 0 && SearchRes.isCacheFare == false)
                    //{
                    //    bookingLog(ref sbLogger, "Search6 ", DateTime.Now.ToString());
                    //    saveTopFare(fsr, SearchRes.Results);
                    //}
                }

                if (SearchRes != null && SearchRes.Results != null && SearchRes.Results.Count() > 0 && SearchRes.Results[0].Count > 0 && SearchRes.Results.LastOrDefault().Count > 0)
                {
                    saveMetaSearchDetails(SearchID, StringHelper.CompressString(JsonConvert.SerializeObject(SearchRes)));
                    //new CacheRedis().setResult(SearchID, StringHelper.CompressString(JsonConvert.SerializeObject(SearchRes)));
                    saveSearchListMeta(fsr, SearchRes.Results[0].Count, strProvider);
                }
            }
            //}

            //if (SearchRes != null && SearchRes.Results != null && SearchRes.Results.Count() > 0 && SearchRes.Results[0].Count > 0 && SearchRes.Results.LastOrDefault().Count > 0)
            //{

            //    saveMetaSearchDetails(SearchID, StringHelper.CompressString(JsonConvert.SerializeObject(SearchRes)));

            //    //new CacheRedis().setResult(SearchID, StringHelper.CompressString(JsonConvert.SerializeObject(SearchRes)));
            //    saveSearchListMeta(fsr, SearchRes.Results[0].Count, strProvider);

            //}
            //else
            //{
            //saveSearchListMeta(fsr, 0, strProvider);
            //  }
            if (FlightUtility.isWriteLogSearch)
            {
                new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(SearchRes), fsr.userSearchID, "Search", "Original Response");
            }
            if (campain.Equals("1015", StringComparison.OrdinalIgnoreCase))
            {
                var response = ResponseMapper.ConvertSkyscannerResponse(ref fsr, ref SearchRes, ref SearchID, ref sec1, ref sec2,
                      ref sec3, ref sec4, ref adults, ref child, ref infants, ref cabin, ref airline, ref siteid, ref campain, ref currency);
                if (FlightUtility.isWriteLogSearch)
                {
                    new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(response), fsr.userSearchID, "Search", "Meta Response");
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            else
            {

                var response = ResponseMapper.ConvertMetaResponse(ref fsr, ref SearchRes, ref SearchID, ref sec1, ref sec2,
                    ref sec3, ref sec4, ref adults, ref child, ref infants, ref cabin, ref airline, ref siteid, ref campain, ref currency);
                if (FlightUtility.isWriteLogSearch)
                {
                    new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(response), fsr.userSearchID, "Search", "Meta Response");
                }
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [HttpPost]
        [Route("FlightVerification")]
        public HttpResponseMessage FlightVerification(string authcode, PriceVerificationRequest priceVerificationRequest)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            try
            {
                if (priceVerificationRequest.userSearchID != priceVerificationRequest.userLogID)
                {
                    StringBuilder sbLogger = new StringBuilder();
                    string pathFlightSearch = System.IO.Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "NewLog\\Search", priceVerificationRequest.userLogID + ".txt");
                    if (System.IO.Directory.Exists(pathFlightSearch))
                    {
                        string strFlightSearch = "";
                        using (System.IO.StreamReader r = new System.IO.StreamReader(pathFlightSearch))
                        {
                            strFlightSearch = r.ReadToEnd();
                        }
                        sbLogger.Append(strFlightSearch);
                        sbLogger.Append(Environment.NewLine);
                    }
                    else
                    {
                        try
                        {
                            string strFlightSearch = "";
                            using (System.IO.StreamReader r = new System.IO.StreamReader(pathFlightSearch))
                            {
                                strFlightSearch = r.ReadToEnd();
                            }
                            sbLogger.Append(strFlightSearch);
                            sbLogger.Append(Environment.NewLine);
                        }
                        catch
                        {


                        }
                    }
                    new ServicesHub.LogWriter_New(sbLogger.ToString(), priceVerificationRequest.userSearchID, "Search", "");
                }
            }
            catch
            {


            }
            new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(priceVerificationRequest), priceVerificationRequest.userSearchID, "Search", "FlightVerification Original Request");
            PriceVerificationResponse priceVResponse = null;
            try
            {
                if (priceVerificationRequest.flightResult.Count == 1)
                {
                    if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.TripJack)
                    {
                        priceVResponse = new FlightMapper().TjVerifyThePrice(priceVerificationRequest);
                    }
                    //else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Travelogy)
                    //{
                    //    priceVResponse = new FlightMapper().TgyVerifyThePrice(priceVerificationRequest);
                    //}
                    else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Tbo)
                    {
                        priceVResponse = new FlightMapper().TboVerifyThePrice(priceVerificationRequest);
                    }
                    else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.FareBoutique)
                    {
                        priceVResponse = new FlightMapper().FareBoutiqueVerifyThePrice(priceVerificationRequest);
                    }
                    else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.OneDFare)
                    {
                        priceVResponse = new FlightMapper().OneDFareVerifyThePrice(priceVerificationRequest);
                    }
                    else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.SatkarTravel)
                    {
                        priceVResponse = new FlightMapper().SatkarTravelVerifyThePrice(priceVerificationRequest);
                    }
                    else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.AirIQ)
                    {
                        priceVResponse = new FlightMapper().AirIQVerifyThePrice(priceVerificationRequest);
                    }
                    else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Ease2Fly)
                    {
                        priceVResponse = new FlightMapper().E2FVerifyThePrice(priceVerificationRequest);
                    }
                    else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.GFS)
                    {
                        priceVResponse = new FlightMapper().GFSVerifyThePrice(priceVerificationRequest);
                    }
                    else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Amadeus)
                    {
                        priceVResponse = new FlightMapper().AmadeusVerifyThePrice(priceVerificationRequest);
                    }
                    else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Travelopedia)
                    {
                        priceVResponse = new FlightMapper().TravelopediaVerifyThePrice(priceVerificationRequest);
                    }
                    else
                    {
                        priceVResponse = new PriceVerificationResponse()
                        {
                            fareQuoteResponse = new FareQuoteResponse()
                            {
                                fareIncreaseAmount = 0,
                                flightResult = priceVerificationRequest.flightResult,
                                isFareChange = false,
                                responseStatus = new ResponseStatus(),
                                ErrorCode = 0,
                                IsGSTMandatory = false,
                                TjBookingID = "0",
                                TjReturnBookingID = "0",
                                tgy_Block_Ticket_Allowed = new List<bool>(),
                                tgy_Flight_Key = new List<string>(),
                            },
                            fareRuleResponse = new List<FareRuleResponses>(),
                            responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Success" }
                        };
                        foreach (var item in priceVerificationRequest.flightResult)
                        {
                            priceVResponse.fareQuoteResponse.VerifiedTotalPrice += item.Fare.NetFare;
                        }
                    }
                }
                else
                {
                    if (priceVerificationRequest.flightResult[0].Fare.gdsType == priceVerificationRequest.flightResult[1].Fare.gdsType)
                    {
                        if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.TripJack)
                        {
                            priceVResponse = new FlightMapper().TjVerifyThePrice(priceVerificationRequest);
                        }
                        //else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Travelogy)
                        //{
                        //    priceVResponse = new FlightMapper().TgyVerifyThePrice(priceVerificationRequest);
                        //}
                        else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Tbo)
                        {
                            priceVResponse = new FlightMapper().TboVerifyThePrice(priceVerificationRequest);
                        }
                        else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.FareBoutique)
                        {
                            priceVResponse = new FlightMapper().FareBoutiqueVerifyThePrice(priceVerificationRequest);
                        }
                        else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.SatkarTravel)
                        {
                            priceVResponse = new FlightMapper().SatkarTravelVerifyThePrice(priceVerificationRequest);
                        }
                        else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.AirIQ)
                        {
                            priceVResponse = new FlightMapper().AirIQVerifyThePrice(priceVerificationRequest);
                        }
                        else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Ease2Fly)
                        {
                            priceVResponse = new FlightMapper().E2FVerifyThePrice(priceVerificationRequest);
                        }
                        else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.GFS)
                        {
                            priceVResponse = new FlightMapper().GFSVerifyThePrice(priceVerificationRequest);
                        }
                        else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Amadeus)
                        {
                            priceVResponse = new FlightMapper().AmadeusVerifyThePrice(priceVerificationRequest);
                        }
                        else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Travelopedia)
                        {
                            priceVResponse = new FlightMapper().TravelopediaVerifyThePrice(priceVerificationRequest);
                        }
                        else
                        {
                            priceVResponse = new PriceVerificationResponse()
                            {
                                fareQuoteResponse = new FareQuoteResponse()
                                {
                                    fareIncreaseAmount = 0,
                                    flightResult = priceVerificationRequest.flightResult,
                                    isFareChange = false,
                                    responseStatus = new ResponseStatus(),
                                    ErrorCode = 0,
                                    IsGSTMandatory = false,
                                    TjBookingID = "0",
                                    TjReturnBookingID = "0",
                                    tgy_Block_Ticket_Allowed = new List<bool>(),
                                    tgy_Flight_Key = new List<string>(),
                                },
                                fareRuleResponse = new List<FareRuleResponses>(),
                                responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Success" }
                            };
                            foreach (var item in priceVerificationRequest.flightResult)
                            {
                                priceVResponse.fareQuoteResponse.VerifiedTotalPrice += item.Fare.NetFare;
                            }
                        }
                    }
                    else
                    {
                        priceVResponse = new PriceVerificationResponse()
                        {
                            fareQuoteResponse = new FareQuoteResponse()
                            {
                                fareIncreaseAmount = 0,
                                flightResult = priceVerificationRequest.flightResult,
                                isFareChange = false,
                                responseStatus = new ResponseStatus(),
                                ErrorCode = 0,
                                IsGSTMandatory = false,
                                TjBookingID = "0",
                                TjReturnBookingID = "0",
                                tgy_Block_Ticket_Allowed = new List<bool>(),
                                tgy_Flight_Key = new List<string>(),
                            },
                            fareRuleResponse = new List<FareRuleResponses>(),
                            responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Success" }
                        };
                        foreach (var item in priceVerificationRequest.flightResult)
                        {
                            priceVResponse.fareQuoteResponse.VerifiedTotalPrice += item.Fare.NetFare;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ServicesHub.LogWriter_New(ex.ToString(), priceVerificationRequest.userSearchID, "Exeption", "FlightVerification Exeption");

                priceVResponse = new PriceVerificationResponse()
                {
                    fareQuoteResponse = new FareQuoteResponse(),
                    fareRuleResponse = new List<FareRuleResponses>(),
                    responseStatus = new ResponseStatus() { status = TransactionStatus.Error, message = ex.ToString() }
                };
            }
            new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(priceVResponse), priceVerificationRequest.userSearchID, "Search", "FlightVerification Original Response");
            return Request.CreateResponse(HttpStatusCode.OK, priceVResponse);
        }

        [HttpPost]
        [Route("FlightVerificationGF")]
        public HttpResponseMessage FlightVerificationGF(string authcode, PriceVerificationRequest priceVerificationRequest)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            try
            {
                if (priceVerificationRequest.userSearchID != priceVerificationRequest.userLogID)
                {
                    StringBuilder sbLogger = new StringBuilder();
                    string pathFlightSearch = System.IO.Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "NewLog\\Search", priceVerificationRequest.userLogID + ".txt");
                    if (System.IO.Directory.Exists(pathFlightSearch))
                    {
                        string strFlightSearch = "";
                        using (System.IO.StreamReader r = new System.IO.StreamReader(pathFlightSearch))
                        {
                            strFlightSearch = r.ReadToEnd();
                        }
                        sbLogger.Append(strFlightSearch);
                        sbLogger.Append(Environment.NewLine);
                    }
                    else
                    {
                        try
                        {
                            string strFlightSearch = "";
                            using (System.IO.StreamReader r = new System.IO.StreamReader(pathFlightSearch))
                            {
                                strFlightSearch = r.ReadToEnd();
                            }
                            sbLogger.Append(strFlightSearch);
                            sbLogger.Append(Environment.NewLine);
                        }
                        catch
                        {


                        }
                    }
                    new ServicesHub.LogWriter_New(sbLogger.ToString(), priceVerificationRequest.userSearchID, "Search", "");
                }
            }
            catch
            {


            }
            new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(priceVerificationRequest), priceVerificationRequest.userSearchID, "Search", "FlightVerification Original Request");
            PriceVerificationResponse priceVResponse = null;
            try
            {
                if (priceVerificationRequest.flightResult.Count == 1)
                {
                    if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.TripJack)
                    {
                        priceVResponse = new FlightMapper().TjVerifyThePrice(priceVerificationRequest);
                    }

                    else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Tbo)
                    {
                        priceVResponse = new FlightMapper().TboVerifyThePrice(priceVerificationRequest);
                    }

                    else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.OneDFare)
                    {
                        priceVResponse = new FlightMapper().OneDFareVerifyThePrice(priceVerificationRequest);
                    }
                    else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Amadeus)
                    {
                        priceVResponse = new FlightMapper().AmadeusVerifyThePrice(priceVerificationRequest);
                    }
                    else
                    {
                        priceVResponse = new PriceVerificationResponse()
                        {
                            fareQuoteResponse = new FareQuoteResponse()
                            {
                                fareIncreaseAmount = 0,
                                flightResult = priceVerificationRequest.flightResult,
                                isFareChange = false,
                                responseStatus = new ResponseStatus(),
                                ErrorCode = 0,
                                IsGSTMandatory = false,
                                TjBookingID = "0",
                                TjReturnBookingID = "0",
                                tgy_Block_Ticket_Allowed = new List<bool>(),
                                tgy_Flight_Key = new List<string>(),
                            },
                            fareRuleResponse = new List<FareRuleResponses>(),
                            responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Success" }
                        };
                        foreach (var item in priceVerificationRequest.flightResult)
                        {
                            priceVResponse.fareQuoteResponse.VerifiedTotalPrice += item.Fare.NetFare;
                        }
                    }
                }
                else
                {
                    if (priceVerificationRequest.flightResult[0].Fare.gdsType == priceVerificationRequest.flightResult[1].Fare.gdsType)
                    {
                        if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.TripJack)
                        {
                            priceVResponse = new FlightMapper().TjVerifyThePrice(priceVerificationRequest);
                        }

                        else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Tbo)
                        {
                            priceVResponse = new FlightMapper().TboVerifyThePrice(priceVerificationRequest);
                        }

                        else if (priceVerificationRequest.flightResult[0].Fare.gdsType == GdsType.Amadeus)
                        {
                            priceVResponse = new FlightMapper().AmadeusVerifyThePrice(priceVerificationRequest);
                        }
                        else
                        {
                            priceVResponse = new PriceVerificationResponse()
                            {
                                fareQuoteResponse = new FareQuoteResponse()
                                {
                                    fareIncreaseAmount = 0,
                                    flightResult = priceVerificationRequest.flightResult,
                                    isFareChange = false,
                                    responseStatus = new ResponseStatus(),
                                    ErrorCode = 0,
                                    IsGSTMandatory = false,
                                    TjBookingID = "0",
                                    TjReturnBookingID = "0",
                                    tgy_Block_Ticket_Allowed = new List<bool>(),
                                    tgy_Flight_Key = new List<string>(),
                                },
                                fareRuleResponse = new List<FareRuleResponses>(),
                                responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Success" }
                            };
                            foreach (var item in priceVerificationRequest.flightResult)
                            {
                                priceVResponse.fareQuoteResponse.VerifiedTotalPrice += item.Fare.NetFare;
                            }
                        }
                    }
                    else
                    {
                        priceVResponse = new PriceVerificationResponse()
                        {
                            fareQuoteResponse = new FareQuoteResponse()
                            {
                                fareIncreaseAmount = 0,
                                flightResult = priceVerificationRequest.flightResult,
                                isFareChange = false,
                                responseStatus = new ResponseStatus(),
                                ErrorCode = 0,
                                IsGSTMandatory = false,
                                TjBookingID = "0",
                                TjReturnBookingID = "0",
                                tgy_Block_Ticket_Allowed = new List<bool>(),
                                tgy_Flight_Key = new List<string>(),
                            },
                            fareRuleResponse = new List<FareRuleResponses>(),
                            responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Success" }
                        };
                        foreach (var item in priceVerificationRequest.flightResult)
                        {
                            priceVResponse.fareQuoteResponse.VerifiedTotalPrice += item.Fare.NetFare;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ServicesHub.LogWriter_New(ex.ToString(), priceVerificationRequest.userSearchID, "Exeption", "FlightVerification Exeption");

                priceVResponse = new PriceVerificationResponse()
                {
                    fareQuoteResponse = new FareQuoteResponse(),
                    fareRuleResponse = new List<FareRuleResponses>(),
                    responseStatus = new ResponseStatus() { status = TransactionStatus.Error, message = ex.ToString() }
                };
            }
            new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(priceVResponse), priceVerificationRequest.userSearchID, "Search", "FlightVerification Original Response");
            return Request.CreateResponse(HttpStatusCode.OK, priceVResponse);
        }



        [HttpPost]
        [Route("GfPriceVrify")]
        public HttpResponseMessage GfPriceVrify(string authcode, GfPriceVerifyRequest fsr)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            GfPriceVerifyResponse pvr = new ServicesHub.Amadeus.AmadeusServiceMappking().GfPriceVerify(fsr);
            return Request.CreateResponse(HttpStatusCode.OK, pvr);

        }
        [HttpPost]
        [Route("SaveBookingDetails_WithOutPax")]
        public HttpResponseMessage SaveBookingDetails_WithOutPax(string authcode, FlightBookingRequest flightBookingRequest)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            try
            {
                CallDeleteFiles();
                deleteSearchListMeta();
            }
            catch (Exception ex)
            {

            }
            try
            {

                var response = new FlightMapper().SaveBookingDetails_WithOutPax(flightBookingRequest);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                FlightBookingResponse flightSearchResponse = new FlightBookingResponse() { responseStatus = new Core.ResponseStatus() { status = Core.TransactionStatus.Error, message = ex.ToString() } };
                return Request.CreateResponse(HttpStatusCode.OK, flightSearchResponse);
            }
        }
        [HttpPost]
        [Route("Update_BookingPaxDetail")]
        public HttpResponseMessage Update_BookingPaxDetail(string authcode, FlightBookingRequest flightBookingRequest)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            try
            {

                var response = new FlightMapper().Update_BookingPaxDetail(flightBookingRequest);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                FlightBookingResponse flightSearchResponse = new FlightBookingResponse() { responseStatus = new Core.ResponseStatus() { status = Core.TransactionStatus.Error, message = ex.ToString() } };
                return Request.CreateResponse(HttpStatusCode.OK, flightSearchResponse);
            }
        }


        [HttpPost]
        [Route("BookFlight")]
        public HttpResponseMessage BookFlight(string authcode, FlightBookingRequest bookRequest)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(bookRequest), bookRequest.bookingID.ToString(), "Booking", "BookFlight Original Request");

            FlightBookingResponse BookResponse = new FlightBookingResponse(bookRequest);
            BookResponse.bookingStatus = BookingStatus.InProgress;
            try
            {
                //int itinCtr = 0;
                if (GlobalData.isMakeBooking && bookRequest.paymentDetails != null &&
                    !string.IsNullOrEmpty(bookRequest.paymentDetails.OnlinePaymentStauts) &&
                    bookRequest.paymentDetails.OnlinePaymentStauts.Equals("captured", StringComparison.OrdinalIgnoreCase) &&
                    bookRequest.paymentDetails.IsReturnHashMatched)
                {
                    BookResponse.paymentStatus = PaymentStatus.Completed;
                    BookingAction action = BookingAction.Inporgress;

                    #region check boking Action



                    if (bookRequest.isMakeBookingInprogress == false)
                    {
                        string fromCountry = FlightUtility.GetAirport(bookRequest.flightResult.FirstOrDefault().FlightSegments.FirstOrDefault().Segments.FirstOrDefault().Origin).countryCode;
                        string toCountry = FlightUtility.GetAirport(bookRequest.flightResult.FirstOrDefault().FlightSegments.FirstOrDefault().Segments.LastOrDefault().Destination).countryCode;
                        if (bookRequest.flightResult.Count == 1)
                        {
                            var supplierData = FlightUtility.BookingManagementsList.Where(o => (o.SiteId == bookRequest.siteID) &&
                                                (((o.FromCountry.Any() && o.FromCountry.Contains(fromCountry)) || o.FromCountry.Any() == false)) &&
                                                (o.FromCountry_Not.Contains(fromCountry) == false) &&
                                                (((o.ToCountry.Any() && o.ToCountry.Contains(toCountry)) || o.ToCountry.Any() == false)) &&
                                                (o.ToCountry_Not.Contains(fromCountry) == false) &&
                                                ((o.AffiliateId.Any() && o.AffiliateId.Contains(bookRequest.sourceMedia)) || o.AffiliateId.Any() == false) &&
                                                (o.AffiliateId_Not.Contains(bookRequest.sourceMedia) == false) &&
                                                ((o.FareType.Any() && o.FareType.Contains(bookRequest.flightResult[0].Fare.mojoFareType)) || o.FareType.Any() == false) &&
                                                (o.Supplier == bookRequest.flightResult[0].Fare.gdsType)).ToList();

                            new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(supplierData), bookRequest.bookingID.ToString(), "Booking", "supplierData From Booking Manegement ");
                            if (supplierData != null || supplierData.Any())
                            {
                                action = supplierData.FirstOrDefault().BookingAction;

                                if (bookRequest.flightResult.FirstOrDefault().FlightSegments.FirstOrDefault().Segments.FirstOrDefault().CabinClass == CabinType.Business)
                                {
                                    action = BookingAction.Inporgress;
                                }
                            }
                        }
                        else
                        {
                            if (bookRequest.flightResult[0].Fare.gdsType == bookRequest.flightResult[1].Fare.gdsType)
                            {
                                var supplierData = FlightUtility.BookingManagementsList.Where(o => (o.SiteId == bookRequest.siteID) &&
                                             (((o.FromCountry.Any() && o.FromCountry.Contains(fromCountry)) || o.FromCountry.Any() == false)) &&
                                             (o.FromCountry_Not.Contains(fromCountry) == false) &&
                                             (((o.ToCountry.Any() && o.ToCountry.Contains(toCountry)) || o.ToCountry.Any() == false)) &&
                                             (o.ToCountry_Not.Contains(fromCountry) == false) &&
                                             ((o.AffiliateId.Any() && o.AffiliateId.Contains(bookRequest.sourceMedia)) || o.AffiliateId.Any() == false) &&
                                             (o.AffiliateId_Not.Contains(bookRequest.sourceMedia) == false) &&
                                             ((o.FareType.Any() && o.FareType.Contains(bookRequest.flightResult[0].Fare.mojoFareType)) || o.FareType.Any() == false) &&
                                             (o.Supplier == bookRequest.flightResult[0].Fare.gdsType)).ToList();
                                if (supplierData != null || supplierData.Any())
                                {
                                    action = supplierData.FirstOrDefault().BookingAction;
                                }
                                if (action == BookingAction.MakePnr)
                                {
                                    var supplierData2 = FlightUtility.BookingManagementsList.Where(o => (o.SiteId == bookRequest.siteID) &&
                                           (((o.FromCountry.Any() && o.FromCountry.Contains(fromCountry)) || o.FromCountry.Any() == false)) &&
                                           (o.FromCountry_Not.Contains(fromCountry) == false) &&
                                           (((o.ToCountry.Any() && o.ToCountry.Contains(toCountry)) || o.ToCountry.Any() == false)) &&
                                           (o.ToCountry_Not.Contains(fromCountry) == false) &&
                                           ((o.AffiliateId.Any() && o.AffiliateId.Contains(bookRequest.sourceMedia)) || o.AffiliateId.Any() == false) &&
                                           (o.AffiliateId_Not.Contains(bookRequest.sourceMedia) == false) &&
                                           ((o.FareType.Any() && o.FareType.Contains(bookRequest.flightResult[1].Fare.mojoFareType)) || o.FareType.Any() == false) &&
                                           (o.Supplier == bookRequest.flightResult[0].Fare.gdsType)).ToList();
                                    if (supplierData2 != null || supplierData2.Any())
                                    {
                                        action = supplierData2.FirstOrDefault().BookingAction;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //else 2
                        StringBuilder sbLog = new StringBuilder();
                        sbLog.Append(Environment.NewLine + "bookRequest.isMakeBookingInprogress:" + bookRequest.isMakeBookingInprogress);
                        new ServicesHub.LogWriter_New(sbLog.ToString(), bookRequest.bookingID.ToString(), "Booking", "BookFlight Else 2 ");
                    }
                    #endregion
                    StringBuilder sbLog2 = new StringBuilder();
                    if (action == BookingAction.MakePnr)
                    {
                        bool isMakeBooking = true;
                        new FlightMapper().bookingLog(ref sbLog2, "first time set isMakeBooking", " ");
                        foreach (var item in bookRequest.flightResult)
                        {
                            foreach (var fs in item.FlightSegments)
                            {
                                foreach (var seg in fs.Segments)
                                {
                                    if (seg.CabinClass == CabinType.Business)
                                    {
                                        new FlightMapper().bookingLog(ref sbLog2, "set isMakeBooking false due to CabinClass is Business", " ");
                                        isMakeBooking = false;
                                    }
                                }
                            }
                        }

                        if (bookRequest.travelType == Core.TravelType.International && (bookRequest.flightResult[0].Fare.gdsType == GdsType.TripJack))
                        {
                            isMakeBooking = false;
                        }
                        else
                        {
                            isMakeBooking = true;
                        }

                        if ((bookRequest.flightResult.Count > 1) && (bookRequest.flightResult[0].Fare.gdsType == GdsType.FareBoutique
                            || bookRequest.flightResult[0].Fare.gdsType == GdsType.AirIQ
                            || bookRequest.flightResult[0].Fare.gdsType == GdsType.SatkarTravel
                            || bookRequest.flightResult[0].Fare.gdsType == GdsType.GFS
                            || bookRequest.flightResult[0].Fare.gdsType == GdsType.Ease2Fly
                            || bookRequest.flightResult[0].Fare.gdsType == GdsType.Travelopedia
                            || bookRequest.flightResult[1].Fare.gdsType == GdsType.FareBoutique
                            || bookRequest.flightResult[1].Fare.gdsType == GdsType.AirIQ
                            || bookRequest.flightResult[1].Fare.gdsType == GdsType.SatkarTravel
                            || bookRequest.flightResult[1].Fare.gdsType == GdsType.GFS
                            || bookRequest.flightResult[1].Fare.gdsType == GdsType.Ease2Fly
                            || bookRequest.flightResult[1].Fare.gdsType == GdsType.Travelopedia))
                        {
                            isMakeBooking = false;
                        }

                        if (isMakeBooking)
                        {
                            if (bookRequest.flightResult[0].Fare.gdsType == GdsType.Travelogy)
                            {
                                //new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Call Travelogy");
                                //new ServicesHub.Travelogy.TravelogyServiceMapping().BookFlight(bookRequest, ref BookResponse);
                            }
                            else if (bookRequest.flightResult[0].Fare.gdsType == GdsType.TripJack)
                            {
                                new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Call TripJack");
                                new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookRequest, ref BookResponse);
                            }
                            else if (bookRequest.flightResult[0].Fare.gdsType == GdsType.Tbo)
                            {
                                new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Call Tbo");
                                new ServicesHub.Tbo.TboServiceMapping().BookFlight(bookRequest, ref BookResponse);
                            }
                            else if (bookRequest.flightResult[0].Fare.gdsType == GdsType.FareBoutique)
                            {
                                new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Call FareBoutique");
                                new ServicesHub.FareBoutique.FareBoutiqueServiceMapping().BookFlight(bookRequest, ref BookResponse);
                            }
                            else if (bookRequest.flightResult[0].Fare.gdsType == GdsType.OneDFare)
                            {
                                new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Call OneDFare");
                                new ServicesHub.OneDFare.OneDFareServiceMapping().BookFlight(bookRequest, ref BookResponse);
                            }
                            else if (bookRequest.flightResult[0].Fare.gdsType == GdsType.SatkarTravel)
                            {
                                new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Call SatkarTravel");
                                new ServicesHub.SatkarTravel.SatkarTravelServiceMapping().BookFlight(bookRequest, ref BookResponse);
                            }
                            else if (bookRequest.flightResult[0].Fare.gdsType == GdsType.AirIQ)
                            {
                                new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Call AirIQ");
                                new ServicesHub.AirIQ.AirIQServiceMapping().BookFlight(bookRequest, ref BookResponse);
                            }

                            else if (bookRequest.flightResult[0].Fare.gdsType == GdsType.Ease2Fly)
                            {
                                new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Call Ease2Fly");
                                new ServicesHub.Ease2Fly.Ease2FlyServiceMapping().BookFlight(bookRequest, ref BookResponse);
                            }

                            else if (bookRequest.flightResult[0].Fare.gdsType == GdsType.GFS)
                            {
                                new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Call GFS");
                                new ServicesHub.GFS.GFSServiceMapping().BookFlight(bookRequest, ref BookResponse);
                            }
                            else if (bookRequest.flightResult[0].Fare.gdsType == GdsType.Travelopedia)
                            {
                                new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Call Travelopedia");
                                new ServicesHub.Travelopedia.TravelopediaServiceMapping().BookFlight(bookRequest, ref BookResponse);
                            }
                        }
                        else
                        {
                            new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Booking InProgress due to else 3");
                            BookResponse.responseStatus.status = TransactionStatus.Success;
                            BookResponse.responseStatus.message = "InProgress";
                            BookResponse.ReturnPNR = "";
                            BookResponse.TvoBookingID = 0;
                            BookResponse.TvoReturnBookingID = 0;
                            BookResponse.bookingStatus = BookingStatus.InProgress;
                            BookResponse.invoice = new List<Invoice>();
                            foreach (var item in bookRequest.flightResult)
                            {
                                BookResponse.isTickted.Add(true);
                            }
                        }
                    }
                    else if (action == BookingAction.Inporgress)
                    {
                        new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Booking InProgress due BookingAction.Inporgress else 4");

                        if (bookRequest.flightResult[0].Fare.gdsType == GdsType.OneDFare)
                        {
                            new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Call OneDFare");
                            new ServicesHub.OneDFare.OneDFareServiceMapping().BookFlightInProgress(bookRequest, ref BookResponse);
                        }

                        BookResponse.responseStatus.status = TransactionStatus.Success;
                        BookResponse.responseStatus.message = "InProgress";
                        BookResponse.ReturnPNR = "";
                        BookResponse.TvoBookingID = 0;
                        BookResponse.TvoReturnBookingID = 0;
                        BookResponse.bookingStatus = BookingStatus.InProgress;
                        BookResponse.invoice = new List<Invoice>();
                        foreach (var item in bookRequest.flightResult)
                        {
                            BookResponse.isTickted.Add(true);
                        }
                    }
                    else
                    {
                        new ServicesHub.LogWriter_New(sbLog2.ToString(), bookRequest.bookingID.ToString(), "Booking", "Booking Fail due last else else 5");
                        BookResponse.responseStatus.status = TransactionStatus.Success;
                        BookResponse.responseStatus.message = "Fail";
                        BookResponse.ReturnPNR = "";
                        BookResponse.TvoBookingID = 0;
                        BookResponse.TvoReturnBookingID = 0;
                        BookResponse.bookingStatus = BookingStatus.Failed;
                        BookResponse.invoice = new List<Invoice>();
                        foreach (var item in bookRequest.flightResult)
                        {
                            BookResponse.isTickted.Add(true);
                        }
                    }
                }
                else
                {
                    //Else 1
                    StringBuilder sbLog = new StringBuilder();

                    sbLog.Append(Environment.NewLine + "GlobalData.isMakeBooking:" + GlobalData.isMakeBooking);
                    sbLog.Append(Environment.NewLine + "bookRequest.paymentDetails != null:" + bookRequest.paymentDetails != null);
                    sbLog.Append(Environment.NewLine + "!string.IsNullOrEmpty(bookRequest.paymentDetails.OnlinePaymentStauts):" + string.IsNullOrEmpty(bookRequest.paymentDetails.OnlinePaymentStauts));
                    sbLog.Append(Environment.NewLine + "bookRequest.paymentDetails.OnlinePaymentStauts.Equals(\"captured\", StringComparison.OrdinalIgnoreCase):" + bookRequest.paymentDetails.OnlinePaymentStauts.Equals("captured", StringComparison.OrdinalIgnoreCase));
                    sbLog.Append(Environment.NewLine + "bookRequest.paymentDetails.IsReturnHashMatched:" + bookRequest.paymentDetails.IsReturnHashMatched);
                    new ServicesHub.LogWriter_New(sbLog.ToString(), bookRequest.bookingID.ToString(), "Booking", "BookFlight Else 1 ");
                    BookResponse.responseStatus.status = TransactionStatus.Success;
                    BookResponse.responseStatus.message = "Booking pending due to PaymentStatus:" + bookRequest.paymentDetails.OnlinePaymentStauts;
                    BookResponse.ReturnPNR = "";
                    BookResponse.TvoBookingID = 0;
                    BookResponse.TvoReturnBookingID = 0;
                    BookResponse.bookingStatus = BookingStatus.Pending;
                    foreach (var item in bookRequest.flightResult)
                    {
                        BookResponse.isTickted.Add(true);
                    }
                    BookResponse.paymentStatus = PaymentStatus.CardDecline;
                }
                DAL.Booking.SaveBookingDetails obj = new DAL.Booking.SaveBookingDetails();
                obj.SaveFMJ_FlightBookingTransactionDetailsWithTickNo(ref bookRequest, ref BookResponse);
                //itinCtr ++;
            }
            catch (Exception ex)
            {
                new ServicesHub.LogWriter_New(ex.ToString(), bookRequest.bookingID.ToString(), "error", "BookFlight Exeption");
            }

            new ServicesHub.LogWriter_New(JsonConvert.SerializeObject(BookResponse), bookRequest.bookingID.ToString(), "Booking", "BookFlight Original Response");

            return Request.CreateResponse(HttpStatusCode.OK, BookResponse);

        }


        [HttpPost]
        [Route("GetBookingDetails")]
        public HttpResponseMessage GetBookingDetails(string authcode, FlightBookingRequest bookRequest)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            FlightBookingResponse BookResponse = new FlightBookingResponse(bookRequest);
            //new ServicesHub.Tbo.ServiceMapping().BookFlight(bookRequest, ref BookResponse);
            return Request.CreateResponse(HttpStatusCode.OK, BookResponse);

        }
        [HttpPost]
        [Route("GetCalendarFare")]
        public HttpResponseMessage GetCalendarFare(string authcode, FlightSearchRequest flightSearchRequest)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            //new ServicesHub.Tbo.ServiceMapping().getCalendarFare(flightSearchRequest);
            return Request.CreateResponse(HttpStatusCode.OK, true);

        }
        [HttpGet]
        [Route("UpdateStaticData")]
        public HttpResponseMessage UpdateStaticData()
        {
            Core.FlightUtility.LoadMasterData();
            return Request.CreateResponse(HttpStatusCode.OK, GlobalData.ServerID);
        }





        //[HttpPost]
        //[Route("BookFlight")]
        //public HttpResponseMessage BookFlight(string authorizationCode, BookRequest bookRequest)
        //{
        //    if (!SystemValidation.IsauthorizeRequest(authorizationCode))
        //    {
        //        return Request.CreateResponse(HttpStatusCode.Forbidden);
        //    }
        //    try
        //    {
        //        bookRequest.TokenId = getTokenID();
        //        var Url = ConfigurationManager.AppSettings["bookUrl"].ToString();
        //        try
        //        {
        //            new ServiceMapping.LogWriter(Newtonsoft.Json.JsonConvert.SerializeObject(bookRequest), "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //        }
        //        catch { }
        //        var response = GetResponse(Url, JsonConvert.SerializeObject(bookRequest));
        //        try
        //        {
        //            new ServiceMapping.LogWriter(response.ToString(), "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //        }
        //        catch { }
        //        if (!string.IsNullOrEmpty(response))
        //        {
        //            BookResponse bookResponse = JsonConvert.DeserializeObject<BookResponse>(response.ToString());
        //            return Request.CreateResponse(HttpStatusCode.OK, bookResponse);
        //        }
        //        else
        //        {
        //            BookResponse bookResponse = new BookResponse()
        //            {
        //                Response = new ResponseBR()
        //                {
        //                    Error = new Error()
        //                    {
        //                        ErrorCode = 100,
        //                        ErrorMessage = "Internal Error From Web Services."
        //                    }
        //                }
        //            };
        //            return Request.CreateResponse(HttpStatusCode.OK, bookResponse);
        //        }
        //    }
        //    catch
        //    {
        //        BookResponse bookResponse = new BookResponse()
        //        {
        //            Response = new ResponseBR()
        //            {
        //                Error = new Error()
        //                {
        //                    ErrorCode = 100,
        //                    ErrorMessage = ex.ToString()
        //                }
        //            }
        //        };
        //        return Request.CreateResponse(HttpStatusCode.OK, bookResponse);
        //    }
        //}
        //[HttpPost]
        //[Route("TicketRequestLCC")]
        //public HttpResponseMessage TicketRequestLCC(string authorizationCode, LccTicketingRequest lccTicketingRequest)
        //{
        //    if (!SystemValidation.IsauthorizeRequest(authorizationCode))
        //    {
        //        return Request.CreateResponse(HttpStatusCode.Forbidden);
        //    }
        //    try
        //    {
        //        lccTicketingRequest.TokenId = getTokenID();
        //        var Url = ConfigurationManager.AppSettings["ticketUrl"].ToString();
        //        try
        //        {
        //            //new ServiceMapping.LogWriter(Url, "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //            new ServiceMapping.LogWriter(Newtonsoft.Json.JsonConvert.SerializeObject(lccTicketingRequest), "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //        }
        //        catch { }
        //        var response = GetResponse(Url, JsonConvert.SerializeObject(lccTicketingRequest));
        //        //var response =JsonConvert.SerializeObject( new LccTicketingResponse());
        //        try
        //        {
        //            new ServiceMapping.LogWriter(response.ToString(), "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //        }
        //        catch { }
        //        if (!string.IsNullOrEmpty(response))
        //        {
        //            LccTicketingResponse bookResponse = JsonConvert.DeserializeObject<LccTicketingResponse>(response.ToString());
        //            return Request.CreateResponse(HttpStatusCode.OK, bookResponse);
        //        }
        //        else
        //        {
        //            LccTicketingResponse bookResponse = new LccTicketingResponse()
        //            {
        //                Response = new ResponseLTRes()
        //                {
        //                    Error = new Error()
        //                    {
        //                        ErrorCode = 100,
        //                        ErrorMessage = "Internal Error From Web Services."
        //                    }
        //                }
        //            };
        //            return Request.CreateResponse(HttpStatusCode.OK, bookResponse);
        //        }
        //    }
        //    catch
        //    {
        //        LccTicketingResponse bookResponse = new LccTicketingResponse()
        //        {
        //            Response = new ResponseLTRes()
        //            {
        //                Error = new Error()
        //                {
        //                    ErrorCode = 100,
        //                    ErrorMessage = ex.ToString()
        //                }
        //            }
        //        };
        //        return Request.CreateResponse(HttpStatusCode.OK, bookResponse);
        //    }
        //}
        //[HttpPost]
        //[Route("TicketRequestGDS")]
        //public HttpResponseMessage TicketRequestLCC(string authorizationCode, GdsTicketingRequest gdsTicketingRequest)
        //{
        //    if (!SystemValidation.IsauthorizeRequest(authorizationCode))
        //    {
        //        return Request.CreateResponse(HttpStatusCode.Forbidden);
        //    }
        //    try
        //    {
        //        gdsTicketingRequest.TokenId = getTokenID();
        //        var Url = ConfigurationManager.AppSettings["ticketUrl"].ToString();
        //        try
        //        {
        //            new ServiceMapping.LogWriter(Url, "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //            new ServiceMapping.LogWriter(Newtonsoft.Json.JsonConvert.SerializeObject(gdsTicketingRequest), "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //        }
        //        catch { }
        //        var response = GetResponse(Url, JsonConvert.SerializeObject(gdsTicketingRequest));
        //        try
        //        {
        //            new ServiceMapping.LogWriter(response.ToString(), "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //        }
        //        catch { }
        //        if (!string.IsNullOrEmpty(response))
        //        {
        //            LccTicketingResponse bookResponse = JsonConvert.DeserializeObject<LccTicketingResponse>(response.ToString());
        //            return Request.CreateResponse(HttpStatusCode.OK, bookResponse);
        //        }
        //        else
        //        {
        //            LccTicketingResponse bookResponse = new LccTicketingResponse()
        //            {
        //                Response = new ResponseLTRes()
        //                {
        //                    Error = new Error()
        //                    {
        //                        ErrorCode = 100,
        //                        ErrorMessage = "Internal Error From Web Services."
        //                    }
        //                }
        //            };
        //            return Request.CreateResponse(HttpStatusCode.OK, bookResponse);
        //        }
        //    }
        //    catch
        //    {
        //        LccTicketingResponse bookResponse = new LccTicketingResponse()
        //        {
        //            Response = new ResponseLTRes()
        //            {
        //                Error = new Error()
        //                {
        //                    ErrorCode = 100,
        //                    ErrorMessage = ex.ToString()
        //                }
        //            }
        //        };
        //        return Request.CreateResponse(HttpStatusCode.OK, bookResponse);
        //    }
        //}
        //[HttpPost]
        //[Route("GetCalendarFare")]
        //public HttpResponseMessage GetCalendarFare(string authorizationCode, CalendarFareRequest calendarFareRequest)
        //{
        //    if (!SystemValidation.IsauthorizeRequest(authorizationCode))
        //    {
        //        return Request.CreateResponse(HttpStatusCode.Forbidden);
        //    }
        //    try
        //    {
        //        calendarFareRequest.TokenId = getTokenID();
        //        var Url = ConfigurationManager.AppSettings["CalendarFareUrl"].ToString();
        //        try
        //        {
        //            new ServiceMapping.LogWriter(Newtonsoft.Json.JsonConvert.SerializeObject(calendarFareRequest), "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //        }
        //        catch { }
        //        var response = GetResponse(Url, JsonConvert.SerializeObject(calendarFareRequest));
        //        try
        //        {
        //            new ServiceMapping.LogWriter(response.ToString(), "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //        }
        //        catch { }
        //        if (!string.IsNullOrEmpty(response))
        //        {
        //            CalendarFareResponse calendarFareResponse = JsonConvert.DeserializeObject<CalendarFareResponse>(response.ToString());
        //            return Request.CreateResponse(HttpStatusCode.OK, calendarFareResponse);
        //        }
        //        else
        //        {
        //            CalendarFareResponse calendarFareResponse = new CalendarFareResponse()
        //            {
        //                Response = new ResponseCFR()
        //                {
        //                    Error = new Error()
        //                    {
        //                        ErrorCode = 100,
        //                        ErrorMessage = "Internal Error From Web Services."
        //                    }
        //                }
        //            };
        //            return Request.CreateResponse(HttpStatusCode.OK, calendarFareResponse);
        //        }
        //    }
        //    catch
        //    {
        //        CalendarFareResponse calendarFareResponse = new CalendarFareResponse()
        //        {
        //            Response = new ResponseCFR()
        //            {
        //                Error = new Error()
        //                {
        //                    ErrorCode = 100,
        //                    ErrorMessage = ex.ToString()
        //                }
        //            }
        //        };
        //        return Request.CreateResponse(HttpStatusCode.OK, calendarFareResponse);
        //    }
        //}
        //[HttpPost]
        //[Route("GetFareRule")]
        //public HttpResponseMessage GetFareRule(string authorizationCode, FareRuleRequest fareRuleRequest)
        //{
        //    if (!SystemValidation.IsauthorizeRequest(authorizationCode))
        //    {
        //        return Request.CreateResponse(HttpStatusCode.Forbidden);
        //    }
        //    try
        //    {
        //        fareRuleRequest.TokenId = getTokenID();
        //        var Url = ConfigurationManager.AppSettings["fareRuleUrl"].ToString();
        //        try
        //        {
        //            new ServiceMapping.LogWriter(Newtonsoft.Json.JsonConvert.SerializeObject(fareRuleRequest), "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //        }
        //        catch { }
        //        var response = GetResponse(Url, JsonConvert.SerializeObject(fareRuleRequest));
        //        try
        //        {
        //            new ServiceMapping.LogWriter(response.ToString(), "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //        }
        //        catch { }
        //        if (!string.IsNullOrEmpty(response))
        //        {
        //            FareRuleResponse fareRuleResponse = JsonConvert.DeserializeObject<FareRuleResponse>(response.ToString());
        //            return Request.CreateResponse(HttpStatusCode.OK, fareRuleResponse);
        //        }
        //        else
        //        {
        //            FareRuleResponse fareRuleResponse = new FareRuleResponse()
        //            {
        //                Response = new ResponseFR()
        //                {
        //                    Error = new Error()
        //                    {
        //                        ErrorCode = 100,
        //                        ErrorMessage = "Internal Error From Web Services."
        //                    }
        //                }
        //            };
        //            return Request.CreateResponse(HttpStatusCode.OK, fareRuleResponse);
        //        }
        //    }
        //    catch
        //    {
        //        FareRuleResponse fareRuleResponse = new FareRuleResponse()
        //        {
        //            Response = new ResponseFR()
        //            {
        //                Error = new Error()
        //                {
        //                    ErrorCode = 100,
        //                    ErrorMessage = ex.ToString()
        //                }
        //            }
        //        };
        //        return Request.CreateResponse(HttpStatusCode.OK, fareRuleResponse);
        //    }
        //}
        //[HttpPost]
        //[Route("CreateSSR")]
        //public HttpResponseMessage CreateSSR(string authorizationCode, string isLcc, SSRRequest SSRRequest)
        //{
        //    if (!SystemValidation.IsauthorizeRequest(authorizationCode))
        //    {
        //        return Request.CreateResponse(HttpStatusCode.Forbidden);
        //    }
        //    try
        //    {
        //        SSRRequest.TokenId = getTokenID();
        //        var Url = ConfigurationManager.AppSettings["SsrUrl"].ToString();
        //        try
        //        {
        //            new ServiceMapping.LogWriter(Newtonsoft.Json.JsonConvert.SerializeObject(SSRRequest), "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //        }
        //        catch { }
        //        var response = GetResponse(Url, JsonConvert.SerializeObject(SSRRequest));
        //        try
        //        {
        //            new ServiceMapping.LogWriter(response.ToString(), "TBO-" + DateTime.Today.ToString("ddMMyyy"));
        //        }
        //        catch { }
        //        if (!string.IsNullOrEmpty(response))
        //        {
        //            if (isLcc.Equals("true", StringComparison.OrdinalIgnoreCase))
        //            {
        //                SSRResponseforLCC sSRResponseforLCC = JsonConvert.DeserializeObject<SSRResponseforLCC>(response.ToString());
        //                return Request.CreateResponse(HttpStatusCode.OK, sSRResponseforLCC);
        //            }
        //            else
        //            {
        //                SSRResponseforNONLCC sSRResponseforNONLCC = JsonConvert.DeserializeObject<SSRResponseforNONLCC>(response.ToString());
        //                return Request.CreateResponse(HttpStatusCode.OK, sSRResponseforNONLCC);
        //            }
        //        }
        //        else
        //        {
        //            if (isLcc.Equals("true", StringComparison.OrdinalIgnoreCase))
        //            {
        //                SSRResponseforLCC sSRResponseforLCC = new SSRResponseforLCC()
        //                {
        //                    Response = new ResponseSSRLCC()
        //                    {
        //                        Error = new Error()
        //                        {
        //                            ErrorCode = 100,
        //                            ErrorMessage = "Internal Error From Web Services."
        //                        }
        //                    }
        //                };
        //                return Request.CreateResponse(HttpStatusCode.OK, sSRResponseforLCC);
        //            }
        //            else
        //            {
        //                SSRResponseforNONLCC sSRResponseforNONLCC = new SSRResponseforNONLCC()
        //                {
        //                    Response = new ResponseSSRNONLCC()
        //                    {
        //                        Error = new Error()
        //                        {
        //                            ErrorCode = 100,
        //                            ErrorMessage = "Internal Error From Web Services."
        //                        }
        //                    }
        //                };
        //                return Request.CreateResponse(HttpStatusCode.OK, sSRResponseforNONLCC);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        if (isLcc.Equals("true", StringComparison.OrdinalIgnoreCase))
        //        {
        //            SSRResponseforLCC sSRResponseforLCC = new SSRResponseforLCC()
        //            {
        //                Response = new ResponseSSRLCC()
        //                {
        //                    Error = new Error()
        //                    {
        //                        ErrorCode = 100,
        //                        ErrorMessage = ex.ToString()
        //                    }
        //                }
        //            };
        //            return Request.CreateResponse(HttpStatusCode.OK, sSRResponseforLCC);
        //        }
        //        else
        //        {
        //            SSRResponseforNONLCC sSRResponseforNONLCC = new SSRResponseforNONLCC()
        //            {
        //                Response = new ResponseSSRNONLCC()
        //                {
        //                    Error = new Error()
        //                    {
        //                        ErrorCode = 100,
        //                        ErrorMessage = ex.ToString()
        //                    }
        //                }
        //            };
        //            return Request.CreateResponse(HttpStatusCode.OK, sSRResponseforNONLCC);
        //        }
        //    }
        //}
        //private string GetResponse(string url, string requestData)
        //{
        //    string response = string.Empty;
        //    try
        //    {
        //        byte[] data = Encoding.UTF8.GetBytes(requestData);
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //        request.Method = "POST";
        //        request.ContentType = "application/json";
        //        //request.Headers.Add("Accept-Encoding", "gzip");
        //        Stream dataStream = request.GetRequestStream();
        //        dataStream.Write(data, 0, data.Length);
        //        dataStream.Close();
        //        WebResponse webResponse = request.GetResponse();
        //        var rsp = webResponse.GetResponseStream();
        //        //if (rsp == null)
        //        //{
        //        //    //throw exception
        //        //}
        //        //using (StreamReader readStream = new StreamReader(new GZipStream(rsp, CompressionMode.Decompress)))
        //        //{
        //        //    response = readStream.ReadToEnd();
        //        //}
        //        using (StreamReader reader = new StreamReader(rsp))
        //        {
        //            // Read the content.
        //            response = reader.ReadToEnd();
        //        }
        //        return response;
        //    }
        //    catch (WebException webEx)
        //    {
        //        //WebResponse wresponse = webEx.Response;
        //        //Stream stream = wresponse.GetResponseStream();
        //        //String responseMessage = new StreamReader(stream).ReadToEnd();
        //        //return responseMessage;
        //        return "";
        //    }
        //}

        private string getSearchID()
        {
            //    var nonceString = new StringBuilder();
            //    for (int i = 0; i < 15; i++)
            //    {
            //        nonceString.Append(validChars[random.Next(0, validChars.Length - 1)]);
            //    }
            //Guid guidValue = Guid.NewGuid();

            //Guid hashed = new Guid(md5.ComputeHash(guidValue.ToByteArray()));
            return (DateTime.Now.ToString("ddMMyyHHmmss") + Guid.NewGuid().ToString("N"));
        }
        private string getUserSessionID()
        {
            string UserSessionID = System.Text.RegularExpressions.Regex.Replace(Guid.NewGuid().ToString(), "[^a-zA-Z0-9]+", "");
            UserSessionID = UserSessionID.Length > 20 ? UserSessionID.Substring(0, 20) : UserSessionID;
            return UserSessionID;
        }
        public static string GetIpAddress()
        {
            var request = HttpContext.Current.Request;
            // Look for a proxy address first
            var ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            // If there is no proxy, get the standard remote address
            if (string.IsNullOrWhiteSpace(ip)
                || string.Equals(ip, "unknown", StringComparison.OrdinalIgnoreCase))
                ip = request.ServerVariables["REMOTE_ADDR"];
            else
            {
                //extract first IP
                var index = ip.IndexOf(',');
                if (index > 0)
                    ip = ip.Substring(0, index);

                //remove port
                index = ip.IndexOf(':');
                if (index > 0)
                    ip = ip.Substring(0, index);
            }

            return ip;
        }
        private void saveSearchListMeta(FlightSearchRequest flightSearchRequest, int totalResult, string Provider)
        {
            var save = Task.Run(async () =>
            {
                await new DAL.Deal.UserSearchHistory().SaveUserSearchHistoryMeta(flightSearchRequest, totalResult, Provider, GlobalData.ServerID);
            });
        }
        private void deleteSearchListMeta()
        {
            var save = Task.Run(async () =>
            {
                await new DAL.Deal.UserSearchHistory().DeleteUserSearchHistoryMeta();
            });
        }
        private void saveSearchList(FlightSearchRequest flightSearchRequest, bool isCommingMeta, int totalResult)
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            var save = Task.Run(async () =>
            {
                await new DAL.Deal.UserSearchHistory().SaveUserSearchHistory(flightSearchRequest, isCommingMeta, ip, totalResult);
            });
        }
        public void saveMetaSearchDetails(string SearchKey, string ResultData)
        {
            var save = Task.Run(async () =>
            {
                await new DALFlightCache().SaveMetaSearchDetails(SearchKey, ResultData);
            });
        }
        public bool CheckCredential(string strsiteid, string campain, string pwd, string ip)
        {
            bool returnData = false;
            if (campain == "1001" || campain == "1013" || campain == "1010" || campain == "1015")
            {
                if (campain == "1001")//WGO
                {
                    returnData = pwd.Equals("Mojoindiaflights321", StringComparison.OrdinalIgnoreCase);
                }
                if (campain == "1013")//Kayak
                {
                    returnData = pwd.Equals("Mojoindiaflights321", StringComparison.OrdinalIgnoreCase);
                }
                if (campain == "1010")//JetCost
                {
                    returnData = pwd.Equals("Mojoindiaflights321", StringComparison.OrdinalIgnoreCase);
                }
                if (campain == "1015")//SKYIN
                {
                    returnData = pwd.Equals("Mojoindiaflights321", StringComparison.OrdinalIgnoreCase);
                }
            }

            return returnData;
        }
        private void saveTopFare(FlightSearchRequest flightSearchRequest, List<List<FlightResult>> Results)
        {
            var save = Task.Run(async () =>
            {
                await new DAL.Deal.UserSearchFareDeal().SaveUserSearchFareDeal(flightSearchRequest, Results);
            });
        }
        private bool CheckIfTimeIsBetweenShift(DateTime time)
        {
            var NightShiftStart = new TimeSpan(22, 0, 0);
            var NightShiftEnd = new TimeSpan(9, 0, 0);
            return NightShiftStart <= time.TimeOfDay && time.TimeOfDay >= NightShiftEnd;
        }

        private void CallDeleteFiles()
        {
            var save = Task.Run(async () =>
            {
                await deletefiles();
            });
        }
        public async System.Threading.Tasks.Task deletefiles()
        {
            string pathFlightSearch = System.IO.Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "NewLog\\Search\\");
            string[] files = System.IO.Directory.GetFiles(pathFlightSearch);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddMinutes(-30))
                    fi.Delete();
            }
        }
        public void bookingLog(ref StringBuilder sbLogger, string requestTitle, string logText)
        {
            sbLogger.Append(Environment.NewLine + "---------------------------------------------" + requestTitle + "" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
            sbLogger.Append(Environment.NewLine + logText);
            sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
        }

    }
    public class FlightMapper
    {
        public Core.Flight.FlightSearchResponse GetFlightResultMultiGDS(FlightSearchRequest request)
        {
            request.device = request.device == Device.None ? Device.Desktop : request.device;
            StringBuilder sbLogger = new StringBuilder();
            Core.Flight.FlightSearchResponse response = new FlightSearchResponse(request);
            Task<FlightSearchResponseShort> tripJack = null;
            Task<FlightSearchResponseShort> OneDFare = null;
            Task<FlightSearchResponseShort> tbo = null;
            Task<FlightSearchResponseShort> fareBoutique = null;
            Task<FlightSearchResponseShort> satkarTravel = null;
            Task<FlightSearchResponseShort> AirIQGDS = null;
            Task<FlightSearchResponseShort> Ease2Fly = null;
            Task<FlightSearchResponseShort> GFS = null;
            Task<FlightSearchResponseShort> Travelopedia = null;

            bool istripJack = false;
            bool isOneDFare = false;
            bool istbo = false;
            bool isfareBoutique = false;
            bool isSatkarTravel = false;
            bool isAirIQGDS = false;
            bool isEase2Fly = false;
            bool isGFS = false;
            bool isTravelopedia = false;

            int tripJackSeq = 0, OneDFareSeq = 0, tboSeq = 0, fareBoutiqueSeq = 0, satkarTravelSeq = 0, AirIQTravelSeq = 0, Ease2FlySeq = 0, GFSSeq = 0, TravelopediaSeq = 0;
            #region CheckSupplier
            if (request.segment[0].orgArp == null)
            {
                request.segment[0].orgArp = FlightUtility.GetAirport(request.segment[0].originAirport);
            }
            if (request.segment[0].destArp == null)
            {
                request.segment[0].destArp = FlightUtility.GetAirport(request.segment[0].destinationAirport);
            }

            var supplierData = FlightUtility.SupplierList.Where(o => (o.siteId == request.siteId) &&
               ((o.FromAirport.Any() && o.FromAirport.Contains(request.segment[0].orgArp.airportCode)) || o.FromAirport.Any() == false) &&
               ((o.ToAirport.Any() && o.ToAirport.Contains(request.segment[0].destArp.airportCode)) || o.ToAirport.Any() == false) &&
               ((o.ToCountry.Any() && o.ToCountry.Contains(request.segment[0].orgArp.countryCode)) || o.ToCountry.Any() == false) &&
               ((o.ToCountry.Any() && o.ToCountry.Contains(request.segment[0].destArp.countryCode)) || o.ToCountry.Any() == false) &&
               ((o.SourceMedia.Any() && o.SourceMedia.Contains(request.sourceMedia)) || o.SourceMedia.Any() == false) &&
               (o.SourceMedia_Not.Contains(request.sourceMedia) == false) &&
               (o.device == Device.None || o.device == request.device)).ToList();

            foreach (var item in supplierData)
            {
                if (item.Provider == GdsType.TripJack)
                {
                    tripJackSeq = item.FarePriority;
                    istripJack = true;
                }
                if (item.Provider == GdsType.OneDFare)
                {
                    OneDFareSeq = item.FarePriority;
                    isOneDFare = true;
                }
                if (item.Provider == GdsType.Tbo)
                {
                    tboSeq = item.FarePriority;
                    istbo = true;
                }
                if (item.Provider == GdsType.FareBoutique)
                {
                    fareBoutiqueSeq = item.FarePriority;
                    isfareBoutique = true;
                }

                if (item.Provider == GdsType.SatkarTravel)
                {
                    satkarTravelSeq = item.FarePriority;
                    isSatkarTravel = true;
                }

                if (item.Provider == GdsType.AirIQ)
                {
                    AirIQTravelSeq = item.FarePriority;
                    isAirIQGDS = true;
                }

                if (item.Provider == GdsType.Ease2Fly)
                {
                    Ease2FlySeq = item.FarePriority;
                    isEase2Fly = true;
                }

                if (item.Provider == GdsType.GFS)
                {
                    GFSSeq = item.FarePriority;
                    isGFS = true;
                }

                if (item.Provider == GdsType.Travelopedia)
                {
                    TravelopediaSeq = item.FarePriority;
                    isTravelopedia = true;
                }
            }

            #endregion


            if (request.cabinType != Core.CabinType.Economy)
            {
                istbo = true;
                istripJack = false;
                isfareBoutique = false;
                isAirIQGDS = false;
                isGFS = false;
                isOneDFare = false;
                isSatkarTravel = false;
                isEase2Fly = false;
                isTravelopedia = false;
            }
            if (request.segment.Count > 1)
            {
                if (request.travelType == Core.TravelType.International)
                {
                    isOneDFare = false;
                    istbo = true;
                    isfareBoutique = false;
                    isAirIQGDS = false;
                    isSatkarTravel = false;
                    isEase2Fly = false;
                    isGFS = false;
                    istripJack = false;
                    isTravelopedia = false;
                }
                isOneDFare = false;
                //     isTravelopedia = false;
            }
            bool isfareBoutiqueR = false;
            bool isAirIQGDSR = false;
            bool isSatkarTravelR = false;
            bool isEase2FlyR = false;
            bool isGFSR = false;
            bool isTravelopediaR = false;

            if (isSatkarTravel || isAirIQGDS || isfareBoutique || isEase2Fly || isGFS || isTravelopedia)/* */
            {
                if (request.travelType == TravelType.Domestic && request.segment.Count > 1)
                {
                    var kkkR = new DAL.FixDepartueRoute.RoutesDetails().GetAvailableProvider(request.segment[1].originAirport, request.segment[1].destinationAirport, request.segment[1].travelDate.ToString("yyyy-MM-dd"));
                    if (isAirIQGDS)
                    {
                        isAirIQGDSR = kkkR.Contains(GdsType.AirIQ);
                    }
                    if (isfareBoutique)
                    {
                        isfareBoutiqueR = kkkR.Contains(GdsType.FareBoutique);
                    }
                    if (isSatkarTravel)
                    {
                        isSatkarTravelR = kkkR.Contains(GdsType.SatkarTravel);
                    }
                    if (isEase2Fly)
                    {
                        isEase2FlyR = kkkR.Contains(GdsType.Ease2Fly);
                    }
                    if (isGFS)
                    {
                        isGFSR = kkkR.Contains(GdsType.GFS);
                    }
                    if (isTravelopedia)
                    {
                        isTravelopediaR = kkkR.Contains(GdsType.Travelopedia);
                    }
                }
                var kkk = new DAL.FixDepartueRoute.RoutesDetails().GetAvailableProvider(request.segment[0].orgArp.airportCode, request.segment[0].destArp.airportCode, request.segment[0].travelDate.ToString("yyyy-MM-dd"));
                if (isAirIQGDS)
                {
                    isAirIQGDS = kkk.Contains(GdsType.AirIQ);
                }
                if (isfareBoutique)
                {
                    isfareBoutique = kkk.Contains(GdsType.FareBoutique);
                }
                if (isSatkarTravel)
                {
                    isSatkarTravel = kkk.Contains(GdsType.SatkarTravel);
                }
                if (isEase2Fly)
                {
                    isEase2Fly = kkk.Contains(GdsType.Ease2Fly);
                }
                if (isGFS)
                {
                    isGFS = kkk.Contains(GdsType.GFS);
                }
                if (isTravelopedia)
                {
                    isTravelopedia = kkk.Contains(GdsType.Travelopedia);
                }
            }
            if (istripJack) tripJack = GetSearchResultTripJack(request);
            if (isOneDFare) OneDFare = GetSearchResultOneDFare(request);
            if (istbo) tbo = GetSearchResultTbo(request);
            if (isfareBoutique || isfareBoutiqueR) fareBoutique = GetSearchResultFareBoutique(request, isfareBoutique, isfareBoutiqueR);
            if (isSatkarTravel || isSatkarTravelR) satkarTravel = GetSearchResultSatkarTravel(request, isSatkarTravel, isSatkarTravelR);
            if (isAirIQGDS || isAirIQGDSR) AirIQGDS = GetSearchResultAirIQGDS(request, isAirIQGDS, isAirIQGDSR);
            if (isEase2Fly || isEase2FlyR) Ease2Fly = GetSearchResultEase2Fly(request, isEase2Fly, isEase2FlyR);
            if (isGFS || isGFSR) GFS = GetSearchResultGFS(request, isGFS, isGFSR);
            if (isTravelopedia || isTravelopediaR) Travelopedia = GetSearchResultTravelopedia(request, isTravelopedia, isTravelopediaR);

            List<Task> taskList = new List<Task>();
            if (istripJack) taskList.Add(tripJack);
            if (isOneDFare) taskList.Add(OneDFare);
            if (istbo) taskList.Add(tbo);
            if (isfareBoutique || isfareBoutiqueR) taskList.Add(fareBoutique);
            if (isSatkarTravel || isSatkarTravelR) taskList.Add(satkarTravel);
            if (isAirIQGDS || isAirIQGDSR) taskList.Add(AirIQGDS);
            if (isEase2Fly || isEase2FlyR) taskList.Add(Ease2Fly);
            if (isGFS || isGFSR) taskList.Add(GFS);
            if (isTravelopedia || isTravelopediaR) taskList.Add(Travelopedia);

            TimeSpan timeSpan = TimeSpan.FromSeconds(15); //TODO Reduce timing to under 10 seconds.
            Task.WaitAll(taskList.ToArray(), timeSpan);

            DistinctFlightResult(ref request, ref response, (isOneDFare ? OneDFare.Result : new FlightSearchResponseShort()), (istripJack ? tripJack.Result : new FlightSearchResponseShort()),
                (istbo ? tbo.Result : new FlightSearchResponseShort()),
                (isfareBoutique || isfareBoutiqueR ? fareBoutique.Result : new FlightSearchResponseShort()),
                (isSatkarTravel || isSatkarTravelR ? satkarTravel.Result : new FlightSearchResponseShort()),
                (isAirIQGDS || isAirIQGDSR ? AirIQGDS.Result : new FlightSearchResponseShort()),
                (isEase2Fly || isEase2FlyR ? Ease2Fly.Result : new FlightSearchResponseShort()),
                (isGFS || isGFSR ? GFS.Result : new FlightSearchResponseShort()),
                (isTravelopedia || isTravelopediaR ? Travelopedia.Result : new FlightSearchResponseShort()));


            if (response != null && response.Results != null && response.Results.Count() > 0 && response.Results[0].Count > 0 && response.Results.LastOrDefault().Count > 0)
            {
                MarkupCalculator objMarkupCalculator = new MarkupCalculator();
                objMarkupCalculator.SetMarkup(ref request, ref response);
                for (int i = 0; i < response.Results.Count; i++)
                {
                    #region Set Commisson 
                    foreach (var item in response.Results[i])
                    {
                        if (item.Fare.gdsType==GdsType.Tbo && item.Fare.pLBEarned>0)
                        {

                        }
                        var commisionData = FlightUtility.AirlineCommissionRuleList.Where(o =>
                                     ((o.SourceMedia.Any() && o.SourceMedia.Contains(request.sourceMedia)) || o.SourceMedia.Any() == false) &&
                                     ((o.Provider.Any() && o.Provider.Contains(item.Fare.gdsType)) || o.Provider.Any() == false) &&
                                     ((o.Airline.Any() && o.Airline.Contains(item.FlightSegments[0].Segments[0].Airline)) || o.Airline.Any() == false)
                                     ).ToList();
                        if (commisionData.Any())
                        {
                            item.Fare.Markup -= (item.Fare.CommissionEarned+item.Fare.pLBEarned);
                            item.Fare.grandTotal = (item.Fare.BaseFare + item.Fare.Tax + item.Fare.OtherCharges + item.Fare.ServiceFee + item.Fare.ConvenienceFee + item.Fare.Markup);
                            item.Fare.markupID += " Comm(-)";
                        }
                    }
                    #endregion
                    response.Results[i] = response.Results[i].OrderBy(o => o.Fare.grandTotal).ToList();
                }
            }
            if (FlightUtility.isWriteLogSearch)
            {
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            }
            return response;
        }

        public Core.Flight.FlightSearchResponse GetFlightResultALLGDS(FlightSearchRequest request)
        {
            StringBuilder sbLogger = new StringBuilder();
            Core.Flight.FlightSearchResponse response = new FlightSearchResponse(request);
            Task<FlightSearchResponseShort> tripJack = null;
            Task<FlightSearchResponseShort> OneDFare = null;
            Task<FlightSearchResponseShort> tbo = null;
            Task<FlightSearchResponseShort> fareBoutique = null;
            Task<FlightSearchResponseShort> satkarTravel = null;
            Task<FlightSearchResponseShort> AirIQGDS = null;
            Task<FlightSearchResponseShort> Ease2Fly = null;
            Task<FlightSearchResponseShort> GFS = null;
            Task<FlightSearchResponseShort> Travelopedia = null;

            bool istripJack = true;
            bool isOneDFare = true;
            bool istbo = true;
            bool isfareBoutique = true;
            bool isSatkarTravel = true;
            bool isAirIQGDS = false;
            bool isEase2Fly = true;
            bool isGFS = true;
            bool isTravelopedia = true;

            int tripJackSeq = 0, OneDFareSeq = 0, tboSeq = 0, fareBoutiqueSeq = 0, satkarTravelSeq = 0, AirIQTravelSeq = 0, Ease2FlySeq = 0, GFSSeq = 0, TravelopediaSeq = 0;


            if (request.cabinType != Core.CabinType.Economy)
            {
                istbo = true;
                istripJack = false;
                isfareBoutique = false;
                isAirIQGDS = false;
                isGFS = false;
                isOneDFare = false;
                isSatkarTravel = false;
                isEase2Fly = false;
                isTravelopedia = false;
            }
            if (request.segment.Count > 1)
            {
                if (request.travelType == Core.TravelType.International)
                {
                    isOneDFare = false;
                    istbo = true;
                    isfareBoutique = false;
                    isAirIQGDS = false;
                    isSatkarTravel = false;
                    isEase2Fly = false;
                    isGFS = false;
                    istripJack = false;
                    isTravelopedia = false;
                }
                isOneDFare = false;
                //     isTravelopedia = false;
            }
            bool isfareBoutiqueR = false;
            bool isAirIQGDSR = false;
            bool isSatkarTravelR = false;
            bool isEase2FlyR = false;
            bool isGFSR = false;
            bool isTravelopediaR = false;




            if (request.travelType == TravelType.Domestic && request.segment.Count > 1)
            {
                var kkkR = new DAL.FixDepartueRoute.RoutesDetails().GetAvailableProvider(request.segment[1].originAirport, request.segment[1].destinationAirport, request.segment[1].travelDate.ToString("yyyy-MM-dd"));
                if (isAirIQGDS)
                {
                    isAirIQGDSR = kkkR.Contains(GdsType.AirIQ);
                }
                if (isfareBoutique)
                {
                    isfareBoutiqueR = kkkR.Contains(GdsType.FareBoutique);
                }
                if (isSatkarTravel)
                {
                    isSatkarTravelR = kkkR.Contains(GdsType.SatkarTravel);
                }
                if (isEase2Fly)
                {
                    isEase2FlyR = kkkR.Contains(GdsType.Ease2Fly);
                }
                if (isGFS)
                {
                    isGFSR = kkkR.Contains(GdsType.GFS);
                }
            }
            var kkk = new DAL.FixDepartueRoute.RoutesDetails().GetAvailableProvider(request.segment[0].orgArp.airportCode, request.segment[0].destArp.airportCode, request.segment[0].travelDate.ToString("yyyy-MM-dd"));
            if (isAirIQGDS)
            {
                isAirIQGDS = kkk.Contains(GdsType.AirIQ);
            }
            if (isfareBoutique)
            {
                isfareBoutique = kkk.Contains(GdsType.FareBoutique);
            }
            if (isSatkarTravel)
            {
                isSatkarTravel = kkk.Contains(GdsType.SatkarTravel);
            }
            if (isEase2Fly)
            {
                isEase2Fly = kkk.Contains(GdsType.Ease2Fly);
            }
            if (isGFS)
            {
                isGFS = kkk.Contains(GdsType.GFS);
            }

            if (istripJack) tripJack = GetSearchResultTripJack(request);
            if (isOneDFare) OneDFare = GetSearchResultOneDFare(request);
            if (istbo) tbo = GetSearchResultTbo(request);
            if (isfareBoutique || isfareBoutiqueR) fareBoutique = GetSearchResultFareBoutique(request, isfareBoutique, isfareBoutiqueR);
            if (isSatkarTravel || isSatkarTravelR) satkarTravel = GetSearchResultSatkarTravel(request, isSatkarTravel, isSatkarTravelR);
            if (isAirIQGDS || isAirIQGDSR) AirIQGDS = GetSearchResultAirIQGDS(request, isAirIQGDS, isAirIQGDSR);
            if (isEase2Fly || isEase2FlyR) Ease2Fly = GetSearchResultEase2Fly(request, isEase2Fly, isEase2FlyR);
            if (isGFS || isGFSR) GFS = GetSearchResultGFS(request, isGFS, isGFSR);
            if (isTravelopedia || isTravelopediaR) Travelopedia = GetSearchResultTravelopedia(request, isTravelopedia, isTravelopediaR);

            List<Task> taskList = new List<Task>();
            if (istripJack) taskList.Add(tripJack);
            if (isOneDFare) taskList.Add(OneDFare);
            if (istbo) taskList.Add(tbo);
            if (isfareBoutique || isfareBoutiqueR) taskList.Add(fareBoutique);
            if (isSatkarTravel || isSatkarTravelR) taskList.Add(satkarTravel);
            if (isAirIQGDS || isAirIQGDSR) taskList.Add(AirIQGDS);
            if (isEase2Fly || isEase2FlyR) taskList.Add(Ease2Fly);
            if (isGFS || isGFSR) taskList.Add(GFS);
            if (isTravelopedia || isTravelopediaR) taskList.Add(Travelopedia);

            TimeSpan timeSpan = TimeSpan.FromSeconds(15); //TODO Reduce timing to under 10 seconds.
            Task.WaitAll(taskList.ToArray(), timeSpan);

            DistinctFlightResult(ref request, ref response, (isOneDFare ? OneDFare.Result : new FlightSearchResponseShort()), (istripJack ? tripJack.Result : new FlightSearchResponseShort()),
                (istbo ? tbo.Result : new FlightSearchResponseShort()),
                (isfareBoutique || isfareBoutiqueR ? fareBoutique.Result : new FlightSearchResponseShort()),
                (isSatkarTravel || isSatkarTravelR ? satkarTravel.Result : new FlightSearchResponseShort()),
                (isAirIQGDS || isAirIQGDSR ? AirIQGDS.Result : new FlightSearchResponseShort()),
                (isEase2Fly || isEase2FlyR ? Ease2Fly.Result : new FlightSearchResponseShort()),
                (isGFS || isGFSR ? GFS.Result : new FlightSearchResponseShort()),
                (isTravelopedia || isTravelopediaR ? Travelopedia.Result : new FlightSearchResponseShort()));


            if (response != null && response.Results != null && response.Results.Count() > 0 && response.Results[0].Count > 0 && response.Results.LastOrDefault().Count > 0)
            {
                MarkupCalculator objMarkupCalculator = new MarkupCalculator();
                objMarkupCalculator.SetNoMarkup(ref request, ref response);
                for (int i = 0; i < response.Results.Count; i++)
                {
                    //#region Set Commisson 
                    //foreach (var item in response.Results[i])
                    //{
                    //    var commisionData = FlightUtility.AirlineCommissionRuleList.Where(o =>
                    //                 ((o.SourceMedia.Any() && o.SourceMedia.Contains(request.sourceMedia)) || o.SourceMedia.Any() == false) &&
                    //                 ((o.Provider.Any() && o.Provider.Contains(item.Fare.gdsType)) || o.Provider.Any() == false) &&
                    //                 ((o.Airline.Any() && o.Airline.Contains(item.FlightSegments[0].Segments[0].Airline)) || o.Airline.Any() == false)
                    //                 ).ToList();
                    //    if (commisionData.Any())
                    //    {
                    //        item.Fare.Markup -= (item.Fare.CommissionEarned);
                    //        item.Fare.grandTotal = (item.Fare.BaseFare + item.Fare.Tax + item.Fare.OtherCharges + item.Fare.ServiceFee + item.Fare.ConvenienceFee + item.Fare.Markup);
                    //        item.Fare.markupID += " Comm(-)";
                    //    }
                    //}
                    //#endregion
                    response.Results[i] = response.Results[i].OrderBy(o => o.Fare.grandTotal).ToList();
                }
            }
            if (FlightUtility.isWriteLogSearch)
            {
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            }
            return response;
        }
        public void DistinctFlightResult(ref FlightSearchRequest fsr, ref FlightSearchResponse OrgResponse, FlightSearchResponseShort oneDRes, FlightSearchResponseShort TjRes, FlightSearchResponseShort TboRes,
            FlightSearchResponseShort fareBoutique, FlightSearchResponseShort STRes, FlightSearchResponseShort AirIQRes, FlightSearchResponseShort E2FRes, FlightSearchResponseShort GFSRes, FlightSearchResponseShort TravelopediaRes)
        {
            try
            {

                OrgResponse.TraceId = TboRes.TraceId;
                OrgResponse.FB_booking_token_id = fareBoutique.FB_booking_token_id;
                OrgResponse.tgy_Search_Key = TravelopediaRes.tgy_Search_Key;
                if (true)
                {
                    int ctr = 0;
                    List<FlightResult> strComb = new List<FlightResult>();
                    strComb = strComb.Union(oneDRes.Results != null && oneDRes.Results.Count > 0 && oneDRes.Results[0].Count > 0 ? oneDRes.Results[0] : new List<FlightResult>()).ToList()
                        .Union(TboRes.Results != null && TboRes.Results.Count > 0 && TboRes.Results[0].Count > 0 ? TboRes.Results[0] : new List<FlightResult>()).ToList()
                        .Union(fareBoutique.Results != null && fareBoutique.Results.Count > 0 && fareBoutique.Results[0].Count > 0 ? fareBoutique.Results[0] : new List<FlightResult>()).ToList()
                        .Union(TjRes.Results != null && TjRes.Results.Count > 0 && TjRes.Results[0].Count > 0 ? TjRes.Results[0] : new List<FlightResult>()).ToList()
                        .Union(STRes.Results != null && STRes.Results.Count > 0 && STRes.Results[0].Count > 0 ? STRes.Results[0] : new List<FlightResult>()).ToList()
                        .Union(AirIQRes.Results != null && AirIQRes.Results.Count > 0 && AirIQRes.Results[0].Count > 0 ? AirIQRes.Results[0] : new List<FlightResult>()).ToList()
                        .Union(E2FRes.Results != null && E2FRes.Results.Count > 0 && E2FRes.Results[0].Count > 0 ? E2FRes.Results[0] : new List<FlightResult>()).ToList()
                        .Union(GFSRes.Results != null && GFSRes.Results.Count > 0 && GFSRes.Results[0].Count > 0 ? GFSRes.Results[0] : new List<FlightResult>()).ToList()
                        .Union(TravelopediaRes.Results != null && TravelopediaRes.Results.Count > 0 && TravelopediaRes.Results[0].Count > 0 ? TravelopediaRes.Results[0] : new List<FlightResult>()).ToList();
                    var strList = strComb.GroupBy(o => new { o.ResultCombination }).Select(o => o.FirstOrDefault().ResultCombination);
                    List<FlightResult> resutlDep = new List<FlightResult>();
                    foreach (string str in strList)
                    {
                        ctr++;
                        try
                        {
                            if (ctr == 108)
                            {

                            }
                            List<FlightResult> result = strComb.Where(o => o.ResultCombination == str).ToList();

                            FlightResult firstResult = result.FirstOrDefault();
                            firstResult.ResultID = "OTB" + ctr.ToString();

                            // if (result.Count > 1 && firstResult.gdsType != GdsType.Travelogy)
                            if (result.Count > 1 && firstResult.gdsType != GdsType.Travelopedia)
                            {
                                //  var tgyResult = result.Where(o => o.gdsType == GdsType.Travelogy).FirstOrDefault();
                                var tgyResult = result.Where(o => o.gdsType == GdsType.Travelopedia).FirstOrDefault();
                                if (tgyResult != null)
                                {
                                    firstResult.tgy_Flight_Key = tgyResult.tgy_Flight_Key;
                                    firstResult.Tgy_Flight_Id = tgyResult.Tgy_Flight_Id;
                                    firstResult.Tgy_Flight_No = tgyResult.Tgy_Flight_No;
                                }
                            }

                            if (result.Count > 1 && firstResult.gdsType == GdsType.SatkarTravel)
                            {
                                var STResult = result.Where(o => o.gdsType == GdsType.SatkarTravel).FirstOrDefault();
                                if (STResult != null)
                                {
                                    firstResult.ST_ResultSessionID = STResult.ST_ResultSessionID;
                                }
                            }


                            List<Fare> fare = new List<Fare>();
                            int fareCtr = 0;
                            foreach (var item in result)
                            {
                                if (item.gdsType == GdsType.SatkarTravel)
                                {
                                    firstResult.ST_ResultSessionID = item.ST_ResultSessionID;
                                }
                                foreach (var flist in item.FareList)
                                {
                                    flist.FM_FareID = (++fareCtr).ToString();
                                    fare.Add(flist);
                                }
                            }
                            firstResult.Fare = fare.Where(o => o.isBlock == false).OrderBy(o => o.grandTotal).FirstOrDefault();
                            firstResult.FareList = fare;

                            if (firstResult.Fare != null)
                            {
                                resutlDep.Add(firstResult);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    OrgResponse.Results.Add(resutlDep);
                }
                if ((oneDRes.Results != null && oneDRes.Results.Count > 1 && oneDRes.Results[1].Count > 0) ||
                    (TjRes.Results != null && TjRes.Results.Count > 1 && TjRes.Results[1].Count > 0) ||
                    (TboRes.Results != null && TboRes.Results.Count > 1 && TboRes.Results[1].Count > 0) ||
                    (STRes.Results != null && STRes.Results.Count > 1 && STRes.Results[1].Count > 0) ||
                    (AirIQRes.Results != null && AirIQRes.Results.Count > 1 && AirIQRes.Results[1].Count > 0) ||
                    (E2FRes.Results != null && E2FRes.Results.Count > 1 && E2FRes.Results[1].Count > 0) ||
                    (fareBoutique.Results != null && fareBoutique.Results.Count > 1 && fareBoutique.Results[1].Count > 0) ||
                    (GFSRes.Results != null && GFSRes.Results.Count > 1 && GFSRes.Results[1].Count > 0) ||
                    (TravelopediaRes.Results != null && TravelopediaRes.Results.Count > 1 && TravelopediaRes.Results[1].Count > 0))
                {
                    int ctr = 0;
                    List<FlightResult> strComb = new List<FlightResult>();
                    strComb = strComb.Union(oneDRes.Results != null && oneDRes.Results.Count > 1 && oneDRes.Results[1].Count > 0 ? oneDRes.Results[1] : new List<FlightResult>()).ToList()
                                .Union(TboRes.Results != null && TboRes.Results.Count > 1 && TboRes.Results[1].Count > 0 ? TboRes.Results[1] : new List<FlightResult>()).ToList()
                                .Union(TjRes.Results != null && TjRes.Results.Count > 1 && TjRes.Results[1].Count > 0 ? TjRes.Results[1] : new List<FlightResult>()).ToList()
                                .Union(STRes.Results != null && STRes.Results.Count > 1 ? STRes.Results[1] : new List<FlightResult>()).ToList()
                                .Union(AirIQRes.Results != null && AirIQRes.Results.Count > 1 ? AirIQRes.Results[1] : new List<FlightResult>()).ToList()
                                .Union(E2FRes.Results != null && E2FRes.Results.Count > 1 ? E2FRes.Results[1] : new List<FlightResult>()).ToList()
                                .Union(fareBoutique.Results != null && fareBoutique.Results.Count > 1 ? fareBoutique.Results[1] : new List<FlightResult>()).ToList()
                                .Union(GFSRes.Results != null && GFSRes.Results.Count > 1 ? GFSRes.Results[1] : new List<FlightResult>()).ToList()
                                .Union(TravelopediaRes.Results != null && TravelopediaRes.Results.Count > 1 ? TravelopediaRes.Results[1] : new List<FlightResult>()).ToList();
                    var strList = strComb.GroupBy(o => new { o.ResultCombination }).Select(o => o.FirstOrDefault().ResultCombination);
                    List<FlightResult> resutlret = new List<FlightResult>();
                    foreach (string str in strList)
                    {
                        ctr++;
                        try
                        {
                            List<FlightResult> result = strComb.Where(o => o.ResultCombination == str).ToList();
                            FlightResult firstResult = result.FirstOrDefault();
                            firstResult.ResultID = "INB" + ctr.ToString();
                            //  if (result.Count > 1 && firstResult.gdsType != GdsType.Travelogy)
                            if (result.Count > 1 && firstResult.gdsType != GdsType.Travelopedia)
                            {
                                //  var tgyResult = result.Where(o => o.gdsType == GdsType.Travelogy).FirstOrDefault();
                                var tgyResult = result.Where(o => o.gdsType == GdsType.Travelopedia).FirstOrDefault();
                                if (tgyResult != null)
                                {
                                    firstResult.tgy_Flight_Key = tgyResult.tgy_Flight_Key;
                                    firstResult.Tgy_Flight_Id = tgyResult.Tgy_Flight_Id;
                                    firstResult.Tgy_Flight_No = tgyResult.Tgy_Flight_No;
                                }
                            }
                            List<Fare> fare = new List<Fare>();
                            int fareCtr = 0;
                            foreach (var item in result)
                            {
                                foreach (var flist in item.FareList)
                                {
                                    if (item.gdsType == GdsType.SatkarTravel)
                                    {
                                        firstResult.ST_ResultSessionID = item.ST_ResultSessionID;
                                    }
                                    flist.FM_FareID = (++fareCtr).ToString();
                                    fare.Add(flist);
                                }
                            }
                            firstResult.Fare = fare.Where(o => o.isBlock == false).OrderBy(o => o.grandTotal).FirstOrDefault();
                            firstResult.FareList = fare;

                            if (firstResult.Fare != null)
                            {
                                resutlret.Add(firstResult);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    OrgResponse.Results.Add(resutlret);
                }

            }

            catch (Exception ex)
            {


            }
            if (fsr.travelType == TravelType.Domestic && fsr.tripType == TripType.RoundTrip)
            {
                if (OrgResponse.Results.Count == 1)
                {
                    OrgResponse.Results = new List<List<FlightResult>>();
                }
            }
        }


        #region GoogleFlghtSearch

        public Core.Flight.FlightSearchResponse GetFlightResultMultiGDSGF(FlightSearchRequest request)
        {
            StringBuilder sbLogger = new StringBuilder();
            Core.Flight.FlightSearchResponse response = new FlightSearchResponse(request);
            Task<FlightSearchResponseShort> tripJack = null;
            Task<FlightSearchResponseShort> tbo = null;

            bool istripJack = false;
            bool istbo = false;

            int tripJackSeq = 0, tboSeq = 0;
            #region CheckSupplier
            if (request.segment[0].orgArp == null)
            {
                request.segment[0].orgArp = FlightUtility.GetAirport(request.segment[0].originAirport);
            }
            if (request.segment[0].destArp == null)
            {
                request.segment[0].destArp = FlightUtility.GetAirport(request.segment[0].destinationAirport);
            }

            //var supplierData = FlightUtility.SupplierList.Where(o => (o.siteId == request.siteId) &&
            //   ((o.FromAirport.Any() && o.FromAirport.Contains(request.segment[0].orgArp.airportCode)) || o.FromAirport.Any() == false) &&
            //   ((o.ToAirport.Any() && o.ToAirport.Contains(request.segment[0].destArp.airportCode)) || o.ToAirport.Any() == false) &&
            //   ((o.ToCountry.Any() && o.ToCountry.Contains(request.segment[0].orgArp.countryCode)) || o.ToCountry.Any() == false) &&
            //   ((o.ToCountry.Any() && o.ToCountry.Contains(request.segment[0].destArp.countryCode)) || o.ToCountry.Any() == false) &&
            //   ((o.SourceMedia.Any() && o.SourceMedia.Contains(request.sourceMedia)) || o.SourceMedia.Any() == false) &&
            //   (o.SourceMedia_Not.Contains(request.sourceMedia) == false)).ToList();

            //foreach (var item in supplierData)
            //{
            //    if (item.Provider == GdsType.TripJack)
            //    {
            //        tripJackSeq = item.FarePriority;
            //        istripJack = true;
            //    }
            //    if (item.Provider == GdsType.Tbo)
            //    {
            //        tboSeq = item.FarePriority;
            //        istbo = true;
            //    }
            //}

            #endregion

            //if (request.segment.Count > 1)
            //{
            //    istbo = request.travelType == Core.TravelType.International ? false : true;
            //    istripJack = request.travelType == Core.TravelType.International ? false : true;
            //}


            istripJack = true;
            istbo = true;

            if (istripJack) tripJack = GetSearchResultTripJack(request);
            if (istbo) tbo = GetSearchResultTbo(request);


            List<Task> taskList = new List<Task>();
            if (istripJack) taskList.Add(tripJack);
            if (istbo) taskList.Add(tbo);

            TimeSpan timeSpan = TimeSpan.FromSeconds(15);
            Task.WaitAll(taskList.ToArray(), timeSpan);

            DistinctFlightResultGF(ref response, (istripJack ? tripJack.Result : new FlightSearchResponseShort()),
                (istbo ? tbo.Result : new FlightSearchResponseShort()));


            if (response != null && response.Results != null && response.Results.Count() > 0 && response.Results[0].Count > 0 && response.Results.LastOrDefault().Count > 0)
            {
                MarkupCalculator objMarkupCalculator = new MarkupCalculator();
                objMarkupCalculator.SetNoMarkup(ref request, ref response);
                for (int i = 0; i < response.Results.Count; i++)
                {
                    #region Set Commisson 
                    foreach (var item in response.Results[i])
                    {
                        var commisionData = FlightUtility.AirlineCommissionRuleList.Where(o =>
                                     ((o.SourceMedia.Any() && o.SourceMedia.Contains(request.sourceMedia)) || o.SourceMedia.Any() == false) &&
                                     ((o.Provider.Any() && o.Provider.Contains(item.Fare.gdsType)) || o.Provider.Any() == false) &&
                                     ((o.Airline.Any() && o.Airline.Contains(item.FlightSegments[0].Segments[0].Airline)) || o.Airline.Any() == false)
                                     ).ToList();
                        if (commisionData.Any())
                        {
                            item.Fare.Markup -= (item.Fare.CommissionEarned);
                            item.Fare.grandTotal = (item.Fare.BaseFare + item.Fare.Tax + item.Fare.OtherCharges + item.Fare.ServiceFee + item.Fare.ConvenienceFee + item.Fare.Markup);
                            item.Fare.markupID += "GF Comm(-)";
                        }
                    }
                    #endregion
                    response.Results[i] = response.Results[i].OrderBy(o => o.Fare.grandTotal).ToList();
                }
            }
            if (FlightUtility.isWriteLogSearch)
            {
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            }
            return response;
        }


        public void DistinctFlightResultGF(ref FlightSearchResponse OrgResponse, FlightSearchResponseShort TjRes, FlightSearchResponseShort TboRes)
        {
            try
            {

                OrgResponse.TraceId = TboRes.TraceId;
                if (true)
                {
                    int ctr = 0;
                    List<FlightResult> strComb = new List<FlightResult>();
                    strComb = strComb.Union(TboRes.Results != null && TboRes.Results.Count > 0 && TboRes.Results[0].Count > 0 ? TboRes.Results[0] : new List<FlightResult>()).ToList()
                        .Union(TjRes.Results != null && TjRes.Results.Count > 0 && TjRes.Results[0].Count > 0 ? TjRes.Results[0] : new List<FlightResult>()).ToList();
                    var strList = strComb.GroupBy(o => new { o.ResultCombination }).Select(o => o.FirstOrDefault().ResultCombination);
                    List<FlightResult> resutlDep = new List<FlightResult>();
                    foreach (string str in strList)
                    {
                        ctr++;
                        try
                        {
                            List<FlightResult> result = strComb.Where(o => o.ResultCombination == str).ToList();

                            FlightResult firstResult = result.FirstOrDefault();
                            firstResult.ResultID = "OTB" + ctr.ToString();

                            if (result.Count > 1 && firstResult.gdsType != GdsType.Travelogy)
                            {
                                var tgyResult = result.Where(o => o.gdsType == GdsType.Travelogy).FirstOrDefault();
                                if (tgyResult != null)
                                {
                                    firstResult.tgy_Flight_Key = tgyResult.tgy_Flight_Key;
                                    firstResult.Tgy_Flight_Id = tgyResult.Tgy_Flight_Id;
                                    firstResult.Tgy_Flight_No = tgyResult.Tgy_Flight_No;
                                }
                            }

                            if (result.Count > 1 && firstResult.gdsType == GdsType.SatkarTravel)
                            {
                                var STResult = result.Where(o => o.gdsType == GdsType.SatkarTravel).FirstOrDefault();
                                if (STResult != null)
                                {
                                    firstResult.ST_ResultSessionID = STResult.ST_ResultSessionID;
                                }
                            }


                            List<Fare> fare = new List<Fare>();
                            foreach (var item in result)
                            {
                                if (item.gdsType == GdsType.SatkarTravel)
                                {
                                    firstResult.ST_ResultSessionID = item.ST_ResultSessionID;
                                }
                                foreach (var flist in item.FareList)
                                {
                                    fare.Add(flist);
                                }
                            }
                            firstResult.Fare = fare.Where(o => o.isBlock == false).OrderBy(o => o.grandTotal).FirstOrDefault();
                            firstResult.FareList = fare;

                            if (firstResult.Fare != null)
                            {
                                resutlDep.Add(firstResult);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    OrgResponse.Results.Add(resutlDep);
                }
                if ((TjRes.Results != null && TjRes.Results.Count > 1 && TjRes.Results[1].Count > 0) ||
                    (TboRes.Results != null && TboRes.Results.Count > 1 && TboRes.Results[1].Count > 0))
                {
                    int ctr = 0;
                    List<FlightResult> strComb = new List<FlightResult>();
                    strComb = strComb.Union(TboRes.Results != null && TboRes.Results.Count > 1 && TboRes.Results[1].Count > 0 ? TboRes.Results[1] : new List<FlightResult>()).ToList()
                                .Union(TjRes.Results != null && TjRes.Results.Count > 1 && TjRes.Results[1].Count > 0 ? TjRes.Results[1] : new List<FlightResult>()).ToList();
                    var strList = strComb.GroupBy(o => new { o.ResultCombination }).Select(o => o.FirstOrDefault().ResultCombination);
                    List<FlightResult> resutlret = new List<FlightResult>();
                    foreach (string str in strList)
                    {
                        ctr++;
                        try
                        {
                            List<FlightResult> result = strComb.Where(o => o.ResultCombination == str).ToList();
                            FlightResult firstResult = result.FirstOrDefault();
                            firstResult.ResultID = "INB" + ctr.ToString();
                            if (result.Count > 1 && firstResult.gdsType != GdsType.Travelogy)
                            {
                                var tgyResult = result.Where(o => o.gdsType == GdsType.Travelogy).FirstOrDefault();
                                if (tgyResult != null)
                                {
                                    firstResult.tgy_Flight_Key = tgyResult.tgy_Flight_Key;
                                    firstResult.Tgy_Flight_Id = tgyResult.Tgy_Flight_Id;
                                    firstResult.Tgy_Flight_No = tgyResult.Tgy_Flight_No;
                                }
                            }
                            List<Fare> fare = new List<Fare>();
                            foreach (var item in result)
                            {
                                foreach (var flist in item.FareList)
                                {
                                    fare.Add(flist);
                                }
                            }
                            firstResult.Fare = fare.Where(o => o.isBlock == false).OrderBy(o => o.grandTotal).FirstOrDefault();
                            firstResult.FareList = fare;

                            if (firstResult.Fare != null)
                            {
                                resutlret.Add(firstResult);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    OrgResponse.Results.Add(resutlret);
                }
            }
            catch (Exception ex)
            {


            }
        }



        #endregion


        #region searchFunction
        public Task<FlightSearchResponseShort> GetSearchResultTripJack(FlightSearchRequest searchRequest)
        {
            return Task<FlightSearchResponseShort>.Factory.StartNew(() => new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResults(searchRequest));
        }
        //public Task<FlightSearchResponseShort> GetSearchResultTravology(FlightSearchRequest searchRequest)
        //{
        //    return Task<FlightSearchResponseShort>.Factory.StartNew(() => new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightResults(searchRequest));
        //}
        public Task<FlightSearchResponseShort> GetSearchResultOneDFare(FlightSearchRequest searchRequest)
        {
            return Task<FlightSearchResponseShort>.Factory.StartNew(() => new ServicesHub.OneDFare.OneDFareServiceMapping().GetFlightResults(searchRequest));
        }
        public Task<FlightSearchResponseShort> GetSearchResultTbo(FlightSearchRequest searchRequest)
        {
            return Task<FlightSearchResponseShort>.Factory.StartNew(() => new ServicesHub.Tbo.TboServiceMapping().GetFlightResults(searchRequest));
        }
        public Task<FlightSearchResponseShort> GetSearchResultFareBoutique(FlightSearchRequest searchRequest, bool isfareBoutique, bool isfareBoutiqueR)
        {
            return Task<FlightSearchResponseShort>.Factory.StartNew(() => new ServicesHub.FareBoutique.FareBoutiqueServiceMapping().GetFlightResults(searchRequest, isfareBoutique, isfareBoutiqueR));
        }

        public Task<FlightSearchResponseShort> GetSearchResultSatkarTravel(FlightSearchRequest searchRequest, bool isSatkarTravel, bool isSatkarTravelR)
        {
            return Task<FlightSearchResponseShort>.Factory.StartNew(() => new ServicesHub.SatkarTravel.SatkarTravelServiceMapping().GetFlightResults(searchRequest, isSatkarTravel, isSatkarTravelR));
        }


        public Task<FlightSearchResponseShort> GetSearchResultAirIQGDS(FlightSearchRequest searchRequest, bool isAirIQGDS, bool isAirIQGDSR)
        {
            return Task<FlightSearchResponseShort>.Factory.StartNew(() => new ServicesHub.AirIQ.AirIQServiceMapping().GetFlightResults(searchRequest, isAirIQGDS, isAirIQGDSR));
        }

        public Task<FlightSearchResponseShort> GetSearchResultEase2Fly(FlightSearchRequest searchRequest, bool isEase2Fly, bool isEase2FlyR)
        {
            return Task<FlightSearchResponseShort>.Factory.StartNew(() => new ServicesHub.Ease2Fly.Ease2FlyServiceMapping().GetFlightResults(searchRequest, isEase2Fly, isEase2FlyR));
        }

        public Task<FlightSearchResponseShort> GetSearchResultGFS(FlightSearchRequest searchRequest, bool isGFS, bool isGFSR)
        {
            return Task<FlightSearchResponseShort>.Factory.StartNew(() => new ServicesHub.GFS.GFSServiceMapping().GetFlightResults(searchRequest, isGFS, isGFSR));
        }

        public Task<FlightSearchResponseShort> GetSearchResultTravelopedia(FlightSearchRequest searchRequest, bool isTravelopedia, bool isTravelopediaR)
        {
            return Task<FlightSearchResponseShort>.Factory.StartNew(() => new ServicesHub.Travelopedia.TravelopediaServiceMapping().GetFlightResults(searchRequest, isTravelopedia, isTravelopediaR));
        }

        #endregion

        public FlightBookingResponse saveBookingDetails(FlightBookingRequest flightBookingRequest)
        {
            StringBuilder sbLogger = new StringBuilder();
            if (flightBookingRequest != null)
            {
                sbLogger.Append(Environment.NewLine + "---------------------------------------------Booking Original Request---------------------------------------------");
                sbLogger.Append(Environment.NewLine + "-----------------------" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------");
                sbLogger.Append(Environment.NewLine + JsonConvert.SerializeObject(flightBookingRequest));
                sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            }
            try
            {
                flightBookingRequest.bookingID = DAL.IdGenrator.Get("BookingID");
                flightBookingRequest.prodID = 1;
                //flightBookingRequest.transactionID = DAL.IdGenrator.Get("TransactionID");

                DAL.Booking.SaveBookingDetails obj = new DAL.Booking.SaveBookingDetails();
                obj.SaveFlightBookingDetails(flightBookingRequest);
            }
            catch (Exception ex)
            {
                sbLogger.Append(Environment.NewLine + "---------------------------------------------Booking save Error---------------------------------------------");
                sbLogger.Append(Environment.NewLine + "-----------------------" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------");
                sbLogger.Append(Environment.NewLine + ex.ToString());
                sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                //new LogWriter(sbLogger.ToString(), "BookingSaveExeption_" + DateTime.Today.ToString("ddMMyy"), "Booking");
            }

            FlightBookingResponse bookingResponse = new FlightBookingResponse()
            {
                adults = flightBookingRequest.adults,
                bookingID = flightBookingRequest.bookingID,
                child = flightBookingRequest.child,
                infants = flightBookingRequest.infants,
                infantsWs = flightBookingRequest.infantsWs,
                emailID = flightBookingRequest.emailID,
                flightResult = flightBookingRequest.flightResult,
                //bookResult = flightBookingRequest.bookResult,
                PriceID = flightBookingRequest.PriceID,
                mobileNo = flightBookingRequest.mobileNo,
                passengerDetails = flightBookingRequest.passengerDetails,
                paymentDetails = flightBookingRequest.paymentDetails,
                phoneNo = flightBookingRequest.phoneNo,
                prodID = flightBookingRequest.prodID,
                userSearchID = flightBookingRequest.userSearchID,
                userSessionID = flightBookingRequest.userSessionID,
                transactionID = flightBookingRequest.transactionID,
                //currencyCode = flightBookingRequest.currencyCode,
                siteID = flightBookingRequest.siteID,
                sourceMedia = flightBookingRequest.sourceMedia,
                //InsuranceAmount = flightBookingRequest.InsuranceAmount,
                //InsuranceID = flightBookingRequest.InsuranceID,
                //isInsuranceBye = flightBookingRequest.isInsuranceBye,
                //isCancellaionPolicyBye = flightBookingRequest.isCancellaionPolicyBye,
                //CancellaionPolicyAmount = flightBookingRequest.CancellaionPolicyAmount,
                //isTravelAssistanceBye = flightBookingRequest.isTravelAssistanceBye,
                //TravelAssistanceAmount = flightBookingRequest.TravelAssistanceAmount,
                //FlexibleTicketAmount = flightBookingRequest.FlexibleTicketAmount,
                //emailID2 = flightBookingRequest.emailID2,
                //isFlexibleTicket = flightBookingRequest.isFlexibleTicket,
                updatedBookingAmount = 0,
                userIP = flightBookingRequest.userIP,
                PNR = "",
                responseStatus = new ResponseStatus(),
                CouponAmount = flightBookingRequest.CouponAmount,
                CouponCode = flightBookingRequest.CouponCode,
                //RequestedSeat = flightBookingRequest.RequestedSeat
                bookingStatus = BookingStatus.Incomplete,
                paymentStatus = PaymentStatus.PaymentPending
            };
            return bookingResponse;
        }

        public FlightBookingResponse SaveBookingDetails_WithOutPax(FlightBookingRequest flightBookingRequest)
        {
            StringBuilder sbLogger = new StringBuilder();
            if (flightBookingRequest.bookingID == 0)
            {

                string pathFlightSearch = System.IO.Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "NewLog\\Search", flightBookingRequest.userSearchID + ".txt");
                if (System.IO.Directory.Exists(pathFlightSearch))
                {
                    string strFlightSearch = "";
                    using (System.IO.StreamReader r = new System.IO.StreamReader(pathFlightSearch))
                    {
                        strFlightSearch = r.ReadToEnd();
                    }
                    sbLogger.Append(strFlightSearch);
                    sbLogger.Append(Environment.NewLine);
                }
                else
                {
                    try
                    {
                        string strFlightSearch = "";
                        using (System.IO.StreamReader r = new System.IO.StreamReader(pathFlightSearch))
                        {
                            strFlightSearch = r.ReadToEnd();
                        }
                        sbLogger.Append(strFlightSearch);
                        sbLogger.Append(Environment.NewLine);
                    }
                    catch
                    {

                    }
                }
            }
            if (flightBookingRequest != null)
            {
                bookingLog(ref sbLogger, "SaveBookingDetails_WithOutPax Original Request", JsonConvert.SerializeObject(flightBookingRequest));
            }
            try
            {
                if (flightBookingRequest.bookingID == 0)
                {
                    flightBookingRequest.bookingID = DAL.IdGenrator.Get("BookingID");
                }
                flightBookingRequest.prodID = 1;

                DAL.Booking.SaveBookingDetails obj = new DAL.Booking.SaveBookingDetails();
                obj.SaveFlightBookingDetails_WithOutPax(flightBookingRequest);


            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "SaveBookingDetails_WithOutPax Booking save Error", ex.ToString());
                new ServicesHub.LogWriter_New(sbLogger.ToString(), (flightBookingRequest.bookingID > 0 ? flightBookingRequest.bookingID.ToString() : flightBookingRequest.userSearchID), "Error", "SaveBookingDetails_WithOutPax Exeption");
            }

            FlightBookingResponse bookingResponse = new FlightBookingResponse(flightBookingRequest);

            bookingLog(ref sbLogger, "SaveBookingDetails_WithOutPax Original Response", JsonConvert.SerializeObject(bookingResponse));
            new ServicesHub.LogWriter_New(sbLogger.ToString(), (flightBookingRequest.bookingID > 0 ? flightBookingRequest.bookingID.ToString() : flightBookingRequest.userSearchID), "Booking", "");
            return bookingResponse;
        }

        public FlightBookingResponse Update_BookingPaxDetail(FlightBookingRequest flightBookingRequest)
        {
            StringBuilder sbLogger = new StringBuilder();
            if (flightBookingRequest != null)
            {
                bookingLog(ref sbLogger, "Update_BookingPaxDetail Original Request", JsonConvert.SerializeObject(flightBookingRequest));
            }
            try
            {
                DAL.Booking.SaveBookingDetails obj = new DAL.Booking.SaveBookingDetails();
                obj.UpdateBookingPaxDetail(flightBookingRequest);
            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "Update_BookingPaxDetail Booking save Error", ex.ToString());
                new ServicesHub.LogWriter_New(sbLogger.ToString(), (flightBookingRequest.bookingID > 0 ? flightBookingRequest.bookingID.ToString() : flightBookingRequest.userSearchID), "Error", "Update_BookingPaxDetail Exeption");
            }

            FlightBookingResponse bookingResponse = new FlightBookingResponse(flightBookingRequest);

            bookingLog(ref sbLogger, "Update_BookingPaxDetail Original Response", JsonConvert.SerializeObject(bookingResponse));
            new ServicesHub.LogWriter_New(sbLogger.ToString(), (flightBookingRequest.bookingID > 0 ? flightBookingRequest.bookingID.ToString() : flightBookingRequest.userSearchID), "Booking", "");
            return bookingResponse;
        }
        public PriceVerificationResponse TboVerifyThePriceOld(PriceVerificationRequest request)
        {
            PriceVerificationResponse response = new PriceVerificationResponse() { responseStatus = new ResponseStatus() };
            Task<FareQuoteResponse> fareQuote = null;
            Task<List<FareRuleResponses>> fareRule = null;
            if (request.isFareRule) fareRule = getFareRuleOld(request);

            if (request.isFareQuote) fareQuote = getFareQuoteOld(request);

            //System.Threading.Tasks.Task.Delay(10 * 1000).ContinueWith((_) => getFareRule(request));



            List<Task> taskList = new List<Task>();
            if (request.isFareQuote) taskList.Add(fareQuote);
            if (request.isFareRule) taskList.Add(fareRule);

            if (request.isFareQuote || request.isFareRule)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(100); //TODO Reduce timing to under 10 seconds.
                Task.WaitAll(taskList.ToArray(), timeSpan);

                response.fareQuoteResponse = fareQuote.Result;
                response.fareRuleResponse = fareRule.Result;
                response.responseStatus.status = response.fareQuoteResponse.responseStatus.status;
                response.responseStatus.message = response.fareQuoteResponse.responseStatus.message;
            }
            if (request.isSSR && response.responseStatus.status == TransactionStatus.Success)
            {
                GetSsrDetails(request);
            }
            return response;
        }
        public PriceVerificationResponse TboVerifyThePrice(PriceVerificationRequest request)
        {
            PriceVerificationResponse response = new PriceVerificationResponse() { responseStatus = new ResponseStatus() };

            //var fareRule = getFareRule(request);

            //var fareQuote = getFareQuote(request);

            response.fareRuleResponse = getFareRule(request);

            response.fareQuoteResponse = getFareQuote(request);

            response.responseStatus.status = response.fareQuoteResponse.responseStatus.status;
            response.responseStatus.message = response.fareQuoteResponse.responseStatus.message;

            if (request.isSSR && response.responseStatus.status == TransactionStatus.Success)
            {
                GetSsrDetails(request);
            }
            return response;
        }

        public PriceVerificationResponse FareBoutiqueVerifyThePrice(PriceVerificationRequest request)
        {
            PriceVerificationResponse response = new PriceVerificationResponse() { responseStatus = new ResponseStatus() };

            response.fareQuoteResponse = new ServicesHub.FareBoutique.FareBoutiqueServiceMapping().GetFareQuote(request);

            return response;
        }
        public PriceVerificationResponse OneDFareVerifyThePrice(PriceVerificationRequest request)
        {
            PriceVerificationResponse response = new PriceVerificationResponse() { responseStatus = new ResponseStatus() };

            response.fareQuoteResponse = new ServicesHub.OneDFare.OneDFareServiceMapping().GetFareQuote(request);

            return response;
        }

        public PriceVerificationResponse SatkarTravelVerifyThePrice(PriceVerificationRequest request)
        {
            PriceVerificationResponse response = new PriceVerificationResponse() { responseStatus = new ResponseStatus() };
            response.fareQuoteResponse = new ServicesHub.SatkarTravel.SatkarTravelServiceMapping().GetFareQuote(request);
            return response;
        }

        public PriceVerificationResponse AirIQVerifyThePrice(PriceVerificationRequest request)
        {
            PriceVerificationResponse response = new PriceVerificationResponse() { responseStatus = new ResponseStatus() };
            response.fareQuoteResponse = new ServicesHub.AirIQ.AirIQServiceMapping().GetFareQuote(request);
            return response;
        }
        public PriceVerificationResponse AmadeusVerifyThePrice(PriceVerificationRequest request)
        {
            PriceVerificationResponse response = new PriceVerificationResponse() { responseStatus = new ResponseStatus() };
            response.fareQuoteResponse = new FareQuoteResponse() { flightResult = new List<FlightResult>(), isFareChange = false, responseStatus = new ResponseStatus(), fareIncreaseAmount = 0 };
            try
            {
                int ctr = 0;
                foreach (FlightResult fr in request.flightResult)
                {
                    //string strRequest = new AirIQRequestMappking().getFareQuoteRequest(request);

                    //bookingLog(ref sbLogger, "AirIQ FareQuote Request", strRequest);

                    response.fareQuoteResponse.fareIncreaseAmount = 0;
                    response.fareQuoteResponse.VerifiedTotalPrice = request.flightResult[ctr].Fare.PublishedFare;
                    response.fareQuoteResponse.isFareChange = false;
                    ctr++;
                }
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public PriceVerificationResponse E2FVerifyThePrice(PriceVerificationRequest request)
        {
            PriceVerificationResponse response = new PriceVerificationResponse() { responseStatus = new ResponseStatus() };
            response.fareQuoteResponse = new ServicesHub.Ease2Fly.Ease2FlyServiceMapping().GetFareQuote(request);
            return response;
        }

        public PriceVerificationResponse GFSVerifyThePrice(PriceVerificationRequest request)
        {
            PriceVerificationResponse response = new PriceVerificationResponse() { responseStatus = new ResponseStatus() };
            response.fareQuoteResponse = new ServicesHub.GFS.GFSServiceMapping().GetFareQuote(request);
            return response;
        }

        public PriceVerificationResponse TjVerifyThePrice(PriceVerificationRequest request)
        {
            PriceVerificationResponse response = new PriceVerificationResponse() { responseStatus = new ResponseStatus() };
            response.fareQuoteResponse = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(request);
            return response;
        }

        //public PriceVerificationResponse TgyVerifyThePrice(PriceVerificationRequest request)
        //{
        //    PriceVerificationResponse response = new PriceVerificationResponse() { responseStatus = new ResponseStatus() };

        //    response.fareQuoteResponse = new ServicesHub.Travelogy.TravelogyServiceMapping().GetFlightRePrice(request);

        //    //Task<FareQuoteResponse> fareQuote = null;
        //    //Task<List<FareRuleResponses>> fareRule = null;
        //    //if (request.isFareQuote) fareQuote = getFareQuote(request);
        //    //if (request.isFareRule) fareRule = getFareRule(request);
        //    //List<Task> taskList = new List<Task>();
        //    //if (request.isFareQuote) taskList.Add(fareQuote);
        //    //if (request.isFareRule) taskList.Add(fareRule);
        //    //if (request.isFareQuote || request.isFareRule)
        //    //{
        //    //    TimeSpan timeSpan = TimeSpan.FromSeconds(100); //TODO Reduce timing to under 10 seconds.
        //    //    Task.WaitAll(taskList.ToArray(), timeSpan);
        //    //    response.fareQuoteResponse = fareQuote.Result;
        //    //    response.fareRuleResponse = fareRule.Result;
        //    //    response.responseStatus.status = response.fareQuoteResponse.responseStatus.status;
        //    //    response.responseStatus.message = response.fareQuoteResponse.responseStatus.message;
        //    //}
        //    //if (request.isSSR && response.responseStatus.status == TransactionStatus.Success)
        //    //{
        //    //    GetSsrDetails(request);
        //    //}

        //    return response;
        //}

        public PriceVerificationResponse TravelopediaVerifyThePrice(PriceVerificationRequest request)
        {
            PriceVerificationResponse response = new PriceVerificationResponse() { responseStatus = new ResponseStatus() };

            response.fareQuoteResponse = new ServicesHub.Travelopedia.TravelopediaServiceMapping().GetFlightRePrice(request);

            return response;
        }

        public FareQuoteResponse getFareQuote(PriceVerificationRequest request)
        {
            var ServiceMappine = new ServicesHub.Tbo.TboServiceMapping();
            return ServiceMappine.GetFareQuote(request);

            // return response;
        }
        public Task<FareQuoteResponse> getFareQuoteOld(PriceVerificationRequest request)
        {
            var ServiceMappine = new ServicesHub.Tbo.TboServiceMapping();
            Task<FareQuoteResponse> response = Task<FareQuoteResponse>.Factory.StartNew(() => ServiceMappine.GetFareQuote(request));

            return response;
        }
        public List<FareRuleResponses> getFareRule(PriceVerificationRequest request)
        {
            var ServiceMappine = new ServicesHub.Tbo.TboServiceMapping();
            return ServiceMappine.GetFareRule(request);

            // return response;
        }
        public Task<List<FareRuleResponses>> getFareRuleOld(PriceVerificationRequest request)
        {
            var ServiceMappine = new ServicesHub.Tbo.TboServiceMapping();
            Task<List<FareRuleResponses>> response = Task<List<FareRuleResponses>>.Factory.StartNew(() => ServiceMappine.GetFareRule(request));

            return response;
        }
        public bool GetSsrDetails(PriceVerificationRequest request)
        {
            var ServiceMappine = new ServicesHub.Tbo.TboServiceMapping();
            ServiceMappine.GetSsrDetails(request);
            return true;
        }
        public void bookingLog(ref StringBuilder sbLogger, string requestTitle, string logText)
        {
            sbLogger.Append(Environment.NewLine + "---------------------------------------------" + requestTitle + "" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
            sbLogger.Append(Environment.NewLine + logText);
            sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
        }
        private void saveFlightResult(string SearchKey, string ResultData, int ExpireHour)
        {
            var save = Task.Run(async () =>
            {
                await new DALFlightCache().SaveFlightData(SearchKey, ResultData, ExpireHour);
            });
        }



    }
    public class ResponseMapper
    {
        public static Core.Meta.Comman.Flights ConvertMetaResponse(ref FlightSearchRequest flightSearchRequest, ref FlightSearchResponse SearchRes, ref string SearchID, ref string sec1,
            ref string sec2, ref string sec3, ref string sec4, ref string adults, ref string children, ref string infants, ref string cabin, ref string airline, ref string siteid,
            ref string campain, ref string currency)
        {
            StringBuilder sbLogger = new StringBuilder();
            Core.Meta.Comman.Flights flights = new Core.Meta.Comman.Flights();
            flights.flights = new List<Core.Meta.Comman.Flight>();
            Affiliate aff = FlightUtility.GetAffiliate(campain);
            //List<Core.Meta.PaymentFee> paymentFees = getPaymentFee(campain, SearchRes.Results.Count());
            if (SearchRes != null && SearchRes.Results != null && SearchRes.Results.Count() > 0 && SearchRes.Results[0].Count > 0 && SearchRes.Results.LastOrDefault().Count > 0)
            {
                if (SearchRes.Results.Count == 1)
                {
                    #region for one way
                    foreach (FlightResult result in SearchRes.Results[0])
                    {
                        Core.Meta.Comman.Flight flight = new Core.Meta.Comman.Flight();
                        result.Fare.ConvenienceFee = 0.00M;
                        flight.price = result.Fare.grandTotal.ToString("f2");
                        //flight.paymentFees = paymentFees;
                        flight.paymentFees = getPaymentFee(ref aff, result.Fare.grandTotal, SearchRes.Results.Count());
                        flight.currency = result.Fare.Currency;
                        flight.Class = result.cabinClass.ToString();
                        flight.marketingAirline = result.valCarrier;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(flight.price + "_");
                        //sb.Append(result.ResultIndex + "_");

                        if (result.FlightSegments != null && result.FlightSegments.Count > 0)
                        {
                            flight.segment = new List<Core.Meta.Comman.Segment>();

                            foreach (var fSeg in result.FlightSegments)
                            {
                                Core.Meta.Comman.Segment segment = new Core.Meta.Comman.Segment();
                                segment.leg = new List<Core.Meta.Comman.Leg>();
                                foreach (Segment seg in fSeg.Segments)
                                {
                                    Core.Meta.Comman.Leg leg = new Core.Meta.Comman.Leg();
                                    leg.airline = seg.Airline;
                                    leg.operatingAirline = seg.OperatingCarrier;
                                    leg.flightNumber = seg.FlightNumber;
                                    leg.origin = seg.Origin;
                                    leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                    leg.departureTime = seg.DepTime.ToString("HH:mm");
                                    leg.destination = seg.Destination;
                                    leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                    leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                    leg.equipment = seg.equipmentType;
                                    leg.cabin = seg.CabinClass.ToString();
                                    sb.Append(leg.flightNumber + "_");
                                    segment.leg.Add(leg);
                                }
                                flight.segment.Add(segment);
                            }
                        }
                        //result.ResultID = StringHelper.CompressString(sb.ToString());
                        //string kk = StringHelper.DecompressString(result.ResultID);
                        flight.url = "https://www.flightsmojo.in/flight/itinerary?sec1=" + sec1 + "&sec2=" + sec2 + "&adults=" + adults + "&child=" + children + "&infants=" + infants + "&cabin=" + cabin + "&airline=" + airline + "&siteid=" + siteid + "&campain=" + campain + "&currency=" + result.Fare.Currency + "&TranId=" + SearchID + "&Id=" + sb.ToString();

                        flights.flights.Add(flight);
                    }
                    #endregion
                }
                else
                {
                    #region Make Combination SpecialReturn
                    List<FlightResult> depSpecialReturn = SearchRes.Results[0].Where(k => k.Fare.FareType == FareType.SPECIALRETURN).ToList();
                    List<FlightResult> retSpecialReturn = SearchRes.Results[1].Where(k => k.Fare.FareType == FareType.SPECIALRETURN).ToList();
                    if (depSpecialReturn != null && depSpecialReturn.Count > 0 && retSpecialReturn != null && retSpecialReturn.Count > 0)
                    {
                        for (int i = 0; i < depSpecialReturn.Count; i++)
                        {
                            List<FlightResult> retSpecialReturnMatch = retSpecialReturn.Where(k => k.Fare.msri.Contains(depSpecialReturn[i].Fare.sri)).ToList();
                            for (int j = 0; j < retSpecialReturnMatch.Count; j++)
                            {

                                TimeSpan ts = retSpecialReturnMatch[j].FlightSegments[0].Segments[0].DepTime - depSpecialReturn[i].FlightSegments[0].Segments.Last().ArrTime;
                                if (ts.TotalMinutes > 240)
                                {
                                    Core.Meta.Comman.Flight flight = new Core.Meta.Comman.Flight();
                                    flight.paymentFees = getPaymentFee(ref aff, depSpecialReturn[i].Fare.grandTotal, SearchRes.Results.Count());
                                    flight.price = (depSpecialReturn[i].Fare.grandTotal + retSpecialReturnMatch[j].Fare.grandTotal).ToString("f2");
                                    flight.currency = depSpecialReturn[i].Fare.Currency;
                                    flight.Class = depSpecialReturn[i].cabinClass.ToString();
                                    flight.marketingAirline = depSpecialReturn[i].valCarrier;
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append(flight.price + "_");
                                    //sb.Append(result.ResultIndex + "_");

                                    if (depSpecialReturn[i].FlightSegments != null && depSpecialReturn[i].FlightSegments.Count > 0 && retSpecialReturnMatch[j].FlightSegments != null && retSpecialReturnMatch[j].FlightSegments.Count > 0)
                                    {
                                        flight.segment = new List<Core.Meta.Comman.Segment>();
                                        foreach (var fSeg in depSpecialReturn[i].FlightSegments)
                                        {
                                            Core.Meta.Comman.Segment segment = new Core.Meta.Comman.Segment();
                                            segment.leg = new List<Core.Meta.Comman.Leg>();
                                            foreach (Segment seg in fSeg.Segments)
                                            {
                                                Core.Meta.Comman.Leg leg = new Core.Meta.Comman.Leg();
                                                leg.airline = seg.Airline;
                                                leg.operatingAirline = seg.OperatingCarrier;
                                                leg.flightNumber = seg.FlightNumber;
                                                leg.origin = seg.Origin;
                                                leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                                leg.departureTime = seg.DepTime.ToString("HH:mm");
                                                leg.destination = seg.Destination;
                                                leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                                leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                                leg.equipment = seg.equipmentType;
                                                leg.cabin = seg.CabinClass.ToString();
                                                sb.Append(leg.flightNumber + "_");
                                                segment.leg.Add(leg);
                                            }
                                            flight.segment.Add(segment);
                                        }
                                        foreach (var fSeg in retSpecialReturnMatch[j].FlightSegments)
                                        {
                                            Core.Meta.Comman.Segment segment = new Core.Meta.Comman.Segment();
                                            segment.leg = new List<Core.Meta.Comman.Leg>();
                                            foreach (Segment seg in fSeg.Segments)
                                            {
                                                Core.Meta.Comman.Leg leg = new Core.Meta.Comman.Leg();
                                                leg.airline = seg.Airline;
                                                leg.operatingAirline = seg.OperatingCarrier;
                                                leg.flightNumber = seg.FlightNumber;
                                                leg.origin = seg.Origin;
                                                leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                                leg.departureTime = seg.DepTime.ToString("HH:mm");
                                                leg.destination = seg.Destination;
                                                leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                                leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                                leg.equipment = seg.equipmentType;
                                                leg.cabin = seg.CabinClass.ToString();
                                                sb.Append(leg.flightNumber + "_");
                                                segment.leg.Add(leg);
                                            }
                                            flight.segment.Add(segment);
                                        }
                                    }
                                    //result.ResultID = StringHelper.CompressString(sb.ToString());
                                    //string kk = StringHelper.DecompressString(result.ResultID);
                                    flight.url = "https://www.flightsmojo.in/flight/itinerary?sec1=" + sec1 + "&sec2=" + sec2 + "&adults=" + adults + "&child=" + children + "&infants=" + infants + "&cabin=" + cabin + "&airline=" + airline + "&siteid=" + siteid + "&campain=" + campain + "&currency=" + depSpecialReturn[i].Fare.Currency + "&TranId=" + SearchID + "&Id=" + sb.ToString();

                                    flights.flights.Add(flight);
                                }
                            }
                        }
                    }
                    #endregion
                    #region Make Combination top 10
                    List<FlightResult> dep = SearchRes.Results[0].Where(k => k.Fare.FareType != FareType.SPECIALRETURN).ToList();
                    List<FlightResult> ret = SearchRes.Results[1].Where(k => k.Fare.FareType != FareType.SPECIALRETURN).ToList();
                    for (int i = 0; i < dep.Count && i < 10; i++)
                    {
                        for (int j = 0; j < ret.Count && j < 10; j++)
                        {
                            TimeSpan ts = ret[j].FlightSegments[0].Segments[0].DepTime - dep[i].FlightSegments[0].Segments.Last().ArrTime;
                            if (ts.TotalMinutes > 240)
                            {
                                Core.Meta.Comman.Flight flight = new Core.Meta.Comman.Flight();
                                flight.paymentFees = getPaymentFee(ref aff, dep[i].Fare.grandTotal, SearchRes.Results.Count());
                                flight.price = (dep[i].Fare.grandTotal + ret[j].Fare.grandTotal).ToString("f2");
                                flight.currency = dep[i].Fare.Currency;
                                flight.Class = dep[i].cabinClass.ToString();
                                flight.marketingAirline = dep[i].valCarrier;
                                StringBuilder sb = new StringBuilder();
                                sb.Append(flight.price + "_");
                                //sb.Append(result.ResultIndex + "_");

                                if (dep[i].FlightSegments != null && dep[i].FlightSegments.Count > 0 && ret[j].FlightSegments != null && ret[j].FlightSegments.Count > 0)
                                {
                                    flight.segment = new List<Core.Meta.Comman.Segment>();
                                    foreach (var fSeg in dep[i].FlightSegments)
                                    {
                                        Core.Meta.Comman.Segment segment = new Core.Meta.Comman.Segment();
                                        segment.leg = new List<Core.Meta.Comman.Leg>();
                                        foreach (Segment seg in fSeg.Segments)
                                        {
                                            Core.Meta.Comman.Leg leg = new Core.Meta.Comman.Leg();
                                            leg.airline = seg.Airline;
                                            leg.operatingAirline = seg.OperatingCarrier;
                                            leg.flightNumber = seg.FlightNumber;
                                            leg.origin = seg.Origin;
                                            leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                            leg.departureTime = seg.DepTime.ToString("HH:mm");
                                            leg.destination = seg.Destination;
                                            leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                            leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                            leg.equipment = seg.equipmentType;
                                            leg.cabin = seg.CabinClass.ToString();
                                            sb.Append(leg.flightNumber + "_");
                                            segment.leg.Add(leg);
                                        }
                                        flight.segment.Add(segment);
                                    }
                                    foreach (var fSeg in ret[j].FlightSegments)
                                    {
                                        Core.Meta.Comman.Segment segment = new Core.Meta.Comman.Segment();
                                        segment.leg = new List<Core.Meta.Comman.Leg>();
                                        foreach (Segment seg in fSeg.Segments)
                                        {
                                            Core.Meta.Comman.Leg leg = new Core.Meta.Comman.Leg();
                                            leg.airline = seg.Airline;
                                            leg.operatingAirline = seg.OperatingCarrier;
                                            leg.flightNumber = seg.FlightNumber;
                                            leg.origin = seg.Origin;
                                            leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                            leg.departureTime = seg.DepTime.ToString("HH:mm");
                                            leg.destination = seg.Destination;
                                            leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                            leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                            leg.equipment = seg.equipmentType;
                                            leg.cabin = seg.CabinClass.ToString();
                                            sb.Append(leg.flightNumber + "_");
                                            segment.leg.Add(leg);
                                        }
                                        flight.segment.Add(segment);
                                    }
                                }
                                //result.ResultID = StringHelper.CompressString(sb.ToString());
                                //string kk = StringHelper.DecompressString(result.ResultID);
                                flight.url = "https://www.flightsmojo.in/flight/itinerary?sec1=" + sec1 + "&sec2=" + sec2 + "&adults=" + adults + "&child=" + children + "&infants=" + infants + "&cabin=" + cabin + "&airline=" + airline + "&siteid=" + siteid + "&campain=" + campain + "&currency=" + dep[i].Fare.Currency + "&TranId=" + SearchID + "&Id=" + sb.ToString();

                                flights.flights.Add(flight);
                            }
                        }
                    }
                    #endregion
                    #region Make Combination after 10
                    for (int i = 10; i < dep.Count && i < 30; i++)
                    {
                        for (int j = 10; j < ret.Count && j < 30; j++)
                        {
                            TimeSpan ts = ret[j].FlightSegments[0].Segments[0].DepTime - dep[i].FlightSegments[0].Segments.Last().ArrTime;
                            if (ts.TotalMinutes > 240)
                            {
                                Core.Meta.Comman.Flight flight = new Core.Meta.Comman.Flight();
                                flight.paymentFees = getPaymentFee(ref aff, dep[i].Fare.grandTotal, SearchRes.Results.Count());
                                flight.price = (dep[i].Fare.grandTotal + ret[j].Fare.grandTotal).ToString("f2");
                                flight.currency = dep[i].Fare.Currency;
                                flight.Class = dep[i].cabinClass.ToString();
                                flight.marketingAirline = dep[i].valCarrier;
                                StringBuilder sb = new StringBuilder();
                                sb.Append(flight.price + "_");
                                //sb.Append(result.ResultIndex + "_");

                                if (dep[i].FlightSegments != null && dep[i].FlightSegments.Count > 0 && ret[j].FlightSegments != null && ret[j].FlightSegments.Count > 0)
                                {
                                    flight.segment = new List<Core.Meta.Comman.Segment>();
                                    foreach (var fSeg in dep[i].FlightSegments)
                                    {
                                        Core.Meta.Comman.Segment segment = new Core.Meta.Comman.Segment();
                                        segment.leg = new List<Core.Meta.Comman.Leg>();
                                        foreach (Segment seg in fSeg.Segments)
                                        {
                                            Core.Meta.Comman.Leg leg = new Core.Meta.Comman.Leg();
                                            leg.airline = seg.Airline;
                                            leg.operatingAirline = seg.OperatingCarrier;
                                            leg.flightNumber = seg.FlightNumber;
                                            leg.origin = seg.Origin;
                                            leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                            leg.departureTime = seg.DepTime.ToString("HH:mm");
                                            leg.destination = seg.Destination;
                                            leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                            leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                            leg.equipment = seg.equipmentType;
                                            leg.cabin = seg.CabinClass.ToString();
                                            sb.Append(leg.flightNumber + "_");
                                            segment.leg.Add(leg);
                                        }
                                        flight.segment.Add(segment);
                                    }
                                    foreach (var fSeg in ret[j].FlightSegments)
                                    {
                                        Core.Meta.Comman.Segment segment = new Core.Meta.Comman.Segment();
                                        segment.leg = new List<Core.Meta.Comman.Leg>();
                                        foreach (Segment seg in fSeg.Segments)
                                        {
                                            Core.Meta.Comman.Leg leg = new Core.Meta.Comman.Leg();
                                            leg.airline = seg.Airline;
                                            leg.operatingAirline = seg.OperatingCarrier;
                                            leg.flightNumber = seg.FlightNumber;
                                            leg.origin = seg.Origin;
                                            leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                            leg.departureTime = seg.DepTime.ToString("HH:mm");
                                            leg.destination = seg.Destination;
                                            leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                            leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                            leg.equipment = seg.equipmentType;
                                            leg.cabin = seg.CabinClass.ToString();
                                            sb.Append(leg.flightNumber + "_");
                                            segment.leg.Add(leg);
                                        }
                                        flight.segment.Add(segment);
                                    }
                                }
                                //result.ResultID = StringHelper.CompressString(sb.ToString());
                                //string kk = StringHelper.DecompressString(result.ResultID);
                                flight.url = "https://www.flightsmojo.in/flight/itinerary?sec1=" + sec1 + "&sec2=" + sec2 + "&adults=" + adults + "&child=" + children + "&infants=" + infants + "&cabin=" + cabin + "&airline=" + airline + "&siteid=" + siteid + "&campain=" + campain + "&currency=" + dep[i].Fare.Currency + "&TranId=" + SearchID + "&Id=" + sb.ToString();

                                flights.flights.Add(flight);


                            }
                        }
                    }
                    #endregion
                }
            }
            return flights;

        }

        public static Core.Meta.Skyscanner.Flights ConvertSkyscannerResponse(ref FlightSearchRequest flightSearchRequest, ref FlightSearchResponse SearchRes, ref string SearchID, ref string sec1,
        ref string sec2, ref string sec3, ref string sec4, ref string adults, ref string children, ref string infants, ref string cabin, ref string airline, ref string siteid,
        ref string campain, ref string currency)
        {

            Core.Meta.Skyscanner.Flights flights = new Core.Meta.Skyscanner.Flights();
            flights.flights = new List<Core.Meta.Skyscanner.Flight>();
            //List<Core.Meta.PaymentFee> paymentFees = getPaymentFee(campain, SearchRes.Results.Count());
            Affiliate aff = FlightUtility.GetAffiliate(campain);
            if (SearchRes != null && SearchRes.Results != null && SearchRes.Results.Count() > 0 && SearchRes.Results[0].Count > 0 && SearchRes.Results.LastOrDefault().Count > 0)
            {
                if (SearchRes.Results.Count == 1)
                {
                    #region for one way
                    foreach (FlightResult result in SearchRes.Results[0])
                    {
                        Core.Meta.Skyscanner.Flight flight = new Core.Meta.Skyscanner.Flight();
                        result.Fare.ConvenienceFee = 0.00M;
                        flight.paymentFees = getPaymentFee(ref aff, result.Fare.grandTotal, SearchRes.Results.Count());
                        flight.totalPrice = (result.Fare.grandTotal + result.Fare.ConvenienceFee).ToString("f2");
                        flight.price = result.Fare.grandTotal.ToString("f2");
                        flight.ConvenienceFee = result.Fare.ConvenienceFee.ToString("f2");

                        flight.currency = result.Fare.Currency;
                        flight.Class = result.cabinClass.ToString();
                        flight.marketingAirline = result.valCarrier;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(flight.price + "_");
                        //sb.Append(result.ResultIndex + "_");

                        if (result.FlightSegments != null && result.FlightSegments.Count > 0)
                        {
                            flight.segment = new List<Core.Meta.Skyscanner.Segment>();

                            foreach (var fSeg in result.FlightSegments)
                            {
                                Core.Meta.Skyscanner.Segment segment = new Core.Meta.Skyscanner.Segment();
                                segment.leg = new List<Core.Meta.Skyscanner.Leg>();
                                foreach (Segment seg in fSeg.Segments)
                                {
                                    Core.Meta.Skyscanner.Leg leg = new Core.Meta.Skyscanner.Leg();
                                    leg.airline = seg.Airline;
                                    leg.operatingAirline = seg.OperatingCarrier;
                                    leg.flightNumber = seg.FlightNumber;
                                    leg.origin = seg.Origin;
                                    leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                    leg.departureTime = seg.DepTime.ToString("HH:mm");
                                    leg.destination = seg.Destination;
                                    leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                    leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                    leg.equipment = seg.equipmentType;
                                    leg.cabin = seg.CabinClass.ToString();
                                    sb.Append(leg.flightNumber + "_");
                                    segment.leg.Add(leg);
                                }
                                flight.segment.Add(segment);
                            }
                        }
                        //result.ResultID = StringHelper.CompressString(sb.ToString());
                        //string kk = StringHelper.DecompressString(result.ResultID);
                        flight.url = "https://www.flightsmojo.in/flight/itinerary?sec1=" + sec1 + "&sec2=" + sec2 + "&adults=" + adults + "&child=" + children + "&infants=" + infants + "&cabin=" + cabin + "&airline=" + airline + "&siteid=" + siteid + "&campain=" + campain + "&currency=" + result.Fare.Currency + "&TranId=" + SearchID + "&Id=" + sb.ToString() + "&tt=" + flightSearchRequest.tripType;

                        flights.flights.Add(flight);
                    }
                    #endregion
                }
                else
                {
                    #region Make Combination Special Return
                    List<FlightResult> depSpecialReturn = SearchRes.Results[0].Where(k => k.Fare.FareType == FareType.SPECIALRETURN).ToList();
                    List<FlightResult> retSpecialReturn = SearchRes.Results[1].Where(k => k.Fare.FareType == FareType.SPECIALRETURN).ToList();
                    if (depSpecialReturn != null && depSpecialReturn.Count > 0 && retSpecialReturn != null && retSpecialReturn.Count > 0)
                    {
                        for (int i = 0; i < depSpecialReturn.Count; i++)
                        {
                            List<FlightResult> retSpecialReturnMatch = retSpecialReturn.Where(k => k.Fare.msri.Contains(depSpecialReturn[i].Fare.sri)).ToList();
                            for (int j = 0; j < retSpecialReturnMatch.Count; j++)
                            {
                                TimeSpan ts = retSpecialReturnMatch[j].FlightSegments[0].Segments[0].DepTime - depSpecialReturn[i].FlightSegments[0].Segments.Last().ArrTime;
                                if (ts.TotalMinutes > 240)
                                {
                                    Core.Meta.Skyscanner.Flight flight = new Core.Meta.Skyscanner.Flight();
                                    flight.paymentFees = getPaymentFee(ref aff, depSpecialReturn[i].Fare.grandTotal, SearchRes.Results.Count());
                                    //flight.paymentFees = paymentFees;
                                    flight.price = (depSpecialReturn[i].Fare.grandTotal + retSpecialReturnMatch[j].Fare.grandTotal).ToString("f2");
                                    flight.totalPrice = (depSpecialReturn[i].Fare.grandTotal + retSpecialReturnMatch[j].Fare.grandTotal + depSpecialReturn[i].Fare.ConvenienceFee + retSpecialReturnMatch[j].Fare.ConvenienceFee).ToString("f2");
                                    flight.ConvenienceFee = (depSpecialReturn[i].Fare.ConvenienceFee + retSpecialReturnMatch[j].Fare.ConvenienceFee).ToString("f2");

                                    flight.currency = depSpecialReturn[i].Fare.Currency;
                                    flight.Class = depSpecialReturn[i].cabinClass.ToString();
                                    flight.marketingAirline = depSpecialReturn[i].valCarrier;
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append(flight.price + "_");
                                    //sb.Append(result.ResultIndex + "_");

                                    if (depSpecialReturn[i].FlightSegments != null && depSpecialReturn[i].FlightSegments.Count > 0 && retSpecialReturnMatch[j].FlightSegments != null && retSpecialReturnMatch[j].FlightSegments.Count > 0)
                                    {
                                        flight.segment = new List<Core.Meta.Skyscanner.Segment>();
                                        foreach (var fSeg in depSpecialReturn[i].FlightSegments)
                                        {
                                            Core.Meta.Skyscanner.Segment segment = new Core.Meta.Skyscanner.Segment();
                                            segment.leg = new List<Core.Meta.Skyscanner.Leg>();
                                            foreach (Segment seg in fSeg.Segments)
                                            {
                                                Core.Meta.Skyscanner.Leg leg = new Core.Meta.Skyscanner.Leg();
                                                leg.airline = seg.Airline;
                                                leg.operatingAirline = seg.OperatingCarrier;
                                                leg.flightNumber = seg.FlightNumber;
                                                leg.origin = seg.Origin;
                                                leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                                leg.departureTime = seg.DepTime.ToString("HH:mm");
                                                leg.destination = seg.Destination;
                                                leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                                leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                                leg.equipment = seg.equipmentType;
                                                leg.cabin = seg.CabinClass.ToString();
                                                sb.Append(leg.flightNumber + "_");
                                                segment.leg.Add(leg);
                                            }
                                            flight.segment.Add(segment);
                                        }
                                        foreach (var fSeg in retSpecialReturnMatch[j].FlightSegments)
                                        {
                                            Core.Meta.Skyscanner.Segment segment = new Core.Meta.Skyscanner.Segment();
                                            segment.leg = new List<Core.Meta.Skyscanner.Leg>();
                                            foreach (Segment seg in fSeg.Segments)
                                            {
                                                Core.Meta.Skyscanner.Leg leg = new Core.Meta.Skyscanner.Leg();
                                                leg.airline = seg.Airline;
                                                leg.operatingAirline = seg.OperatingCarrier;
                                                leg.flightNumber = seg.FlightNumber;
                                                leg.origin = seg.Origin;
                                                leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                                leg.departureTime = seg.DepTime.ToString("HH:mm");
                                                leg.destination = seg.Destination;
                                                leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                                leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                                leg.equipment = seg.equipmentType;
                                                leg.cabin = seg.CabinClass.ToString();
                                                sb.Append(leg.flightNumber + "_");
                                                segment.leg.Add(leg);
                                            }
                                            flight.segment.Add(segment);
                                        }
                                    }
                                    //result.ResultID = StringHelper.CompressString(sb.ToString());
                                    //string kk = StringHelper.DecompressString(result.ResultID);
                                    flight.url = "https://www.flightsmojo.in/flight/itinerary?sec1=" + sec1 + "&sec2=" + sec2 + "&adults=" + adults + "&child=" + children + "&infants=" + infants + "&cabin=" + cabin + "&airline=" + airline + "&siteid=" + siteid + "&campain=" + campain + "&currency=" + depSpecialReturn[i].Fare.Currency + "&TranId=" + SearchID + "&Id=" + sb.ToString() + "&tt=" + flightSearchRequest.tripType;

                                    flights.flights.Add(flight);
                                }
                            }
                        }
                    }
                    #endregion
                    #region Make Combination top 10
                    List<FlightResult> dep = SearchRes.Results[0].Where(k => k.Fare.FareType != FareType.SPECIALRETURN).ToList();
                    List<FlightResult> ret = SearchRes.Results[1].Where(k => k.Fare.FareType != FareType.SPECIALRETURN).ToList();
                    for (int i = 0; i < dep.Count && i < 10; i++)
                    {
                        for (int j = 0; j < ret.Count && j < 10; j++)
                        {
                            TimeSpan ts = ret[j].FlightSegments[0].Segments[0].DepTime - dep[i].FlightSegments[0].Segments.Last().ArrTime;
                            if (ts.TotalMinutes > 240)
                            {
                                Core.Meta.Skyscanner.Flight flight = new Core.Meta.Skyscanner.Flight();

                                //flight.paymentFees = paymentFees;
                                flight.paymentFees = getPaymentFee(ref aff, dep[i].Fare.grandTotal, SearchRes.Results.Count());
                                flight.price = (dep[i].Fare.grandTotal + ret[j].Fare.grandTotal).ToString("f2");
                                flight.totalPrice = (dep[i].Fare.grandTotal + ret[j].Fare.grandTotal + dep[i].Fare.ConvenienceFee + ret[j].Fare.ConvenienceFee).ToString("f2");
                                flight.ConvenienceFee = (dep[i].Fare.ConvenienceFee + ret[j].Fare.ConvenienceFee).ToString("f2");

                                flight.currency = dep[i].Fare.Currency;
                                flight.Class = dep[i].cabinClass.ToString();
                                flight.marketingAirline = dep[i].valCarrier;
                                StringBuilder sb = new StringBuilder();
                                sb.Append(flight.price + "_");
                                //sb.Append(result.ResultIndex + "_");

                                if (dep[i].FlightSegments != null && dep[i].FlightSegments.Count > 0 && ret[j].FlightSegments != null && ret[j].FlightSegments.Count > 0)
                                {
                                    flight.segment = new List<Core.Meta.Skyscanner.Segment>();
                                    foreach (var fSeg in dep[i].FlightSegments)
                                    {
                                        Core.Meta.Skyscanner.Segment segment = new Core.Meta.Skyscanner.Segment();
                                        segment.leg = new List<Core.Meta.Skyscanner.Leg>();
                                        foreach (Segment seg in fSeg.Segments)
                                        {
                                            Core.Meta.Skyscanner.Leg leg = new Core.Meta.Skyscanner.Leg();
                                            leg.airline = seg.Airline;
                                            leg.operatingAirline = seg.OperatingCarrier;
                                            leg.flightNumber = seg.FlightNumber;
                                            leg.origin = seg.Origin;
                                            leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                            leg.departureTime = seg.DepTime.ToString("HH:mm");
                                            leg.destination = seg.Destination;
                                            leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                            leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                            leg.equipment = seg.equipmentType;
                                            leg.cabin = seg.CabinClass.ToString();
                                            sb.Append(leg.flightNumber + "_");
                                            segment.leg.Add(leg);
                                        }
                                        flight.segment.Add(segment);
                                    }
                                    foreach (var fSeg in ret[j].FlightSegments)
                                    {
                                        Core.Meta.Skyscanner.Segment segment = new Core.Meta.Skyscanner.Segment();
                                        segment.leg = new List<Core.Meta.Skyscanner.Leg>();
                                        foreach (Segment seg in fSeg.Segments)
                                        {
                                            Core.Meta.Skyscanner.Leg leg = new Core.Meta.Skyscanner.Leg();
                                            leg.airline = seg.Airline;
                                            leg.operatingAirline = seg.OperatingCarrier;
                                            leg.flightNumber = seg.FlightNumber;
                                            leg.origin = seg.Origin;
                                            leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                            leg.departureTime = seg.DepTime.ToString("HH:mm");
                                            leg.destination = seg.Destination;
                                            leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                            leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                            leg.equipment = seg.equipmentType;
                                            leg.cabin = seg.CabinClass.ToString();
                                            sb.Append(leg.flightNumber + "_");
                                            segment.leg.Add(leg);
                                        }
                                        flight.segment.Add(segment);
                                    }
                                }
                                //result.ResultID = StringHelper.CompressString(sb.ToString());
                                //string kk = StringHelper.DecompressString(result.ResultID);
                                flight.url = "https://www.flightsmojo.in/flight/itinerary?sec1=" + sec1 + "&sec2=" + sec2 + "&adults=" + adults + "&child=" + children + "&infants=" + infants + "&cabin=" + cabin + "&airline=" + airline + "&siteid=" + siteid + "&campain=" + campain + "&currency=" + dep[i].Fare.Currency + "&TranId=" + SearchID + "&Id=" + sb.ToString() + "&tt=" + flightSearchRequest.tripType;

                                flights.flights.Add(flight);
                            }
                        }
                    }
                    #endregion
                    #region Make Combination after 10
                    for (int i = 10; i < dep.Count && i < 30; i++)
                    {
                        for (int j = 10; j < ret.Count && j < 30; j++)
                        {
                            TimeSpan ts = ret[j].FlightSegments[0].Segments[0].DepTime - dep[i].FlightSegments[0].Segments.Last().ArrTime;
                            if (ts.TotalMinutes > 240)
                            {
                                Core.Meta.Skyscanner.Flight flight = new Core.Meta.Skyscanner.Flight();

                                // flight.paymentFees = paymentFees;
                                flight.paymentFees = getPaymentFee(ref aff, dep[i].Fare.grandTotal, SearchRes.Results.Count());
                                flight.price = (dep[i].Fare.grandTotal + ret[j].Fare.grandTotal).ToString("f2");
                                flight.totalPrice = (dep[i].Fare.grandTotal + ret[j].Fare.grandTotal + dep[i].Fare.ConvenienceFee + ret[j].Fare.ConvenienceFee).ToString("f2");
                                flight.ConvenienceFee = (dep[i].Fare.ConvenienceFee + ret[j].Fare.ConvenienceFee).ToString("f2");

                                flight.currency = dep[i].Fare.Currency;
                                flight.Class = dep[i].cabinClass.ToString();
                                flight.marketingAirline = dep[i].valCarrier;
                                StringBuilder sb = new StringBuilder();
                                sb.Append(flight.price + "_");
                                //sb.Append(result.ResultIndex + "_");

                                if (dep[i].FlightSegments != null && dep[i].FlightSegments.Count > 0 && ret[j].FlightSegments != null && ret[j].FlightSegments.Count > 0)
                                {
                                    flight.segment = new List<Core.Meta.Skyscanner.Segment>();
                                    foreach (var fSeg in dep[i].FlightSegments)
                                    {
                                        Core.Meta.Skyscanner.Segment segment = new Core.Meta.Skyscanner.Segment();
                                        segment.leg = new List<Core.Meta.Skyscanner.Leg>();
                                        foreach (Segment seg in fSeg.Segments)
                                        {
                                            Core.Meta.Skyscanner.Leg leg = new Core.Meta.Skyscanner.Leg();
                                            leg.airline = seg.Airline;
                                            leg.operatingAirline = seg.OperatingCarrier;
                                            leg.flightNumber = seg.FlightNumber;
                                            leg.origin = seg.Origin;
                                            leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                            leg.departureTime = seg.DepTime.ToString("HH:mm");
                                            leg.destination = seg.Destination;
                                            leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                            leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                            leg.equipment = seg.equipmentType;
                                            leg.cabin = seg.CabinClass.ToString();
                                            sb.Append(leg.flightNumber + "_");
                                            segment.leg.Add(leg);
                                        }
                                        flight.segment.Add(segment);
                                    }
                                    foreach (var fSeg in ret[j].FlightSegments)
                                    {
                                        Core.Meta.Skyscanner.Segment segment = new Core.Meta.Skyscanner.Segment();
                                        segment.leg = new List<Core.Meta.Skyscanner.Leg>();
                                        foreach (Segment seg in fSeg.Segments)
                                        {
                                            Core.Meta.Skyscanner.Leg leg = new Core.Meta.Skyscanner.Leg();
                                            leg.airline = seg.Airline;
                                            leg.operatingAirline = seg.OperatingCarrier;
                                            leg.flightNumber = seg.FlightNumber;
                                            leg.origin = seg.Origin;
                                            leg.departureDate = seg.DepTime.ToString("yyyy-MM-dd");
                                            leg.departureTime = seg.DepTime.ToString("HH:mm");
                                            leg.destination = seg.Destination;
                                            leg.arrivalDate = seg.ArrTime.ToString("yyyy-MM-dd");
                                            leg.arrivalTime = seg.ArrTime.ToString("HH:mm");
                                            leg.equipment = seg.equipmentType;
                                            leg.cabin = seg.CabinClass.ToString();
                                            sb.Append(leg.flightNumber + "_");
                                            segment.leg.Add(leg);
                                        }
                                        flight.segment.Add(segment);
                                    }
                                }
                                //result.ResultID = StringHelper.CompressString(sb.ToString());
                                //string kk = StringHelper.DecompressString(result.ResultID);
                                flight.url = "https://www.flightsmojo.in/flight/itinerary?sec1=" + sec1 + "&sec2=" + sec2 + "&adults=" + adults + "&child=" + children + "&infants=" + infants + "&cabin=" + cabin + "&airline=" + airline + "&siteid=" + siteid + "&campain=" + campain + "&currency=" + dep[i].Fare.Currency + "&TranId=" + SearchID + "&Id=" + sb.ToString() + "&tt=" + flightSearchRequest.tripType;

                                flights.flights.Add(flight);
                            }
                        }
                    }
                    #endregion
                }
            }
            return flights;
        }

        public bool checkResultIsExist(ref FlightSearchResponse response, string resultReference)
        {
            bool retVal = false;
            //string resultUniqID = StringHelper.DecompressString(resultReference);
            if (response != null && response.Results != null && response.Results.Count() > 0 && response.Results[0].Count > 0 && response.Results.LastOrDefault().Count > 0)
            {
                if (response.Results.Count == 1)
                {
                    foreach (FlightResult result in response.Results[0])
                    {
                        if (!retVal)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(result.Fare.grandTotal.ToString("f2") + "_");

                            if (result.FlightSegments != null && result.FlightSegments.Count > 0)
                            {
                                foreach (var fSeg in result.FlightSegments)
                                {
                                    foreach (Segment seg in fSeg.Segments)
                                    {
                                        sb.Append(seg.FlightNumber + "_");
                                    }
                                }
                            }
                            if (resultReference.Equals(sb.ToString(), StringComparison.OrdinalIgnoreCase))
                            {
                                response.result1Index = result.ResultID;
                                retVal = true;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < response.Results[0].Count; i++)
                    {
                        for (int j = 0; j < response.Results[1].Count; j++)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append((response.Results[0][i].Fare.grandTotal + response.Results[1][j].Fare.grandTotal).ToString("f2") + "_");

                            if (response.Results[0][i].FlightSegments != null && response.Results[0][i].FlightSegments.Count > 0)
                            {
                                foreach (var fSeg in response.Results[0][i].FlightSegments)
                                {
                                    foreach (Segment seg in fSeg.Segments)
                                    {
                                        sb.Append(seg.FlightNumber + "_");
                                    }
                                }
                            }
                            if (response.Results[1][j].FlightSegments != null && response.Results[1][j].FlightSegments.Count > 0)
                            {
                                foreach (var fSeg in response.Results[1][j].FlightSegments)
                                {
                                    foreach (Segment seg in fSeg.Segments)
                                    {
                                        sb.Append(seg.FlightNumber + "_");
                                    }
                                }
                            }
                            if (resultReference.Equals(sb.ToString(), StringComparison.OrdinalIgnoreCase))
                            {
                                response.result1Index = response.Results[0][i].ResultID;
                                response.result2Index = response.Results[1][j].ResultID;
                                retVal = true;
                            }
                        }
                    }
                }
            }
            return retVal;
        }

        private static List<Core.Meta.PaymentFee> getPaymentFee(string sourceMedia, int tripCount)
        {
            List<Core.Meta.PaymentFee> payment = new List<Core.Meta.PaymentFee>();
            if (sourceMedia == "1013")
            {

                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "VI", CardName = "Visa Credit" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "AX", CardName = "American Express" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "MC", CardName = "Mastercard Credit" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "ME", CardName = "Maestro" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "VD", CardName = "Visa Debit" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "VE", CardName = "Visa Electron" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "MD", CardName = "Mastercard Debit" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "PTM", CardName = "Paytm" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "MW", CardName = "Mobile Wallet" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "UPI", CardName = "UPI" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = "0.00", FeeCode = "NB", CardName = "Net Banking" });
            }
            else if (sourceMedia == "1001")
            {
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "VI", CardName = "Visa Credit" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "AX", CardName = "American Express" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "MC", CardName = "Mastercard Credit" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "ME", CardName = "Maestro" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "VD", CardName = "Visa Debit" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "VE", CardName = "Visa Electron" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "MD", CardName = "Mastercard Debit" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "PTM", CardName = "Paytm" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "MW", CardName = "Mobile Wallet" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 0).ToString(), FeeCode = "UPI", CardName = "UPI" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = (tripCount * 300).ToString(), FeeCode = "NB", CardName = "Net Banking" });
            }
            else
            {
                payment.Add(new Core.Meta.PaymentFee() { Amount = "0.00", FeeCode = "VI", CardName = "Visa Credit" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = "0.00", FeeCode = "AX", CardName = "American Express" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = "0.00", FeeCode = "MC", CardName = "Mastercard Credit" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = "0.00", FeeCode = "ME", CardName = "Maestro" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = "0.00", FeeCode = "VD", CardName = "Visa Debit" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = "0.00", FeeCode = "VE", CardName = "Visa Electron" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = "0.00", FeeCode = "MD", CardName = "Mastercard Debit" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = "0.00", FeeCode = "PTM", CardName = "Paytm" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = "0.00", FeeCode = "MW", CardName = "Mobile Wallet" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = "0.00", FeeCode = "UPI", CardName = "UPI" });
                payment.Add(new Core.Meta.PaymentFee() { Amount = "0.00", FeeCode = "NB", CardName = "Net Banking" });

            }

            return payment;
        }
        private static List<Core.Meta.PaymentFee> getPaymentFee(ref Affiliate aff, decimal amount, int tripCount)
        {
            List<Core.Meta.PaymentFee> payment = new List<Core.Meta.PaymentFee>();


            payment.Add(new Core.Meta.PaymentFee() { Amount = ((aff.CardConFee.IndexOf("%") != -1 ? ((Convert.ToDecimal(aff.CardConFee.Replace("%", "")) * amount) / 100) : (Convert.ToDecimal(aff.CardConFee) * tripCount))).ToString(), FeeCode = "VI", CardName = "Visa Credit" });
            payment.Add(new Core.Meta.PaymentFee() { Amount = ((aff.CardConFee.IndexOf("%") != -1 ? ((Convert.ToDecimal(aff.CardConFee.Replace("%", "")) * amount) / 100) : (Convert.ToDecimal(aff.CardConFee) * tripCount))).ToString(), FeeCode = "AX", CardName = "American Express" });
            payment.Add(new Core.Meta.PaymentFee() { Amount = ((aff.CardConFee.IndexOf("%") != -1 ? ((Convert.ToDecimal(aff.CardConFee.Replace("%", "")) * amount) / 100) : (Convert.ToDecimal(aff.CardConFee) * tripCount))).ToString(), FeeCode = "MC", CardName = "Mastercard Credit" });
            payment.Add(new Core.Meta.PaymentFee() { Amount = ((aff.CardConFee.IndexOf("%") != -1 ? ((Convert.ToDecimal(aff.CardConFee.Replace("%", "")) * amount) / 100) : (Convert.ToDecimal(aff.CardConFee) * tripCount))).ToString(), FeeCode = "ME", CardName = "Maestro" });
            payment.Add(new Core.Meta.PaymentFee() { Amount = ((aff.CardConFee.IndexOf("%") != -1 ? ((Convert.ToDecimal(aff.CardConFee.Replace("%", "")) * amount) / 100) : (Convert.ToDecimal(aff.CardConFee) * tripCount))).ToString(), FeeCode = "VD", CardName = "Visa Debit" });
            payment.Add(new Core.Meta.PaymentFee() { Amount = ((aff.CardConFee.IndexOf("%") != -1 ? ((Convert.ToDecimal(aff.CardConFee.Replace("%", "")) * amount) / 100) : (Convert.ToDecimal(aff.CardConFee) * tripCount))).ToString(), FeeCode = "VE", CardName = "Visa Electron" });
            payment.Add(new Core.Meta.PaymentFee() { Amount = ((aff.CardConFee.IndexOf("%") != -1 ? ((Convert.ToDecimal(aff.CardConFee.Replace("%", "")) * amount) / 100) : (Convert.ToDecimal(aff.CardConFee) * tripCount))).ToString(), FeeCode = "MD", CardName = "Mastercard Debit" });
            payment.Add(new Core.Meta.PaymentFee() { Amount = ((aff.UPIConFee.IndexOf("%") != -1 ? ((Convert.ToDecimal(aff.UPIConFee.Replace("%", "")) * amount) / 100) : (Convert.ToDecimal(aff.UPIConFee) * tripCount))).ToString(), FeeCode = "PTM", CardName = "Paytm" });
            payment.Add(new Core.Meta.PaymentFee() { Amount = ((aff.WalletConFee.IndexOf("%") != -1 ? ((Convert.ToDecimal(aff.WalletConFee.Replace("%", "")) * amount) / 100) : (Convert.ToDecimal(aff.WalletConFee) * tripCount))).ToString(), FeeCode = "MW", CardName = "Mobile Wallet" });
            payment.Add(new Core.Meta.PaymentFee() { Amount = ((aff.UPIConFee.IndexOf("%") != -1 ? ((Convert.ToDecimal(aff.UPIConFee.Replace("%", "")) * amount) / 100) : (Convert.ToDecimal(aff.UPIConFee) * tripCount))).ToString(), FeeCode = "UPI", CardName = "UPI" });
            payment.Add(new Core.Meta.PaymentFee() { Amount = ((aff.NetBankingConFee.IndexOf("%") != -1 ? ((Convert.ToDecimal(aff.NetBankingConFee.Replace("%", "")) * amount) / 100) : (Convert.ToDecimal(aff.NetBankingConFee) * tripCount))).ToString(), FeeCode = "NB", CardName = "Net Banking" });


            return payment;
        }
    }
    public class CacheRedis
    {
        public void setResult(string SearchID, string data)
        {
            //string ss = string.Empty;
            try
            {
                string clusterEndpoint = "clustercfg.mymojo.z6gs6g.aps1.cache.amazonaws.com:6379";
                ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(clusterEndpoint);
                IDatabase cache = redisConnection.GetDatabase();

                if (!cache.KeyExists(SearchID))
                {
                    cache.StringSet(SearchID, data, TimeSpan.FromHours(6));
                    //ss += "CatchValSet:" + Environment.NewLine;
                }
                redisConnection.Close();
                //  new ServicesHub.LogWriter_New(data, SearchID, "Cache", "SetCacheRedis");
            }
            catch (Exception ex)
            {
                new ServicesHub.LogWriter_New(ex.ToString(), SearchID, "Cache", "ExeptionSetCacheRedis");
            }
        }
        public string getResult(string SearchID)
        {
            //string ss = string.Empty;
            try
            {
                //   new ServicesHub.LogWriter_New(DateTime.Now.ToString(), SearchID + "1_", "Cache", "SetCacheRedis1");
                string clusterEndpoint = "clustercfg.mymojo.z6gs6g.aps1.cache.amazonaws.com:6379";
                ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(clusterEndpoint);
                IDatabase cache = redisConnection.GetDatabase();
                if (cache.KeyExists(SearchID))
                {
                    //     new ServicesHub.LogWriter_New(DateTime.Now.ToString(), SearchID + "2_", "Cache", "SetCacheRedis2");
                    return cache.StringGet(SearchID);

                }
                redisConnection.Close();
                // new ServicesHub.LogWriter_New(DateTime.Now.ToString(), SearchID + "3_", "Cache", "SetCacheRedis3");
                //new ServicesHub.LogWriter_New(data, SearchID, "Cache", "SetCacheRedis");
                return "";

            }
            catch (Exception ex)
            {
                new ServicesHub.LogWriter_New(ex.ToString(), SearchID, "Cache", "ExeptionGetCacheRedis");
                return "";
            }
        }
    }
}
