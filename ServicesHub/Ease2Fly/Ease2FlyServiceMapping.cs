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

namespace ServicesHub.Ease2Fly
{
    public class Ease2FlyServiceMapping
    {
        public static string AuthToken = "";

        string Url = ConfigurationManager.AppSettings["E2F_Url"].ToString();
        string ApiKey = ConfigurationManager.AppSettings["E2F_ApiKey"].ToString();
        public void getTokenID()
        {
            var strResponse = GetTokenResponse(Url + "tp-api/login", new Ease2FlyRequestMappking().getSearchToken());
            Ease2FlyClass.TokenResponse res = Newtonsoft.Json.JsonConvert.DeserializeObject<Ease2FlyClass.TokenResponse>(strResponse);
            AuthToken = res.result.token;
        }

        public FlightSearchResponseShort GetFlightResults(FlightSearchRequest request, bool isEase2Fly, bool isEase2FlyR)
        {
            string errorMsg = string.Empty;
            StringBuilder sbLogger = new StringBuilder();
            FlightSearchResponseShort flightResponse = new FlightSearchResponseShort(request);

            if (FlightUtility.isWriteLogSearch)
            {
                bookingLog(ref sbLogger, "Ease2Fly Request", JsonConvert.SerializeObject(request));
            }
            for (int i = 0; i < request.segment.Count; i++)
            {

                if (i == 0 && isEase2Fly == false)
                {
                    flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                }
                else if (i == 1 && isEase2FlyR == false)
                {
                    flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                }
                else
                {
                    string org = request.segment[i].originAirport.ToLower();
                    string dest = request.segment[i].destinationAirport.ToLower();
                    string DepDate = request.segment[i].travelDate.ToString("yyyy-MM-dd");
                    int adult = request.adults;
                    int Chd = request.child;
                    int Inf = request.infants;
                    var url = string.Empty;
                    WebClient client = new WebClient();
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (string.IsNullOrEmpty(AuthToken))
                    {
                        getTokenID();
                    }
                    client.Headers.Add("Authorization", "Bearer " + AuthToken);
                    client.Headers.Add("efly_api_key", ApiKey);
                    url = Url + "tp-api/search-flights?origin=" + org + "&destination=" + dest + "&airline=&departuredate=" + DepDate + "&adults=" + adult;
                    if (request.child > 0)
                    {
                        url = url + "&child=" + Chd;
                    }
                    else
                    {
                        url = url + "&child=";
                    }
                    if (request.infants > 0)
                    {
                        url = url + "&infant=" + Inf;
                    }
                    else
                    {
                        url = url + "&infant=";
                    }
                    if (FlightUtility.isWriteLogSearch)
                    {
                        bookingLog(ref sbLogger, "Ease2Fly Request URL", url);
                    }
                    var kk = client.DownloadString(url);
                    if (FlightUtility.isWriteLogSearch)
                    {
                        bookingLog(ref sbLogger, "Ease2Fly Response", kk.ToString());
                    }
                    Ease2FlyClass.FlightResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<Ease2FlyClass.FlightResponse>(kk.ToString());
                    new Ease2FlyResponseMapping().getResults(request, ref Response, ref flightResponse);
                }
            }
            if (FlightUtility.isWriteLogSearch)
            {
                flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                bookingLog(ref sbLogger, "Ease2Fly errorMsg", errorMsg);
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
                    string strRequest = new Ease2FlyRequestMappking().getBookingRequest(request, ctr);

                    bookingLog(ref sbLogger, "Ease2Fly Book Flight Request", strRequest);
                    var response = GetResponse(Url + "tp-api/book-ticket", strRequest);
                    bookingLog(ref sbLogger, "Ease2Fly Book Flight Response", response);
                    if (!string.IsNullOrEmpty(response))
                    {
                        Ease2FlyClass.BookResponse bookResponse = JsonConvert.DeserializeObject<Ease2FlyClass.BookResponse>(response.ToString());
                        bookingLog(ref sbLogger, "Ease2Fly Book bookResponse", JsonConvert.SerializeObject(bookResponse));
                        if (bookResponse.status == true)
                        {
                            if (bookResponse.result.booking_status.Equals("CONFIRMED", StringComparison.OrdinalIgnoreCase))
                            {
                                _response.PNR = bookResponse.result.pnr_no;
                                _response.E2FBookingID = bookResponse.result.booking_id;
                                bookingLog(ref sbLogger, "Ease2Fly Book Flight Response E2FBookingID", _response.E2FBookingID.ToString());
                                _response.invoice.Add(new Invoice() { InvoiceAmount = request.flightResult[0].Fare.PublishedFare, InvoiceNo = "" });


                                //WebClient client = new WebClient();
                                //var url = Url + "tp-api/show-ticket?booking_id=" + bookResponse.result.booking_id;
                                //client.Headers[HttpRequestHeader.ContentType] = "application/json";
                                //client.Headers.Add("Authorization", "Bearer " + AuthToken);
                                //client.Headers.Add("efly_api_key", ApiKey);
                                //var kk = client.DownloadString(url);
                                //Ease2Fly_GetDetails.GetDetails details = JsonConvert.DeserializeObject<Ease2Fly_GetDetails.GetDetails>(kk.ToString());
                                //bookingLog(ref sbLogger, "Ease2Fly Travel Get Passenger Details", kk);
                                //_response.PNR = details.result.pnr_no;
                                //bookingLog(ref sbLogger, "AirIQ Travel PNR", _response.PNR);
                                //  _response.invoice.Add(new Invoice() { InvoiceAmount = details.result.display_fare, InvoiceNo = "" });

                                _response.bookingStatus = BookingStatus.Ticketed;
                            }
                            else
                            {
                                _response.bookingStatus = BookingStatus.InProgress;
                                _response.responseStatus.message = bookResponse.error;
                            }

                        }
                        else if (bookResponse.status == false)
                        {
                            _response.bookingStatus = BookingStatus.InProgress;
                            _response.responseStatus.message = bookResponse.error;
                        }
                        else
                        {
                            //   bookingLog(ref sbLogger, " AirIQ  Else 1 ", "bookResponse.errorCode:" + bookResponse.message + "bookResponse.replyCode:" + bookResponse.message + ("replyMsg:" + bookResponse.message));
                            _response.bookingStatus = BookingStatus.InProgress;
                            _response.responseStatus.message = "InProgress";
                        }
                    }
                    ctr++;
                }
            }
            catch (Exception ex)
            {
                _response.bookingStatus = BookingStatus.InProgress;
                _response.responseStatus.message = "Booking InProgress";
                bookingLog(ref sbLogger, "Ease2Fly Exption", ex.ToString());
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Error");
            }

            bookingLog(ref sbLogger, "Ease2Fly  return Response", JsonConvert.SerializeObject(_response));
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Booking");
        }




        public void bookingLog(ref StringBuilder sbLogger, string requestTitle, string logText)
        {
            sbLogger.Append(Environment.NewLine + "---------------------------------------------" + requestTitle + "" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
            sbLogger.Append(Environment.NewLine + logText);
            sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
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

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                WebResponse webResponse = request.GetResponse();
                var rsp = webResponse.GetResponseStream();
                using (StreamReader reader = new StreamReader(rsp))
                {
                    response = reader.ReadToEnd();
                }
                return response;
            }
            catch (WebException webEx)
            {
                if (webEx != null && webEx.Response != null)
                {
                    new ServicesHub.LogWriter_New(webEx.ToString(), "E2F GetTokenResponse" + DateTime.Today.ToString("ddMMyy"), "Exeption");
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
            return response;
        }

        private string GetResponse(string url, string requestData)
        {
            string response = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(AuthToken))
                {
                    getTokenID();
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("Authorization", "Bearer " + AuthToken);
                request.Headers.Add("efly_api_key", ApiKey);
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
                return response;
            }
            catch (WebException webEx)
            {

                if (webEx != null && webEx.Response != null)
                {
                    new ServicesHub.LogWriter_New(webEx.ToString(), "E2F GetResponse" + DateTime.Today.ToString("ddMMyy"), "Exeption");
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
            return response;
        }


        public void getSectors()
        {
            try
            {
                string segm = string.Empty;
                WebClient client = new WebClient();
                var url = Url + "tp-api/search-destinations";
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                if (string.IsNullOrEmpty(AuthToken))
                {
                    getTokenID();
                }
                client.Headers.Add("Authorization", "Bearer " + AuthToken);
                client.Headers.Add("efly_api_key", ApiKey);
                var kk = client.DownloadString(url);
                var dobj = JsonConvert.DeserializeObject<dynamic>(kk);
                var sec1 = string.Empty;
                foreach (var sectors in dobj["result"])
                {
                    Ease2FlySegment seg = new Ease2FlySegment();
                    seg.origin = sectors["origin"];
                    seg.destination = sectors["destination"];
                    segm = JsonConvert.SerializeObject(seg);

                    WebClient client1 = new WebClient();

                    var urlDate = Url + "tp-api/search-dates?origin=" + sectors["origin"].ToString() + "&destination=" + sectors["destination"].ToString() + "&airline=";
                    client1.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client1.Headers.Add("Authorization", "Bearer " + AuthToken);
                    client1.Headers.Add("efly_api_key", ApiKey);
                    var kk1 = client.DownloadString(urlDate);

                    var kk11 = JsonConvert.DeserializeObject<dynamic>(kk1);
                    string date = string.Empty;
                    if (kk11["result"] != null)
                    {
                        foreach (var avldate in kk11["result"])
                        {
                            if (!string.IsNullOrEmpty(avldate["full_date"].ToString()))
                            {
                                date += (string.IsNullOrEmpty(date) ? avldate["full_date"].ToString() : ("_" + avldate["full_date"].ToString()));
                            }
                        }
                        new DAL.FixDepartueRoute.RoutesDetails().SaveSatkarRouteswithDate(sectors["origin"].ToString(), sectors["destination"].ToString(), date, (int)GdsType.Ease2Fly);
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
