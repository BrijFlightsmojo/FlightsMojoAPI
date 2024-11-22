using Core;
using Core.Flight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.TripJack
{
    public class TripJackServiceMapping
    {
        public static string TripJackApikey = ConfigurationManager.AppSettings["TripJackApikey"].ToString();
        public static string TripJackSearchUrl = ConfigurationManager.AppSettings["TripJackSearchUrl"].ToString();
        public static string TripJackReviewUrl = ConfigurationManager.AppSettings["TripJackReviewUrl"].ToString();
        public static string TripJackBookUrl = ConfigurationManager.AppSettings["TripJackBookUrl"].ToString();
        public static string TripJackBookingDetailsUrl = ConfigurationManager.AppSettings["TripJackBookingDetailsUrl"].ToString();
        public FlightSearchResponseShort GetFlightResults(FlightSearchRequest request)
        {
            FlightSearchResponseShort flightResponse = new FlightSearchResponseShort(request);
            StringBuilder sbLogger = new StringBuilder();
            try
            {
                string strRequest = new TripJackRequestMapping().getFlightSearchRequest(request);
                if (FlightUtility.isWriteLogSearch)
                {
                    bookingLog(ref sbLogger, "TripJack End Point Url", TripJackSearchUrl);
                    bookingLog(ref sbLogger, "TripJack Request", strRequest);
                }
                // string strResponse = MakeServiceCallPOST(TripJackSearchUrl, strRequest, ref sbLogger);


                string strResponse = MakeServiceCallPOSTSearch(TripJackSearchUrl, strRequest, ref sbLogger);
                if (FlightUtility.isWriteLogSearch)
                {
                    bookingLog(ref sbLogger, "TripJack Response", strResponse);
                }
                if (!string.IsNullOrEmpty(strResponse))
                {
                    TripJackClass.TripJackFlightSearchResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<TripJackClass.TripJackFlightSearchResponse>(strResponse);
                    bookingLog(ref sbLogger, "TripJack Response1", JsonConvert.SerializeObject(Response));
                    if (Response != null && Response.status != null && Response.status.success)
                    {
                        new TripJackResponseMapping().getResults(request, ref Response, ref flightResponse);
                        //if (FlightUtility.isWriteLog && !request.isMetaRequest)
                        //{
                        //    bookingLog(ref sbLogger, "Original Response", JsonConvert.SerializeObject(flightResponse));
                        //}

                        //SuccessCount();
                    }

                }
            }
            catch (Exception ex)
            {
                //StringBuilder sbLoggerEx = new StringBuilder();
                bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
                bookingLog(ref sbLogger, "Exception", ex.ToString());
                new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "tripjack Search Exeption");
                //LogCreater.CreateLogFile(sbLoggerEx.ToString(), "Log\\TripJack\\SearchExeption", request.userSearchID );
            }
            //if (FlightUtility.isWriteLog && !request.isMetaRequest)
            //{
            //    //LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\TripJack\\Search", request.userSearchID );
            //}
            //if (flightResponse.Results.Count == 0 || (flightResponse.Results.Count > 0 && flightResponse.Results.FirstOrDefault().Count == 0))
            //{
            //    new LogWriter("" + request.segment[0].originAirport + request.segment[0].destinationAirport + request.segment[0].travelDate.ToString("ddMMM"), "Tripjack" + DateTime.Today.ToString("ddMMMyy") , "NoResult");
            //}
            if (FlightUtility.isWriteLogSearch)
            {
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
                //LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\TripJack\\Search", request.userSearchID );
            }
            if (flightResponse.Results.Count == 0 || (flightResponse.Results.Count > 0 && flightResponse.Results.FirstOrDefault().Count == 0))
            {
                //new LogWriter("No" + Environment.NewLine, "Tj" + DateTime.Today.ToString("ddMMMyy"), "NoResult");
            }
            if (flightResponse.Results.Count == 1 || (flightResponse.Results.Count > 1 && flightResponse.Results.FirstOrDefault().Count == 1))
            {
                //new LogWriter("Yes" + Environment.NewLine, "Tj" + DateTime.Today.ToString("ddMMMyy"), "NoResult");
                //SuccessCount();
            }
            return flightResponse;
        }
        public FareQuoteResponse GetFlightReview(PriceVerificationRequest request)
        {
            FareQuoteResponse response = new FareQuoteResponse();
            response.responseStatus = new ResponseStatus();
            StringBuilder sbLogger = new StringBuilder();
            try
            {
                string strRequest = new TripJackRequestMapping().getFlightReviewRequest(request);
                bookingLog(ref sbLogger, "TripJack fareQuote Request", strRequest);

                string strResponse = MakeServiceCallPOST(TripJackReviewUrl, strRequest, ref sbLogger);
                bookingLog(ref sbLogger, "TripJack fareQuote Response", strResponse);
                if (!string.IsNullOrEmpty(strResponse))
                {
                    TripJackClass.Review.TripJackReviewResponse tjResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TripJackClass.Review.TripJackReviewResponse>(strResponse);
                    bookingLog(ref sbLogger, "TripJack fareQuote Response1", JsonConvert.SerializeObject(tjResponse));
                    if (tjResponse != null && tjResponse.status != null && tjResponse.status.success)
                    {
                        response.TjBookingID = tjResponse.bookingId;
                        response.VerifiedTotalPrice = tjResponse.totalPriceInfo.totalFareDetail.fC.TF;
                        bookingLog(ref sbLogger, "TripJack fareQuote Response", "Verified Success");
                    }
                    else
                    {
                        response.isRunFareQuoteFalse = true;
                        response.responseStatus = new ResponseStatus() { status = TransactionStatus.Error, message = "Error" };
                        bookingLog(ref sbLogger, "TripJack fareQuote Response", "Verified Fail");
                    }
                }
            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
                bookingLog(ref sbLogger, "Exception", ex.ToString());
                new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "tripjack FareQuote Exeption");
            }
            //if (FlightUtility.isWriteLogSearch)
            //{
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            //}
            return response;
        }


        public void BookFlight(FlightBookingRequest request, ref FlightBookingResponse response)
        {
            StringBuilder sbLogger = new StringBuilder();
            try
            {
                string strRequest = new TripJackRequestMapping().getFlightBookRequest(request);
                bookingLog(ref sbLogger, "Trip Jack BookFlight Request", strRequest);

                string strResponse = MakeServiceCallPOST(TripJackBookUrl, strRequest, ref sbLogger);
                bookingLog(ref sbLogger, "Trip Jack BookFlight Response", strResponse);

                if (!string.IsNullOrEmpty(strResponse))
                {
                    var dobj = JsonConvert.DeserializeObject<dynamic>(strResponse);
                    if (((string)dobj["status"]["success"]).Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        if (dobj["order"] != null && ((string)dobj["order"]["status"]).Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                        {
                            List<string> str = new List<string>();
                            List<string> tktList = new List<string>();
                            foreach (var travellerInfos in dobj["itemInfos"]["AIR"]["travellerInfos"])
                            {
                                foreach (var pnrDetails in travellerInfos["pnrDetails"])
                                {
                                    if (!str.Contains((string)pnrDetails.First))
                                    {
                                        str.Add((string)pnrDetails.First);
                                    }
                                }
                                if (travellerInfos["ticketNumberDetails"] != null)
                                {
                                    List<string> tkt = new List<string>();
                                    foreach (var tktNo in travellerInfos["ticketNumberDetails"])
                                    {
                                        if (!tkt.Contains((string)tktNo.First))
                                        {
                                            tkt.Add((string)tktNo.First);
                                        }
                                    }
                                    string strTkt = string.Empty;
                                    foreach (var t in tkt)
                                    {
                                        strTkt += (string.IsNullOrEmpty(strTkt) ? t : ("," + t));
                                    }
                                    if (!string.IsNullOrEmpty(strTkt))
                                        tktList.Add(strTkt);
                                }
                            }
                            if (str.Count >= 1) { response.PNR = str[0]; }
                            if (str.Count >= 2) { response.ReturnPNR = str[1]; }
                            else if (request.PriceID.Count >= 2) { response.ReturnPNR = str[0]; }
                            for (int i = 0; i < tktList.Count; i++)
                            {
                                response.passengerDetails[i].ticketNo = tktList[i];
                            }
                            if (!string.IsNullOrEmpty(response.PNR))
                            {
                                response.invoice = new List<Invoice>();
                                response.invoice.Add(new Invoice() { InvoiceAmount = (dobj["order"]["amount"] != null ? Convert.ToDecimal(dobj["order"]["amount"]) : 0), InvoiceNo = request.TjBookingID });
                            }
                            response.bookingStatus = BookingStatus.Ticketed;
                        }
                        else
                        {
                            if (request.travelType != Core.TravelType.International)
                            {
                                string strRequestGetBookingDetails = new TripJackRequestMapping().getFlightBookingDetailsRequest(request);
                                bookingLog(ref sbLogger, "Trip Jack GetBookingDetails Request", strRequestGetBookingDetails);
                                string strResponseGetBookingDetails = MakeServiceCallPOST(TripJackBookingDetailsUrl, strRequest, ref sbLogger);
                                bookingLog(ref sbLogger, "Trip Jack GetBookingDetails Response", strResponseGetBookingDetails);
                                if (!string.IsNullOrEmpty(strResponseGetBookingDetails))
                                {
                                    var daynamicObj = JsonConvert.DeserializeObject<dynamic>(strResponseGetBookingDetails);
                                    if (((string)daynamicObj["status"]["success"]).Equals("true", StringComparison.OrdinalIgnoreCase) && (daynamicObj["order"] != null && ((string)daynamicObj["order"]["status"]).Equals("SUCCESS", StringComparison.OrdinalIgnoreCase)))
                                    {
                                        List<string> str = new List<string>();
                                        List<string> tktList = new List<string>();
                                        foreach (var travellerInfos in daynamicObj["itemInfos"]["AIR"]["travellerInfos"])
                                        {
                                            foreach (var pnrDetails in travellerInfos["pnrDetails"])
                                            {
                                                if (!str.Contains((string)pnrDetails.First))
                                                {
                                                    str.Add((string)pnrDetails.First);
                                                }
                                            }
                                            if (travellerInfos["ticketNumberDetails"] != null)
                                            {
                                                List<string> tkt = new List<string>();
                                                foreach (var tktNo in travellerInfos["ticketNumberDetails"])
                                                {
                                                    if (!tkt.Contains((string)tktNo.First))
                                                    {
                                                        tkt.Add((string)tktNo.First);
                                                    }
                                                }
                                                string strTkt = string.Empty;
                                                foreach (var t in tkt)
                                                {
                                                    strTkt += (string.IsNullOrEmpty(strTkt) ? t : ("," + t));
                                                }
                                                if (!string.IsNullOrEmpty(strTkt))
                                                    tktList.Add(strTkt);
                                            }
                                        }
                                        if (str.Count >= 1) { response.PNR = str[0]; }
                                        if (str.Count >= 2) { response.ReturnPNR = str[1]; }
                                        else if (request.PriceID.Count >= 2) { response.ReturnPNR = str[0]; }
                                        for (int i = 0; i < tktList.Count; i++)
                                        {
                                            response.passengerDetails[i].ticketNo = tktList[i];
                                        }
                                        if (!string.IsNullOrEmpty(response.PNR))
                                        {
                                            response.invoice = new List<Invoice>();
                                            response.invoice.Add(new Invoice() { InvoiceAmount = (daynamicObj["order"]["amount"] != null ? Convert.ToDecimal(daynamicObj["order"]["amount"]) : 0), InvoiceNo = request.TjBookingID });
                                        }
                                        response.bookingStatus = BookingStatus.Ticketed;
                                    }
                                    else
                                    {
                                        response.bookingStatus = BookingStatus.InProgress;
                                        response.responseStatus.message = "Booking InProgress";
                                        bookingLog(ref sbLogger, "Trip Jack Booking No PNR Found", response.responseStatus.message);
                                    }
                                }
                                else
                                {
                                    response.bookingStatus = BookingStatus.InProgress;
                                    response.responseStatus.message = "Booking InProgress";
                                    bookingLog(ref sbLogger, "Trip Jack Booking No PNR Found2", response.responseStatus.message);
                                }
                            }
                            else
                            {
                                response.bookingStatus = BookingStatus.InProgress;
                                response.responseStatus.message = "Booking InProgress";
                                bookingLog(ref sbLogger, "Trip Jack Booking Hold", response.responseStatus.message);
                            }
                        }
                    }
                    else
                    {
                        if (dobj["errors"] != null && dobj["errors"].HasValues && dobj["errors"][0]["message"] != null)
                        {
                            response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = (string)dobj["errors"][0]["message"] };
                        }
                        else
                        {
                            response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Other resion" };
                        }
                        response.bookingStatus = BookingStatus.Failed;
                        bookingLog(ref sbLogger, " Trip Jack  Else 2 ", response.responseStatus.message);
                    }
                }
                else
                {
                    bookingLog(ref sbLogger, " Trip Jack  Else 1 ", "Booking Fail due to Trip Jack booking Respons is empty");
                    response.bookingStatus = BookingStatus.Failed;
                    response.responseStatus.message = "Booking Fail due to Trip Jack booking Respons is empty";
                }
            }
            catch (Exception ex)
            {
                response.bookingStatus = BookingStatus.Failed;
                response.responseStatus.message = ex.ToString();
                bookingLog(ref sbLogger, "Trip Jack Exption", ex.ToString());
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Error");
            }

            bookingLog(ref sbLogger, "Trip Jack  return Response", JsonConvert.SerializeObject(response));
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Booking");
        }




        public void BookFlightCRM(FlightBookingRequest request, ref FlightBookingResponse response)
        {
            StringBuilder sbLogger = new StringBuilder();
            try
            {
                string strRequest = new TripJackRequestMapping().getFlightBookRequest(request);
                bookingLog(ref sbLogger, "Trip Jack BookFlight Request", strRequest);

                string strResponse = MakeServiceCallPOST(TripJackBookUrl, strRequest, ref sbLogger);
                bookingLog(ref sbLogger, "Trip Jack BookFlight Response", strResponse);

                if (!string.IsNullOrEmpty(strResponse))
                {
                    var dobj = JsonConvert.DeserializeObject<dynamic>(strResponse);
                    if (((string)dobj["status"]["success"]).Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        if (dobj["order"] != null && ((string)dobj["order"]["status"]).Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                        {
                            List<string> str = new List<string>();
                            List<string> tktList = new List<string>();
                            foreach (var travellerInfos in dobj["itemInfos"]["AIR"]["travellerInfos"])
                            {
                                foreach (var pnrDetails in travellerInfos["pnrDetails"])
                                {
                                    if (!str.Contains((string)pnrDetails.First))
                                    {
                                        str.Add((string)pnrDetails.First);
                                    }
                                }
                                if (travellerInfos["ticketNumberDetails"] != null)
                                {
                                    List<string> tkt = new List<string>();
                                    foreach (var tktNo in travellerInfos["ticketNumberDetails"])
                                    {
                                        if (!tkt.Contains((string)tktNo.First))
                                        {
                                            tkt.Add((string)tktNo.First);
                                        }
                                    }
                                    string strTkt = string.Empty;
                                    foreach (var t in tkt)
                                    {
                                        strTkt += (string.IsNullOrEmpty(strTkt) ? t : ("," + t));
                                    }
                                    if (!string.IsNullOrEmpty(strTkt))
                                        tktList.Add(strTkt);
                                }
                            }
                            if (str.Count >= 1) { response.PNR = str[0]; }
                            if (str.Count >= 2) { response.ReturnPNR = str[1]; }
                            else if (request.PriceID.Count >= 2) { response.ReturnPNR = str[0]; }
                            for (int i = 0; i < tktList.Count; i++)
                            {
                                response.passengerDetails[i].ticketNo = tktList[i];
                            }
                            if (!string.IsNullOrEmpty(response.PNR))
                            {
                                response.invoice = new List<Invoice>();
                                response.invoice.Add(new Invoice() { InvoiceAmount = (dobj["order"]["amount"] != null ? Convert.ToDecimal(dobj["order"]["amount"]) : 0), InvoiceNo = request.TjBookingID });
                            }
                            response.bookingStatus = BookingStatus.Ticketed;
                        }
                        else
                        {

                            string strRequestGetBookingDetails = new TripJackRequestMapping().getFlightBookingDetailsRequest(request);
                            bookingLog(ref sbLogger, "Trip Jack GetBookingDetails Request", strRequestGetBookingDetails);
                            string strResponseGetBookingDetails = MakeServiceCallPOST(TripJackBookingDetailsUrl, strRequest, ref sbLogger);
                            bookingLog(ref sbLogger, "Trip Jack GetBookingDetails Response", strResponseGetBookingDetails);
                            if (!string.IsNullOrEmpty(strResponseGetBookingDetails))
                            {
                                var daynamicObj = JsonConvert.DeserializeObject<dynamic>(strResponseGetBookingDetails);
                                if (((string)daynamicObj["status"]["success"]).Equals("true", StringComparison.OrdinalIgnoreCase) && (daynamicObj["order"] != null && ((string)daynamicObj["order"]["status"]).Equals("SUCCESS", StringComparison.OrdinalIgnoreCase)))
                                {
                                    List<string> str = new List<string>();
                                    List<string> tktList = new List<string>();
                                    foreach (var travellerInfos in daynamicObj["itemInfos"]["AIR"]["travellerInfos"])
                                    {
                                        foreach (var pnrDetails in travellerInfos["pnrDetails"])
                                        {
                                            if (!str.Contains((string)pnrDetails.First))
                                            {
                                                str.Add((string)pnrDetails.First);
                                            }
                                        }
                                        if (travellerInfos["ticketNumberDetails"] != null)
                                        {
                                            List<string> tkt = new List<string>();
                                            foreach (var tktNo in travellerInfos["ticketNumberDetails"])
                                            {
                                                if (!tkt.Contains((string)tktNo.First))
                                                {
                                                    tkt.Add((string)tktNo.First);
                                                }
                                            }
                                            string strTkt = string.Empty;
                                            foreach (var t in tkt)
                                            {
                                                strTkt += (string.IsNullOrEmpty(strTkt) ? t : ("," + t));
                                            }
                                            if (!string.IsNullOrEmpty(strTkt))
                                                tktList.Add(strTkt);
                                        }
                                    }
                                    if (str.Count >= 1) { response.PNR = str[0]; }
                                    if (str.Count >= 2) { response.ReturnPNR = str[1]; }
                                    else if (request.PriceID.Count >= 2) { response.ReturnPNR = str[0]; }
                                    for (int i = 0; i < tktList.Count; i++)
                                    {
                                        response.passengerDetails[i].ticketNo = tktList[i];
                                    }
                                    if (!string.IsNullOrEmpty(response.PNR))
                                    {
                                        response.invoice = new List<Invoice>();
                                        response.invoice.Add(new Invoice() { InvoiceAmount = (daynamicObj["order"]["amount"] != null ? Convert.ToDecimal(daynamicObj["order"]["amount"]) : 0), InvoiceNo = request.TjBookingID });
                                    }
                                    response.bookingStatus = BookingStatus.Ticketed;
                                }
                            }
                            else
                            {
                                response.bookingStatus = BookingStatus.InProgress;
                                response.responseStatus.message = "Booking InProgress";
                                bookingLog(ref sbLogger, "Trip Jack Booking Hold", response.responseStatus.message);
                            }
                        }
                    }
                    else
                    {
                        if (dobj["errors"] != null && dobj["errors"].HasValues && dobj["errors"][0]["message"] != null)
                        {
                            response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = (string)dobj["errors"][0]["message"] };
                        }
                        else
                        {
                            response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Other resion" };
                        }
                        response.bookingStatus = BookingStatus.Failed;
                        bookingLog(ref sbLogger, " Trip Jack  Else 2 ", response.responseStatus.message);
                    }
                }
                else
                {
                    bookingLog(ref sbLogger, " Trip Jack  Else 1 ", "Booking Fail due to Trip Jack booking Respons is empty");
                    response.bookingStatus = BookingStatus.Failed;
                    response.responseStatus.message = "Booking Fail due to Trip Jack booking Respons is empty";
                }
            }
            catch (Exception ex)
            {
                response.bookingStatus = BookingStatus.Failed;
                response.responseStatus.message = ex.ToString();

                bookingLog(ref sbLogger, "Trip Jack Exption", ex.ToString());
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString() + "_CRM", "CRM");
            }

            bookingLog(ref sbLogger, "Trip Jack  return Response", JsonConvert.SerializeObject(response));
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "CRM");
        }


        public void bookingLog(ref StringBuilder sbLogger, string requestTitle, string logText)
        {
            sbLogger.Append(Environment.NewLine + "---------------------------------------------" + requestTitle + "" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
            sbLogger.Append(Environment.NewLine + logText);
            sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
        }
        private string MakeServiceCallPOST(string uri, string data, ref System.Text.StringBuilder sbLogger)
        {
            string responseFromServer = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.Expect100Continue = true;
            System.Net.WebRequest request = WebRequest.Create(uri);
            request.Method = "POST";
            string postData = data;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/json";
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("apikey", TripJackApikey);
            try
            {
                using (Stream strmWriter = request.GetRequestStream())
                {
                    strmWriter.Write(byteArray, 0, byteArray.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                    }
                    else
                    {
                        Stream responseStream = null;
                        if (response.Headers.Get("Content-Encoding") == "gzip")
                        {
                            responseStream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                        }
                        else
                        {
                            responseStream = response.GetResponseStream();
                        }
                        StreamReader reader = new System.IO.StreamReader(responseStream);
                        responseFromServer = reader.ReadToEnd();
                    }
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
                        responseFromServer = reader.ReadToEnd();

                    }
                }
            }
            catch { }
            return responseFromServer;
        }

        private string MakeServiceCallPOSTSearch(string uri, string data, ref System.Text.StringBuilder sbLogger)
        {
            string responseFromServer = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.Expect100Continue = true;
            System.Net.WebRequest request = WebRequest.Create(uri);
            request.Method = "POST";
            string postData = data;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/json";
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("apikey", TripJackApikey);
            //   request.Timeout = 12000;
            try
            {
                using (Stream strmWriter = request.GetRequestStream())
                {
                    strmWriter.Write(byteArray, 0, byteArray.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        string message = String.Format("POST failed. Received HTTP {0}", response.StatusCode);
                    }
                    else
                    {
                        Stream responseStream = null;
                        if (response.Headers.Get("Content-Encoding") == "gzip")
                        {
                            responseStream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                        }
                        else
                        {
                            responseStream = response.GetResponseStream();
                        }
                        StreamReader reader = new System.IO.StreamReader(responseStream);
                        responseFromServer = reader.ReadToEnd();
                    }
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
                        responseFromServer = reader.ReadToEnd();

                    }
                }
            }
            catch { }
            return responseFromServer;
        }


        //private string GetResponse(string url, string requestData)
        //{
        //    string response = string.Empty;
        //    try
        //    {
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
        //        byte[] data = Encoding.UTF8.GetBytes(requestData);
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //        request.Method = "POST";
        //        request.ContentType = "application/json";
        //        request.Headers.Add("Accept-Encoding", "gzip");
        //        request.Headers.Add("apikey",TripJackApikey);
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
        //public static void SuccessCount()
        //{
        //    string filePath = "counter.txt";

        //    int counter = ReadCounterFromFile(filePath);
        //    counter++;
        //    WriteCounterToFile(filePath, counter);

        //    //Console.WriteLine($"Counter updated to: {counter}");
        //}

        //public static int ReadCounterFromFile(string filePath)
        //{
        //    if (File.Exists(filePath))
        //    {
        //        try
        //        {
        //            string content = File.ReadAllText(filePath);
        //            if (int.TryParse(content, out int counter))
        //            {
        //                return counter;
        //            }
        //            else
        //            {
        //                Console.WriteLine("File content is not a valid integer. Initializing counter to 0.");
        //                return 0;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error reading from file: {ex.Message}");
        //            return 0;
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Counter file not found. Initializing counter to 0.");
        //        return 0;
        //    }
        //}

        ////public static void WriteCounterToFile(string filePath, int counter)
        ////{
        ////    try
        ////    {
        ////        File.WriteAllText(filePath, counter.ToString());
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Console.WriteLine($"Error writing to file: {ex.Message}");
        ////    }
        ////}
    }
}
