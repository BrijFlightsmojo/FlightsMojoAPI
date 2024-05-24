using Core;
using Core.Flight;
using Newtonsoft.Json;
using ServicesHub.Travelopedia.TravelopediaClass.SectorResponse;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelopedia
{
    public class TravelopediaServiceMapping
    {
        public static string TravelopediaSearchUrl = ConfigurationManager.AppSettings["TravelopediaSearchUrl"].ToString();
        public static string TravelopediaRePriceUrl = ConfigurationManager.AppSettings["TravelopediaRePriceUrl"].ToString();
        public static string TravelopediaAir_TempBooking = ConfigurationManager.AppSettings["TravelopediaAir_TempBooking"].ToString();
        public static string TravelopediaAir_Ticketing = ConfigurationManager.AppSettings["TravelopediaAir_Ticketing"].ToString();
        public static string TravelopediaAddPayment = ConfigurationManager.AppSettings["TravelopediaAddPayment"].ToString();
        public static string TravelopediaAir_TicketCancellation = ConfigurationManager.AppSettings["TravelopediaAir_TicketCancellation"].ToString();
        public static string TravelopediaGetInvoice = ConfigurationManager.AppSettings["TravelopediaGetInvoice"].ToString();
        public static string TravelopediaGetSectors = ConfigurationManager.AppSettings["TravelopediaGetSectors"].ToString();

        public FlightSearchResponseShort GetFlightResults(FlightSearchRequest request, bool isTravelopedia, bool isTravelopediaR)
        {
            FlightSearchResponseShort flightResponse = new FlightSearchResponseShort(request);
            StringBuilder sbLogger = new StringBuilder();
            if (FlightUtility.isWriteLog && !request.isMetaRequest)
            {
                bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
            }
            try
            {
                string strRequest = new TravelopediaRequestMapping().getFlightSearchRequest(request);
                //LogCreater.CreateLogFile(strRequest, "Log\\Travelopedia\\SendLog", FolderName, "SearchRequest.txt");

                strRequest = strRequest.Replace("-", "/");
                if (FlightUtility.isWriteLog && !request.isMetaRequest)
                {
                    bookingLog(ref sbLogger, "Travelopedia Request", strRequest);
                }
                string strResponse = MakeServiceCallPOST(TravelopediaSearchUrl, strRequest, ref sbLogger);
                //LogCreater.CreateLogFile(strResponse, "Log\\Travelopedia\\SendLog", FolderName, "SearchResponse.txt");
                if (FlightUtility.isWriteLog && !request.isMetaRequest)
                {
                    bookingLog(ref sbLogger, "Travelopedia Response", strResponse);
                }
                if (!string.IsNullOrEmpty(strResponse))
                {
                    TravelopediaClass.TravelopediaFlightSearchResponse Response = Newtonsoft.Json.JsonConvert.DeserializeObject<TravelopediaClass.TravelopediaFlightSearchResponse>(strResponse);
                    if (Response != null && Response.Response_Header != null && Response.Response_Header.Error_Code == "0000" && Response.Response_Header.Error_Desc.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        new TravelopediaResponseMapping().getResults(request, ref Response, ref flightResponse);
                        if (FlightUtility.isWriteLog && !request.isMetaRequest)
                        {
                            bookingLog(ref sbLogger, "Original Response", JsonConvert.SerializeObject(flightResponse));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder sbLoggerEx = new StringBuilder();
                bookingLog(ref sbLoggerEx, "Original Request", JsonConvert.SerializeObject(request));
                bookingLog(ref sbLogger, "Original Ex Response", JsonConvert.SerializeObject(flightResponse));
                bookingLog(ref sbLoggerEx, "Exception", ex.ToString());
                LogCreater.CreateLogFile(sbLoggerEx.ToString(), "Log\\Travelopedia\\SearchExeption", request.userSearchID + ".txt");
            }
            if (FlightUtility.isWriteLog && !request.isMetaRequest)
            {
                LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\Travelopedia\\Search", request.userSearchID + ".txt");
            }
            return flightResponse;
        }
        public FareQuoteResponse GetFlightRePrice(PriceVerificationRequest request)
        {
            FareQuoteResponse response = new FareQuoteResponse() { tgy_Flight_Key = new List<string>(), tgy_Block_Ticket_Allowed = new List<bool>() };
            response.responseStatus = new ResponseStatus();
            StringBuilder sbLogger = new StringBuilder();
            //bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));

            string strRequest = new TravelopediaRequestMapping().getFlightReviewRequest(request);
            //LogCreater.CreateLogFile(strRequest, "Log\\Travelopedia\\SendLog", FolderName, "FareReviewRequest.txt");
            bookingLog(ref sbLogger, "Flight RePrice Request", strRequest);
            string strResponse = MakeServiceCallPOST(TravelopediaRePriceUrl, strRequest, ref sbLogger);
            //LogCreater.CreateLogFile(strResponse, "Log\\Travelopedia\\SendLog", FolderName, "FareReviewResponse.txt");
            bookingLog(ref sbLogger, "Flight RePrice Response", strResponse);
            if (!string.IsNullOrEmpty(strResponse))
            {
                TravelopediaClass.RePrice.TravelopediaRePriceResponse tgyResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TravelopediaClass.RePrice.TravelopediaRePriceResponse>(strResponse);
                if (tgyResponse != null && tgyResponse.Response_Header != null && tgyResponse.Response_Header.Error_Code == "0000" &&
                    tgyResponse.Response_Header.Error_Desc.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    response.VerifiedTotalPrice = 0;
                    foreach (TravelopediaClass.RePrice.AirRepriceResponse arr in tgyResponse.AirRepriceResponses)
                    {
                        foreach (TravelopediaClass.RePrice.Fare fare in arr.Flight.Fares)
                        {
                            foreach (TravelopediaClass.RePrice.FareDetail item in fare.FareDetails)
                            {
                                if (item.PAX_Type == 0)
                                {
                                    response.VerifiedTotalPrice += item.Total_Amount * request.adults;
                                }
                                if (item.PAX_Type == 1)
                                {
                                    response.VerifiedTotalPrice += item.Total_Amount * request.child;
                                }
                                if (item.PAX_Type == 2)
                                {
                                    response.VerifiedTotalPrice += item.Total_Amount * request.infants;
                                }
                            }
                            response.tgy_Flight_Key.Add(arr.Flight.Flight_Key);
                            response.tgy_Block_Ticket_Allowed.Add(arr.Flight.Block_Ticket_Allowed);
                        }
                    }
                }
                else
                {
                    response.responseStatus = new ResponseStatus() { status = TransactionStatus.Error, Error_Code = tgyResponse.Response_Header.Error_Code, message = tgyResponse.Response_Header.Error_InnerException, Error_Desc = tgyResponse.Response_Header.Error_Desc };
                }
            }
            LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\Travelopedia\\CheckFare", request.userSearchID + ".txt");
            //LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\Travelopedia\\Booking", request.userSearchID, "FareQuote.txt");
            return response;
        }

      
        public void BookFlight(FlightBookingRequest request, ref FlightBookingResponse response)
        {
            StringBuilder sbLogger = new StringBuilder();
            string strRequest = new TravelopediaRequestMapping().getFlightTemp_BookingRequest(request);

            bookingLog(ref sbLogger, "Flight Temp Booking Request", strRequest);
            string strResponse = MakeServiceCallPOST(TravelopediaAir_TempBooking, strRequest, ref sbLogger);

            bookingLog(ref sbLogger, "Flight Temp Booking Response", strResponse);
            if (!string.IsNullOrEmpty(strResponse))
            {
                ServicesHub.Travelopedia.TravelopediaClass.TempBookingResponse.Travelopedia_TempBookingResponse tempBookingResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ServicesHub.Travelopedia.TravelopediaClass.TempBookingResponse.Travelopedia_TempBookingResponse>(strResponse);
                if (tempBookingResponse != null && tempBookingResponse.Response_Header != null && tempBookingResponse.Response_Header.Error_Code == "0000" &&
                   tempBookingResponse.Response_Header.Error_Desc.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    response.tgy_Booking_RefNo = request.tgy_Booking_RefNo = tempBookingResponse.Booking_RefNo;
                    #region AddPayment
                    string strAddPaymentRequest = new TravelopediaRequestMapping().getAddPayment(request);

                    bookingLog(ref sbLogger, "Flight AddPayment Request", strAddPaymentRequest);
                    string strAddPaymentResponse = MakeServiceCallPOST(TravelopediaAddPayment, strAddPaymentRequest, ref sbLogger);

                    bookingLog(ref sbLogger, "Flight AddPayment Response", strAddPaymentResponse);
                    if (!string.IsNullOrEmpty(strAddPaymentResponse))
                    {
                        //ServicesHub.Travelopedia.TravelopediaClass.BookingDetails.TravelopediaBookingDetailsResponse BookingResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ServicesHub.Travelopedia.TravelopediaClass.BookingDetails.TravelopediaBookingDetailsResponse>(strBookingResponse);
                        //if (BookingResponse != null && BookingResponse.Response_Header != null && BookingResponse.Response_Header.Status_Id == "11")
                        //{
                        //    response.PNR = BookingResponse.AirlinePNRDetails;
                        //    response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Success" };
                        //    response.bookingStatus = BookingStatus.Ticketed;
                        //}
                        //else if (BookingResponse != null && BookingResponse.Response_Header != null && BookingResponse.Response_Header.Status_Id == "22")
                        //{
                        //    response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Success" };
                        //    response.bookingStatus = BookingStatus.InProgress;
                        //}
                        //else
                        //{
                        //    response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = BookingResponse.Response_Header.Error_Desc };
                        //    response.bookingStatus = BookingStatus.Failed;
                        //}
                    }
                    else
                    {
                        response.responseStatus = new ResponseStatus() { status = TransactionStatus.Error, message = "Error" };
                        response.bookingStatus = BookingStatus.Failed;
                        bookingLog(ref sbLogger, "GetBookingDetails Request_Failed_Tgy1", request.bookingID.ToString());
                    }
                    #endregion
                    #region make booking
                    string strBookingRequest = new TravelopediaRequestMapping().getFlight_Ticketing(request);

                    bookingLog(ref sbLogger, "Flight Air_Ticketing Request", strBookingRequest);
                    string strBookingResponse = MakeServiceCallPOST(TravelopediaAir_Ticketing, strBookingRequest, ref sbLogger);

                    bookingLog(ref sbLogger, "Flight Air_Ticketing Response", strBookingResponse);
                    if (!string.IsNullOrEmpty(strBookingResponse))
                    {
                        ServicesHub.Travelopedia.TravelopediaClass.BookingDetails.TravelopediaBookingDetailsResponse BookingResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ServicesHub.Travelopedia.TravelopediaClass.BookingDetails.TravelopediaBookingDetailsResponse>(strBookingResponse);
                        if (BookingResponse != null && BookingResponse.Response_Header != null && BookingResponse.Response_Header.Status_Id == "11")
                        {
                            response.PNR = BookingResponse.AirlinePNRDetails[0].AirlinePNRs[0].Airline_PNR;
                            if (BookingResponse.AirlinePNRDetails.Count > 1)
                            {
                                response.ReturnPNR = BookingResponse.AirlinePNRDetails[1].AirlinePNRs[0].Airline_PNR;
                            }

                            if (request.flightResult.Count > 1 && (response.ReturnPNR == null || response.ReturnPNR == ""))
                            {
                                response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Success" };
                                response.bookingStatus = BookingStatus.InProgress;
                            }
                            else
                            {
                                response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Success" };
                                response.bookingStatus = BookingStatus.Ticketed;
                            }
                            
                        }
                        else if (BookingResponse != null && BookingResponse.Response_Header != null && BookingResponse.Response_Header.Status_Id == "22")
                        {
                            //  response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = "Success" };
                            //response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = BookingResponse.Response_Header.Error_InnerException };
                            response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = BookingResponse.Response_Header.Error_Desc };
                            response.bookingStatus = BookingStatus.InProgress;
                        }
                        else
                        {
                            response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = BookingResponse.Response_Header.Error_Desc };
                            //   response.bookingStatus = BookingStatus.Failed;
                            //response.responseStatus = new ResponseStatus() { status = TransactionStatus.Success, message = BookingResponse.Response_Header.Error_InnerException };
                            response.bookingStatus = BookingStatus.InProgress;
                        }
                    }
                    else
                    {
                        response.responseStatus = new ResponseStatus() { status = TransactionStatus.Error, message = "Error" };
                        response.bookingStatus = BookingStatus.Failed;
                        bookingLog(ref sbLogger, "GetBookingDetails Request_Failed_tgy2", request.bookingID.ToString());
                    }
                    #endregion
                    #region Invoice & TicketNo
                    if (response.bookingStatus == BookingStatus.Ticketed)
                    {
                        string strInvoiceRequest = new TravelopediaRequestMapping().getFlight_TicketingInvoice(request);

                        bookingLog(ref sbLogger, "Flight Air_TicketingInvoice Request", strInvoiceRequest);
                        string strInvoiceResponse = MakeServiceCallPOST(TravelopediaGetInvoice, strInvoiceRequest, ref sbLogger);
                        bookingLog(ref sbLogger, "Flight Air_TicketingInvoice Response", strInvoiceResponse);

                        if (!string.IsNullOrEmpty(strInvoiceResponse))
                        {
                            var daynamicObj = JsonConvert.DeserializeObject<dynamic>(strInvoiceResponse);
                            response.invoice = new List<Invoice>();
                            try
                            {
                                for (int i = 0; i < request.flightResult.Count; i++)
                                {
                                    response.invoice.Add(new Invoice() { InvoiceAmount = (daynamicObj["AirPNRDetails"][i]["Gross_Amount"] != null ? Convert.ToDecimal(daynamicObj["AirPNRDetails"][i]["Gross_Amount"]) : 0), InvoiceNo = daynamicObj["Invoice_Number"] });
                                }
                                response.bookingStatus = BookingStatus.Ticketed;
                            }
                            catch (Exception ex)
                            {
                                ex.ToString();
                            }

                        }
                    }
                    #endregion

                }
            }
            else
            {
                response.responseStatus = new ResponseStatus() { status = TransactionStatus.Error, message = "Error" };
                response.bookingStatus = BookingStatus.Failed;
                bookingLog(ref sbLogger, "GetBookingDetails Request_Failed_tgy3", request.bookingID.ToString());
            }
            LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\Travelopedia\\Booking", request.userSearchID + ".txt");
        }


        //#endregion



        public void CancelBooking(string strRequest, ref System.Text.StringBuilder sbLogger)
        {
            //string strRequest = new TravelopediaRequestMapping().getFlightTemp_BookingRequest(request);

            bookingLog(ref sbLogger, "Flight Cancel Request", strRequest);
            string strResponse = MakeServiceCallPOST(TravelopediaAir_TicketCancellation, strRequest, ref sbLogger);

            bookingLog(ref sbLogger, "Flight Temp Booking Response", strResponse);

            LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\Travelopedia\\Booking", "cancel.txt");
        }

        //public FlightBookingResponse GetFlightBookingDetails(FlightBookingRequest request)
        //{
        //    FlightBookingResponse response = new FlightBookingResponse();
        //    StringBuilder sbLogger = new StringBuilder();
        //    //bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
        //    string strRequest = new TravelopediaRequestMapping().getFlightBookingDetailsRequest(request);
        //    //LogCreater.CreateLogFile(strRequest, "Log\\Travelopedia\\SendLog", FolderName, "BookingDetailsRequest.txt");
        //    bookingLog(ref sbLogger, "FlightBooking Request", strRequest);
        //    string strResponse = MakeServiceCallPOST(TravelopediaBookingDetailsUrl, strRequest, ref sbLogger);
        //    //LogCreater.CreateLogFile(strResponse, "Log\\Travelopedia\\SendLog", FolderName, "BookingDetailsResponse.txt");
        //    bookingLog(ref sbLogger, "FlightBooking Response", strResponse);
        //    if (!string.IsNullOrEmpty(strResponse))
        //    {
        //        //TravelopediaClass.Booking.TravelopediaBookingResponse tjResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TravelopediaClass.Booking.TravelopediaBookingResponse>(strResponse);
        //        //response.TjBookingID = tjResponse.bookingId;
        //        //if (tjResponse != null && tjResponse.status != null && tjResponse.status.success)
        //        //{
        //        //    bookingLog(ref sbLogger, "FlightReview Response", JsonConvert.SerializeObject(tjResponse));
        //        //}
        //    }
        //    LogCreater.CreateLogFile(sbLogger.ToString(), "Log\\Travelopedia\\Booking", request.userSearchID, "booking.txt");
        //    return response;
        //}

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
            // Set the Method property of the request to POST.
            request.Method = "POST";
            string postData = data;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/json";
            request.Headers.Add("Accept-Encoding", "gzip");


            //// Set the ContentLength property of the WebRequest.
            //request.ContentLength = byteArray.Length;

            //Stream dataStream = request.GetRequestStream();
            //// Write the data to the request stream.
            //dataStream.Write(byteArray, 0, byteArray.Length);
            //// Close the Stream object.
            //dataStream.Close();
            // Get the response.

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
                        //if (responseStream != null)
                        //    xdoc = XDocument.Load(new StreamReader(responseStream));
                    }

                }
                //WebResponse response = request.GetResponse();
                //dataStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);

                //// Open the stream using a StreamReader for easy access.
                //StreamReader reader = new StreamReader(dataStream);
                //// Read the content.
                //responseFromServer = reader.ReadToEnd();

                //// Clean up the streams.
                //reader.Close();
                //dataStream.Close();
                //response.Close();
            }
            catch (WebException webex)
            {
                WebResponse errResp = webex.Response;
                Stream responseStream = null;
                if (errResp != null && errResp.Headers != null && errResp.Headers.Get("Content-Encoding") != null && errResp.Headers.Get("Content-Encoding") == "gzip")
                {
                    responseStream = new System.IO.Compression.GZipStream(errResp.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                }
                else if (errResp != null)
                {
                    responseStream = errResp.GetResponseStream();
                }
                if (responseStream != null)
                {
                    StreamReader reader = new StreamReader(responseStream);
                    responseFromServer = reader.ReadToEnd();
                }
                if (string.IsNullOrWhiteSpace(responseFromServer) == false)
                    sbLogger.Append("ErrorCatch1: Request" + data + "  Response:-" + responseFromServer);
                else
                    sbLogger.Append("ErrorCatch1: Request" + webex.ToString());
            }
            catch (Exception ex)
            {
                sbLogger.Append("ErrorCatch2: Error" + ex.ToString());
            }
            // Get the response.

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
        //        request.Headers.Add("apikey",TravelopediaApikey);
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

        public void getSectors()
        {
            StringBuilder sbLogger = new StringBuilder();
            string segm = string.Empty;
            WebClient client = new WebClient();
            string strRequest = new TravelopediaRequestMapping().getRouteRequest();
            bookingLog(ref sbLogger, "Sectors Request", strRequest);
            string strResponse = MakeServiceCallPOST(TravelopediaGetSectors, strRequest, ref sbLogger);
            bookingLog(ref sbLogger, "Sectors Response", strResponse);
            var dobj = JsonConvert.DeserializeObject<dynamic>(strResponse);
            //    var kk = JsonConvert.DeserializeObject<SectorResponse>(strResponse);
            // var dobj = JsonConvert.DeserializeObject<SectorResponse>(kk);
            var sec1 = string.Empty;
            foreach (var SectorsPIs in dobj["SectorsPIs"])
            {
                string origin = SectorsPIs["Origin"].ToString();
                string destination = SectorsPIs["Destination"].ToString();
                string AvlDate = SectorsPIs["AvailableDates"].ToString();
                string str = AvlDate.ToString();
                string[] split = str.Split('|');
                string date = string.Empty;
                foreach (var AvailableDates in SectorsPIs)
                {
                    foreach (string aveldate in split)
                    {


                        if (!string.IsNullOrEmpty(AvailableDates.ToString()))
                        {
                            string dt = DateTime.ParseExact(aveldate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                            if (date.IndexOf(dt) == -1)
                                date += (string.IsNullOrEmpty(date) ? dt : ("_" + dt));
                        }
                    }
                }
                new DAL.FixDepartueRoute.RoutesDetails().SaveSatkarRouteswithDate(origin, destination, date, (int)GdsType.Travelopedia);
            }
        }
    }
}
