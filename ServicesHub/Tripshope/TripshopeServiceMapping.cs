using Core;
using Core.Flight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tripshope
{
    public class TripshopeServiceMapping
    {
        string URL = ConfigurationManager.AppSettings["TS_Url"].ToString();
        public FlightSearchResponseShort GetFlightResults(FlightSearchRequest request)
        {
            string errorMsg = string.Empty;
            FlightSearchResponseShort flightResponse = new FlightSearchResponseShort(request);
            StringBuilder sbLogger = new StringBuilder();
            try
            {
                string strRequest = new TripshopeRequestMappking().getFlightSearchRequest(request);
                if (FlightUtility.isWriteLog)
                {
                    bookingLog(ref sbLogger, "TripShope Request", strRequest);
                }
                var strResponse = GetResponseSearch(URL + "nextra-flight-search.api", strRequest, ref errorMsg);

                if (FlightUtility.isWriteLog)
                {
                    bookingLog(ref sbLogger, "TripShope Response", strResponse.Trim());
                }
                if (!string.IsNullOrEmpty(strResponse))
                {
                    TripshopeClass.SearchResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<TripshopeClass.SearchResponse>(strResponse);
                    bookingLog(ref sbLogger, "TripShope Response1", JsonConvert.SerializeObject(Response));
                    if (Response != null && Response.flightsearchresponse != null && Response.flightsearchresponse.statuscode == "200")/*&& Response.flightsearchresponse.totalresults > 0 */
                    {
                        new TripshopeResponseMapping().getResults(request, ref Response, ref flightResponse);
                    }
                    else
                    {
                        flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                        errorMsg += "Error";
                        flightResponse.response.status = Core.TransactionStatus.Error;
                        flightResponse.response.message = "no result found";
                    }
                }

                if (FlightUtility.isWriteLog)
                {
                    bookingLog(ref sbLogger, "TripShope errorMsg", errorMsg);
                    new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
                }
            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
                bookingLog(ref sbLogger, "Exception", ex.ToString());
                new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "Tripshope Search Exeption");
            }
            return flightResponse;
        }



        public List<FareRuleResponses> GetFareRule(Core.Flight.PriceVerificationRequest request)
        {
            string errorMsg = string.Empty;
            StringBuilder sbLogger = new StringBuilder();
            List<FareRuleResponses> _response = new List<FareRuleResponses>();
            try
            {
                var Url = URL + "nextra-flight-farerules.api";
                bookingLog(ref sbLogger, "TripShope Search URL", Url);
                int ctr = 0;
                foreach (var item in request.flightResult)
                {
                    string strRequest = new TripshopeRequestMappking().GetFareRuleRequest(request, ctr);
                    bookingLog(ref sbLogger, "TripShope FareRule Request", strRequest);
                    var strResponse = GetResponseSearch(Url, strRequest, ref errorMsg);
                    bookingLog(ref sbLogger, "TripShope FareRule Response", strResponse);
                    ctr++;
                }
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
                bookingLog(ref sbLogger, "Exception", ex.ToString());
                new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "TripShope FareRule Exeption");
            }
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            return _response;
        }


        public Core.Flight.FareQuoteResponse GetFareQuote(Core.Flight.PriceVerificationRequest request)
        {
            string errorMsg = string.Empty;
            StringBuilder sbLogger = new StringBuilder();

            bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));

            FareQuoteResponse _response = new FareQuoteResponse() { flightResult = new List<FlightResult>(), isFareChange = false, responseStatus = new ResponseStatus(), fareIncreaseAmount = 0 };// Newfare = new List<Fare>(),
            try
            {
                int ctr = 0;
                foreach (FlightResult fr in request.flightResult)
                {
                    var Url = URL + "nextra-flight-reprice.api";
                    bookingLog(ref sbLogger, "TripShope fareQuote Request Url", Url);
                    string strRequest = new TripshopeRequestMappking().getFareQuoteRequest(request, ctr);

                    bookingLog(ref sbLogger, "TripShope fareQuote Request", strRequest);

                    var strResponse = GetResponseSearch(Url, strRequest, ref errorMsg);

                    bookingLog(ref sbLogger, "TripShope fareQuote Response", strResponse);

                    if (!string.IsNullOrEmpty(strResponse))
                    {
                        GetFareQuoteResponse.Root res = JsonConvert.DeserializeObject<GetFareQuoteResponse.Root>(strResponse);
                        new TripshopeResponseMapping().getFareQuoteResponse(ref request, ref res, ref _response, ctr);
                    }
                    else
                    {
                        _response.responseStatus.status = TransactionStatus.Error;
                    }
                    ctr++;
                }
            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
                bookingLog(ref sbLogger, "Exception", ex.ToString());
                new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "TripShope FareQuote Exeption");
            }
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            return _response;
        }

        //public Core.Flight.SSRResponse GetSSR(Core.Flight.PriceVerificationRequest request)
        //{
        //    string errorMsg = string.Empty;
        //    StringBuilder sbLogger = new StringBuilder();

        //    bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));

        //    FareQuoteResponse _response = new FareQuoteResponse() { flightResult = new List<FlightResult>(), isFareChange = false, responseStatus = new ResponseStatus(), fareIncreaseAmount = 0 };// Newfare = new List<Fare>(),
        //    try
        //    {
        //        int ctr = 0;
        //        foreach (FlightResult fr in request.flightResult)
        //        {
        //            var Url = URL + "nextra-flight-getssr.api";

        //            string strRequest = new TripshopeRequestMappking().getSSRRequest(request, ctr);

        //            bookingLog(ref sbLogger, "TripShope SSR Request", strRequest);

        //            var strResponse = GetResponseSearch(Url, strRequest, ref errorMsg);

        //            bookingLog(ref sbLogger, "TripShope SSR Response", strResponse);

        //            if (!string.IsNullOrEmpty(strResponse))
        //            {
        //                //  GetFareQuoteResponse.Root res = JsonConvert.DeserializeObject<GetFareQuoteResponse.Root>(strResponse);
        //                // new TripshopeResponseMapping().getFareQuoteResponse(ref request, ref res, ref _response, ctr);
        //            }
        //            else
        //            {
        //                //  _response.responseStatus.status = TransactionStatus.Error;
        //            }
        //            ctr++;
        //        }
        //        new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
        //    }
        //    catch (Exception ex)
        //    {
        //        bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
        //        bookingLog(ref sbLogger, "Exception", ex.ToString());
        //        new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "TripShope SSR Exeption");
        //    }
        //    new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
        //    return true;
        //}

        public bool GetSsrDetails(Core.Flight.PriceVerificationRequest request)
        {
            string errorMsg = string.Empty;
            StringBuilder sbLogger = new StringBuilder();


            var Url = URL + "nextra-flight-getssr.api";
            bookingLog(ref sbLogger, "TripShope SSR Request Url", Url);
            int ctr = 0;
            foreach (var item in request.flightResult)
            {
                string strRequest = new TripshopeRequestMappking().getSSRRequest(request, ctr);

                bookingLog(ref sbLogger, "TripShope SSR Request", strRequest);

                var strResponse = GetResponseSearch(Url, strRequest, ref errorMsg);

                bookingLog(ref sbLogger, "TripShope SSR Response", strResponse);

                ctr++;
            }
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            return true;
        }


        public void BookFlight(FlightBookingRequest request, ref FlightBookingResponse _response)
        {
            string errorMsg = string.Empty;
            StringBuilder sbLogger = new StringBuilder();
            try
            {
                int ctr = 0;
                bool isMakeBooking = true;
                foreach (var item in request.flightResult)
                {
                    if (isMakeBooking)
                    {
                        //if (item.IsLCC)
                        //{
                        #region Make booking for LCC
                        var Url = URL + "nextra-flight-booking.api";
                        string strRequest = new TripshopeRequestMappking().getLccTicketingRequest(request, ctr);
                        bookingLog(ref sbLogger, "Tripshope Ticketing Request", strRequest.Trim());
                        var response = GetResponseSearch(Url, strRequest, ref errorMsg);
                        bookingLog(ref sbLogger, "Tripshope Ticketing Response", response.Trim());
                        if (!string.IsNullOrEmpty(response))
                        {
                            BookResponse bookResponse = JsonConvert.DeserializeObject<BookResponse>(response.ToString());
                            if (bookResponse.ApiStatus.statusCode == "200" || bookResponse.ApiStatus.status.Equals("OK", StringComparison.OrdinalIgnoreCase))
                            {
                                request.txid = bookResponse.ApiStatus.result;

                                var UrlD = URL + "/nextra-flight-eticket.api";
                                string strRequestD = new TripshopeRequestMappking().GetTicketingRequest(request, ctr);
                                bookingLog(ref sbLogger, "Tripshope Get Ticket Details Request", strRequestD.Trim());

                                var responseD = GetResponseSearch(UrlD, strRequestD, ref errorMsg);
                                bookingLog(ref sbLogger, "Tripshope Get Ticket Details Response", responseD.Trim());

                                TicketingDetailsResponse.GetTicketingDetailsResponse TDRes = JsonConvert.DeserializeObject<TicketingDetailsResponse.GetTicketingDetailsResponse>(responseD.ToString());

                                if (!string.IsNullOrEmpty(TDRes.ApiStatus.Result.flightRecord.airline_pnr) && TDRes.ApiStatus.Result.flightRecord.airline_pnr.ToString() != "pending") /*&& TDRes.ApiStatus.Result.flightRecord.airline_pnr.Equals("pending", StringComparison.OrdinalIgnoreCase)*/
                                {
                                    _response.PNR = TDRes.ApiStatus.Result.flightRecord.airline_pnr;
                                    bookingLog(ref sbLogger, "Tripshope Ticketing PNR", _response.PNR);
                                    _response.responseStatus.message += "OutBoundPnr-" + _response.PNR;
                                    _response.bookingStatus = BookingStatus.Ticketed;
                                    _response.invoice.Add(new Invoice() { InvoiceAmount = TDRes.ApiStatus.Result.flightRecord.total_base_tax, InvoiceNo = TDRes.ApiStatus.Result.flightRecord.txid });
                                }
                                else
                                {
                                    _response.responseStatus.message = "InProgress";
                                    _response.bookingStatus = BookingStatus.InProgress;
                                }



                            }
                            else
                            {
                                _response.responseStatus.message = bookResponse.ApiStatus.message;
                                _response.bookingStatus = BookingStatus.InProgress;
                            }
                        }
                        else
                        {
                            _response.responseStatus.message = "InProgress";
                            _response.bookingStatus = BookingStatus.InProgress;
                        }
                        //}
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                _response.bookingStatus = BookingStatus.InProgress;
                _response.responseStatus.message = "Booking InProgress";
                bookingLog(ref sbLogger, "Tripshope Flight Booking Exption", ex.ToString());
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Error");
            }
            bookingLog(ref sbLogger, "Tripshope  return Response", JsonConvert.SerializeObject(_response));
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Booking");
            //return _response;
        }


        public void GetBookingDetails()
        {

        }


        public void bookingLog(ref StringBuilder sbLogger, string requestTitle, string logText)
        {
            sbLogger.Append(Environment.NewLine + "---------------------------------------------" + requestTitle + "" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
            sbLogger.Append(Environment.NewLine + logText);
            sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
        }

        private string GetResponseSearch(string url, string requestData, ref string Msg)
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
    }
}
