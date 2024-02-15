using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using System.Data;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using ServicesHub.Tbo.TboClass;

namespace ServicesHub.Tbo
{
    public class TboAuthentication
    {
        static TboAuthentication()
        {
            if (ConfigurationManager.AppSettings["TvoApiMode"].ToString().Equals("live", StringComparison.OrdinalIgnoreCase))
            {
                    apiUrlAuth = ConfigurationManager.AppSettings["apiUrlAuth"].ToString();
                    apiUrlLogout = ConfigurationManager.AppSettings["apiUrlLogout"].ToString();
                    AgencyBalanceUrl = ConfigurationManager.AppSettings["AgencyBalanceUrl"].ToString();
                    airSearchUrl = ConfigurationManager.AppSettings["airSearchUrl"].ToString();
                    fareRuleUrl = ConfigurationManager.AppSettings["fareRuleUrl"].ToString();
                    FareQuoteUrl = ConfigurationManager.AppSettings["FareQuoteUrl"].ToString();
                    priceRBDUrl = ConfigurationManager.AppSettings["priceRBDUrl"].ToString();
                    SsrUrl = ConfigurationManager.AppSettings["SsrUrl"].ToString();
                    bookUrl = ConfigurationManager.AppSettings["bookUrl"].ToString();
                    ticketUrl = ConfigurationManager.AppSettings["ticketUrl"].ToString();
                    cancelUrl = ConfigurationManager.AppSettings["cancelUrl"].ToString();
                    sendChangeUrl = ConfigurationManager.AppSettings["sendChangeUrl"].ToString();
                    statusChangeUrl = ConfigurationManager.AppSettings["statusChangeUrl"].ToString();
                    CalendarFareUrl = ConfigurationManager.AppSettings["CalendarFareUrl"].ToString();
                    CalendarFareUpdate = ConfigurationManager.AppSettings["CalendarFareUpdate"].ToString();
                    BookingDetailsUrl = ConfigurationManager.AppSettings["BookingDetailsUrl"].ToString();
            }
            else
            {
                    apiUrlAuth = ConfigurationManager.AppSettings["apiUrlAuthTest"].ToString();
                    apiUrlLogout = ConfigurationManager.AppSettings["apiUrlLogoutTest"].ToString();
                    AgencyBalanceUrl = ConfigurationManager.AppSettings["AgencyBalanceUrlTest"].ToString();
                    airSearchUrl = ConfigurationManager.AppSettings["airSearchUrlTest"].ToString();
                    fareRuleUrl = ConfigurationManager.AppSettings["fareRuleUrlTest"].ToString();
                    FareQuoteUrl = ConfigurationManager.AppSettings["FareQuoteUrlTest"].ToString();
                    priceRBDUrl = ConfigurationManager.AppSettings["priceRBDUrlTest"].ToString();
                    SsrUrl = ConfigurationManager.AppSettings["SsrUrlTest"].ToString();
                    bookUrl = ConfigurationManager.AppSettings["bookUrlTest"].ToString();
                    ticketUrl = ConfigurationManager.AppSettings["ticketUrlTest"].ToString();
                    cancelUrl = ConfigurationManager.AppSettings["cancelUrlTest"].ToString();
                    sendChangeUrl = ConfigurationManager.AppSettings["sendChangeUrlTest"].ToString();
                    statusChangeUrl = ConfigurationManager.AppSettings["statusChangeUrlTest"].ToString();
                    CalendarFareUrl = ConfigurationManager.AppSettings["CalendarFareUrlTest"].ToString();
                    CalendarFareUpdate = ConfigurationManager.AppSettings["CalendarFareUpdateTest"].ToString();
                    BookingDetailsUrl = ConfigurationManager.AppSettings["BookingDetailsUrlTest"].ToString();
            }
        }
        public TboAuthentication()
        {

        }

        public static string apiUrlAuth { get; set; }
        public static string apiUrlLogout { get; set; }
        public static string AgencyBalanceUrl { get; set; }
        public static string airSearchUrl { get; set; }
        public static string fareRuleUrl { get; set; }
        public static string FareQuoteUrl { get; set; }
        public static string priceRBDUrl { get; set; }
        public static string SsrUrl { get; set; }
        public static string bookUrl { get; set; }
        public static string ticketUrl { get; set; }
        public static string cancelUrl { get; set; }
        public static string sendChangeUrl { get; set; }
        public static string statusChangeUrl { get; set; }
        public static string CalendarFareUrl { get; set; }
        public static string CalendarFareUpdate { get; set; }
        public static string BookingDetailsUrl { get; set; }
        public string getTokenID()
        {
            TboToken token = null;
            if (System.Web.HttpRuntime.Cache["TokenID"] != null)
            {
                token = (TboToken)System.Web.HttpRuntime.Cache["TokenID"];
                if (token.creationDateTime.AddHours(1) < DateTime.Now && token != null)
                {
                    Authenticate();
                    token = getToken();
                    System.Web.HttpRuntime.Cache["TokenID"] = token;
                }
            }
            else
            {
                token = getToken();
                if (token == null)
                {
                    Authenticate();
                    token = getToken();
                }
                else if (token.creationDateTime.AddHours(1) < DateTime.Now)
                {
                    Authenticate();
                    token = getToken();
                    System.Web.HttpRuntime.Cache["TokenID"] = token;
                }
                System.Web.HttpRuntime.Cache["TokenID"] = token;
            }
            return token.tokenID;
        }
        public TboToken getToken()
        {
            TboToken token = null;

            DataSet ds = new DAL.Tbo.Tbo_DataSetGet().getToken();
            if (ds != null)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (token == null)
                    {
                        token = new TboToken()
                        {
                            id = Convert.ToInt32(dr["id"]),
                            tokenID = dr["tokenID"].ToString(),
                            status = dr["status"].ToString(),
                            creationDateTime = Convert.ToDateTime(dr["creationDateTime"]),
                            MemberId = Convert.ToInt32(dr["MemberId"]),
                            AgencyId = Convert.ToInt32(dr["AgencyId"])
                        };
                    }
                    else
                    {
                        LogoutToken(dr["tokenID"].ToString(), Convert.ToInt32(dr["MemberId"]), Convert.ToInt32(dr["AgencyId"]), Convert.ToInt32(dr["id"]));
                    }

                }
            }
            return token;
        }
        private void Authenticate()
        {
            StringBuilder sbLogger = new StringBuilder();
            string Url = apiUrlAuth;
            bookingLog(ref sbLogger, "Authentication Url", Url);
            LoginRequest loginRequest = new LoginRequest();
            if (ConfigurationManager.AppSettings["TvoApiMode"].ToString().Equals("live", StringComparison.OrdinalIgnoreCase))
            {
                //loginRequest.UserName = "DELF245";
                //loginRequest.Password = "Flight@Live#245";
                //loginRequest.ClientId = "tboprod";

                loginRequest.UserName = "DELF792";
                loginRequest.Password = "@nApi#Live-0190";
                loginRequest.ClientId = "tboprod";
            }
            else
            {
                loginRequest.UserName = "mojo";
                loginRequest.Password = "mojo@1234";
                loginRequest.ClientId = "ApiIntegrationNew";
            }
            loginRequest.EndUserIp = "103.160.243.202";
           
            var response = GetResponse(Url, JsonConvert.SerializeObject(loginRequest));
            bookingLog(ref sbLogger, "Authentication Request", JsonConvert.SerializeObject(loginRequest));
            bookingLog(ref sbLogger, "Authentication Response", response);
            new LogWriter(sbLogger.ToString(), "Tbo_Authentication", "Tbo");
            LoginResponse loginRes = JsonConvert.DeserializeObject<LoginResponse>(response.ToString());
            if (loginRes!=null && loginRes.Status == 1)
            {
                new DAL.Tbo.Tbo_DataSetGet().setToken(loginRes.TokenId, loginRes.Member.MemberId, loginRes.Member.AgencyId);
            }
        }
        private void LogoutToken(string tokenID, int MemberId, int AgencyId, int id)
        {
            string Url = apiUrlLogout;
            LogOutRequest logoutReq = new LogOutRequest();
            logoutReq.TokenMemberId = MemberId;
            logoutReq.TokenAgencyId = AgencyId;
            logoutReq.EndUserIp = "103.160.243.202";
            logoutReq.ClientId = "ApiIntegration";
            logoutReq.TokenId = tokenID;
            var response = GetResponse(Url, JsonConvert.SerializeObject(logoutReq));
            LogOutResponse logoutRes = JsonConvert.DeserializeObject<LogOutResponse>(response.ToString());
            if (true || logoutRes.Status == 1)
            {
                new DAL.Tbo.Tbo_DataSetGet().setLogout(id);

            }
        }
        public void bookingLog(ref StringBuilder sbLogger, string requestTitle, string logText)
        {
            sbLogger.Append(Environment.NewLine + "---------------------------------------------" + requestTitle + "" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
            sbLogger.Append(Environment.NewLine + logText);
            sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
        }
        private string GetResponse(string url, string requestData)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls| SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            string response = string.Empty;
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                //request.Headers.Add("Accept-Encoding", "gzip");
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                WebResponse webResponse = request.GetResponse();
                var rsp = webResponse.GetResponseStream();
                //if (rsp == null)
                //{
                //    //throw exception
                //}
                //using (StreamReader readStream = new StreamReader(new GZipStream(rsp, CompressionMode.Decompress)))
                //{
                //    response = readStream.ReadToEnd();
                //}
                using (StreamReader reader = new StreamReader(rsp))
                {
                    // Read the content.
                    response = reader.ReadToEnd();
                }
                return response;
            }
            catch (WebException webEx)
            {
                //WebResponse wresponse = webEx.Response;
                //Stream stream = wresponse.GetResponseStream();
                //String responseMessage = new StreamReader(stream).ReadToEnd();
                //return responseMessage;
                return "";
            }
        }
    }
}
