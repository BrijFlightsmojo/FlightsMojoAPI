using Core;
using Core.Flight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServicesHub.Amadeus
{
    public class AmadeusServiceMappking
    {
       

      
        public GfPriceVerifyResponse GfPriceVerify(GfPriceVerifyRequest request)
        {
            StringBuilder sbLogger = new StringBuilder();
            bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));
            GfPriceVerifyResponse response = new GfPriceVerifyResponse();
            response.responseStatus = new ResponseStatus();
            response.fare = new Fare();
            try
            {

                AmadeusRequestMapping objAmadeusRequestMapping = new AmadeusRequestMapping();
                int adtQnty = 0, chdQnty = 0, infQnty = 0;
                string InformativeBestPricingWithoutPNRRequest = objAmadeusRequestMapping.Fare_InformativeBestPricingWithoutPNR(request, ref adtQnty, ref chdQnty, ref infQnty);
                bookingLog(ref sbLogger, "InformativeBestPricingWithoutPNRRequest", InformativeBestPricingWithoutPNRRequest);
                string strResponse = GetResponseStr(InformativeBestPricingWithoutPNRRequest, AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Fare_InformativeBestPricingWithoutPNR), ref sbLogger);
                bookingLog(ref sbLogger, "InformativeBestPricingWithoutPNR Response", strResponse);
                new AmadeusResponseMapping_XML().getPriceVerifyResponse(strResponse, request, ref response, ref adtQnty, ref chdQnty, ref infQnty);
                bookingLog(ref sbLogger, "Original response", JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "Exeption 1", ex.ToString());
            }
            if (response.responseStatus.status == TransactionStatus.Success)
            {
                new LogWriter(sbLogger.ToString(), ("SkyBirdPriceVerify_" + request.flightResult[0].FlightSegments[0].Segments[0].Origin + "_" + request.flightResult[0].FlightSegments[0].Segments.LastOrDefault().Destination), "Amadeus");
            }
            else
            {
                new LogWriter(sbLogger.ToString(), ("SkyBirdPriceVerify_" + request.flightResult[0].FlightSegments[0].Segments[0].Origin + "_" + request.flightResult[0].FlightSegments[0].Segments.LastOrDefault().Destination), "Amadeus");
            }
            return response;
        }

        public void BookFlight(FlightBookingRequest BookingRequest, ref FlightBookingResponse bookingResponse)
        {
            StringBuilder sbLogger = new StringBuilder();
            string controlNumber = string.Empty;
            AmadeusRequestMapping objAmadeusRequestMapping = new AmadeusRequestMapping();
            //bookingLog(ref sbLogger, "Booking Original Request", JsonConvert.SerializeObject(BookingRequest));

            AmadeusSessionTemplate objCurrentSession = null;
            #region 1- Air_SellFromRecommendation
            string AirSellRequest = objAmadeusRequestMapping.Air_SellFromRecommendation_1(BookingRequest);
            XDocument AirSellResponse = GetResponse(AirSellRequest, AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Air_SellFromRecommendation));


            bookingLog(ref sbLogger, "Air_SellFromRecommendation Request", AirSellRequest);
            if (AirSellResponse != null)
                bookingLog(ref sbLogger, "Air_SellFromRecommendation response", AirSellResponse.ToString(SaveOptions.DisableFormatting));

            objCurrentSession = getSessionInformation(AirSellResponse);
            #endregion

            if (verifySeatAvailabilityAirSell(AirSellResponse, ref sbLogger) == true)
            {
                #region 2-PNR_AddElementRequest
                string PNR_AddElementRequest = objAmadeusRequestMapping.PNR_AddElementRequest_New(BookingRequest, objCurrentSession);
                XDocument PNR_AddElementResponse = GetResponse(PNR_AddElementRequest, AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.PNR_AddMultiElements));

                bookingLog(ref sbLogger, "PNR_AddElementRequest Request", PNR_AddElementRequest);
                bookingLog(ref sbLogger, "PNR_AddElementRequest response", PNR_AddElementResponse.ToString(SaveOptions.DisableFormatting));
                #endregion
                if (PNR_AddElementResponse != null)
                {
                    #region getSessionSequenceNo
                    objCurrentSession = getSessionInformation(PNR_AddElementResponse);
                    #endregion

                    #region 3-Fare_PricePNRWithBookingClassRequest

                    string Fare_PricePNRWithBookingClassRequest = objAmadeusRequestMapping.FarePricePNRWithBookingClassRequest3(BookingRequest.flightResult[0].valCarrier, objCurrentSession, BookingRequest);
                    XDocument Fare_PricePNRWithBookingClass_Response = GetResponse(Fare_PricePNRWithBookingClassRequest, AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Fare_PricePNRWithBookingClass));


                    bookingLog(ref sbLogger, "Fare_PricePNRWithBookingClass Request", Fare_PricePNRWithBookingClassRequest);
                    bookingLog(ref sbLogger, "Fare_PricePNRWithBookingClass response", Fare_PricePNRWithBookingClass_Response.ToString(SaveOptions.DisableFormatting));
                    #endregion
                    if (Fare_PricePNRWithBookingClass_Response != null)
                    {
                        #region getSessionSequenceNo
                        objCurrentSession = getSessionInformation(Fare_PricePNRWithBookingClass_Response);
                        #endregion
                        XNamespace ns = "http://xml.amadeus.com/TPCBRR_18_1_1A";
                        Boolean isErrorExist = Fare_PricePNRWithBookingClass_Response.Descendants(ns + "applicationError").Any();
                        Boolean isPriceChanged = false;
                        if (isErrorExist == false)
                        {
                            StringBuilder strTST = new StringBuilder();
                            #region getTstReferenceFrom_FarePricingInformation
                            var fareReference = from fdata in Fare_PricePNRWithBookingClass_Response.Descendants(ns + "fareReference")
                                                where fdata.Element(ns + "referenceType").Value == "TST"
                                                select new
                                                {
                                                    referenceType = fdata.Element(ns + "referenceType").Value,
                                                    uniqueReference = fdata.Element(ns + "uniqueReference").Value
                                                };
                            if (fareReference != null && fareReference.ToList().Count > 0)
                            {
                                foreach (var item in fareReference)
                                {

                                    strTST.Append("<psaList>");
                                    strTST.Append("<itemReference>");
                                    strTST.Append("<referenceType>" + item.referenceType + "</referenceType>");
                                    strTST.Append("<uniqueReference>" + item.uniqueReference + "</uniqueReference>");
                                    strTST.Append("</itemReference>");
                                    strTST.Append("</psaList>");
                                }
                            }
                            #endregion
                            #region fareDataInformation
                            if (true)
                            {
                                //try
                                //{
                                //    decimal amount = (from fareDataInfo in Fare_PricePNRWithBookingClass_Response.Descendants(ns + "fareDataSupInformation")
                                //                      where fareDataInfo.Element(ns + "fareDataQualifier").Value == "712"
                                //                      select new
                                //                      {
                                //                          fareAmount = Convert.ToDecimal(fareDataInfo.Element(ns + "fareAmount").Value),
                                //                          //  fareCurrency = fareDataInfo.Element(ns + "fareCurrency").Value,
                                //                      }).Sum(x => x.fareAmount);

                                //    decimal publisFare = BookingRequest.updatedBookingAmount > 0 ? BookingRequest.updatedBookingAmount : (((BookingRequest.adults * (BookingRequest.flightResult.fare.adultFare + BookingRequest.flightResult.fare.adultTax))) +
                                //        ((BookingRequest.child * (BookingRequest.flightResult[0].Fare.childFare + BookingRequest.flightResult.fare.childTax))) +
                                //        ((BookingRequest.infants * (BookingRequest.flightResult.fare.infantFare + BookingRequest.flightResult.fare.infantFare))) +
                                //        ((BookingRequest.infantsWs * (BookingRequest.flightResult.fare.infantWsFare + BookingRequest.flightResult.fare.infantWsTax))));
                                //    if (amount - publisFare > 2)
                                //    {
                                //        bookingResponse.responseStatus.status = TransactionStatus.Error;
                                //        bookingResponse.responseStatus.message = "Increase Passenger Fare";
                                //        bookingResponse.updatedBookingAmount = amount;
                                //        isErrorExist = false; isPriceChanged = true;
                                //    }
                                //}
                                //catch (Exception)
                                //{

                                //}
                            }
                            #endregion



                            if (string.IsNullOrEmpty(strTST.ToString().Trim()) == false)
                            {
                                #region 4-Ticket_CreateTSTFromPricing
                                StringBuilder strXml = new StringBuilder();
                                strXml.Append("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">");
                                strXml.Append(objAmadeusRequestMapping.GetSessionSoapHeader(AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Ticket_CreateTSTFromPricing), objCurrentSession, "InSeries"));
                                strXml.Append("<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
                                strXml.Append("<Ticket_CreateTSTFromPricing xmlns=\"http://xml.amadeus.com/TAUTCQ_04_1_1A\">");
                                strXml.Append(strTST.ToString());
                                strXml.Append("</Ticket_CreateTSTFromPricing>");
                                strXml.Append("</s:Body>");
                                strXml.Append("</s:Envelope>");

                                XDocument Ticket_CreateTSTResponse = GetResponse(strXml.ToString(), AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Ticket_CreateTSTFromPricing));

                                bookingLog(ref sbLogger, "Ticket_CreateTSTFromPricing Request", strXml.ToString());
                                bookingLog(ref sbLogger, "Ticket_CreateTSTFromPricing response", Ticket_CreateTSTResponse.ToString(SaveOptions.DisableFormatting));
                                if (Ticket_CreateTSTResponse != null)
                                {
                                    #region getSessionSequenceNo
                                    objCurrentSession = getSessionInformation(Ticket_CreateTSTResponse);
                                    #endregion
                                    ns = "http://xml.amadeus.com/TAUTCR_04_1_1A";
                                    isErrorExist = Ticket_CreateTSTResponse.Descendants(ns + "applicationError").Any();
                                }
                                #endregion
                            }
                            if (isErrorExist == false)
                            {
                                #region 5-PNRSave
                                StringBuilder strPnrSave = new StringBuilder();
                                strPnrSave.Append("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">");
                                strPnrSave.Append(objAmadeusRequestMapping.GetSessionSoapHeader(AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.PNR_AddMultiElements), objCurrentSession, "InSeries"));
                                strPnrSave.Append("<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
                                strPnrSave.Append("<PNR_AddMultiElements xmlns=\"http://xml.amadeus.com/PNRADD_16_1_1A\">");
                                strPnrSave.Append("<pnrActions>");
                                strPnrSave.Append("<optionCode>11</optionCode>");
                                strPnrSave.Append("</pnrActions>");
                                strPnrSave.Append("</PNR_AddMultiElements>");
                                strPnrSave.Append("</s:Body>");
                                strPnrSave.Append("</s:Envelope>");

                                XDocument PNRSaveResponse = GetResponse(strPnrSave.ToString(), AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.PNR_AddMultiElements));

                                bookingLog(ref sbLogger, "PNR_AddMultiElements Request", strPnrSave.ToString());
                                bookingLog(ref sbLogger, "PNR_AddMultiElements response", PNRSaveResponse.ToString(SaveOptions.DisableFormatting));

                                if (PNRSaveResponse != null)
                                {
                                    //bookingLog(ref sbLogger, "PNR_AddMultiElements-Save-Response", PNRSaveResponse.ToString(SaveOptions.DisableFormatting));
                                    #region getSessionSequenceNo
                                    objCurrentSession = getSessionInformation(PNRSaveResponse);
                                    #endregion
                                    try
                                    {
                                        #region ExtractPNRfromResponse
                                        ns = "http://xml.amadeus.com/PNRACC_17_1_1A";
                                        var PNR_Reply = from allokseg in PNRSaveResponse.Descendants(ns + "reservationInfo").Descendants(ns + "reservation")
                                                        select new
                                                        {
                                                            controlNumber = allokseg.Element(ns + "controlNumber").Value
                                                        };

                                        if (PNR_Reply != null && PNR_Reply.ToList().Count > 0)
                                        {
                                            foreach (var pnrData in PNR_Reply)
                                            {
                                                controlNumber = (controlNumber == string.Empty ? pnrData.controlNumber : (controlNumber + "|" + pnrData.controlNumber));
                                            }
                                        }
                                        bookingResponse.PNR = controlNumber;
                                        #endregion
                                    }
                                    catch //(Exception ex)
                                    {

                                    }
                                    if (string.IsNullOrEmpty(controlNumber) == false)
                                    {
                                        bookingResponse.responseStatus.status = TransactionStatus.Success;
                                        bookingResponse.responseStatus.message = "Success";
                                    }
                                    else
                                    {
                                        bookingResponse.responseStatus.status = TransactionStatus.Error;
                                        bookingResponse.responseStatus.message = "PNR Creation Failed";
                                    }
                                }
                                else
                                {
                                    bookingResponse.responseStatus.status = TransactionStatus.Error;
                                    bookingResponse.responseStatus.message = "PNR Save Error";
                                }
                                #endregion
                            }
                            else
                            {
                                if (isPriceChanged == false)
                                {
                                    bookingResponse.responseStatus.status = TransactionStatus.Error;
                                    bookingResponse.responseStatus.message = "TST Error";
                                }
                            }
                        }
                        else
                        {
                            bookingResponse.responseStatus.status = TransactionStatus.Error;
                            bookingResponse.responseStatus.message = "Unable to Search Price";
                        }
                    }
                }
                else
                {
                    bookingResponse.responseStatus.status = TransactionStatus.Error;
                    bookingResponse.responseStatus.message = "Add Passenger XML is null";
                }
            }
            else
            {
                bookingResponse.responseStatus.status = TransactionStatus.Error;
                bookingResponse.responseStatus.message = "Segment Not Available";
            }
            #region SignOut
            StringBuilder strSignOut = new StringBuilder();
            strSignOut.Append("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            strSignOut.Append(objAmadeusRequestMapping.GetSessionSoapHeader(AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Security_SignOut), objCurrentSession, "End"));
            strSignOut.Append("<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            strSignOut.Append("<Security_SignOut xmlns=\"http://xml.amadeus.com/VLSSOQ_04_1_1A\"></Security_SignOut>");
            strSignOut.Append("</s:Body>");
            strSignOut.Append("</s:Envelope>");


            XDocument signOutRes = GetResponse(strSignOut.ToString(), AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Security_SignOut));
            bookingLog(ref sbLogger, "Security_SignOut Request", strSignOut.ToString());
            bookingLog(ref sbLogger, "Security_SignOut response", signOutRes.ToString(SaveOptions.DisableFormatting));
            #endregion
            new ServicesHub.LogWriter_New(sbLogger.ToString(), BookingRequest.bookingID.ToString(), "Booking");
        }

      

        private string returnSiteName(SiteId SiteId)
        {
            switch (SiteId)
            {

                case SiteId.FlightsMojoIN: return "FLIGHTSMOJO Website";

                default: return (SiteId.ToString().ToUpper() + " Website");
            }
        }

        public void bookingLog(ref StringBuilder sbLogger, string requestTitle, string logText)
        {
            sbLogger.Append(Environment.NewLine + "---------------------------------------------" + requestTitle + "" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
            sbLogger.Append(Environment.NewLine + logText);
            sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
        }
    
        public XDocument GetResponse(string _request, string soapAction)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string strResult = string.Empty;
            XDocument xdoc = null;
            //  ServicePointManager.DefaultConnectionLimit = 5;
            #region TopPart
            byte[] bytes = new ASCIIEncoding().GetBytes(_request);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AmadeusConfiguration.targetURL);
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Method = "POST";
            request.Headers.Add("SOAPAction", "http://webservices.amadeus.com/" + soapAction);
            request.ContentLength = bytes.Length;
            request.ContentType = "text/xml; charset=utf-8";
            //request.Timeout = 50000;
            request.Timeout = 100000;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Proxy = null;
            request.KeepAlive = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.ConnectionLimit = 100;
            request.ServicePoint.MaxIdleTime = 5000;

            #endregion


            try
            {
                using (Stream strmWriter = request.GetRequestStream())
                {
                    strmWriter.Write(bytes, 0, bytes.Length);
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

                        //StreamReader reader = null;
                        // reader = new System.IO.StreamReader(responseStream);
                        //strResult = reader.ReadToEnd();
                        if (responseStream != null)
                            xdoc = XDocument.Load(new StreamReader(responseStream));
                    }

                }
            }
            catch (WebException webex)
            {
                if (webex.Message.Contains("timed out") == false && webex.Response != null)
                {
                    WebResponse errResp = webex.Response;
                    // using (Stream respStream = errResp.GetResponseStream())
                    // {
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
                    string text = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(text) == false)
                        xdoc = XDocument.Parse(text);
                    //}
                }

            }
            return xdoc;
        }

        public XDocument GetResponse(string _request, string soapAction, ref StringBuilder sbLogger)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string strResult = string.Empty;
            XDocument xdoc = null;
            //  ServicePointManager.DefaultConnectionLimit = 5;
            #region TopPart
            byte[] bytes = new ASCIIEncoding().GetBytes(_request);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AmadeusConfiguration.targetURL);
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Method = "POST";
            request.Headers.Add("SOAPAction", "http://webservices.amadeus.com/" + soapAction);
            request.ContentLength = bytes.Length;
            request.ContentType = "text/xml; charset=utf-8";
            //request.Timeout = 50000;
            request.Timeout = 1000000;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Proxy = null;
            request.KeepAlive = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.ConnectionLimit = 100;
            request.ServicePoint.MaxIdleTime = 50000;

            #endregion
            try
            {
                using (Stream strmWriter = request.GetRequestStream())
                {
                    strmWriter.Write(bytes, 0, bytes.Length);
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

                        //StreamReader reader = null;
                        // reader = new System.IO.StreamReader(responseStream);
                        //strResult = reader.ReadToEnd();
                        if (responseStream != null)
                            xdoc = XDocument.Load(new StreamReader(responseStream));
                    }

                }
            }
            catch (WebException webex)
            {
                try
                {
                    if (webex != null)
                    {
                        if (webex.Message.Contains("timed out") == false && webex.Response != null)
                        {
                            WebResponse errResp = webex.Response;
                            // using (Stream respStream = errResp.GetResponseStream())
                            // {
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
                            string text = reader.ReadToEnd();
                            if (string.IsNullOrWhiteSpace(text) == false)
                                xdoc = XDocument.Parse(text);
                            //}
                        }
                    }
                    else
                    {
                        bookingLog(ref sbLogger, "Catch-Null Webex", "");
                    }
                }
                catch (Exception ex)
                {
                    bookingLog(ref sbLogger, "Catch-1", ex.ToString());
                }

            }
            return xdoc;
        }
        public string GetResponseStr(string _request, string soapAction, ref StringBuilder sbLogger)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string strResult = string.Empty;
            //XDocument xdoc = null;
            //  ServicePointManager.DefaultConnectionLimit = 5;
            #region TopPart
            byte[] bytes = new ASCIIEncoding().GetBytes(_request);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AmadeusConfiguration.targetURL);
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Method = "POST";
            request.Headers.Add("SOAPAction", "http://webservices.amadeus.com/" + soapAction);
            request.ContentLength = bytes.Length;
            request.ContentType = "text/xml; charset=utf-8";
            //request.Timeout = 50000;
            request.Timeout = 1000000;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Proxy = null;
            request.KeepAlive = false;
            request.ServicePoint.UseNagleAlgorithm = false;
            request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.ConnectionLimit = 100;
            request.ServicePoint.MaxIdleTime = 50000;

            #endregion
            try
            {
                using (Stream strmWriter = request.GetRequestStream())
                {
                    strmWriter.Write(bytes, 0, bytes.Length);
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
                        strResult = reader.ReadToEnd();
                        //if (responseStream != null)
                        //    xdoc = XDocument.Load(new StreamReader(responseStream));
                    }

                }
            }
            catch (WebException webex)
            {
                try
                {
                    if (webex != null)
                    {
                        if (webex.Message.Contains("timed out") == false && webex.Response != null)
                        {
                            WebResponse errResp = webex.Response;
                            // using (Stream respStream = errResp.GetResponseStream())
                            // {
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
                            strResult = reader.ReadToEnd();
                            //if (string.IsNullOrWhiteSpace(text) == false)
                            //    xdoc = XDocument.Parse(text);

                            //}
                        }
                    }
                    else
                    {
                        bookingLog(ref sbLogger, "Catch-Null Webex", "");
                    }
                }
                catch (Exception ex)
                {
                    bookingLog(ref sbLogger, "Catch-1", ex.ToString());
                }

            }
            return strResult;
        }

        private Boolean verifySeatAvailabilityAirSell(XDocument xdoc, ref StringBuilder sbLogger)
        {
            #region EnsureSeatAvailable
            Boolean isSegmentSellAble = false;
            if (xdoc != null)
            {
                #region NameSpace
                XNamespace ns = "http://xml.amadeus.com/ITARES_05_2_IA";
                #endregion

                var AllOKSegments = from allokseg in xdoc.Descendants(ns + "segmentInformation")
                                    from acDetails in allokseg.Descendants(ns + "actionDetails")
                                    from fltdetails in allokseg.Descendants(ns + "flightDetails")
                                    select new
                                    {
                                        statusCode = acDetails.Element(ns + "statusCode").Value,
                                        DestFrom = fltdetails.Element(ns + "boardPointDetails").Element(ns + "trueLocationId").Value,
                                        DestTo = fltdetails.Element(ns + "offpointDetails").Element(ns + "trueLocationId").Value,
                                    };
                if (AllOKSegments.ToList().Count > 0)
                {
                    var statusOkOnly = (from okonly in AllOKSegments
                                        where okonly.statusCode == "OK"
                                        select okonly).ToList();
                    if (AllOKSegments.ToList().Count == statusOkOnly.Count)
                    {
                        isSegmentSellAble = true;
                        sbLogger.Append(Environment.NewLine + "---------------------------------------------Air_SellFromRecommendation_Segment Status " + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
                        sbLogger.Append(Environment.NewLine + "All segments have OK status");
                        sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                    }
                    else
                    {
                        sbLogger.Append(Environment.NewLine + "---------------------------------------------Air_SellFromRecommendation_Segment Status " + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
                        foreach (var item in AllOKSegments)
                        {
                            sbLogger.Append(Environment.NewLine + item.DestFrom + " ------- " + item.DestTo + "------" + item.statusCode);
                        }
                        sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                    }
                }
            }
            #endregion

            return isSegmentSellAble;
        }

        public AmadeusSessionTemplate getSessionInformation(XDocument xmlDoc)
        {
            AmadeusSessionTemplate objSessionObject = new AmadeusSessionTemplate();
            if (xmlDoc != null)
            {
                #region ExtractSessionValue
                XNamespace ns = "http://xml.amadeus.com/2010/06/Session_v3";
                var sessionKey = from sesskey in xmlDoc.Descendants(ns + "Session")
                                 select new
                                 {
                                     TransactionStatusCode = sesskey.Attribute("TransactionStatusCode").Value,
                                     SessionId = sesskey.Element(ns + "SessionId").Value,
                                     SequenceNumber = sesskey.Element(ns + "SequenceNumber").Value,
                                     SecurityToken = sesskey.Element(ns + "SecurityToken").Value,
                                 };
                if (sessionKey != null && sessionKey.ToList().Count > 0)
                {
                    foreach (var item in sessionKey)
                    {
                        objSessionObject.TransactionStatusCode = item.TransactionStatusCode;
                        objSessionObject.SecurityToken = item.SecurityToken;
                        objSessionObject.SessionId = item.SessionId;
                        objSessionObject.SequenceNumber = Convert.ToInt16(item.SequenceNumber);
                    }
                }
                #endregion
            }
            return objSessionObject;
        }

    }
    public class AmadeusSessionTemplate
    {
        public string TransactionStatusCode { get; set; }
        public string SessionId { get; set; }
        public int SequenceNumber { get; set; }
        public string SecurityToken { get; set; }
    }
}
