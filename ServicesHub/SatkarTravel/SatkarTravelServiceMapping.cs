using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Flight;
using Core;
using System.Configuration;
using Newtonsoft.Json;


namespace ServicesHub.SatkarTravel
{

    public class SatkarTravelServiceMapping
    {
        public static string AuthToken = "";
        //string ST_Url = ConfigurationManager.AppSettings["ST_Url"].ToString();
        string ST_UrlToken = ConfigurationManager.AppSettings["ST_UrlToken"].ToString();
        string ST_UrlSerch = ConfigurationManager.AppSettings["ST_UrlSerch"].ToString();
        string ST_UrlFareQuote = ConfigurationManager.AppSettings["ST_UrlFareQuote"].ToString();
        string ST_UrlBooking = ConfigurationManager.AppSettings["ST_UrlBooking"].ToString();
        string ST_UrlGetDetail = ConfigurationManager.AppSettings["ST_UrlGetDetail"].ToString();

        //string ST_TokenID = ConfigurationManager.AppSettings["ST_tokenId"].ToString();

        string ST_URL = ConfigurationManager.AppSettings["ST_Url"].ToString();
        string ST_UserName = ConfigurationManager.AppSettings["ST_UserName"].ToString();
        string ST_PWD = ConfigurationManager.AppSettings["ST_PWD"].ToString();
        string ST_Auth = ConfigurationManager.AppSettings["ST_Auth"].ToString();


        public void getTokenID()
        {
            var strResponse = GetTokenResponse(ST_UrlToken, new SatkarTravelRequestMappking().getSearchToken());
            SatkarTravelClass.TokenResponse res = Newtonsoft.Json.JsonConvert.DeserializeObject<SatkarTravelClass.TokenResponse>(strResponse);
            AuthToken = res.tokenId;
        }

        public FlightSearchResponseShort GetFlightResults(FlightSearchRequest request, bool isSatkarTravel, bool isSatkarTravelR)
        {
            string errorMsg = string.Empty;
            FlightSearchResponseShort flightResponse = new FlightSearchResponseShort(request);
            StringBuilder sbLogger = new StringBuilder();
            try
            {
                for (int i = 0; i < request.segment.Count; i++)
                {
                    if (i == 0 && isSatkarTravel == false)
                    {
                        flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                    }
                    else if (i == 1 && isSatkarTravelR == false)
                    {
                        flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                    }
                    else
                    {
                        string strRequest = new SatkarTravelRequestMappking().getFlightSearchRequest(request, i);
                        if (FlightUtility.isWriteLog)
                        {
                            bookingLog(ref sbLogger, "Satkar Travel Request", strRequest);
                        }
                        var strResponse = GetResponseSearch(ST_UrlSerch, strRequest, ref errorMsg);

                        if (FlightUtility.isWriteLog)
                        {
                            bookingLog(ref sbLogger, "Satkar Travel Response", strResponse);
                        }
                        if (!string.IsNullOrEmpty(strResponse))
                        {
                            SatkarTravelClass.FlightResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<SatkarTravelClass.FlightResponse>(strResponse);
                            bookingLog(ref sbLogger, "Satkar Travel Response1", JsonConvert.SerializeObject(Response));
                            if (Response != null && Response.error != null && Response.error.errorCode == 0 && Response.responseStatus == 1)
                            {
                                new SatkarTravelResponseMapping().getResults(request, ref Response, ref flightResponse);
                            }
                            else
                            {
                                flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                                errorMsg += "Error";
                                flightResponse.response.status = Core.TransactionStatus.Error;
                                flightResponse.response.message = "no result found";
                            }
                        }
                    }
                    if (FlightUtility.isWriteLog)
                    {
                        bookingLog(ref sbLogger, "Satkar Travel errorMsg", errorMsg);
                        new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
                    }
                }
            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
                bookingLog(ref sbLogger, "Exception", ex.ToString());
                new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "Satkar Search Exeption");
            }
            return flightResponse;
        }
        public Core.Flight.FareQuoteResponse GetFareQuote(Core.Flight.PriceVerificationRequest request)
        {
            StringBuilder sbLogger = new StringBuilder();

            FareQuoteResponse _response = new FareQuoteResponse() { flightResult = new List<FlightResult>(), isFareChange = false, responseStatus = new ResponseStatus(), fareIncreaseAmount = 0 };

            try
            {
                int ctr = 0;
                foreach (FlightResult fr in request.flightResult)
                {
                    string strRequest = new SatkarTravelRequestMappking().getFareQuoteRequest(request);
                    bookingLog(ref sbLogger, "ST FareQuote Request", strRequest);
                    var strResponse = GetResponse(ST_UrlFareQuote, strRequest);
                    bookingLog(ref sbLogger, "ST FareQuote Response", strResponse);

                    if (!string.IsNullOrEmpty(strResponse))
                    {
                        ST_FareQuote.FareQuoteResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<ST_FareQuote.FareQuoteResponse>(strResponse);
                        bookingLog(ref sbLogger, "ST FareQuote Response1", JsonConvert.SerializeObject(Response));
                        new SatkarTravelResponseMapping().getFareQuoteResponse(ref request, ref Response, ref _response, ctr);
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
                    string strRequest = new SatkarTravelRequestMappking().getBookingRequest(request, ctr);

                    bookingLog(ref sbLogger, "Satkar Travel Book Flight Request", strRequest);
                    var response = GetResponse(ST_UrlBooking, strRequest, ref sbLogger);
                    bookingLog(ref sbLogger, "Satkar Travel Book Flight Response", response);
                    if (!string.IsNullOrEmpty(response))
                    {
                        ST_BookFlight.BookResponse bookResponse = JsonConvert.DeserializeObject<ST_BookFlight.BookResponse>(response.ToString());
                        bookingLog(ref sbLogger, "ST Travel Book Flight Response", JsonConvert.SerializeObject(bookResponse));
                        if (bookResponse.status == 1 || bookResponse.bookingstatus.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
                        {
                            //_response.ST_Request_id = bookResponse.reqId;
                            //bookingLog(ref sbLogger, "Satkar Travel Request ID", _response.ST_Request_id);
                            _response.ST_tx_id = bookResponse.txid;
                            WebClient client = new WebClient();
                            var url = ST_UrlGetDetail + bookResponse.txid;
                            client.Headers[HttpRequestHeader.ContentType] = "application/json";
                            client.Headers.Add("Authorization", AuthToken);
                            var kk = client.DownloadString(url);
                            ST_GetDetails.GetDetails details = JsonConvert.DeserializeObject<ST_GetDetails.GetDetails>(kk.ToString());
                            bookingLog(ref sbLogger, "Satkar Travel Get Passenger Details", kk);
                            _response.PNR = details.FlightBookingSecotorDetails.FirstOrDefault().airlinepnr;
                            bookingLog(ref sbLogger, "Satkar Travel PNR", _response.PNR);
                            _response.invoice.Add(new Invoice() { InvoiceAmount = details.totalfare, InvoiceNo = "" });
                            _response.bookingStatus = BookingStatus.Ticketed;
                        }
                        else
                        {
                            //     bookingLog(ref sbLogger, " FareBoutique  Else 1 ", "bookResponse.errorCode:" + bookResponse.errorCode + "bookResponse.replyCode:" + bookResponse.replyCode + ("replyMsg:" + bookResponse.replyMsg));
                            _response.bookingStatus = BookingStatus.InProgress;
                            _response.responseStatus.message = "Booking InProgress Due to" + (string.IsNullOrEmpty(bookResponse.errorMsg) ? "" : ("replyMsg:" + bookResponse.errorMsg)) + (string.IsNullOrEmpty(bookResponse.errorMsg) ? "" : ("ErrorMsg:" + bookResponse.errorMsg));
                        }
                    }
                    ctr++;
                }
            }
            catch (Exception ex)
            {
                _response.bookingStatus = BookingStatus.InProgress;
                _response.responseStatus.message = "Booking InProgress";
                bookingLog(ref sbLogger, "Satkar Travel Exption", ex.ToString());
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Error");
            }

            bookingLog(ref sbLogger, "Satkar Travel  return Response", JsonConvert.SerializeObject(_response));
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Booking");
        }


        public void getSectors()
        {
            string segm = string.Empty;
            WebClient client = new WebClient();
            var url = ST_UrlGetDetail + "sectors";
            client.Headers[HttpRequestHeader.ContentType] = "application/json";
            if (string.IsNullOrEmpty(AuthToken))
            {
                getTokenID();
            }
            client.Headers.Add("Authorization", AuthToken);
            var kk = client.DownloadString(url);
            var dobj = JsonConvert.DeserializeObject<dynamic>(kk);
            var sec1 = string.Empty;
            foreach (var sectors in dobj)
            {
                stSegment seg = new stSegment();
                seg.origin = sectors["origin"];
                seg.destination = sectors["destination"];
                segm = JsonConvert.SerializeObject(seg);
                var sec2 = getsectorsWithdate(segm, ST_UrlGetDetail + "availabledate");
                var kk1 = JsonConvert.DeserializeObject<dynamic>(sec2);
                string date = string.Empty;
                foreach (var avldate in kk1["availabledate"])
                {
                    if (!string.IsNullOrEmpty(avldate.ToString()))
                    {
                        if (date.IndexOf(avldate.ToString()) == -1)
                            date += (string.IsNullOrEmpty(date) ? avldate : ("_" + avldate));
                    }
                }
                new DAL.FixDepartueRoute.RoutesDetails().SaveSatkarRouteswithDate(sectors["origin"].ToString(), sectors["destination"].ToString(), date, (int)GdsType.SatkarTravel);
            }
        }



        private string getsectorsWithdate(string sectors, string url)
        {
            if (string.IsNullOrEmpty(AuthToken))
            {
                getTokenID();
            }
            string response = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                byte[] data = Encoding.UTF8.GetBytes(sectors);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", AuthToken);
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

        public void bookingLog(ref StringBuilder sbLogger, string requestTitle, string logText)
        {
            sbLogger.Append(Environment.NewLine + "---------------------------------------------" + requestTitle + "" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
            sbLogger.Append(Environment.NewLine + logText);
            sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
        }
        private string GetResponse(string url, string requestData)
        {
            if (string.IsNullOrEmpty(AuthToken))
            {
                getTokenID();
            }
            string response = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", AuthToken);
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
        private string GetResponse(string url, string requestData, ref StringBuilder sblogger)
        {
            if (string.IsNullOrEmpty(AuthToken))
            {
                getTokenID();
            }
            string response = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", AuthToken);
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
        private string GetResponse(string url, string requestData, ref string Msg)
        {
            if (string.IsNullOrEmpty(AuthToken))
            {
                getTokenID();
            }
            string response = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", AuthToken);
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
        private string GetResponseSearch(string url, string requestData, ref string Msg)
        {
            if (string.IsNullOrEmpty(AuthToken))
            {
                getTokenID();
            }
            string response = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", AuthToken);
                //  request.Timeout = 10000;
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
        private string GetTokenResponse(string url, string requestData)
        {
            string response = string.Empty;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", ST_Auth);
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
