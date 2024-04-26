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

namespace ServicesHub.FareBoutique
{
    public class FareBoutiqueServiceMapping
    {

        string FB_Url = ConfigurationManager.AppSettings["FB_Url"].ToString();
        string FB_UrlSector = ConfigurationManager.AppSettings["FB_UrlSector"].ToString();
        string FB_Ip = ConfigurationManager.AppSettings["FB_Ip"].ToString();
        string FB_TokenID = ConfigurationManager.AppSettings["FB_TokenID"].ToString();
        public FlightSearchResponseShort GetFlightResults(FlightSearchRequest request, bool isfareBoutique, bool isfareBoutiqueR)
        {
            string errorMsg = string.Empty;
            FlightSearchResponseShort flightResponse = new FlightSearchResponseShort(request);

            StringBuilder sbLogger = new StringBuilder();
            //if (FlightUtility.isWriteLog)
            //{
            //    bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
            //}
            for (int i = 0; i < request.segment.Count; i++)
            {
                if (i == 0 && isfareBoutique == false)
                {
                    flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                }
                else if (i == 1 && isfareBoutiqueR == false)
                {
                    flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                }
                else
                {
                    string strRequest = new FareBoutiqueRequestMappking().getFlightSearchRequest(request, FB_TokenID, FB_Ip, i);
                    if (FlightUtility.isWriteLogSearch)
                    {
                        bookingLog(ref sbLogger, "FareBoutique Request", strRequest);
                    }
                    var strResponse = GetResponseSearch(FB_Url + "search", strRequest, ref errorMsg);

                    if (FlightUtility.isWriteLogSearch)
                    {
                        bookingLog(ref sbLogger, "FareBoutique Response", strResponse);
                    }
                    if (!string.IsNullOrEmpty(strResponse))
                    {
                        FareBoutiqueClass.FlightResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<FareBoutiqueClass.FlightResponse>(strResponse);
                        bookingLog(ref sbLogger, "FB search Response2", JsonConvert.SerializeObject(Response));
                        if (Response.errorCode == 0 || (!string.IsNullOrEmpty(Response.replyCode) && Response.replyCode.Equals("success", StringComparison.OrdinalIgnoreCase)))
                        {
                            new FareBoutiqueResponseMapping().getResults(request, ref Response, ref flightResponse);
                        }
                        else
                        {
                            flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                            errorMsg += Response.errorMessage;
                            flightResponse.response.status = Core.TransactionStatus.Error;
                            flightResponse.response.message = "no result found";
                        }
                    }
                }
            }
            if (FlightUtility.isWriteLogSearch)
            {
                bookingLog(ref sbLogger, "FareBoutique errorMsg", errorMsg);
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");

            }
            return flightResponse;
        }
        public Core.Flight.FareQuoteResponse GetFareQuote(Core.Flight.PriceVerificationRequest request)
        {
            StringBuilder sbLogger = new StringBuilder();

            FareQuoteResponse _response = new FareQuoteResponse() { flightResult = new List<FlightResult>(), isFareChange = false, responseStatus = new ResponseStatus(), fareIncreaseAmount = 0 };// Newfare = new List<Fare>(),

            try
            {
                int ctr = 0;
                foreach (FlightResult fr in request.flightResult)
                {
                    string strRequest = new FareBoutiqueRequestMappking().getFareQuoteRequest(request, FB_TokenID, FB_Ip);

                    bookingLog(ref sbLogger, "FB FareQuote Request", strRequest);

                    var strResponse = GetResponse(FB_Url + "fare_quote", strRequest);

                    bookingLog(ref sbLogger, "FB FareQuote Response", strResponse);

                    if (!string.IsNullOrEmpty(strResponse))
                    {
                        FB_FareQuote.FareQuoteResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<FB_FareQuote.FareQuoteResponse>(strResponse);
                        bookingLog(ref sbLogger, "FB FareQuote Response2", JsonConvert.SerializeObject(Response));
                        new FareBoutiqueResponseMapping().getFareQuoteResponse(ref request, ref Response, ref _response, ctr);
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
                new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "FB FareQuote Exeption");
            }
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            return _response;
        }
        public void BookFlight(FlightBookingRequest request, ref FlightBookingResponse _response)
        {
            StringBuilder sbLogger = new StringBuilder();
            try
            {
                int ctr = 0;
                foreach (var item in request.flightResult)
                {
                    string strRequest = new FareBoutiqueRequestMappking().getBookingRequest(request, ctr, FB_TokenID, FB_Ip);

                    bookingLog(ref sbLogger, "FareBoutique Book Flight Request", strRequest);
                    var response = GetResponse(FB_Url + "book_flight", strRequest, ref sbLogger);
                    bookingLog(ref sbLogger, "FareBoutique Book Flight Response", response);
                    if (!string.IsNullOrEmpty(response))
                    {
                        FB_BookFlight.BookResponse bookResponse = JsonConvert.DeserializeObject<FB_BookFlight.BookResponse>(response.ToString());
                        bookingLog(ref sbLogger, "FareBoutique Book Flight BookingResponse", JsonConvert.SerializeObject(bookResponse));
                        //if (bookResponse.errorCode == 0 || bookResponse.replyCode.Equals("success", StringComparison.OrdinalIgnoreCase))
                        if (bookResponse.replyCode.Equals("success", StringComparison.OrdinalIgnoreCase))
                        {
                            _response.Fb_Reference_id = bookResponse.data.reference_id;
                            bookingLog(ref sbLogger, "FareBoutique reference_id", bookResponse.data.reference_id);
                            string strRequestCheckBooking = new FareBoutiqueRequestMappking().getCheckBookingRequest(request, ctr, FB_TokenID, _response.Fb_Reference_id, FB_Ip);
                            bookingLog(ref sbLogger, "FareBoutique CheckBooking Request", strRequestCheckBooking);
                            var responseBookingDetails = GetResponse(FB_Url + "booking_details", strRequestCheckBooking, ref sbLogger);
                            bookingLog(ref sbLogger, "FareBoutique CheckBooking Response", responseBookingDetails);

                            FB_Booking_Details.Booking_Details BookingDetails = JsonConvert.DeserializeObject<FB_Booking_Details.Booking_Details>(responseBookingDetails.ToString());

                            if (BookingDetails.replyCode.Equals("success", StringComparison.OrdinalIgnoreCase) || BookingDetails.replyCode.Equals("0", StringComparison.OrdinalIgnoreCase))
                            {
                                _response.PNR = BookingDetails.data.flight_pnrs;
                                bookingLog(ref sbLogger, " FareBoutique  PNR ", "_response.PNR:" + _response.PNR);
                                _response.responseStatus.message += "; OutBoundPnr-" + _response.PNR;
                                _response.invoice.Add(new Invoice() { InvoiceAmount = BookingDetails.data.total_amount, InvoiceNo = _response.Fb_Reference_id });
                                _response.bookingStatus = BookingStatus.Ticketed;
                            }
                            else if (bookResponse.replyCode.Equals("error", StringComparison.OrdinalIgnoreCase))
                            {
                                bookingLog(ref sbLogger, " FareBoutique  Else 2 ", "replyMsg:" + bookResponse.replyMsg);
                                _response.bookingStatus = BookingStatus.InProgress;
                                _response.responseStatus.message = "Booking InProgress Due to" + (string.IsNullOrEmpty(bookResponse.replyMsg) ? "" : ("replyMsg:" + bookResponse.replyMsg));
                            }
                            else
                            {
                                bookingLog(ref sbLogger, " FareBoutique  Else 3 ", "BookingDetails.replyMsg:" + bookResponse.replyMsg);
                                _response.bookingStatus = BookingStatus.InProgress;
                                _response.responseStatus.message = "InProgress";
                            }
                        }
                        else
                        {
                            bookingLog(ref sbLogger, "FareBoutique  Else 1", "bookResponse.errorCode:" + bookResponse.replyMsg);
                            _response.bookingStatus = BookingStatus.InProgress;
                            _response.responseStatus.message = "Booking InProgress Due to" + (string.IsNullOrEmpty(bookResponse.replyMsg) ? "" : ("replyMsg:" + bookResponse.replyMsg));
                        }
                    }
                    ctr++;
                }
            }
            catch (Exception ex)
            {
                _response.bookingStatus = BookingStatus.InProgress;
                _response.responseStatus.message = "Booking InProgress";
                bookingLog(ref sbLogger, "FareBoutique Exption", ex.ToString());
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Error");
            }

            bookingLog(ref sbLogger, "FareBoutique  return Response", JsonConvert.SerializeObject(_response));
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Booking");
            //return _response;
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
                    new ServicesHub.LogWriter_New(webEx.ToString(), "FB GetResponse" + DateTime.Today.ToString("ddMMyy"), "Exeption");
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

        private string GetResponse(string url, string requestData, ref StringBuilder sblogger)
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
                    new ServicesHub.LogWriter_New(webEx.ToString(), "FB GetResponse1" + DateTime.Today.ToString("ddMMyy"), "Exeption");
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

        private string GetResponse(string url, string requestData, ref string Msg)
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
                    new ServicesHub.LogWriter_New(webEx.ToString(), "FB GetResponse2" + DateTime.Today.ToString("ddMMyy"), "Exeption");
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
                //request.Timeout = 15000;
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
                    new ServicesHub.LogWriter_New(webEx.ToString(), "FB GetResponseSearch" + DateTime.Today.ToString("ddMMyy"), "Exeption");
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

        public void getSectors()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                string segm = string.Empty;
                WebClient client = new WebClient();

                client.Headers[HttpRequestHeader.ContentType] = "application/json";


                var kk = client.UploadString(FB_UrlSector + "sector", new FareBoutiqueRequestMappking().getRouteRequest(FB_TokenID, FB_Ip));
                var dobj = JsonConvert.DeserializeObject<dynamic>(kk);
                var sec1 = string.Empty;
                foreach (var sectors in dobj["data"])
                {
                    var sec2 = GetResponse(FB_UrlSector + "onward_dates", new FareBoutiqueRequestMappking().getRouteDateRequest(FB_TokenID, FB_Ip, sectors["departure_city_code"].ToString(), sectors["arrival_city_code"].ToString()));
                    var kk1 = JsonConvert.DeserializeObject<dynamic>(sec2);
                    string date = string.Empty;
                    if (kk1["data"] != null)
                    {
                        foreach (var avldate in kk1["data"])
                        {
                            if (!string.IsNullOrEmpty(avldate["flight_date"].ToString()))
                            {
                                if (date.IndexOf(avldate["flight_date"].ToString()) == -1)
                                    date += (string.IsNullOrEmpty(date) ? avldate["flight_date"].ToString() : ("_" + avldate["flight_date"].ToString()));
                            }
                        }
                        new DAL.FixDepartueRoute.RoutesDetails().SaveSatkarRouteswithDate(sectors["departure_city_code"].ToString(), sectors["arrival_city_code"].ToString(), date, (int)GdsType.FareBoutique);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
        }
    }
}
