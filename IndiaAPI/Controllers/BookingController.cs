using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace IndiaAPI.Controllers
{
    [RoutePrefix("Booking")]
    public class BookingController : ApiController
    {
        private static string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static Random random = new Random();
        public static string SecurityCode = "fl1asdfghasdftmoasdfjado2o";
        public static bool authorizeRequest(string securityCodeGet)
        {
            return SecurityCode == securityCodeGet;
        }


        [HttpPost]
        [Route("MakeBooking")]
        public HttpResponseMessage MakeBooking(string authcode, Core.Flight.OfflineBookingRequest request)
        {
            Core.ResponseStatus res = new Core.ResponseStatus();
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            StringBuilder sbLogger = new StringBuilder();
            DAL.Booking.Get_BookingDetails obj = new DAL.Booking.Get_BookingDetails();

            decimal totPrice = 0;
            string SectorRef = string.Empty;
            List<Core.PassengerDetails> Paxlst = new List<Core.PassengerDetails>();
            Core.Flight.FlightBookingRequest bookingRequest = new Core.Flight.FlightBookingRequest();
            Core.Flight.FlightSearchRequest fsr = obj.GetFsr(request.BookingID, request.isReutn, ref totPrice, ref SectorRef, ref bookingRequest);
            if (totPrice > 0 && !string.IsNullOrEmpty(SectorRef) && fsr.segment != null && fsr.segment.Count > 0)
            {
                if (request.gds == Core.GdsType.FareBoutique)
                {
                    #region FareBoutiqueBooking 
                    ServicesHub.FareBoutique.FareBoutiqueServiceMapping objfb = new ServicesHub.FareBoutique.FareBoutiqueServiceMapping();
                    Core.Flight.FlightSearchResponseShort result = objfb.GetFlightResults(fsr, true, false);
                    bookingLog(ref sbLogger, "FareBoutique Offline Booking Result", JsonConvert.SerializeObject(result));
                    var selectedResult = result.Results[0].Where(k => k.ResultCombination.Equals(SectorRef, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (selectedResult != null && selectedResult.Fare != null)
                    {
                        selectedResult.Fare = selectedResult.FareList.FirstOrDefault();
                        Core.Flight.PriceVerificationRequest pvRequest = new Core.Flight.PriceVerificationRequest()
                        {
                            adults = fsr.adults,
                            child = fsr.child,
                            depDate = fsr.segment[0].travelDate.ToString("dd/MM/yyyy"),
                            destination = fsr.segment[0].destinationAirport.ToLower(),
                            flightResult = new List<Core.Flight.FlightResult>(),
                            infants = fsr.infants,
                            infantsWs = 0,
                            isFareQuote = true,
                            isSSR = false,
                            isFareRule = false,
                            origin = fsr.segment[0].originAirport.ToLower(),
                            PhoneNo = "",
                            PriceID = new List<string>(),
                            siteID = fsr.siteId,
                            sourceMedia = fsr.sourceMedia,
                            ST_ResultSessionID = selectedResult.Fare.ST_ResultSessionID,
                            tgy_Request_id = "",
                            tgy_Search_Key = "",
                            TvoTraceId = result.TraceId,
                            userIP = fsr.userIP,
                            userLogID = fsr.userLogID,
                            userSearchID = fsr.userSearchID,
                            userSessionID = fsr.userSessionID,
                        };
                        pvRequest.flightResult.Add(selectedResult);
                        Core.Flight.FareQuoteResponse pvResponse = objfb.GetFareQuote(pvRequest);
                        bookingLog(ref sbLogger, "FareBoutique Offline Booking FareQuoteResponse", JsonConvert.SerializeObject(pvResponse));

                        if (totPrice == pvResponse.VerifiedTotalPrice)
                        {
                            bookingRequest.AdminID = request.AdminID;
                            bookingRequest.adults = fsr.adults;
                            bookingRequest.affiliate = Core.FlightUtility.GetAffiliate(fsr.sourceMedia);
                            bookingRequest.aircraftDetail = new List<Core.Flight.AircraftDetail>();
                            bookingRequest.airline = new List<Core.Flight.Airline>();
                            bookingRequest.airport = new List<Core.Flight.Airport>();
                            bookingRequest.bookingID = request.BookingID;
                            bookingRequest.bookingStatus = Core.BookingStatus.InProgress;
                            bookingRequest.BrowserDetails = "";
                            bookingRequest.CancellaionPolicyAmt = 0;
                            bookingRequest.child = fsr.child;
                            bookingRequest.infants = fsr.infants;
                            bookingRequest.infantsWs = 0;
                            bookingRequest.TvoTraceId = result.TraceId;
                            bookingRequest.convenienceFee = 0;
                            bookingRequest.CouponAmount = 0;
                            bookingRequest.CouponCode = "";
                            bookingRequest.CouponIncreaseAmount = 0;
                            bookingRequest.currencyCode = selectedResult.Fare.Currency;
                            bookingRequest.deepLink = "";
                            bookingRequest.fareIncreaseAmount = 0;
                            bookingRequest.fareRuleResponse = new List<Core.Flight.FareRuleResponses>();
                            bookingRequest.FB_booking_token_id = result.FB_booking_token_id;
                            bookingRequest.flightResult = new List<Core.Flight.FlightResult>();
                            bookingRequest.gatewayType = Core.GetWayType.Razorpay;
                            bookingRequest.GSTAddress = "";
                            bookingRequest.GSTCompany = "";
                            bookingRequest.GSTNo = "";
                            bookingRequest.isBuyCancellaionPolicy = false;
                            bookingRequest.isBuyRefundPolicy = false;
                            bookingRequest.isFareChange = false;
                            bookingRequest.isGST = false;
                            bookingRequest.isMakeBookingInprogress = false;
                            bookingRequest.isTickted = new List<bool>();
                            bookingRequest.isWhatsapp = false;
                            bookingRequest.LastCheckInDate = DateTime.Today;
                            bookingRequest.paymentDetails = new Core.PaymentDetails();
                            bookingRequest.paymentMode = Core.PaymentMode.NONE;
                            bookingRequest.paymentStatus = Core.PaymentStatus.Completed;
                            bookingRequest.PNR = "";
                            bookingRequest.PriceID = new List<string>();
                            bookingRequest.prodID = 1;
                            bookingRequest.razorpayOrderID = "";
                            bookingRequest.razorpayTransectionID = "";
                            bookingRequest.RefundPolicyAmt = 0;
                            bookingRequest.ReturnPNR = "";
                            bookingRequest.siteID = fsr.siteId;
                            bookingRequest.sourceMedia = fsr.sourceMedia;
                            bookingRequest.STSessionID = selectedResult.ST_ResultSessionID;
                            bookingRequest.ST_ResultSessionID = selectedResult.ST_ResultSessionID;
                            bookingRequest.sumFare = new Core.Flight.Fare();
                            bookingRequest.tgy_Block_Ticket_Allowed = new List<bool>();
                            bookingRequest.tgy_Booking_RefNo = "";
                            bookingRequest.tgy_Flight_Key = new List<string>();
                            bookingRequest.tgy_Request_id = "";
                            bookingRequest.tgy_Search_Key = "";
                            bookingRequest.TjBookingID = "";
                            bookingRequest.TjReturnBookingID = "";
                            bookingRequest.transactionID = 0;
                            bookingRequest.travelType = fsr.travelType;
                            bookingRequest.TvoBookingID = 0;
                            bookingRequest.TvoReturnBookingID = 0;
                            bookingRequest.updatedBookingAmount = 0;
                            bookingRequest.userIP = fsr.userIP;
                            bookingRequest.userLogID = fsr.userLogID;
                            bookingRequest.userSearchID = fsr.userSearchID;
                            bookingRequest.userSessionID = fsr.userSessionID;
                            bookingRequest.VerifiedTotalPrice = pvResponse.VerifiedTotalPrice;
                            bookingRequest.flightResult.Add(selectedResult);
                            Core.Flight.FlightBookingResponse bookingResponse = new Core.Flight.FlightBookingResponse(bookingRequest);
                            bookingLog(ref sbLogger, "FareBoutique Offline Booking BookingResponse", JsonConvert.SerializeObject(bookingResponse));

                            objfb.BookFlight(bookingRequest, ref bookingResponse);

                            DAL.Booking.SaveBookingDetails objSaveBookingDetails = new DAL.Booking.SaveBookingDetails();
                            objSaveBookingDetails.SaveFMJ_FlightBookingTransactionDetailsWithTickNo(ref bookingRequest, ref bookingResponse);
                            res = bookingResponse.responseStatus;

                            new ServicesHub.LogWriter_New(sbLogger.ToString(), bookingRequest.bookingID.ToString(), "OfflineBooking", "FareBoutique Offline BookFlight Original Response");
                        }
                        else
                        {
                            res.status = Core.TransactionStatus.Error;
                            res.message = "Increase price";
                            Core.Flight.BookingRemark BR = new Core.Flight.BookingRemark();
                            BR.BookingID = request.BookingID;
                            BR.Booking_Remarks = res.message;
                            BR.ModifiedBy = request.AdminID;
                            int i = DAL.Booking.Get_BookingDetails.SaveRemarks(BR);
                            if (i > 0)
                            {
                                res.message = "success Increase price";
                            }
                            else
                            {
                                res.message = "Unsuccess Increase price";
                            }
                            bookingLog(ref sbLogger, "FareBoutique Offline Booking Else Increase price", res.message);
                            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.BookingID.ToString(), "OfflineBooking", "FareBoutique Offline BookFlight Original Response");
                        }
                    }
                    else
                    {
                        res.status = Core.TransactionStatus.Error;
                        res.message = "Same fare not available";
                        Core.Flight.BookingRemark BR = new Core.Flight.BookingRemark();
                        BR.BookingID = request.BookingID;
                        BR.Booking_Remarks = res.message;
                        BR.ModifiedBy = request.AdminID;
                        int i = DAL.Booking.Get_BookingDetails.SaveRemarks(BR);
                        if (i > 0)
                        {
                            res.message = "success Same fare not available";
                        }
                        else
                        {
                            res.message = "Unsuccess Same fare not available";
                        }

                        bookingLog(ref sbLogger, "FareBoutique Offline Booking Else Same fare not available", res.message);
                        new ServicesHub.LogWriter_New(sbLogger.ToString(), request.BookingID.ToString(), "OfflineBooking", "FareBoutique Offline BookFlight Original Response");

                    }
                    #endregion
                }
                else if (request.gds == Core.GdsType.SatkarTravel)
                {
                    #region SatkarTravelBooking 
                    ServicesHub.SatkarTravel.SatkarTravelServiceMapping objST = new ServicesHub.SatkarTravel.SatkarTravelServiceMapping();
                    Core.Flight.FlightSearchResponseShort result = objST.GetFlightResults(fsr, true, false);
                    bookingLog(ref sbLogger, "SatkarTravel Offline Booking Result", JsonConvert.SerializeObject(result));
                    var selectedResult = result.Results[0].Where(k => k.ResultCombination.Equals(SectorRef, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                    if (selectedResult != null && selectedResult.Fare != null)
                    {
                        selectedResult.Fare = selectedResult.FareList.FirstOrDefault();
                        Core.Flight.PriceVerificationRequest pvRequest = new Core.Flight.PriceVerificationRequest()
                        {
                            adults = fsr.adults,
                            child = fsr.child,
                            depDate = fsr.segment[0].travelDate.ToString("dd/MM/yyyy"),
                            destination = fsr.segment[0].destinationAirport.ToLower(),
                            flightResult = new List<Core.Flight.FlightResult>(),
                            infants = fsr.infants,
                            infantsWs = 0,
                            isFareQuote = true,
                            isSSR = false,
                            isFareRule = false,
                            origin = fsr.segment[0].originAirport.ToLower(),
                            PhoneNo = "",
                            PriceID = new List<string>(),
                            siteID = fsr.siteId,
                            sourceMedia = fsr.sourceMedia,
                            ST_ResultSessionID = selectedResult.Fare.ST_ResultSessionID,
                            tgy_Request_id = "",
                            tgy_Search_Key = "",
                            TvoTraceId = result.TraceId,
                            userIP = fsr.userIP,
                            userLogID = fsr.userLogID,
                            userSearchID = fsr.userSearchID,
                            userSessionID = fsr.userSessionID,
                        };
                        pvRequest.flightResult.Add(selectedResult);
                        Core.Flight.FareQuoteResponse pvResponse = objST.GetFareQuote(pvRequest);
                        bookingLog(ref sbLogger, "SatkarTravel Offline Booking FareQuoteResponse", JsonConvert.SerializeObject(pvResponse));
                        if (totPrice == pvResponse.VerifiedTotalPrice)
                        {
                            bookingRequest.STSessionID = pvResponse.STSessionID;
                            bookingRequest.AdminID = request.AdminID;
                            bookingRequest.adults = fsr.adults;
                            bookingRequest.affiliate = Core.FlightUtility.GetAffiliate(fsr.sourceMedia);
                            bookingRequest.aircraftDetail = new List<Core.Flight.AircraftDetail>();
                            bookingRequest.airline = new List<Core.Flight.Airline>();
                            bookingRequest.airport = new List<Core.Flight.Airport>();
                            bookingRequest.bookingID = request.BookingID;
                            bookingRequest.bookingStatus = Core.BookingStatus.InProgress;
                            bookingRequest.BrowserDetails = "";
                            bookingRequest.CancellaionPolicyAmt = 0;
                            bookingRequest.child = fsr.child;
                            bookingRequest.infants = fsr.infants;
                            bookingRequest.infantsWs = 0;
                            bookingRequest.TvoTraceId = result.TraceId;
                            bookingRequest.convenienceFee = 0;
                            bookingRequest.CouponAmount = 0;
                            bookingRequest.CouponCode = "";
                            bookingRequest.CouponIncreaseAmount = 0;
                            bookingRequest.currencyCode = selectedResult.Fare.Currency;
                            bookingRequest.deepLink = "";
                            //bookingRequest.emailID = "";
                            bookingRequest.fareIncreaseAmount = 0;
                            bookingRequest.fareRuleResponse = new List<Core.Flight.FareRuleResponses>();
                            bookingRequest.FB_booking_token_id = result.FB_booking_token_id;
                            bookingRequest.flightResult = new List<Core.Flight.FlightResult>();
                            bookingRequest.gatewayType = Core.GetWayType.Razorpay;
                            bookingRequest.GSTAddress = "";
                            bookingRequest.GSTCompany = "";
                            bookingRequest.GSTNo = "";
                            bookingRequest.isBuyCancellaionPolicy = false;
                            bookingRequest.isBuyRefundPolicy = false;
                            bookingRequest.isFareChange = false;
                            bookingRequest.isGST = false;
                            bookingRequest.isMakeBookingInprogress = false;
                            bookingRequest.isTickted = new List<bool>();
                            bookingRequest.isWhatsapp = false;
                            bookingRequest.LastCheckInDate = DateTime.Today;
                            //bookingRequest.mobileNo = "";
                            //bookingRequest.passengerDetails = new List<Core.PassengerDetails>();
                            bookingRequest.paymentDetails = new Core.PaymentDetails();
                            bookingRequest.paymentMode = Core.PaymentMode.NONE;
                            bookingRequest.paymentStatus = Core.PaymentStatus.Completed;
                            //bookingRequest.phoneNo = "";
                            bookingRequest.PNR = "";
                            bookingRequest.PriceID = new List<string>();
                            bookingRequest.prodID = 1;
                            bookingRequest.razorpayOrderID = "";
                            bookingRequest.razorpayTransectionID = "";
                            bookingRequest.RefundPolicyAmt = 0;
                            bookingRequest.ReturnPNR = "";
                            bookingRequest.siteID = fsr.siteId;
                            bookingRequest.sourceMedia = fsr.sourceMedia;
                            bookingRequest.ST_ResultSessionID = selectedResult.ST_ResultSessionID;
                            bookingRequest.sumFare = new Core.Flight.Fare();
                            bookingRequest.tgy_Block_Ticket_Allowed = new List<bool>();
                            bookingRequest.tgy_Booking_RefNo = "";
                            bookingRequest.tgy_Flight_Key = new List<string>();
                            bookingRequest.tgy_Request_id = "";
                            bookingRequest.tgy_Search_Key = "";
                            bookingRequest.TjBookingID = "";
                            bookingRequest.TjReturnBookingID = "";
                            bookingRequest.transactionID = 0;
                            bookingRequest.travelType = fsr.travelType;
                            bookingRequest.TvoBookingID = 0;
                            bookingRequest.TvoReturnBookingID = 0;
                            bookingRequest.updatedBookingAmount = 0;
                            bookingRequest.userIP = fsr.userIP;
                            bookingRequest.userLogID = fsr.userLogID;
                            bookingRequest.userSearchID = fsr.userSearchID;
                            bookingRequest.userSessionID = fsr.userSessionID;
                            bookingRequest.VerifiedTotalPrice = pvResponse.VerifiedTotalPrice;


                            bookingRequest.flightResult.Add(selectedResult);
                            Core.Flight.FlightBookingResponse bookingResponse = new Core.Flight.FlightBookingResponse(bookingRequest);
                            bookingLog(ref sbLogger, "SatkarTravel Offline Booking bookingResponse", JsonConvert.SerializeObject(bookingResponse));
                            objST.BookFlight(bookingRequest, ref bookingResponse);

                            DAL.Booking.SaveBookingDetails objSaveBookingDetails = new DAL.Booking.SaveBookingDetails();
                            objSaveBookingDetails.SaveFMJ_FlightBookingTransactionDetailsWithTickNo(ref bookingRequest, ref bookingResponse);
                            new ServicesHub.LogWriter_New(sbLogger.ToString(), bookingRequest.bookingID.ToString(), "OfflineBooking", "Satkar Offline BookFlight Original Response");
                        }
                        else
                        {
                            res.status = Core.TransactionStatus.Error;
                            res.message = "Increase price";

                            Core.Flight.BookingRemark BR = new Core.Flight.BookingRemark();
                            BR.BookingID = request.BookingID;
                            BR.Booking_Remarks = res.message;
                            BR.ModifiedBy = request.AdminID;
                            int i = DAL.Booking.Get_BookingDetails.SaveRemarks(BR);
                            if (i > 0)
                            {
                                res.message = "success Increase price";
                            }
                            else
                            {
                                res.message = "Unsuccess Increase price";
                            }
                            bookingLog(ref sbLogger, "FareBoutique Offline Booking Else Increase price", res.message);
                            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.BookingID.ToString(), "OfflineBooking", "Satkar Offline BookFlight Original Response");

                        }
                    }
                    else
                    {
                        res.status = Core.TransactionStatus.Error;
                        res.message = "Same fare not available";

                        Core.Flight.BookingRemark BR = new Core.Flight.BookingRemark();
                        BR.BookingID = request.BookingID;
                        BR.Booking_Remarks = res.message;
                        BR.ModifiedBy = request.AdminID;
                        int i = DAL.Booking.Get_BookingDetails.SaveRemarks(BR);
                        if (i > 0)
                        {
                            res.message = "success Same fare not available";
                        }
                        else
                        {
                            res.message = "Unsuccess Same fare not available";
                        }
                        bookingLog(ref sbLogger, "Satkar Offline Booking Else Same fare not available", res.message);
                        new ServicesHub.LogWriter_New(sbLogger.ToString(), request.BookingID.ToString(), "OfflineBooking", "Satkar Offline BookFlight Original Response");
                    }
                    #endregion
                }
                else if (request.gds == Core.GdsType.GFS)
                {
                    #region TripFactaryBooking 
                    ServicesHub.GFS.GFSServiceMapping objGFS = new ServicesHub.GFS.GFSServiceMapping();
                    Core.Flight.FlightSearchResponseShort result = objGFS.GetFlightResults(fsr, true, false);
                    bookingLog(ref sbLogger, "TripFactary Offline Booking bookingResponse", JsonConvert.SerializeObject(result));
                    var selectedResult = result.Results[0].Where(k => k.ResultCombination.Equals(SectorRef, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                    if (selectedResult != null && selectedResult.Fare != null)
                    {
                        selectedResult.Fare = selectedResult.FareList.FirstOrDefault();
                        Core.Flight.PriceVerificationRequest pvRequest = new Core.Flight.PriceVerificationRequest()
                        {
                            adults = fsr.adults,
                            child = fsr.child,
                            depDate = fsr.segment[0].travelDate.ToString("dd/MM/yyyy"),
                            destination = fsr.segment[0].destinationAirport.ToLower(),
                            flightResult = new List<Core.Flight.FlightResult>(),
                            infants = fsr.infants,
                            infantsWs = 0,
                            isFareQuote = true,
                            isSSR = false,
                            isFareRule = false,
                            origin = fsr.segment[0].originAirport.ToLower(),
                            PhoneNo = "",
                            PriceID = new List<string>(),
                            siteID = fsr.siteId,
                            sourceMedia = fsr.sourceMedia,
                            ST_ResultSessionID = selectedResult.Fare.ST_ResultSessionID,
                            tgy_Request_id = "",
                            tgy_Search_Key = "",
                            TvoTraceId = result.TraceId,
                            userIP = fsr.userIP,
                            userLogID = fsr.userLogID,
                            userSearchID = fsr.userSearchID,
                            userSessionID = fsr.userSessionID,
                        };
                        pvRequest.flightResult.Add(selectedResult);
                        Core.Flight.FareQuoteResponse pvResponse = objGFS.GetFareQuote(pvRequest);
                        bookingLog(ref sbLogger, "TripFactary Offline Booking FareQuoteResponse", JsonConvert.SerializeObject(pvResponse));
                        if (totPrice == pvResponse.VerifiedTotalPrice)
                        {
                            bookingRequest.AdminID = request.AdminID;
                            bookingRequest.adults = fsr.adults;
                            bookingRequest.affiliate = Core.FlightUtility.GetAffiliate(fsr.sourceMedia);
                            bookingRequest.aircraftDetail = new List<Core.Flight.AircraftDetail>();
                            bookingRequest.airline = new List<Core.Flight.Airline>();
                            bookingRequest.airport = new List<Core.Flight.Airport>();
                            bookingRequest.bookingID = request.BookingID;
                            bookingRequest.bookingStatus = Core.BookingStatus.InProgress;
                            bookingRequest.BrowserDetails = "";
                            bookingRequest.CancellaionPolicyAmt = 0;
                            bookingRequest.child = fsr.child;
                            bookingRequest.infants = fsr.infants;
                            bookingRequest.infantsWs = 0;
                            bookingRequest.TvoTraceId = result.TraceId;
                            bookingRequest.convenienceFee = 0;
                            bookingRequest.CouponAmount = 0;
                            bookingRequest.CouponCode = "";
                            bookingRequest.CouponIncreaseAmount = 0;
                            bookingRequest.currencyCode = selectedResult.Fare.Currency;
                            bookingRequest.deepLink = "";
                            //bookingRequest.emailID = "";
                            bookingRequest.fareIncreaseAmount = 0;
                            bookingRequest.fareRuleResponse = new List<Core.Flight.FareRuleResponses>();
                            bookingRequest.FB_booking_token_id = result.FB_booking_token_id;
                            bookingRequest.flightResult = new List<Core.Flight.FlightResult>();
                            bookingRequest.gatewayType = Core.GetWayType.Razorpay;
                            bookingRequest.GSTAddress = "";
                            bookingRequest.GSTCompany = "";
                            bookingRequest.GSTNo = "";
                            bookingRequest.isBuyCancellaionPolicy = false;
                            bookingRequest.isBuyRefundPolicy = false;
                            bookingRequest.isFareChange = false;
                            bookingRequest.isGST = false;
                            bookingRequest.isMakeBookingInprogress = false;
                            bookingRequest.isTickted = new List<bool>();
                            bookingRequest.isWhatsapp = false;
                            bookingRequest.LastCheckInDate = DateTime.Today;
                            //bookingRequest.mobileNo = "";
                            //bookingRequest.passengerDetails = new List<Core.PassengerDetails>();
                            bookingRequest.paymentDetails = new Core.PaymentDetails();
                            bookingRequest.paymentMode = Core.PaymentMode.NONE;
                            bookingRequest.paymentStatus = Core.PaymentStatus.Completed;
                            //bookingRequest.phoneNo = "";
                            bookingRequest.PNR = "";
                            bookingRequest.PriceID = new List<string>();
                            bookingRequest.prodID = 1;
                            bookingRequest.razorpayOrderID = "";
                            bookingRequest.razorpayTransectionID = "";
                            bookingRequest.RefundPolicyAmt = 0;
                            bookingRequest.ReturnPNR = "";
                            bookingRequest.siteID = fsr.siteId;
                            bookingRequest.sourceMedia = fsr.sourceMedia;
                            bookingRequest.STSessionID = selectedResult.ST_ResultSessionID;
                            bookingRequest.ST_ResultSessionID = selectedResult.ST_ResultSessionID;
                            bookingRequest.sumFare = new Core.Flight.Fare();
                            bookingRequest.tgy_Block_Ticket_Allowed = new List<bool>();
                            bookingRequest.tgy_Booking_RefNo = "";
                            bookingRequest.tgy_Flight_Key = new List<string>();
                            bookingRequest.tgy_Request_id = "";
                            bookingRequest.tgy_Search_Key = "";
                            bookingRequest.TjBookingID = "";
                            bookingRequest.TjReturnBookingID = "";
                            bookingRequest.transactionID = 0;
                            bookingRequest.travelType = fsr.travelType;
                            bookingRequest.TvoBookingID = 0;
                            bookingRequest.TvoReturnBookingID = 0;
                            bookingRequest.updatedBookingAmount = 0;
                            bookingRequest.userIP = fsr.userIP;
                            bookingRequest.userLogID = fsr.userLogID;
                            bookingRequest.userSearchID = fsr.userSearchID;
                            bookingRequest.userSessionID = fsr.userSessionID;
                            bookingRequest.VerifiedTotalPrice = pvResponse.VerifiedTotalPrice;

                            bookingRequest.flightResult.Add(selectedResult);
                            Core.Flight.FlightBookingResponse bookingResponse = new Core.Flight.FlightBookingResponse(bookingRequest);
                            bookingLog(ref sbLogger, "TripFactary Offline Booking bookingResponse", JsonConvert.SerializeObject(bookingResponse));
                            objGFS.BookFlight(bookingRequest, ref bookingResponse);

                            DAL.Booking.SaveBookingDetails objSaveBookingDetails = new DAL.Booking.SaveBookingDetails();
                            objSaveBookingDetails.SaveFMJ_FlightBookingTransactionDetailsWithTickNo(ref bookingRequest, ref bookingResponse);
                            new ServicesHub.LogWriter_New(sbLogger.ToString(), bookingRequest.bookingID.ToString(), "OfflineBooking", "TripFactory Offline BookFlight Original Response");
                        }
                        else
                        {
                            res.status = Core.TransactionStatus.Error;
                            res.message = "Increase price";
                            Core.Flight.BookingRemark BR = new Core.Flight.BookingRemark();
                            BR.BookingID = request.BookingID;
                            BR.Booking_Remarks = res.message;
                            BR.ModifiedBy = request.AdminID;
                            int i = DAL.Booking.Get_BookingDetails.SaveRemarks(BR);
                            if (i > 0)
                            {
                                res.message = "success Increase price";
                            }
                            else
                            {
                                res.message = "Unsuccess Increase price";
                            }
                            bookingLog(ref sbLogger, "TripFactory Offline Booking Else Increase price", res.message);
                            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.BookingID.ToString(), "OfflineBooking", "TripFactory Offline BookFlight Original Response");

                        }
                    }
                    else
                    {
                        res.status = Core.TransactionStatus.Error;
                        res.message = "Same fare not available";
                        Core.Flight.BookingRemark BR = new Core.Flight.BookingRemark();
                        BR.BookingID = request.BookingID;
                        BR.Booking_Remarks = res.message;
                        BR.ModifiedBy = request.AdminID;
                        int i = DAL.Booking.Get_BookingDetails.SaveRemarks(BR);
                        if (i > 0)
                        {
                            res.message = "success Same fare not available";
                        }
                        else
                        {
                            res.message = "Unsuccess Same fare not available";
                        }
                        bookingLog(ref sbLogger, "TripFactory Offline Booking Else Same fare not available", res.message);
                        new ServicesHub.LogWriter_New(sbLogger.ToString(), request.BookingID.ToString(), "OfflineBooking", "TripFactory Offline BookFlight Original Response");

                    }
                    #endregion
                }
                else if (request.gds == Core.GdsType.Ease2Fly)
                {
                    #region Ease2FlyBooking 
                    ServicesHub.GFS.GFSServiceMapping objE2F = new ServicesHub.GFS.GFSServiceMapping();
                    Core.Flight.FlightSearchResponseShort result = objE2F.GetFlightResults(fsr, true, false);
                    bookingLog(ref sbLogger, "Ease2Fly Offline Booking result", JsonConvert.SerializeObject(result));
                    var selectedResult = result.Results[0].Where(k => k.ResultCombination.Equals(SectorRef, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                    if (selectedResult != null && selectedResult.Fare != null)
                    {
                        selectedResult.Fare = selectedResult.FareList.FirstOrDefault();
                        Core.Flight.PriceVerificationRequest pvRequest = new Core.Flight.PriceVerificationRequest()
                        {
                            adults = fsr.adults,
                            child = fsr.child,
                            depDate = fsr.segment[0].travelDate.ToString("dd/MM/yyyy"),
                            destination = fsr.segment[0].destinationAirport.ToLower(),
                            flightResult = new List<Core.Flight.FlightResult>(),
                            infants = fsr.infants,
                            infantsWs = 0,
                            isFareQuote = true,
                            isSSR = false,
                            isFareRule = false,
                            origin = fsr.segment[0].originAirport.ToLower(),
                            PhoneNo = "",
                            PriceID = new List<string>(),
                            siteID = fsr.siteId,
                            sourceMedia = fsr.sourceMedia,
                            ST_ResultSessionID = selectedResult.Fare.ST_ResultSessionID,
                            tgy_Request_id = "",
                            tgy_Search_Key = "",
                            TvoTraceId = result.TraceId,
                            userIP = fsr.userIP,
                            userLogID = fsr.userLogID,
                            userSearchID = fsr.userSearchID,
                            userSessionID = fsr.userSessionID,
                        };
                        pvRequest.flightResult.Add(selectedResult);
                        Core.Flight.FareQuoteResponse pvResponse = objE2F.GetFareQuote(pvRequest);
                        bookingLog(ref sbLogger, "Ease2Fly Offline Booking FareQuoteResponse", JsonConvert.SerializeObject(pvResponse));
                        if (totPrice == pvResponse.VerifiedTotalPrice)
                        {
                            bookingRequest.AdminID = request.AdminID;
                            bookingRequest.adults = fsr.adults;
                            bookingRequest.affiliate = Core.FlightUtility.GetAffiliate(fsr.sourceMedia);
                            bookingRequest.aircraftDetail = new List<Core.Flight.AircraftDetail>();
                            bookingRequest.airline = new List<Core.Flight.Airline>();
                            bookingRequest.airport = new List<Core.Flight.Airport>();
                            bookingRequest.bookingID = request.BookingID;
                            bookingRequest.bookingStatus = Core.BookingStatus.InProgress;
                            bookingRequest.BrowserDetails = "";
                            bookingRequest.CancellaionPolicyAmt = 0;
                            bookingRequest.child = fsr.child;
                            bookingRequest.infants = fsr.infants;
                            bookingRequest.infantsWs = 0;
                            bookingRequest.TvoTraceId = result.TraceId;
                            bookingRequest.convenienceFee = 0;
                            bookingRequest.CouponAmount = 0;
                            bookingRequest.CouponCode = "";
                            bookingRequest.CouponIncreaseAmount = 0;
                            bookingRequest.currencyCode = selectedResult.Fare.Currency;
                            bookingRequest.deepLink = "";
                            //bookingRequest.emailID = "";
                            bookingRequest.fareIncreaseAmount = 0;
                            bookingRequest.fareRuleResponse = new List<Core.Flight.FareRuleResponses>();
                            bookingRequest.FB_booking_token_id = result.FB_booking_token_id;
                            bookingRequest.flightResult = new List<Core.Flight.FlightResult>();
                            bookingRequest.gatewayType = Core.GetWayType.Razorpay;
                            bookingRequest.GSTAddress = "";
                            bookingRequest.GSTCompany = "";
                            bookingRequest.GSTNo = "";
                            bookingRequest.isBuyCancellaionPolicy = false;
                            bookingRequest.isBuyRefundPolicy = false;
                            bookingRequest.isFareChange = false;
                            bookingRequest.isGST = false;
                            bookingRequest.isMakeBookingInprogress = false;
                            bookingRequest.isTickted = new List<bool>();
                            bookingRequest.isWhatsapp = false;
                            bookingRequest.LastCheckInDate = DateTime.Today;
                            //bookingRequest.mobileNo = "";
                            //bookingRequest.passengerDetails = new List<Core.PassengerDetails>();
                            bookingRequest.paymentDetails = new Core.PaymentDetails();
                            bookingRequest.paymentMode = Core.PaymentMode.NONE;
                            bookingRequest.paymentStatus = Core.PaymentStatus.Completed;
                            //bookingRequest.phoneNo = "";
                            bookingRequest.PNR = "";
                            bookingRequest.PriceID = new List<string>();
                            bookingRequest.prodID = 1;
                            bookingRequest.razorpayOrderID = "";
                            bookingRequest.razorpayTransectionID = "";
                            bookingRequest.RefundPolicyAmt = 0;
                            bookingRequest.ReturnPNR = "";
                            bookingRequest.siteID = fsr.siteId;
                            bookingRequest.sourceMedia = fsr.sourceMedia;
                            bookingRequest.STSessionID = selectedResult.ST_ResultSessionID;
                            bookingRequest.ST_ResultSessionID = selectedResult.ST_ResultSessionID;
                            bookingRequest.sumFare = new Core.Flight.Fare();
                            bookingRequest.tgy_Block_Ticket_Allowed = new List<bool>();
                            bookingRequest.tgy_Booking_RefNo = "";
                            bookingRequest.tgy_Flight_Key = new List<string>();
                            bookingRequest.tgy_Request_id = "";
                            bookingRequest.tgy_Search_Key = "";
                            bookingRequest.TjBookingID = "";
                            bookingRequest.TjReturnBookingID = "";
                            bookingRequest.transactionID = 0;
                            bookingRequest.travelType = fsr.travelType;
                            bookingRequest.TvoBookingID = 0;
                            bookingRequest.TvoReturnBookingID = 0;
                            bookingRequest.updatedBookingAmount = 0;
                            bookingRequest.userIP = fsr.userIP;
                            bookingRequest.userLogID = fsr.userLogID;
                            bookingRequest.userSearchID = fsr.userSearchID;
                            bookingRequest.userSessionID = fsr.userSessionID;
                            bookingRequest.VerifiedTotalPrice = pvResponse.VerifiedTotalPrice;

                            bookingRequest.flightResult.Add(selectedResult);
                            Core.Flight.FlightBookingResponse bookingResponse = new Core.Flight.FlightBookingResponse(bookingRequest);
                            bookingLog(ref sbLogger, "Ease2Fly Offline Booking bookingResponse", JsonConvert.SerializeObject(bookingResponse));

                            objE2F.BookFlight(bookingRequest, ref bookingResponse);

                            DAL.Booking.SaveBookingDetails objSaveBookingDetails = new DAL.Booking.SaveBookingDetails();
                            objSaveBookingDetails.SaveFMJ_FlightBookingTransactionDetailsWithTickNo(ref bookingRequest, ref bookingResponse);
                            new ServicesHub.LogWriter_New(sbLogger.ToString(), bookingRequest.bookingID.ToString(), "OfflineBooking", "E2F Offline BookFlight Original Response");
                        }
                        else
                        {
                            res.status = Core.TransactionStatus.Error;
                            res.message = "Increase price";
                            Core.Flight.BookingRemark BR = new Core.Flight.BookingRemark();
                            BR.BookingID = request.BookingID;
                            BR.Booking_Remarks = res.message;
                            BR.ModifiedBy = request.AdminID;
                            int i = DAL.Booking.Get_BookingDetails.SaveRemarks(BR);
                            if (i > 0)
                            {
                                res.message = "success Increase price";
                            }
                            else
                            {
                                res.message = "Unsuccess";
                            }
                            bookingLog(ref sbLogger, "E2F Offline Booking Else Increase price", res.message);
                            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.BookingID.ToString(), "OfflineBooking", "E2F Offline BookFlight Original Response");

                        }
                    }
                    else
                    {
                        res.status = Core.TransactionStatus.Error;
                        res.message = "Same fare not available";
                        Core.Flight.BookingRemark BR = new Core.Flight.BookingRemark();
                        BR.BookingID = request.BookingID;
                        BR.Booking_Remarks = res.message;
                        BR.ModifiedBy = request.AdminID;
                        int i = DAL.Booking.Get_BookingDetails.SaveRemarks(BR);
                        if (i > 0)
                        {
                            res.message = "success Same fare not available";
                        }
                        else
                        {
                            res.message = "Unsuccess Same fare not available";
                        }

                        bookingLog(ref sbLogger, "E2F Offline Booking Else Same fare not available", res.message);
                        new ServicesHub.LogWriter_New(sbLogger.ToString(), request.BookingID.ToString(), "OfflineBooking", "E2F Offline BookFlight Original Response");

                    }


                    #endregion
                }
                else if (request.gds == Core.GdsType.Tbo || request.gds == Core.GdsType.TripJack || request.gds == Core.GdsType.OneDFare)
                {
                    res.status = Core.TransactionStatus.Error;
                    res.message = "Error";
                }
            }
            else
            {
                res.status = Core.TransactionStatus.Error;
                res.message = "Booking not found in DB";

                bookingLog(ref sbLogger, "Offline Booking Else Booking not found in DB", res.message);
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.BookingID.ToString(), "OfflineBooking", "Offline BookFlight Original Response");

            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        public void bookingLog(ref StringBuilder sbLogger, string requestTitle, string logText)
        {
            sbLogger.Append(Environment.NewLine + "---------------------------------------------" + requestTitle + "" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
            sbLogger.Append(Environment.NewLine + logText);
            sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
        }
    }
}
