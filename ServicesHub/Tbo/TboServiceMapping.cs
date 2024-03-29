using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Flight;
using System.Configuration;
using Core;
using Newtonsoft.Json;

namespace ServicesHub.Tbo
{
    public class TboServiceMapping
    {
        //public void GetFlightResults(FlightSearchRequest request, ref FlightSearchResponse flightResponse)
        //{
        //    bool isTvo = false;
        //    #region CheckSupplier
        //    if (request.segment[0].orgArp == null)
        //    {
        //        request.segment[0].orgArp = FlightUtility.GetAirport(request.segment[0].originAirport);
        //    }
        //    if (request.segment[0].destArp == null)
        //    {
        //        request.segment[0].destArp = FlightUtility.GetAirport(request.segment[0].destinationAirport);
        //    }
        //    var supplierData = FlightUtility.lstFlightProviderList.Where(o => (o.siteId == request.siteId) &&
        //       ((o.FromCountry.Any() && o.FromCountry.Contains(request.segment[0].orgArp.countryCode)) || o.FromCountry.Any() == false) &&
        //       ((o.ToCountry.Any() && o.ToCountry.Contains(request.segment[0].destArp.countryCode)) || o.ToCountry.Any() == false) &&
        //       (o.FromCountry_Not.Contains(request.segment.ElementAt(0).orgArp.countryCode) == false) &&
        //       (o.ToCountry_Not.Contains(request.segment.ElementAt(0).destArp.countryCode) == false) &&
        //       ((o.SourceMedia.Any() && o.SourceMedia.Contains(request.sourceMedia)) || o.SourceMedia.Any() == false) &&
        //       (o.SourceMedia_Not.Contains(request.sourceMedia) == false) && o.Provider == GdsType.Tbo).ToList();
        //    if (supplierData.Count > 0)
        //    {
        //        isTvo = true;
        //    }
        //    #endregion
        //    StringBuilder sbLogger = new StringBuilder();
        //    if (FlightUtility.isWriteLog)
        //    {
        //        bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
        //    }
        //    if (isTvo)
        //    {
        //        Start:
        //        ServicesHub.Tbo.TboAuthentication obj = new ServicesHub.Tbo.TboAuthentication();
        //        string TokenId = obj.getTokenID();
        //        var Url = TboAuthentication.airSearchUrl;
        //        string strRequest = new RequestMappking().getFlightSearchRequest(request, TokenId);
        //        if (FlightUtility.isWriteLog)
        //        {
        //            bookingLog(ref sbLogger, "Tbo Request", strRequest);
        //        }
        //        var strResponse = GetResponse(Url, strRequest);
        //        if (request.isMetaRequest == false && FlightUtility.isWriteLog)
        //        {
        //            bookingLog(ref sbLogger, "Tbo Response", strResponse);
        //            new LogWriter(sbLogger.ToString(), ("Tbo_" + request.segment[0].originAirport + "_" + request.segment[0].destinationAirport), "Tbo");
        //        }
        //        if (!string.IsNullOrEmpty(strResponse))
        //        {
        //            TboClass.FlightResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<TboClass.FlightResponse>(strResponse);
        //            if (Response.Response.ResponseStatus == 3 && Response.Response.Error.ErrorCode == 3 && Response.Response.Error.ErrorMessage.ToUpper() == "INVALID SESSION")
        //            {
        //                new DAL.Tbo.Tbo_DataSetGet().setLogoutByTokenID(TokenId);
        //                System.Web.HttpRuntime.Cache["TokenID"] = new TboClass.TboToken();
        //                goto Start;
        //            }
        //            else if (Response.Response.ResponseStatus == 1 && Response.Response.Error.ErrorCode == 0)
        //            {
        //                new ResponseMapping().getResult(ref request, ref Response, ref flightResponse);
        //            }
        //            else
        //            {
        //                flightResponse.response.status = Core.TransactionStatus.Error;
        //                flightResponse.response.message = "no result found";
        //            }
        //        }
        //    }
        //    else
        //    {
        //        flightResponse.response.status = Core.TransactionStatus.Error;
        //        flightResponse.response.message = "no result found";
        //    }
        //}

        public FlightSearchResponseShort GetFlightResults(FlightSearchRequest request)
        {
            FlightSearchResponseShort flightResponse = new FlightSearchResponseShort(request);

            StringBuilder sbLogger = new StringBuilder();
            //if (FlightUtility.isWriteLog)
            //{
            //    bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
            //}

            Start:
            ServicesHub.Tbo.TboAuthentication obj = new ServicesHub.Tbo.TboAuthentication();
            string TokenId = obj.getTokenID();
            var Url = TboAuthentication.airSearchUrl;
            string strRequest = new TboRequestMappking().getFlightSearchRequest(request, TokenId);
            if (FlightUtility.isWriteLogSearch)
            {
                bookingLog(ref sbLogger, "Tbo Search Request", strRequest);
            }
            var strResponse = GetResponseSearch(Url, strRequest);

            if (FlightUtility.isWriteLogSearch)
            {
                bookingLog(ref sbLogger, "Tbo Search Response", strResponse);
                //new LogWriter(sbLogger.ToString(), request.userSearchID , "Tbo");
            }
            if (!string.IsNullOrEmpty(strResponse))
            {
                TboClass.FlightResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<TboClass.FlightResponse>(strResponse);
                if (Response.Response.ResponseStatus == 3 && Response.Response.Error.ErrorCode == 3 && Response.Response.Error.ErrorMessage.ToUpper() == "INVALID SESSION")
                {
                    new DAL.Tbo.Tbo_DataSetGet().setLogoutByTokenID(TokenId);
                    System.Web.HttpRuntime.Cache["TokenID"] = new TboClass.TboToken();
                    goto Start;
                }
                else if (Response.Response.ResponseStatus == 1 && Response.Response.Error.ErrorCode == 0)
                {
                    new TboResponseMapping().getResults(request, ref Response, ref flightResponse);
                }
                else
                {
                    flightResponse.response.status = Core.TransactionStatus.Error;
                    flightResponse.response.message = "no result found";
                }
            }
            if (FlightUtility.isWriteLogSearch)
            {
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
                //LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\TripJack\\Search", request.userSearchID );
            }
            if (flightResponse.Results.Count == 0 || (flightResponse.Results.Count > 0 && flightResponse.Results.FirstOrDefault().Count == 0))
            {
                new LogWriter("No" + Environment.NewLine, "tbo" + DateTime.Today.ToString("ddMMMyy"), "NoResult");
            }

            return flightResponse;
        }
        public Core.Flight.CalendarFareResponse getCalendarFare(FlightSearchRequest request)
        {
            Core.Flight.CalendarFareResponse _response = new CalendarFareResponse() { response = new ResponseStatus(), SearchResults = new List<SearchResult>() };
            StringBuilder sbLogger = new StringBuilder();
            if (!request.isMetaRequest || FlightUtility.isWriteLog)
            {
                // bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
            }


            Start:
            ServicesHub.Tbo.TboAuthentication obj = new ServicesHub.Tbo.TboAuthentication();
            string TokenId = obj.getTokenID();
            var Url = TboAuthentication.CalendarFareUrl;
            string strRequest = new TboRequestMappking().getCalendarFareUrlRequest(request, TokenId);
            if (!request.isMetaRequest || FlightUtility.isWriteLog)
            {
                bookingLog(ref sbLogger, "Tbo CalendarFare Request", strRequest);
            }
            var strResponse = GetResponse(Url, strRequest);


            if (!request.isMetaRequest || FlightUtility.isWriteLog)
            {
                bookingLog(ref sbLogger, "Tbo CalendarFare Response", strResponse);
                new LogWriter(sbLogger.ToString(), ("Tbo_Cal_" + request.segment[0].originAirport + "_" + request.segment[0].destinationAirport), "Tbo");
            }
            if (!string.IsNullOrEmpty(strResponse))
            {
                TboClass.CalendarFareResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<TboClass.CalendarFareResponse>(strResponse);
                if (Response.Response.ResponseStatus == 3 && Response.Response.Error.ErrorCode == 3 && Response.Response.Error.ErrorMessage.ToUpper() == "INVALID SESSION")
                {
                    new DAL.Tbo.Tbo_DataSetGet().setLogoutByTokenID(TokenId);
                    System.Web.HttpRuntime.Cache["TokenID"] = new TboClass.TboToken();
                    goto Start;
                }
                else if (Response.Response.ResponseStatus == 1 && Response.Response.Error.ErrorCode == 0)
                {
                    new TboResponseMapping().getCalendarFare(ref request, ref Response, ref _response);
                }
                else
                {
                    _response.response.status = Core.TransactionStatus.Error;
                    _response.response.message = "no result found";
                }
            }
            else
            {
                _response.response.status = Core.TransactionStatus.Error;
                _response.response.message = "no result found";
            }
            return _response;
        }
        public Core.Flight.FlightResult getCalendarFareUpdate(CalendarFareUpdateRequest request)
        {
            Core.Flight.FlightResult _response = new FlightResult();
            StringBuilder sbLogger = new StringBuilder();
            if (FlightUtility.isWriteLog)
            {
                // bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
            }


            Start:
            ServicesHub.Tbo.TboAuthentication obj = new ServicesHub.Tbo.TboAuthentication();
            string TokenId = obj.getTokenID();
            var Url = TboAuthentication.CalendarFareUpdate;
            string strRequest = new TboRequestMappking().getCalendarFareUpdateRequest(request, TokenId);
            if (FlightUtility.isWriteLog)
            {
                bookingLog(ref sbLogger, "Tbo CalendarFareUpdate Request", strRequest);
            }
            var strResponse = GetResponse(Url, strRequest);


            if (FlightUtility.isWriteLog)
            {
                bookingLog(ref sbLogger, "Tbo CalendarFareUpdate Response", strResponse);
                new LogWriter(sbLogger.ToString(), "Tbo_CalendarFareUpdate" + DateTime.Today.ToString("ddMMyy"), "Tbo");
            }
            //if (!string.IsNullOrEmpty(strResponse))
            //{
            //    TboClass.CalendarFareResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<TboClass.CalendarFareResponse>(strResponse);
            //    if (Response.Response.ResponseStatus == 3 && Response.Response.Error.ErrorCode == 3 && Response.Response.Error.ErrorMessage.ToUpper() == "INVALID SESSION")
            //    {
            //        new DAL.Tbo.Tbo_DataSetGet().setLogoutByTokenID(TokenId);
            //        System.Web.HttpRuntime.Cache["TokenID"] = new TboClass.TboToken();
            //        goto Start;
            //    }
            //    else if (Response.Response.ResponseStatus == 1 && Response.Response.Error.ErrorCode == 0)
            //    {
            //        new ResponseMapping().getCalendarFare(ref request, ref Response, ref _response);
            //    }
            //    else
            //    {
            //        _response.response.status = Core.TransactionStatus.Error;
            //        _response.response.message = "no result found";
            //    }
            //}
            //else
            //{
            //    _response.response.status = Core.TransactionStatus.Error;
            //    _response.response.message = "no result found";
            //}
            return _response;
        }
        public Core.Flight.FareQuoteResponse GetFareQuote(Core.Flight.PriceVerificationRequest request)
        {
            StringBuilder sbLogger = new StringBuilder();
            if (FlightUtility.isWriteLog)
            {
                bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
            }
            ServicesHub.Tbo.TboAuthentication obj = new ServicesHub.Tbo.TboAuthentication();
            string TokenId = obj.getTokenID();
            FareQuoteResponse _response = new FareQuoteResponse() { flightResult = new List<FlightResult>(), isFareChange = false, responseStatus = new ResponseStatus(), fareIncreaseAmount = 0 };// Newfare = new List<Fare>(),
            try
            {
                int ctr = 0;
                foreach (FlightResult fr in request.flightResult)
                {
                    var Url = TboAuthentication.FareQuoteUrl;

                    string strRequest = new TboRequestMappking().getFareQuoteRequest(fr.Fare.tboResultIndex, request.TvoTraceId, request.userIP, TokenId);

                    bookingLog(ref sbLogger, "TBO fareQuote Request", strRequest);

                    var strResponse = GetResponse(Url, strRequest);

                    bookingLog(ref sbLogger, "TBO fareQuote Response", strResponse);

                    if (!string.IsNullOrEmpty(strResponse))
                    {
                        TboClass.FareQuoteResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<TboClass.FareQuoteResponse>(strResponse);
                        new TboResponseMapping().getFareQuoteResponse(ref request, ref Response, ref _response, ctr);
                    }
                    else
                    {
                        _response.responseStatus.status = TransactionStatus.Error;
                    }
                    ctr++;
                }



                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");

            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
                bookingLog(ref sbLogger, "Exception", ex.ToString());
                new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "TBO FareQuote Exeption");
            }
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            return _response;
        }
        public bool GetSsrDetails(Core.Flight.PriceVerificationRequest request)
        {
            StringBuilder sbLogger = new StringBuilder();

            ServicesHub.Tbo.TboAuthentication obj = new ServicesHub.Tbo.TboAuthentication();
            string TokenId = obj.getTokenID();
            var Url = TboAuthentication.SsrUrl; //ConfigurationManager.AppSettings["SsrUrl"].ToString();
            int ctr = 0;
            foreach (var item in request.flightResult)
            {
                string strRequest = new TboRequestMappking().GetSsrRequest(request, ctr, TokenId);
                if (FlightUtility.isWriteLog)
                {
                    bookingLog(ref sbLogger, "SSR Request", strRequest);
                }
                var strResponse = GetResponse(Url, strRequest);
                if (FlightUtility.isWriteLog)
                {
                    bookingLog(ref sbLogger, "SSR Response", strResponse);
                }
                ctr++;
            }
            if (FlightUtility.isWriteLog)
            {
                //new LogWriter(sbLogger.ToString(), "Tbo_SSR" + DateTime.Today.ToString("ddMMyy"), "Tbo");
                LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\Tbo\\CheckFare", request.userSearchID, "SSR.txt");
            }
            return true;
        }
        public List<FareRuleResponses> GetFareRule(Core.Flight.PriceVerificationRequest request)
        {
            StringBuilder sbLogger = new StringBuilder();
            List<FareRuleResponses> _response = new List<FareRuleResponses>();
            try
            {
                ServicesHub.Tbo.TboAuthentication obj = new ServicesHub.Tbo.TboAuthentication();
                string TokenId = obj.getTokenID();
                var Url = TboAuthentication.fareRuleUrl;
                int ctr = 0;
                foreach (var item in request.flightResult)
                {
                    string strRequest = new TboRequestMappking().GetFareRuleRequest(request, ctr, TokenId);
                    if (FlightUtility.isWriteLog)
                    {
                        bookingLog(ref sbLogger, "TBO FareRule Request", strRequest);
                        // bookingLog(ref sbLogger, "FareRule Request", strRequest);
                    }
                    var strResponse = GetResponse(Url, strRequest);
                    if (FlightUtility.isWriteLog)
                    {
                        bookingLog(ref sbLogger, "TBO FareRule Response", strResponse);
                        // bookingLog(ref sbLogger, "FareRule Response", strResponse);
                    }
                    new TboResponseMapping().getFareRuleResponse(strResponse, ref _response);
                    ctr++;
                }
                if (FlightUtility.isWriteLog)
                {
                    //  LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\Tbo\\CheckFare", request.userSearchID, "FareRule.txt");
                    new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "FareRule");
                }
            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
                bookingLog(ref sbLogger, "Exception", ex.ToString());
                new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "TBO FareRule Exeption");
            }
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            return _response;
        }

        public void BookFlight(FlightBookingRequest request, ref FlightBookingResponse _response)
        {
            StringBuilder sbLogger = new StringBuilder();
            try
            {
                ServicesHub.Tbo.TboAuthentication obj = new ServicesHub.Tbo.TboAuthentication();
                string TokenId = obj.getTokenID();
                int ctr = 0;
                bool isMakeBooking = true;
                foreach (var item in request.flightResult)
                {
                    if (isMakeBooking)
                    {
                        if (item.IsLCC)
                        {
                            #region Make booking for LCC
                            var Url = TboAuthentication.ticketUrl;
                            string strRequest = new TboRequestMappking().getLccTicketingRequest(request, ctr, TokenId);
                            bookingLog(ref sbLogger, "TBO Lcc Ticketing Request", strRequest);
                            var response = GetResponse(Url, strRequest);
                            bookingLog(ref sbLogger, "TBO Lcc Ticketing Response", response);
                            if (!string.IsNullOrEmpty(response))
                            {
                                TboClass.LccTicketingResponse bookResponse = JsonConvert.DeserializeObject<TboClass.LccTicketingResponse>(response.ToString());
                                if (bookResponse.Response.ResponseStatus == "1")
                                {
                                    if (ctr == 0)
                                    {
                                        _response.PNR = bookResponse.Response.Response.PNR;
                                        bookingLog(ref sbLogger, "TBO Response PNR", bookResponse.Response.Response.PNR);
                                        //_response.TvoBookingID = bookResponse.Response.Response.BookingId;
                                        _response.responseStatus.message += "; OutBoundPnr-" + bookResponse.Response.Response.PNR;
                                        bookingLog(ref sbLogger, "TBO Response Status Message OutBoundPnr", bookResponse.Response.Response.PNR);
                                        if (bookResponse.Response.Response.FlightItinerary != null && bookResponse.Response.Response.FlightItinerary.InvoiceAmount > 0)
                                            _response.invoice.Add(new Invoice() { InvoiceAmount = bookResponse.Response.Response.FlightItinerary.InvoiceAmount, InvoiceNo = bookResponse.Response.Response.FlightItinerary.InvoiceNo });
                                        _response.TvoInvoiceNo = bookResponse.Response.Response.FlightItinerary.InvoiceNo;
                                        bookingLog(ref sbLogger, "TBO Response Invoice", bookResponse.Response.Response.FlightItinerary.InvoiceNo);
                                        _response.bookingStatus = BookingStatus.Ticketed;
                                    }
                                    else
                                    {
                                        _response.ReturnPNR = bookResponse.Response.Response.PNR;
                                        bookingLog(ref sbLogger, "TBO Return PNR", _response.ReturnPNR);
                                        _response.TvoReturnBookingID = bookResponse.Response.Response.BookingId;
                                        bookingLog(ref sbLogger, "TBO ReturnBookingID", bookResponse.Response.Response.BookingId.ToString());
                                        _response.responseStatus.message += "; InBoundPnr-" + bookResponse.Response.Response.PNR;
                                        bookingLog(ref sbLogger, "TBO Return Response Status Message InBoundPnr", bookResponse.Response.Response.PNR);
                                        if (bookResponse.Response.Response.FlightItinerary != null && bookResponse.Response.Response.FlightItinerary.InvoiceAmount > 0)
                                            _response.invoice.Add(new Invoice() { InvoiceAmount = bookResponse.Response.Response.FlightItinerary.InvoiceAmount, InvoiceNo = bookResponse.Response.Response.FlightItinerary.InvoiceNo });
                                        _response.TvoInvoiceNo = bookResponse.Response.Response.FlightItinerary.InvoiceNo;
                                        bookingLog(ref sbLogger, "TBO Return Response Invoice", bookResponse.Response.Response.FlightItinerary.InvoiceNo);
                                        _response.bookingStatus = BookingStatus.Ticketed;
                                    }
                                    //_response.bookingStatus = BookingStatus.Ticketed;
                                }
                                else
                                {
                                    _response.bookingStatus = BookingStatus.Failed;
                                    _response.responseStatus.message += "; Booking Fail Due to" + bookResponse.Response.Error.ErrorMessage;
                                    isMakeBooking = false;
                                    bookingLog(ref sbLogger, "TBO  Else1", "Booking Fail Due to :" + bookResponse.Response.Error.ErrorMessage);
                                }
                            }
                            else
                            {
                                _response.bookingStatus = BookingStatus.InProgress;
                                _response.responseStatus.message = "InProgress";
                                bookingLog(ref sbLogger, "TBO  Else InProgress", "Booking InProgress Due to :" + _response.responseStatus.message);
                            }
                            _response.isTickted.Add(true);
                            #endregion
                        }
                        else
                        {
                            #region Make Booking for GDS
                            var Url = TboAuthentication.bookUrl;
                            string strRequest = new TboRequestMappking().getGdsBookingRequest(request, ctr, TokenId);
                            bookingLog(ref sbLogger, "TBO Gds Booking Request", strRequest);
                            var response = GetResponse(Url, strRequest);
                            bookingLog(ref sbLogger, "TBO Gds Booking Response", response);
                            if (!string.IsNullOrEmpty(response))
                            {
                                TboClass.LccTicketingResponse bookResponse = JsonConvert.DeserializeObject<TboClass.LccTicketingResponse>(response.ToString());
                                if (bookResponse.Response.ResponseStatus == "1")
                                {
                                    if (ctr == 0)
                                    {
                                        _response.PNR = bookResponse.Response.Response.PNR;
                                        bookingLog(ref sbLogger, "TBO Gds Booking PNR", bookResponse.Response.Response.PNR);
                                        _response.TvoBookingID = bookResponse.Response.Response.BookingId;
                                        bookingLog(ref sbLogger, "TBO Gds BookingID", bookResponse.Response.Response.BookingId.ToString());
                                        _response.responseStatus.message += "; OutBoundPnr-" + bookResponse.Response.Response.PNR;
                                        bookingLog(ref sbLogger, "TBO Gds Booking Message OutBoundPnr", bookResponse.Response.Response.PNR);
                                        //if (bookResponse.Response.Response.FlightItinerary != null && bookResponse.Response.Response.FlightItinerary.InvoiceAmount > 0)
                                        //{
                                        //    _response.invoice.Add(new Invoice() { InvoiceAmount = bookResponse.Response.Response.FlightItinerary.InvoiceAmount, InvoiceNo = bookResponse.Response.Response.FlightItinerary.InvoiceNo });
                                        //}
                                        _response.bookingStatus = BookingStatus.Ticketed;
                                        if (bookResponse.Response.Response.IsPriceChanged == true)
                                        {
                                            _response.bookingStatus = BookingStatus.InProgress;
                                            _response.responseStatus.message += "; Booking Fail Due to Fare Change In Booking Time";
                                            isMakeBooking = false;
                                            bookingLog(ref sbLogger, "TBO  Else5", "Booking Fail Due to :  Fare Change In Booking Time");
                                        }
                                        else
                                        {
                                            #region Ticketing Non Lcc
                                            var UrlTicketing = TboAuthentication.ticketUrl;
                                            string strRequestTicketing = new TboRequestMappking().getGdsTicketingRequest(request, TokenId, bookResponse.Response.Response.PNR, bookResponse.Response.Response.BookingId);
                                            bookingLog(ref sbLogger, "TBO GDS Ticketing Request", strRequest);
                                            var responseTicketing = GetResponse(UrlTicketing, strRequestTicketing);
                                            bookingLog(ref sbLogger, "TBO GDS Ticketing Response", responseTicketing);
                                            if (!string.IsNullOrEmpty(responseTicketing))
                                            {
                                                TboClass.LccTicketingResponse bookResponseTicketing = JsonConvert.DeserializeObject<TboClass.LccTicketingResponse>(responseTicketing.ToString());
                                                if (bookResponseTicketing.Response.ResponseStatus == "1")
                                                {
                                                    if (bookResponseTicketing.Response.Response.FlightItinerary != null && bookResponseTicketing.Response.Response.FlightItinerary.InvoiceAmount > 0)
                                                        _response.invoice.Add(new Invoice() { InvoiceAmount = bookResponseTicketing.Response.Response.FlightItinerary.InvoiceAmount, InvoiceNo = bookResponseTicketing.Response.Response.FlightItinerary.InvoiceNo });
                                                    _response.TvoInvoiceNo = bookResponse.Response.Response.FlightItinerary.InvoiceNo;
                                                    bookingLog(ref sbLogger, "TBO GDS Ticketing Response InvoiceNo", bookResponse.Response.Response.FlightItinerary.InvoiceNo);
                                                    _response.bookingStatus = BookingStatus.Ticketed;
                                                }
                                                else
                                                {
                                                    _response.bookingStatus = BookingStatus.Failed;
                                                    _response.responseStatus.message += "; Booking Fail Due to" + bookResponseTicketing.Response.Error.ErrorMessage;
                                                    isMakeBooking = false;
                                                    bookingLog(ref sbLogger, "TBO  Else2", "Booking Fail Due to :" + bookResponse.Response.Error.ErrorMessage);
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        _response.ReturnPNR = bookResponse.Response.Response.PNR;
                                        bookingLog(ref sbLogger, "Else TBO Gds Booking PNR", bookResponse.Response.Response.PNR);
                                        _response.TvoReturnBookingID = bookResponse.Response.Response.BookingId;
                                        bookingLog(ref sbLogger, "Else TBO Gds Booking Id", bookResponse.Response.Response.BookingId.ToString());
                                        _response.responseStatus.message += "; InBoundPnr-" + bookResponse.Response.Response.PNR;
                                        bookingLog(ref sbLogger, "Else TBO Gds InBoundPnr", bookResponse.Response.Response.PNR);
                                        //if (bookResponse.Response.Response.FlightItinerary != null && bookResponse.Response.Response.FlightItinerary.InvoiceAmount > 0)
                                        //{
                                        //    _response.invoice.Add(new Invoice() { InvoiceAmount = bookResponse.Response.Response.FlightItinerary.InvoiceAmount, InvoiceNo = bookResponse.Response.Response.FlightItinerary.InvoiceNo });
                                        //}
                                        _response.bookingStatus = BookingStatus.Ticketed;
                                        if (bookResponse.Response.Response.IsPriceChanged == true)
                                        {
                                            _response.bookingStatus = BookingStatus.InProgress;
                                            _response.responseStatus.message += "; Booking Fail Due to Fare Change In RoundTrip Booking Time";
                                            isMakeBooking = false;
                                            bookingLog(ref sbLogger, "TBO  Else6", "Booking Fail Due to :  Fare Change In RoundTrip Booking Time");
                                        }
                                        else
                                        {
                                            #region Ticketing Non Lcc
                                            var UrlTicketing = TboAuthentication.ticketUrl;    //ConfigurationManager.AppSettings["ticketUrl"].ToString();
                                            string strRequestTicketing = new TboRequestMappking().getGdsTicketingRequest(request, TokenId, bookResponse.Response.Response.PNR, bookResponse.Response.Response.BookingId);
                                            bookingLog(ref sbLogger, "Else TBO Gds Ticketing Request", strRequestTicketing);

                                            var responseTicketing = GetResponse(UrlTicketing, strRequestTicketing);
                                            bookingLog(ref sbLogger, "Else TBO Gds Ticketing Response", responseTicketing);

                                            if (!string.IsNullOrEmpty(responseTicketing))
                                            {
                                                TboClass.LccTicketingResponse bookResponseTicketing = JsonConvert.DeserializeObject<TboClass.LccTicketingResponse>(responseTicketing.ToString());
                                                if (bookResponseTicketing.Response.ResponseStatus == "1")
                                                {
                                                    if (bookResponseTicketing.Response.Response.FlightItinerary != null && bookResponseTicketing.Response.Response.FlightItinerary.InvoiceAmount > 0)
                                                        _response.invoice.Add(new Invoice() { InvoiceAmount = bookResponseTicketing.Response.Response.FlightItinerary.InvoiceAmount, InvoiceNo = bookResponseTicketing.Response.Response.FlightItinerary.InvoiceNo });
                                                    _response.TvoInvoiceNo = bookResponse.Response.Response.FlightItinerary.InvoiceNo;
                                                    _response.bookingStatus = BookingStatus.Ticketed;
                                                    bookingLog(ref sbLogger, "Else TBO Gds Ticketing TvoInvoiceNo", bookResponse.Response.Response.FlightItinerary.InvoiceNo);
                                                }
                                                else
                                                {
                                                    _response.bookingStatus = BookingStatus.Failed;
                                                    _response.responseStatus.message += "; Booking Fail Due to" + bookResponseTicketing.Response.Error.ErrorMessage;
                                                    isMakeBooking = false;
                                                    bookingLog(ref sbLogger, "TBO  Else3", "Booking Fail Due to :" + bookResponse.Response.Error.ErrorMessage);
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    //_response.bookingStatus = BookingStatus.Ticketed;
                                }
                                else
                                {
                                    _response.bookingStatus = BookingStatus.Failed;
                                    _response.responseStatus.message += "; Booking Fail Due to" + bookResponse.Response.Error.ErrorMessage;
                                    isMakeBooking = false;
                                    bookingLog(ref sbLogger, "TBO  Else4", "Booking Fail Due to :" + bookResponse.Response.Error.ErrorMessage);
                                }
                            }
                            else
                            {
                                _response.bookingStatus = BookingStatus.InProgress;
                                _response.responseStatus.message = "InProgress";
                                bookingLog(ref sbLogger, "TBO  Else InProgress", "Booking InProgress Due to :" + _response.responseStatus.message);
                            }
                            _response.isTickted.Add(true);
                            #endregion
                        }
                        ctr++;
                    }
                    else
                    {
                        _response.bookingStatus = BookingStatus.InProgress;
                        _response.responseStatus.message += "; Booking Fail Due to Outbound booking fail";
                        bookingLog(ref sbLogger, "TBO  Else5", "Booking Fail Due to :" + _response.responseStatus.message);
                    }
                }
            }
            catch (Exception ex)
            {
                _response.bookingStatus = BookingStatus.InProgress;
                _response.responseStatus.message = "Booking InProgress";
                bookingLog(ref sbLogger, "TBO Flight Booking Exption", ex.ToString());
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Error");
            }
            bookingLog(ref sbLogger, "TBO  return Response", JsonConvert.SerializeObject(_response));
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Booking");
            //return _response;
        }

        //public FlightBookingResponse TicketFlights(FlightBookingRequest request)
        //{
        //    StringBuilder sbLogger = new StringBuilder();
        //    if (FlightUtility.isWriteLog)
        //    {
        //        bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
        //    }
        //    FlightBookingResponse _response = new FlightBookingResponse(request);
           


        //    ServicesHub.Tbo.TboAuthentication obj = new ServicesHub.Tbo.TboAuthentication();
        //    string TokenId = obj.getTokenID();
        //    int ctr = 0;
        //    foreach (var item in request.flightResult)
        //    {
        //        if (request.isTickted[ctr] == false)
        //        {
        //            var Url = TboAuthentication.ticketUrl;    //ConfigurationManager.AppSettings["ticketUrl"].ToString();
        //            string strRequest = "";// new RequestMappking().getGdsTicketingRequest(request, ctr, TokenId);

        //            if (FlightUtility.isWriteLog)
        //            {
        //                bookingLog(ref sbLogger, "GDS Ticketing Request", strRequest);
        //            }
        //            try
        //            {
        //                var response = GetResponse(Url, strRequest);
        //                if (FlightUtility.isWriteLog)
        //                {
        //                    bookingLog(ref sbLogger, "GDS Ticketing Response", response);
        //                }
        //                if (!string.IsNullOrEmpty(response))
        //                {
        //                    TboClass.LccTicketingResponse bookResponse = JsonConvert.DeserializeObject<TboClass.LccTicketingResponse>(response.ToString());
        //                    if (bookResponse.Response.ResponseStatus == "1")
        //                    {
        //                        if (ctr == 0)
        //                        {
        //                            _response.PNR = bookResponse.Response.Response.PNR;
        //                            _response.TvoBookingID = bookResponse.Response.Response.BookingId;
        //                            if (bookResponse.Response.Response.FlightItinerary != null && bookResponse.Response.Response.FlightItinerary.InvoiceAmount > 0)
        //                                _response.invoice.Add(new Invoice() { InvoiceAmount = bookResponse.Response.Response.FlightItinerary.InvoiceAmount, InvoiceNo = bookResponse.Response.Response.FlightItinerary.InvoiceNo });
        //                        }
        //                        else
        //                        {
        //                            _response.ReturnPNR = bookResponse.Response.Response.PNR;
        //                            _response.TvoReturnBookingID = bookResponse.Response.Response.BookingId;
        //                            if (bookResponse.Response.Response.FlightItinerary != null && bookResponse.Response.Response.FlightItinerary.InvoiceAmount > 0)
        //                                _response.invoice.Add(new Invoice() { InvoiceAmount = bookResponse.Response.Response.FlightItinerary.InvoiceAmount, InvoiceNo = bookResponse.Response.Response.FlightItinerary.InvoiceNo });
        //                        }
        //                        _response.bookingStatus = BookingStatus.Ticketed;
        //                    }
        //                    else
        //                    {
        //                        _response.bookingStatus = BookingStatus.Failed;
        //                    }
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                _response.bookingStatus = BookingStatus.Failed;
        //            }
        //        }
        //        _response.isTickted.Add(true);
        //        ctr++;
        //    }
        //    if (FlightUtility.isWriteLog)
        //    {
        //        //new LogWriter(sbLogger.ToString(), "Tbo_NonLccTicketing_" + DateTime.Today.ToString("ddMMyy"), "Tbo");
        //        LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\Booking", request.userSearchID, "NonLccTicketing.txt");
        //    }
        //    return _response;
        //}

        public Core.Flight.FlightBookingResponse GetBookingDetails(FlightBookingRequest request)
        {
            StringBuilder sbLogger = new StringBuilder();

            var Url = TboAuthentication.BookingDetailsUrl; //ConfigurationManager.AppSettings["BookingDetailsUrl"].ToString();
            ServicesHub.Tbo.TboAuthentication obj = new ServicesHub.Tbo.TboAuthentication();
            string TokenId = obj.getTokenID();
            int ctr = 0;
            if (!string.IsNullOrEmpty(request.PNR))
            {
                string strRequest = "";// new RequestMappking().GetBookingDetailsRequestStr(request, ctr, TokenId);

                if (FlightUtility.isWriteLog)
                {
                    bookingLog(ref sbLogger, "GetBookingDetails Request", strRequest);
                }
                var response = GetResponse(Url, strRequest);
                if (FlightUtility.isWriteLog)
                {
                    bookingLog(ref sbLogger, "GetBookingDetails Response", response);
                }

                ctr++;
            }
            if (!string.IsNullOrEmpty(request.ReturnPNR))
            {
                string strRequest = "";// new TboRequestMappking().GetBookingDetailsRequestStr(request, ctr, TokenId);

                if (FlightUtility.isWriteLog)
                {
                    bookingLog(ref sbLogger, "GetBookingDetails Request", strRequest);
                }
                var response = GetResponse(Url, strRequest);
                if (FlightUtility.isWriteLog)
                {
                    bookingLog(ref sbLogger, "GetBookingDetails Response", response);
                }

                ctr++;
            }
            if (FlightUtility.isWriteLog)
            {
                //new LogWriter(sbLogger.ToString(), "Tbo_GetBookingDetails_" + DateTime.Today.ToString("ddMMyy"), "Tbo");
                LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\Booking", request.userSearchID, "GetBookingDetails.txt");
            }
            return null;
        }
        public TboClass.AgencyBalanceResponse GetAgencyBalance()
        {
            //string TokenId = new ServicesHub.Tbo.TboAuthentication().getTokenID();
            StringBuilder sbLogger = new StringBuilder();
            TboClass.AgencyBalanceResponse agencyBalanceResponse = null;
            var Url = TboAuthentication.AgencyBalanceUrl;  // ConfigurationManager.AppSettings["AgencyBalanceUrl"].ToString();
            string strRequest = new TboRequestMappking().GetAgencyBalanceRequest();
            if (FlightUtility.isWriteLog)
            {
                bookingLog(ref sbLogger, "Agency Balance Request", strRequest);
            }
            var response = GetResponse(Url, strRequest);
            if (FlightUtility.isWriteLog)
            {
                bookingLog(ref sbLogger, "Agency Balance Response", response);
            }
            if (!string.IsNullOrEmpty(response))
            {
                agencyBalanceResponse = JsonConvert.DeserializeObject<TboClass.AgencyBalanceResponse>(response.ToString());
            }
            if (FlightUtility.isWriteLog)
            {
                new LogWriter(sbLogger.ToString(), "Tbo_AgencyBalance_" + DateTime.Today.ToString("ddMMyy"), "Tbo");
            }
            return agencyBalanceResponse;
        }
        public void bookingLog(ref StringBuilder sbLogger, string requestTitle, string logText)
        {
            sbLogger.Append(Environment.NewLine + "---------------------------------------------" + requestTitle + "" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
            sbLogger.Append(Environment.NewLine + logText);
            sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
        }
        private string GetResponse(string url, string requestData)
        {
            string response = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                WebResponse webResponse = request.GetResponse();
                var rsp = webResponse.GetResponseStream();
                 using (StreamReader reader = new StreamReader(rsp))
                {
                    response = reader.ReadToEnd();
                }
               
            }
            catch (WebException webEx)
            {
                if (webEx != null)
                {
                    if (webEx.Message.Contains("timed out") == false && webEx.Response != null)
                    {
                        WebResponse errResp = webEx.Response;
                        Stream responseStream = null;
                        if (errResp.Headers.Get("Content-Encoding") == "gzip")
                        {
                            responseStream = new System.IO.Compression.GZipStream(errResp.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                        }
                        else
                        {
                            responseStream = errResp.GetResponseStream();
                        }
                        StreamReader reader = new StreamReader(responseStream);
                        response = reader.ReadToEnd();

                    }
                }
            }
            catch { }
            return response;
        }


        private string GetResponseSearch(string url, string requestData)
        {
            string response = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
              //  request.Timeout = 12000;
                //request.Headers.Add("Accept-Encoding", "gzip");
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                WebResponse webResponse = request.GetResponse();
                var rsp = webResponse.GetResponseStream();
                using (StreamReader reader = new StreamReader(rsp))
                {
                    response = reader.ReadToEnd();
                }               
            }
            catch (WebException webEx)
            {
                if (webEx != null)
                {
                    if (webEx.Message.Contains("timed out") == false && webEx.Response != null)
                    {
                        WebResponse errResp = webEx.Response;
                        Stream responseStream = null;
                        if (errResp.Headers.Get("Content-Encoding") == "gzip")
                        {
                            responseStream = new System.IO.Compression.GZipStream(errResp.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                        }
                        else
                        {
                            responseStream = errResp.GetResponseStream();
                        }
                        StreamReader reader = new StreamReader(responseStream);
                        response = reader.ReadToEnd();

                    }
                }
            }
            catch { }
            return response;
        }
    }
}
