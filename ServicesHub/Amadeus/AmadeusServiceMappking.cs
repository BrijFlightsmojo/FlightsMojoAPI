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

        //public FareQuoteResponse Fare_QuoteItinerary(FareQuoteRequest request)
        //{
        //    StringBuilder sbLogger = new StringBuilder();
        //    string controlNumber = string.Empty;
        //    AmadeusRequestMapping objAmadeusRequestMapping = new AmadeusRequestMapping();

        //    string Fare_QuoteItineraryRequest = objAmadeusRequestMapping.Fare_QuoteItinerary(request);
        //    string response = GetResponseStr(Fare_QuoteItineraryRequest, AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Fare_QuoteItinerary),ref sbLogger);
        //    FareQuoteResponse kk = new AmadeusResponseMapping_XML().FareQuote(response, request);
        //    return kk;
        //}
        //public FareQuoteResponse Fare_QuoteItinerary(FareQuoteRequest request)
        //{
        //    StringBuilder sbLogger = new StringBuilder();
        //    try
        //    {
        //        bookingLog(ref sbLogger, "Original Request", JsonConvert.SerializeObject(request));

        //        string controlNumber = string.Empty;
        //        AmadeusRequestMapping objDanRequestMapping = new AmadeusRequestMapping();

        //        string Fare_QuoteItineraryRequest = objDanRequestMapping.Fare_QuoteItinerary(request);
        //        string response = GetResponseStr(Fare_QuoteItineraryRequest, AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Fare_QuoteItinerary), ref sbLogger);
        //        FareQuoteResponse kk = new AmadeusResponseMapping_XML().FareQuote(response, request);
        //        return kk;
        //    }
        //    catch (Exception ex)
        //    {
        //        bookingLog(ref sbLogger, "Exeption", ex.ToString());
        //        new LogWriter(sbLogger.ToString(), "FareQuote" + DateTime.Today.ToString("ddMMyyy"), "Exeption");
        //        return new FareQuoteResponse() { responsStatus = new ResponseStatus() { status = TransactionStatus.Error } };
        //    }

        //}
     

     
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
