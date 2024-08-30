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

namespace ServicesHub.AirIQ
{
    public class AirIQServiceMapping
    {
        public static string AuthToken = "";

        string Url = ConfigurationManager.AppSettings["AIQ_Url"].ToString();
        string ApiKey = ConfigurationManager.AppSettings["AIQ_ApiKey"].ToString();
        public void getTokenID()
        {
            try
            {
                var strResponse = GetTokenResponse(Url + "login", new AirIQRequestMappking().getSearchToken());
                AirIQClass.TokenResponse res = Newtonsoft.Json.JsonConvert.DeserializeObject<AirIQClass.TokenResponse>(strResponse);
                AuthToken = res.token;
            }
            catch (Exception ex)
            {
                new ServicesHub.LogWriter_New(ex.ToString(), "AIRIQGetToken" + DateTime.Today.ToString("ddMMyy"), "Exeption");
            }
           
        }

        public FlightSearchResponseShort GetFlightResults(FlightSearchRequest request, bool isAirIQGDS, bool isAirIQGDSR)
        {
            string errorMsg = string.Empty;
            FlightSearchResponseShort flightResponse = new FlightSearchResponseShort(request);

            StringBuilder sbLogger = new StringBuilder();
            //if (FlightUtility.isWriteLog)
            //{
            //    bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
            //}

            string strRequest = new AirIQRequestMappking().getFlightSearchRequest(request);
            strRequest = strRequest.Replace("-", "/");
            if (FlightUtility.isWriteLogSearch)
            {
                bookingLog(ref sbLogger, "AirIQ Request", strRequest);
            }
            var strResponse = GetResponseSearch(Url + "search", strRequest);

            if (FlightUtility.isWriteLogSearch)
            {
                bookingLog(ref sbLogger, "AirIQ Response", strResponse);
            }
            if (!string.IsNullOrEmpty(strResponse))
            {
                AirIQClass.FlightResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<AirIQClass.FlightResponse>(strResponse);
                bookingLog(ref sbLogger, "AirIQ Response 1", JsonConvert.SerializeObject(Response));
                if ((Response.code == 200 || (Response.status!=null&& Response.status.Equals("success", StringComparison.OrdinalIgnoreCase))) && Response.code != 0)
                {
                    new AirIQResponseMapping().getResults(request, ref Response, ref flightResponse);
                }
                else
                {
                    errorMsg += Response.message;
                    flightResponse.response.status = Core.TransactionStatus.Error;
                    flightResponse.response.message = "no result found";
                }
            }
            if (FlightUtility.isWriteLogSearch)
            {
                bookingLog(ref sbLogger, "AirIQ errorMsg", errorMsg);
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");

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
                    //string strRequest = new AirIQRequestMappking().getFareQuoteRequest(request);

                    //bookingLog(ref sbLogger, "AirIQ FareQuote Request", strRequest);

                    _response.fareIncreaseAmount = 0;
                    _response.VerifiedTotalPrice = request.flightResult[ctr].Fare.PublishedFare;
                    _response.isFareChange = false;
                    ctr++;
                }

                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");

            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "Exception", ex.ToString());
                new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "AirIQ FareQuote Exeption");
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
                    string strRequest = new AirIQRequestMappking().getBookingRequest(request, ctr);

                    bookingLog(ref sbLogger, "AirIQ Book Flight Request", strRequest);
                    var response = GetResponse(Url + "book", strRequest);
                    bookingLog(ref sbLogger, "AirIQ Book Flight Response", response);
                    if (!string.IsNullOrEmpty(response))
                    {
                        AirIQClass.BookResponse bookResponse = JsonConvert.DeserializeObject<AirIQClass.BookResponse>(response.ToString());
                        bookingLog(ref sbLogger, "AirIQ Book bookResponse", JsonConvert.SerializeObject(bookResponse));
                        if (bookResponse.code.Equals("200", StringComparison.OrdinalIgnoreCase) || bookResponse.status.Equals("success", StringComparison.OrdinalIgnoreCase))
                        {
                            _response.AirIQ_Booking_id = bookResponse.booking_id;
                            _response.AirIQ_AirlineCode = bookResponse.airline_code;
                            _response.AirIQ_Msg = bookResponse.message;

                            WebClient client = new WebClient();
                            var url = Url + "ticket?booking_id=" + bookResponse.booking_id;
                            client.Headers[HttpRequestHeader.ContentType] = "application/json";
                            client.Headers.Add("api-key", ApiKey);
                            client.Headers.Add("Authorization", AuthToken);
                            var kk = client.DownloadString(url);
                            AirIQ_GetDetails.GetDetails details = JsonConvert.DeserializeObject<AirIQ_GetDetails.GetDetails>(kk.ToString());
                            bookingLog(ref sbLogger, "AirIQ Travel Get Passenger Details", kk);
                            _response.PNR = details.data.pnr;
                            bookingLog(ref sbLogger, "AirIQ Travel PNR", _response.PNR);
                            _response.invoice.Add(new Invoice() { InvoiceAmount = details.data.total_amount, InvoiceNo = "" });
                            _response.bookingStatus = BookingStatus.Ticketed;
                        }
                        else
                        {
                            bookingLog(ref sbLogger, "AirIQ  Else 1", "bookResponse.message:" + bookResponse.message);
                            _response.bookingStatus = BookingStatus.InProgress;
                            _response.responseStatus.message = bookResponse.message;
                        }
                    }
                    ctr++;
                }
            }
            catch (Exception ex)
            {
                _response.bookingStatus = BookingStatus.InProgress;
                _response.responseStatus.message = "Booking InProgress";
                bookingLog(ref sbLogger, "AirIQ Exption", ex.ToString());
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Error");
            }

            bookingLog(ref sbLogger, "AirIQ  return Response", JsonConvert.SerializeObject(_response));
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Booking");
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
                request.Headers.Add("api-key", ApiKey);
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
                    new ServicesHub.LogWriter_New(webEx.ToString(), "AIRIQ GetResponse" + DateTime.Today.ToString("ddMMyy"), "Exeption");
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
            if (string.IsNullOrEmpty(AuthToken))
            {
                getTokenID();
            }
            string response = string.Empty;
            try
            {
                StringBuilder sbLogger = new StringBuilder();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("api-key", ApiKey);
                request.Headers.Add("Authorization", AuthToken);
             //   request.Timeout = 10000;
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
                    new ServicesHub.LogWriter_New(webEx.ToString(), "AIRIQGetResponseSearch1" + DateTime.Today.ToString("ddMMyy"), "Exeption");
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
                        new ServicesHub.LogWriter_New(response, "AIRIQGetResponseSearch2" + DateTime.Today.ToString("ddMMyy"), "Exeption");
                    }
                }
            }
            catch { }
            return response;
        }



        private string GetTokenResponse(string url, string requestData)
        {
            string response = string.Empty;
            StringBuilder sbLogger = new StringBuilder();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("api-key", ApiKey);
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
                    new ServicesHub.LogWriter_New(webEx.ToString(), "AIRIQ GetToken" + DateTime.Today.ToString("ddMMyy"), "Exeption");

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


        public void getSectors()
        {
            try
            {
                string segm = string.Empty;
                WebClient client = new WebClient();
                var url = Url + "sectors";
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                if (string.IsNullOrEmpty(AuthToken))
                {
                    getTokenID();
                }
                client.Headers.Add("api-key", ApiKey);
                client.Headers.Add("Authorization", AuthToken);
                var kk = client.DownloadString(url);
                var dobj = JsonConvert.DeserializeObject<dynamic>(kk);
                var sec1 = string.Empty;
                foreach (var sectors in dobj["data"])
                {
                    AirIQSegment seg = new AirIQSegment();
                    seg.origin = sectors["Origin"];
                    seg.destination = sectors["Destination"];
                    segm = JsonConvert.SerializeObject(seg);
                    var sec2 = GetResponse(Url + "availability", segm);
                    var kk1 = JsonConvert.DeserializeObject<dynamic>(sec2);
                    string date = string.Empty;
                    if (kk1["data"] != null)
                    {
                        foreach (var avldate in kk1["data"])
                        {
                            string dt = DateTime.ParseExact(avldate.ToString(), "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            if (date.IndexOf(dt) == -1)
                                date += (string.IsNullOrEmpty(date) ? dt : ("_" + dt));
                        }
                        new DAL.FixDepartueRoute.RoutesDetails().SaveSatkarRouteswithDate(sectors["Origin"].ToString(), sectors["Destination"].ToString(), date, (int)GdsType.AirIQ);
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
