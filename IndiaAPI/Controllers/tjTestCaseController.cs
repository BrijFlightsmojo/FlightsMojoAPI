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
    [RoutePrefix("tjTestCase")]
    public class tjTestCaseController : ApiController
    {
        [HttpGet]
        [Route("test1")]
        public HttpResponseMessage test1(int d = 100)
        {
            var kk = new ServicesHub.TripJack.TripJackServiceMapping().BookFlight1();

            return Request.CreateResponse(HttpStatusCode.OK, kk);

        }
        [HttpGet]
        [Route("Case1")]
        public HttpResponseMessage Case1(int d = 1)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 1;
            fsr.child = 0;
            fsr.infants = 0;
            fsr.tripType = Core.TripType.OneWay;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();
            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "DEL",
                orgArp = Core.FlightUtility.GetAirport("DEL"),
                destinationAirport = "BOM",
                destArp = Core.FlightUtility.GetAirport("BOM"),
                travelDate = DateTime.Today.AddDays(30).AddDays(d)//Convert.ToDateTime("2023-01-01").AddDays(d) //
            });


            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = true;
            string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count == 1 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }

            if (selectResut != null)
            {

                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                //if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                //{
                //    pvReq.flightResult.Add(response.Results[1][0]);
                //}
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

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
                    if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }

                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq);
                    StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }
        [HttpGet]
        [Route("Case2")]
        public HttpResponseMessage Case2(int d = 2)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 1;
            fsr.child = 0;
            fsr.infants = 0;
            fsr.tripType = Core.TripType.OneWay;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();
            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "DEL",
                orgArp = Core.FlightUtility.GetAirport("DEL"),
                destinationAirport = "BOM",
                destArp = Core.FlightUtility.GetAirport("BOM"),
                travelDate = DateTime.Today.AddDays(30).AddDays(d)//Convert.ToDateTime("2023-01-01").AddDays(d) //
            });


            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = false; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count == 2 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }

            if (selectResut != null)
            {

                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                //if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                //{
                //    pvReq.flightResult.Add(response.Results[1][0]);
                //}
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

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
                    if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }

                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }
        [HttpGet]
        [Route("Case3")]
        public HttpResponseMessage Case3(int d = 3)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 2;
            fsr.child = 2;
            fsr.infants = 0;
            fsr.tripType = Core.TripType.OneWay;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();
            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "DEL",
                orgArp = Core.FlightUtility.GetAirport("DEL"),
                destinationAirport = "DXB",
                destArp = Core.FlightUtility.GetAirport("DXB"),
                travelDate = DateTime.Today.AddDays(30).AddDays(d)//Convert.ToDateTime("2023-01-01").AddDays(d) //
            });


            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = true; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count == 1 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }

            if (selectResut != null)
            {

                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                //if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                //{
                //    pvReq.flightResult.Add(response.Results[1][0]);
                //}
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

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
                    if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }

                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }
        [HttpGet]
        [Route("Case4")]
        public HttpResponseMessage Case4(int d = 4)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 2;
            fsr.child = 2;
            fsr.infants = 0;
            fsr.tripType = Core.TripType.OneWay;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();
            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "BOM",
                orgArp = Core.FlightUtility.GetAirport("BOM"),
                destinationAirport = "SIN",
                destArp = Core.FlightUtility.GetAirport("SIN"),
                travelDate = DateTime.Today.AddDays(30).AddDays(d)//Convert.ToDateTime("2023-01-01").AddDays(d) //
            });


            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = false; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count == 2 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }

            if (selectResut != null)
            {

                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                //if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                //{
                //    pvReq.flightResult.Add(response.Results[1][0]);
                //}
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

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
                    if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }

                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [HttpGet]
        [Route("Case5")]
        public HttpResponseMessage Case5(int d = 5)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 3;
            fsr.child = 2;
            fsr.infants = 0;
            fsr.tripType = Core.TripType.OneWay;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();
            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "MAA",
                orgArp = Core.FlightUtility.GetAirport("MAA"),
                destinationAirport = "DMK",
                destArp = Core.FlightUtility.GetAirport("DMK"),
                travelDate = DateTime.Today.AddDays(30).AddDays(d)//Convert.ToDateTime("2023-01-01").AddDays(d) //
            });


            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = true; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);

            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count == 1 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }

            if (selectResut != null)
            {

                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                //if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                //{
                //    pvReq.flightResult.Add(response.Results[1][0]);
                //}
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [HttpGet]
        [Route("Case6")]
        public HttpResponseMessage Case6(int d = 6)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 5;
            fsr.child = 3;
            fsr.infants = 2;
            fsr.tripType = Core.TripType.OneWay;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();
            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "DXB",
                orgArp = Core.FlightUtility.GetAirport("DXB"),
                destinationAirport = "BKK",
                destArp = Core.FlightUtility.GetAirport("BKK"),
                travelDate = DateTime.Today.AddDays(30).AddDays(d)//Convert.ToDateTime("2023-01-01").AddDays(d) //
            });


            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = true; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count == 1 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }

            if (selectResut != null)
            {

                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                //if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                //{
                //    pvReq.flightResult.Add(response.Results[1][0]);
                //}
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Child 1
                    if (bookingReq.child >= 1)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8).AddDays(11);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
                    if (bookingReq.child >= 2)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(11)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 3
                    if (bookingReq.child >= 3)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "ROHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 4
                    if (bookingReq.child >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "KOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-18)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 5
                    if (bookingReq.child >= 5)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "JOHON",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-101)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 6
                    if (bookingReq.child >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-111)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 2
                    if (bookingReq.infants >= 2)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "KANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 1
                    if (bookingReq.infants >= 3)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "SANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(-71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [HttpGet]
        [Route("Case7")]
        public HttpResponseMessage Case7(int d = 7)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 5;
            fsr.child = 3;
            fsr.infants = 2;
            fsr.tripType = Core.TripType.OneWay;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();
            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "DXB",
                orgArp = Core.FlightUtility.GetAirport("DXB"),
                destinationAirport = "BKK",
                destArp = Core.FlightUtility.GetAirport("BKK"),
                travelDate = DateTime.Today.AddDays(30).AddDays(d)//Convert.ToDateTime("2023-01-01").AddDays(d) //
            });


            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = false; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count >= 2 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }

            if (selectResut != null)
            {

                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                //if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                //{
                //    pvReq.flightResult.Add(response.Results[1][0]);
                //}
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Child 1
                    if (bookingReq.child >= 1)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8).AddDays(11);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
                    if (bookingReq.child >= 2)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(11)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 3
                    if (bookingReq.child >= 3)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "ROHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 4
                    if (bookingReq.child >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "KOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-18)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 5
                    if (bookingReq.child >= 5)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "JOHON",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-101)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 6
                    if (bookingReq.child >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-111)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 2
                    if (bookingReq.infants >= 2)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "KANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 1
                    if (bookingReq.infants >= 3)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "SANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(-71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [HttpGet]
        [Route("Case8")]
        public HttpResponseMessage Case8(int d = 8)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 5;
            fsr.child = 4;
            fsr.infants = 3;
            fsr.tripType = Core.TripType.OneWay;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();
            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "BOM",
                orgArp = Core.FlightUtility.GetAirport("BOM"),
                destinationAirport = "MAA",
                destArp = Core.FlightUtility.GetAirport("MAA"),
                travelDate = DateTime.Today.AddDays(d)// Convert.ToDateTime("2023-01-01").AddDays(d) //
            });


            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = true; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);

            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count == 1 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }

            if (selectResut != null)
            {

                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                //if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                //{
                //    pvReq.flightResult.Add(response.Results[1][0]);
                //}
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Child 1
                    if (bookingReq.child >= 1)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8).AddDays(11);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
                    if (bookingReq.child >= 2)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(11)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 3
                    if (bookingReq.child >= 3)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "ROHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 4
                    if (bookingReq.child >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "KOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-18)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 5
                    if (bookingReq.child >= 5)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "JOHON",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-101)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 6
                    if (bookingReq.child >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-111)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 2
                    if (bookingReq.infants >= 2)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "KANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 1
                    if (bookingReq.infants >= 3)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "SANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(-71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [HttpGet]
        [Route("ReturnCase1")]
        public HttpResponseMessage ReturnCase1(int d = 9)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 1;
            fsr.child = 0;
            fsr.infants = 0;
            fsr.tripType = Core.TripType.RoundTrip;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();

            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "DEL",
                orgArp = Core.FlightUtility.GetAirport("DEL"),
                destinationAirport = "BOM",
                destArp = Core.FlightUtility.GetAirport("BOM"),
                travelDate = DateTime.Today.AddDays(d)// Convert.ToDateTime("2023-01-01").AddDays(d) //
            });
            if (fsr.tripType == Core.TripType.RoundTrip)
            {
                fsr.segment.Add(new SearchSegment()
                {
                    originAirport = "BOM",
                    orgArp = Core.FlightUtility.GetAirport("BOM"),
                    destinationAirport = "DEL",
                    destArp = Core.FlightUtility.GetAirport("DEL"),
                    travelDate = DateTime.Today.AddDays(d + 7)
                });
            }

            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = true; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            FlightResult selectReturn = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count == 1 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            foreach (FlightResult item in response.Results[1])
            {
                if (item.FlightSegments[0].Segments.Count == 1 && selectReturn == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectReturn == null)
                        {
                            selectReturn = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            if (selectResut != null)
            {
                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                {
                    pvReq.flightResult.Add(selectReturn);
                }
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Child 1
                    if (bookingReq.child >= 1)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8).AddDays(11);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
                    if (bookingReq.child >= 2)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(11)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 3
                    if (bookingReq.child >= 3)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "ROHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 4
                    if (bookingReq.child >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "KOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-18)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 5
                    if (bookingReq.child >= 5)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "JOHON",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-101)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 6
                    if (bookingReq.child >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-111)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 2
                    if (bookingReq.infants >= 2)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "KANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 1
                    if (bookingReq.infants >= 3)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "SANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(-71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }
        [HttpGet]
        [Route("ReturnCase2")]
        public HttpResponseMessage ReturnCase2(int d = 10)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 1;
            fsr.child = 0;
            fsr.infants = 0;
            fsr.tripType = Core.TripType.RoundTrip;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();

            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "DEL",
                orgArp = Core.FlightUtility.GetAirport("DEL"),
                destinationAirport = "BOM",
                destArp = Core.FlightUtility.GetAirport("BOM"),
                travelDate = DateTime.Today.AddDays(d)// Convert.ToDateTime("2023-01-01").AddDays(d) //
            });
            if (fsr.tripType == Core.TripType.RoundTrip)
            {
                fsr.segment.Add(new SearchSegment()
                {
                    originAirport = "BOM",
                    orgArp = Core.FlightUtility.GetAirport("BOM"),
                    destinationAirport = "DEL",
                    destArp = Core.FlightUtility.GetAirport("DEL"),
                    travelDate = DateTime.Today.AddDays(d + 7)
                });
            }

            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = false; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            FlightResult selectReturn = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count > 1 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            foreach (FlightResult item in response.Results[1])
            {
                if (item.FlightSegments[0].Segments.Count > 1 && selectReturn == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectReturn == null)
                        {
                            selectReturn = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            if (selectResut != null)
            {
                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                {
                    pvReq.flightResult.Add(selectReturn);
                }
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Child 1
                    if (bookingReq.child >= 1)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8).AddDays(11);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
                    if (bookingReq.child >= 2)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(11)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 3
                    if (bookingReq.child >= 3)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "ROHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 4
                    if (bookingReq.child >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "KOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-18)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 5
                    if (bookingReq.child >= 5)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "JOHON",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-101)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 6
                    if (bookingReq.child >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-111)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 2
                    if (bookingReq.infants >= 2)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "KANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 1
                    if (bookingReq.infants >= 3)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "SANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(-71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }
        [HttpGet]
        [Route("ReturnCase3")]
        public HttpResponseMessage ReturnCase3(int d = 11)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 2;
            fsr.child = 2;
            fsr.infants = 0;
            fsr.tripType = Core.TripType.RoundTrip;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();

            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "DEL",
                orgArp = Core.FlightUtility.GetAirport("DEL"),
                destinationAirport = "DXB",
                destArp = Core.FlightUtility.GetAirport("DXB"),
                travelDate = DateTime.Today.AddDays(d)// Convert.ToDateTime("2023-01-01").AddDays(d) //
            });
            if (fsr.tripType == Core.TripType.RoundTrip)
            {
                fsr.segment.Add(new SearchSegment()
                {
                    originAirport = "DXB",
                    orgArp = Core.FlightUtility.GetAirport("DXB"),
                    destinationAirport = "DEL",
                    destArp = Core.FlightUtility.GetAirport("DEL"),
                    travelDate = DateTime.Today.AddDays(d + 7)
                });
            }

            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = true; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            FlightResult selectReturn = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count == 2 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
            {
                foreach (FlightResult item in response.Results[1])
                {
                    if (item.FlightSegments[0].Segments.Count == 1 && selectReturn == null)
                    {
                        foreach (var price in item.FareList)
                        {
                            if (price.FareType == FareType.PUBLISH && selectReturn == null)
                            {
                                selectReturn = item;
                                PriceID.Add(price.tjID);
                            }
                        }
                    }
                }
            }
            if (selectResut != null)
            {
                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                {
                    pvReq.flightResult.Add(selectReturn);
                }
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Child 1
                    if (bookingReq.child >= 1)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8).AddDays(11);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
                    if (bookingReq.child >= 2)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(11)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 3
                    if (bookingReq.child >= 3)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "ROHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 4
                    if (bookingReq.child >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "KOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-18)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 5
                    if (bookingReq.child >= 5)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "JOHON",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-101)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 6
                    if (bookingReq.child >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-111)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 2
                    if (bookingReq.infants >= 2)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "KANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 1
                    if (bookingReq.infants >= 3)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "SANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(-71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }
        [HttpGet]
        [Route("ReturnCase4")]
        public HttpResponseMessage ReturnCase4(int d = 20)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 2;
            fsr.child = 2;
            fsr.infants = 0;
            fsr.tripType = Core.TripType.RoundTrip;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();

            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "BOM",
                orgArp = Core.FlightUtility.GetAirport("BOM"),
                destinationAirport = "SIN",
                destArp = Core.FlightUtility.GetAirport("SIN"),
                travelDate = DateTime.Today.AddDays(d)// Convert.ToDateTime("2023-01-01").AddDays(d) //
            });
            if (fsr.tripType == Core.TripType.RoundTrip)
            {
                fsr.segment.Add(new SearchSegment()
                {
                    originAirport = "SIN",
                    orgArp = Core.FlightUtility.GetAirport("SIN"),
                    destinationAirport = "BOM",
                    destArp = Core.FlightUtility.GetAirport("BOM"),
                    travelDate = DateTime.Today.AddDays(d + 7)
                });
            }

            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = false; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            FlightResult selectReturn = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count > 2 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
            {
                foreach (FlightResult item in response.Results[1])
                {
                    if (item.FlightSegments[0].Segments.Count == 1 && selectReturn == null)
                    {
                        foreach (var price in item.FareList)
                        {
                            if (price.FareType == FareType.PUBLISH && selectReturn == null)
                            {
                                selectReturn = item;
                                PriceID.Add(price.tjID);
                            }
                        }
                    }
                }
            }
            if (selectResut != null)
            {
                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                {
                    pvReq.flightResult.Add(selectReturn);
                }
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Child 1
                    if (bookingReq.child >= 1)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8).AddDays(11);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
                    if (bookingReq.child >= 2)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(11)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 3
                    if (bookingReq.child >= 3)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "ROHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 4
                    if (bookingReq.child >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "KOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-18)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 5
                    if (bookingReq.child >= 5)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "JOHON",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-101)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 6
                    if (bookingReq.child >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-111)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 2
                    if (bookingReq.infants >= 2)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "KANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 1
                    if (bookingReq.infants >= 3)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "SANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(-71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [HttpGet]
        [Route("ReturnCase5")]
        public HttpResponseMessage ReturnCase5(int d = 21)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 3;
            fsr.child = 2;
            fsr.infants = 0;
            fsr.tripType = Core.TripType.RoundTrip;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();

            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "MAA",
                orgArp = Core.FlightUtility.GetAirport("MAA"),
                destinationAirport = "DMK",
                destArp = Core.FlightUtility.GetAirport("DMK"),
                travelDate = DateTime.Today.AddDays(d)// Convert.ToDateTime("2023-01-01").AddDays(d) //
            });
            if (fsr.tripType == Core.TripType.RoundTrip)
            {
                fsr.segment.Add(new SearchSegment()
                {
                    originAirport = "DMK",
                    orgArp = Core.FlightUtility.GetAirport("DMK"),
                    destinationAirport = "MAA",
                    destArp = Core.FlightUtility.GetAirport("MAA"),
                    travelDate = DateTime.Today.AddDays(d + 7)
                });
            }

            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = true; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            FlightResult selectReturn = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count == 2 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
            {
                foreach (FlightResult item in response.Results[1])
                {
                    if (item.FlightSegments[0].Segments.Count == 1 && selectReturn == null)
                    {
                        foreach (var price in item.FareList)
                        {
                            if (price.FareType == FareType.PUBLISH && selectReturn == null)
                            {
                                selectReturn = item;
                                PriceID.Add(price.tjID);
                            }
                        }
                    }
                }
            }
            if (selectResut != null)
            {
                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                {
                    pvReq.flightResult.Add(selectReturn);
                }
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Child 1
                    if (bookingReq.child >= 1)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8).AddDays(11);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
                    if (bookingReq.child >= 2)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(11)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 3
                    if (bookingReq.child >= 3)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "ROHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 4
                    if (bookingReq.child >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "KOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-18)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 5
                    if (bookingReq.child >= 5)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "JOHON",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-101)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 6
                    if (bookingReq.child >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-111)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 2
                    if (bookingReq.infants >= 2)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "KANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 1
                    if (bookingReq.infants >= 3)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "SANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(-71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [HttpGet]
        [Route("ReturnCase6")]
        public HttpResponseMessage ReturnCase6(int d = 22)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 5;
            fsr.child = 3;
            fsr.infants = 2;
            fsr.tripType = Core.TripType.RoundTrip;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();

            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "DXB",
                orgArp = Core.FlightUtility.GetAirport("DXB"),
                destinationAirport = "BKK",
                destArp = Core.FlightUtility.GetAirport("BKK"),
                travelDate = DateTime.Today.AddDays(d)// Convert.ToDateTime("2023-01-01").AddDays(d) //
            });
            if (fsr.tripType == Core.TripType.RoundTrip)
            {
                fsr.segment.Add(new SearchSegment()
                {
                    originAirport = "BKK",
                    orgArp = Core.FlightUtility.GetAirport("BKK"),
                    destinationAirport = "DXB",
                    destArp = Core.FlightUtility.GetAirport("DXB"),
                    travelDate = DateTime.Today.AddDays(d + 7)
                });
            }

            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = true; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            FlightResult selectReturn = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count == 2 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
            {
                foreach (FlightResult item in response.Results[1])
                {
                    if (item.FlightSegments[0].Segments.Count == 1 && selectReturn == null)
                    {
                        foreach (var price in item.FareList)
                        {
                            if (price.FareType == FareType.PUBLISH && selectReturn == null)
                            {
                                selectReturn = item;
                                PriceID.Add(price.tjID);
                            }
                        }
                    }
                }
            }
            if (selectResut != null)
            {
                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                {
                    pvReq.flightResult.Add(selectReturn);
                }
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Child 1
                    if (bookingReq.child >= 1)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8).AddDays(11);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
                    if (bookingReq.child >= 2)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(11)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 3
                    if (bookingReq.child >= 3)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "ROHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 4
                    if (bookingReq.child >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "KOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-18)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 5
                    if (bookingReq.child >= 5)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "JOHON",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-101)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 6
                    if (bookingReq.child >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-111)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 2
                    if (bookingReq.infants >= 2)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "KANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 1
                    if (bookingReq.infants >= 3)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "SANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(-71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [HttpGet]
        [Route("ReturnCase7")]
        public HttpResponseMessage ReturnCase7(int d = 23)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 5;
            fsr.child = 3;
            fsr.infants = 2;
            fsr.tripType = Core.TripType.RoundTrip;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();

            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "DXB",
                orgArp = Core.FlightUtility.GetAirport("DXB"),
                destinationAirport = "BKK",
                destArp = Core.FlightUtility.GetAirport("BKK"),
                travelDate = DateTime.Today.AddDays(d)// Convert.ToDateTime("2023-01-01").AddDays(d) //
            });
            if (fsr.tripType == Core.TripType.RoundTrip)
            {
                fsr.segment.Add(new SearchSegment()
                {
                    originAirport = "BKK",
                    orgArp = Core.FlightUtility.GetAirport("BKK"),
                    destinationAirport = "DXB",
                    destArp = Core.FlightUtility.GetAirport("DXB"),
                    travelDate = DateTime.Today.AddDays(d + 7)
                });
            }

            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = false; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            FlightResult selectReturn = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count > 2 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
            {
                foreach (FlightResult item in response.Results[1])
                {
                    if (item.FlightSegments[0].Segments.Count == 1 && selectReturn == null)
                    {
                        foreach (var price in item.FareList)
                        {
                            if (price.FareType == FareType.PUBLISH && selectReturn == null)
                            {
                                selectReturn = item;
                                PriceID.Add(price.tjID);
                            }
                        }
                    }
                }
            }
            if (selectResut != null)
            {
                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                {
                    pvReq.flightResult.Add(selectReturn);
                }
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Child 1
                    if (bookingReq.child >= 1)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8).AddDays(11);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
                    if (bookingReq.child >= 2)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(11)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 3
                    if (bookingReq.child >= 3)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "ROHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 4
                    if (bookingReq.child >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "KOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-18)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 5
                    if (bookingReq.child >= 5)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "JOHON",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-101)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 6
                    if (bookingReq.child >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-111)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 2
                    if (bookingReq.infants >= 2)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "KANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 1
                    if (bookingReq.infants >= 3)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "SANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(-71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [HttpGet]
        [Route("ReturnCase8")]
        public HttpResponseMessage ReturnCase8(int d = 30)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 3;
            fsr.child = 2;
            fsr.infants = 0;
            fsr.tripType = Core.TripType.RoundTrip;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();

            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "DEL",
                orgArp = Core.FlightUtility.GetAirport("DEL"),
                destinationAirport = "BOM",
                destArp = Core.FlightUtility.GetAirport("BOM"),
                travelDate = DateTime.Today.AddDays(d)// Convert.ToDateTime("2023-01-01").AddDays(d) //
            });
            if (fsr.tripType == Core.TripType.RoundTrip)
            {
                fsr.segment.Add(new SearchSegment()
                {
                    originAirport = "BOM",
                    orgArp = Core.FlightUtility.GetAirport("BOM"),
                    destinationAirport = "DEL",
                    destArp = Core.FlightUtility.GetAirport("DEL"),
                    travelDate = DateTime.Today.AddDays(d + 7)
                });
            }

            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = true; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            FlightResult selectReturn = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count == 1 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            foreach (FlightResult item in response.Results[1])
            {
                if (item.FlightSegments[0].Segments.Count == 1 && selectReturn == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectReturn == null)
                        {
                            selectReturn = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            if (selectResut != null)
            {
                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                {
                    pvReq.flightResult.Add(selectReturn);
                }
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Child 1
                    if (bookingReq.child >= 1)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8).AddDays(11);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
                    if (bookingReq.child >= 2)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(11)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 3
                    if (bookingReq.child >= 3)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "ROHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 4
                    if (bookingReq.child >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "KOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-18)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 5
                    if (bookingReq.child >= 5)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "JOHON",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-101)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 6
                    if (bookingReq.child >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-111)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 2
                    if (bookingReq.infants >= 2)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "KANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 1
                    if (bookingReq.infants >= 3)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "SANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(-71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }

        [HttpGet]
        [Route("ReturnCase9")]
        public HttpResponseMessage ReturnCase9(int d = 31)
        {
            FlightSearchRequest fsr = new FlightSearchRequest();
            //fsr.Sources = new List<string>();
            fsr.adults = 5;
            fsr.child = 3;
            fsr.infants = 2;
            fsr.tripType = Core.TripType.RoundTrip;
            fsr.searchDirectFlight = false;
            fsr.userIP = "180.151.226.194";
            //fsr.OneStopFlight = false;
            //fsr.PreferredAirlines = new List<string>();

            //fsr.PreferredAirlines.Add("All");
            fsr.segment = new List<SearchSegment>();

            fsr.segment.Add(new SearchSegment()
            {
                originAirport = "DEL",
                orgArp = Core.FlightUtility.GetAirport("DEL"),
                destinationAirport = "BOM",
                destArp = Core.FlightUtility.GetAirport("BOM"),
                travelDate = DateTime.Today.AddDays(d)// Convert.ToDateTime("2023-01-01").AddDays(d) //
            });
            if (fsr.tripType == Core.TripType.RoundTrip)
            {
                fsr.segment.Add(new SearchSegment()
                {
                    originAirport = "BOM",
                    orgArp = Core.FlightUtility.GetAirport("BOM"),
                    destinationAirport = "DEL",
                    destArp = Core.FlightUtility.GetAirport("DEL"),
                    travelDate = DateTime.Today.AddDays(d + 7)
                });
            }

            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);
            fsr.siteId = SiteId.FlightsMojoIN;
            fsr.sourceMedia = "1000";
            fsr.userSearchID = DateTime.Now.ToString("ddMMMyy_HHmm");
            fsr.searchDirectFlight = false; string FolderName = fsr.segment[0].originAirport + "-" + fsr.segment[0].destinationAirport + "-" + fsr.adults + "A-" + (fsr.child > 0 ? (fsr.child + "C-") : "") + (fsr.infants > 0 ? (fsr.infants + "C-") : "") + (fsr.searchDirectFlight ? "Direct" : "Connecting");
            FlightSearchResponse response = new FlightSearchResponse(fsr);
            new ServicesHub.TripJack.TripJackServiceMapping().GetFlightResult(fsr, ref response);
            FlightResult selectResut = null;
            FlightResult selectReturn = null;
            List<string> PriceID = new List<string>();
            foreach (FlightResult item in response.Results[0])
            {
                if (item.FlightSegments[0].Segments.Count > 1 && selectResut == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectResut == null)
                        {
                            selectResut = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            foreach (FlightResult item in response.Results[1])
            {
                if (item.FlightSegments[0].Segments.Count >= 1 && selectReturn == null)
                {
                    foreach (var price in item.FareList)
                    {
                        if (price.FareType == FareType.PUBLISH && selectReturn == null)
                        {
                            selectReturn = item;
                            PriceID.Add(price.tjID);
                        }
                    }
                }
            }
            if (selectResut != null)
            {
                PriceVerificationRequest pvReq = new PriceVerificationRequest()
                {
                    adults = fsr.adults,
                    child = fsr.child,
                    infants = fsr.infants,
                    infantsWs = 0,
                    flightResult = new List<FlightResult>(),
                    isFareQuote = true,
                    isFareRule = false,
                    isSSR = false,
                    siteID = fsr.siteId,
                    sourceMedia = fsr.sourceMedia,
                    userIP = fsr.userIP,
                    userSearchID = fsr.userSearchID,
                    userSessionID = fsr.userSessionID,
                    PriceID = PriceID
                };

                pvReq.flightResult.Add(selectResut);
                if (fsr.tripType == TripType.RoundTrip && response.Results.Count > 1)
                {
                    pvReq.flightResult.Add(selectReturn);
                }
                var kk = new ServicesHub.TripJack.TripJackServiceMapping().GetFlightReview(pvReq);
                if (kk.responseStatus.status == TransactionStatus.Success)
                {
                    FlightBookingRequest bookingReq = new FlightBookingRequest()
                    {
                        AdminID = 100,
                        adults = fsr.adults,
                        aircraftDetail = new List<AircraftDetail>(),
                        airline = new List<Airline>(),
                        airport = new List<Airport>(),
                        bookingID = 0,
                        BrowserDetails = "",
                        child = fsr.child,
                        infants = fsr.infants,
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
                        userSearchID = fsr.userSearchID,
                        userSessionID = fsr.userSessionID,
                        TvoTraceId = response.TraceId,
                        TjBookingID = kk.TjBookingID,
                        PriceID = pvReq.PriceID,
                        VerifiedTotalPrice = kk.VerifiedTotalPrice

                    };
                    //for (int i = 0; i < pvResponse.fareQuoteResponse.Newfare.Count; i++)
                    //{
                    //    bookingReq.flightResult[i].Fare = pvResponse.fareQuoteResponse.Newfare[i];
                    //}
                    #region Adult 1
                    PassengerDetails objPassenger = new PassengerDetails
                    {
                        firstName = "KUNDAN",
                        lastName = "KUMAR",
                        passengerType = PassengerType.Adult,
                        title = "Mr",
                        gender = Gender.Male,
                        dateOfBirth = DateTime.Today.AddYears(-25)
                    };
                    if (fsr.travelType == TravelType.International)
                    {
                        objPassenger.passportNumber = "KJHHJKHKJH";
                        objPassenger.expiryDate = DateTime.Today.AddYears(5);
                        objPassenger.nationality = "IN";
                    }
                    objPassenger.nationality = "IN";
                    bookingReq.passengerDetails.Add(objPassenger);
                    #endregion
                    #region Adult 2
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 3
                    if (bookingReq.adults >= 3)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "RANJAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 4
                    if (bookingReq.adults >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "CHOTAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-33)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 5
                    if (bookingReq.adults >= 5)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "NILESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-41)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 6
                    if (bookingReq.adults >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "HONEY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-42)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 7
                    if (bookingReq.adults >= 7)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJAY",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-37)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 8
                    if (bookingReq.adults >= 8)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SANJU",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-23)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Adult 9
                    if (bookingReq.adults >= 9)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "MAHESH",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Adult,
                            title = "Mr",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-44)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KJHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Child 1
                    if (bookingReq.child >= 1)
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8).AddDays(11);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 2
                    if (bookingReq.child >= 2)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(11)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 3
                    if (bookingReq.child >= 3)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "ROHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-31)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 4
                    if (bookingReq.child >= 4)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "KOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-18)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion
                    #region Child 5
                    if (bookingReq.child >= 5)
                    {
                        PassengerDetails objPassenger2 = new PassengerDetails
                        {
                            firstName = "JOHON",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-8).AddDays(-101)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger2.passportNumber = "KJHHJKHKKK";
                            objPassenger2.expiryDate = DateTime.Today.AddYears(8);
                        }
                        bookingReq.passengerDetails.Add(objPassenger2);
                    }
                    #endregion
                    #region Child 6
                    if (bookingReq.child >= 6)
                    {
                        PassengerDetails objPassenger1 = new PassengerDetails
                        {
                            firstName = "SOHAN",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Child,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-6).AddDays(-111)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger1.passportNumber = "KKHHJKHKJK";
                            objPassenger1.expiryDate = DateTime.Today.AddYears(6);
                        }
                        bookingReq.passengerDetails.Add(objPassenger1);
                    }
                    #endregion

                    #region Infant 1
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
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 2
                    if (bookingReq.infants >= 2)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "KANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    #region Infant 1
                    if (bookingReq.infants >= 3)
                    {
                        PassengerDetails objPassenger3 = new PassengerDetails
                        {
                            firstName = "SANHA",
                            lastName = "KUMAR",
                            passengerType = PassengerType.Infant,
                            title = "Master",
                            gender = Gender.Male,
                            dateOfBirth = DateTime.Today.AddYears(-1).AddDays(-71)
                        };
                        if (fsr.travelType == TravelType.International)
                        {
                            objPassenger3.passportNumber = "KJHHJKKKKK";
                            objPassenger3.expiryDate = DateTime.Today.AddYears(10);
                        }
                        bookingReq.passengerDetails.Add(objPassenger3);
                    }
                    #endregion
                    //var response = new FlightMapper().saveBookingDetails(bookingReq);
                    FlightBookingResponse BookResponse = new FlightBookingResponse(bookingReq); StringBuilder sbLogger = new StringBuilder();
                    new ServicesHub.TripJack.TripJackServiceMapping().BookFlight(bookingReq, ref BookResponse, ref sbLogger);
                    new ServicesHub.TripJack.TripJackServiceMapping().GetFlightBookingDetails(bookingReq);


                    return Request.CreateResponse(HttpStatusCode.OK, BookResponse);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, kk);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
        }
    }
}