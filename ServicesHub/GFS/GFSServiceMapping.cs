using Core;
using Core.Flight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.GFS
{
    public class GFSServiceMapping
    {
        string Url = ConfigurationManager.AppSettings["GFS_Url"].ToString();
        string ApiKey = ConfigurationManager.AppSettings["GFS_ApiKey"].ToString();
        string GetUrl = ConfigurationManager.AppSettings["GFS_GetUrl"].ToString();
        public FlightSearchResponseShort GetFlightResults(FlightSearchRequest request, bool isGFS, bool isGFSR)
        {
            string errorMsg = string.Empty;
            StringBuilder sbLogger = new StringBuilder();
            FlightSearchResponseShort flightResponse = new FlightSearchResponseShort(request);

            if (FlightUtility.isWriteLogSearch)
            {
                bookingLog(ref sbLogger, "GFS Request", JsonConvert.SerializeObject(request));
            }
            for (int i = 0; i < request.segment.Count; i++)
            {

                if (i == 0 && isGFS == false)
                {
                    flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                }
                else if (i == 1 && isGFSR == false)
                {
                    flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                }
                else
                {
                    string org = request.segment[i].originAirport.ToLower();
                    string dest = request.segment[i].destinationAirport.ToLower();
                    string DepDate = request.segment[i].travelDate.ToString("yyyyMMdd");
                    int adult = request.adults;
                    int Chd = request.child;
                    int Inf = request.infants;
                    var url = string.Empty;
                    WebClient client = new WebClient();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Headers.Add("api-key", ApiKey);
                    url = Url + "series-search?segment=" + org + "-" + dest + "-" + DepDate + "&pax=" + adult + "-" + Chd + "-" + Inf + "";
                    if (FlightUtility.isWriteLogSearch)
                    {
                        bookingLog(ref sbLogger, "GFS Request URL", url);
                    }
                    var responseStream = new System.IO.Compression.GZipStream(client.OpenRead(url), System.IO.Compression.CompressionMode.Decompress);
                    var reader = new System.IO.StreamReader(responseStream);
                    var kk = reader.ReadToEnd();
                    if (FlightUtility.isWriteLogSearch)
                    {
                        bookingLog(ref sbLogger, "GFS Response", kk.ToString());
                    }
                    GFSClass.FlightResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<GFSClass.FlightResponse>(kk.ToString());
                    new GFSResponseMapping().getResults(request, ref Response, ref flightResponse);
                }
            }
           
            if (FlightUtility.isWriteLogSearch)
            {
                //flightResponse.Results.Add(new List<Core.Flight.FlightResult>());
                bookingLog(ref sbLogger, "GFS errorMsg", errorMsg);
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

                    string strRequest = new GFSRequestMappking().getFareQuoteRequest(request, ctr);

                    bookingLog(ref sbLogger, "GFS FareQuote Request", strRequest);

                    var strResponse = GetResponse(Url + "series-check-avail", strRequest);

                    bookingLog(ref sbLogger, "GFS FareQuote Response", strResponse);

                    if (!string.IsNullOrEmpty(strResponse))
                    {
                        GFSFareQuoteResponse.FareQuoteResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<GFSFareQuoteResponse.FareQuoteResponse>(strResponse);
                        bookingLog(ref sbLogger, "GFS FareQuote Response 1", JsonConvert.SerializeObject(Response));
                        new GFSResponseMapping().getFareQuoteResponse(ref request, ref Response, ref _response, ctr);
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
                new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "GFS FareQuote Exeption");
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
                    string strRequest = new GFSRequestMappking().getBookingRequest(request);

                    bookingLog(ref sbLogger, "GFS Book Flight Request", strRequest);
                    var response = GetResponse(Url + "series-book", strRequest);
                    bookingLog(ref sbLogger, "GFS Book Flight Response", response);
                    if (!string.IsNullOrEmpty(response))
                    {
                        GFS_BookFlightResponse.BookFlightResponse bookResponse = JsonConvert.DeserializeObject<GFS_BookFlightResponse.BookFlightResponse>(response.ToString());
                        bookingLog(ref sbLogger, "GFS Book Flight BookingResponse", JsonConvert.SerializeObject(bookResponse));

                        // if ((bookResponse.success == true) && (bookResponse._data.status.Equals("confirmed", StringComparison.OrdinalIgnoreCase)))
                        // if ((bookResponse.success == true) && (bookResponse._data.status.Equals("test", StringComparison.OrdinalIgnoreCase)))
                        if ((bookResponse.success == true) && (bookResponse._data != null))
                        {
                            _response.booking_reference = bookResponse._data.booking_reference;
                            bookingLog(ref sbLogger, "GFS booking_reference", _response.booking_reference);
                            _response.agent_reference = bookResponse._data.agent_reference;
                            bookingLog(ref sbLogger, "GFS agent_reference", bookResponse._data.agent_reference);

                            #region Get Booking Details
                            var url = string.Empty;
                            WebClient client = new WebClient();
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                            client.Headers[HttpRequestHeader.ContentType] = "application/json";
                            client.Headers.Add("api-key", ApiKey);
                            url = GetUrl + _response.booking_reference;
                            if (FlightUtility.isWriteLogSearch)
                            {
                                bookingLog(ref sbLogger, "GFS Get Booking Details", url);
                            }
                            var responseStream = new System.IO.Compression.GZipStream(client.OpenRead(url), System.IO.Compression.CompressionMode.Decompress);
                            var reader = new System.IO.StreamReader(responseStream);
                            var kk = reader.ReadToEnd();

                            GFS_GetBookingDetails.GetBookingDetails Response = Newtonsoft.Json.JsonConvert.DeserializeObject<GFS_GetBookingDetails.GetBookingDetails>(kk.ToString());

                            //  if ((Response.success == true) && (Response._data.status.Equals("confirmed", StringComparison.OrdinalIgnoreCase)))
                            // if ((Response.success == true) && (Response._data.status.Equals("test", StringComparison.OrdinalIgnoreCase)))
                            if ((Response.success == true) && (Response._data != null) && (Response._data.booking_items[0].status.Equals("confirmed", StringComparison.OrdinalIgnoreCase)))
                            {
                                _response.PNR = Response._data.booking_items[0].confirmations[0].pnr;
                                bookingLog(ref sbLogger, "GFS PNR", _response.PNR);
                                _response.invoice.Add(new Invoice() { InvoiceAmount = Response._data.price_details.total_amount });
                                _response.bookingStatus = BookingStatus.Ticketed;
                            }
                            else
                            {
                                _response.bookingStatus = BookingStatus.InProgress;
                                _response.responseStatus.message = Response.error_msg + "( " + Response.error_code + " )";
                            }

                            #endregion Get Booking Details
                        }
                        else
                        {
                            bookingLog(ref sbLogger, "GFS  Else 1", "bookResponse.errorCode:" + bookResponse.error_msg);
                            _response.bookingStatus = BookingStatus.InProgress;
                            _response.responseStatus.message = bookResponse.error_msg + "( " + bookResponse.error_code + " )";
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
                request.Headers.Add("api-key", ApiKey);
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
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
                    new ServicesHub.LogWriter_New(webEx.ToString(), "GFS GetResponse" + DateTime.Today.ToString("ddMMyy"), "Exeption");
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
                string segm = string.Empty;

                WebClient client = new WebClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers.Add("api-key", ApiKey);
                var url = Url + "series-sectors?operatingDays";
                var responseStream = new System.IO.Compression.GZipStream(client.OpenRead(url), System.IO.Compression.CompressionMode.Decompress);
                var reader = new System.IO.StreamReader(responseStream);
                var kk = reader.ReadToEnd();
                var dobj = JsonConvert.DeserializeObject<dynamic>(kk);
                var sec1 = string.Empty;
                foreach (var sectors in dobj["_data"]["sectors"])
                {
                    GFS_Segment.GFSSegment seg = new GFS_Segment.GFSSegment();
                    seg.origin = sectors["origin"];
                    seg.destination = sectors["destination"];
                    segm = JsonConvert.SerializeObject(seg);


                    segm = JsonConvert.SerializeObject(seg);

                    WebClient client1 = new WebClient();
                    client1.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client1.Headers.Add("api-key", ApiKey);
                    var urlDate = Url + "series-dates?origin=" + sectors["origin"].ToString() + "&destination=" + sectors["destination"].ToString();
                    var responseStream1 = new System.IO.Compression.GZipStream(client.OpenRead(urlDate), System.IO.Compression.CompressionMode.Decompress);
                    var reader1 = new System.IO.StreamReader(responseStream1);
                    var kk1 = reader1.ReadToEnd();

                    var kk11 = JsonConvert.DeserializeObject<dynamic>(kk1);
                    string date = string.Empty;
                    if (kk11["_data"] != null)
                    {
                        foreach (var avldate in kk11["_data"]["sector"]["date"])
                        {
                            //if (!string.IsNullOrEmpty(avldate["date"].ToString()))
                            //{
                            //    date += (string.IsNullOrEmpty(date) ? avldate["date"].ToString() : ("_" + avldate["date"].ToString()));
                            date += (string.IsNullOrEmpty(date) ? avldate.ToString() : ("_" + avldate.ToString()));
                            //}

                        }
                        new DAL.FixDepartueRoute.RoutesDetails().SaveSatkarRouteswithDate(sectors["origin"].ToString(), sectors["destination"].ToString(), date, (int)GdsType.GFS);
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
