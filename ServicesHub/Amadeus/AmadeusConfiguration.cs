using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Amadeus
{
    public class AmadeusConfiguration
    {
        private static string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static Random random = new Random();
        public static string targetURL { get; set; }
        public string accountName { get; set; }
        public string password { get; set; }
        public string pseudoCityCode { get; set; }
        public string wSAP { get; set; }
        public string timeStamp { get; set; }
        public string nonce { get; set; }
        public Int16 numberOfRecommendation { get; set; }
        private string environment { get; set; }
        public AmadeusConfiguration()
        {
            environment = ConfigurationManager.AppSettings["Mode"].ToString().ToUpper();
            accountName = ConfigurationManager.AppSettings["AccountName"].ToString();

            pseudoCityCode = ConfigurationManager.AppSettings["PseudoCityCode"].ToString();
            wSAP = ConfigurationManager.AppSettings["WSAP"].ToString();
            if (environment == "LIVE")
            {
                targetURL = ConfigurationManager.AppSettings["LiveTargetURL"].ToString();
            }
            else
            {
                targetURL = ConfigurationManager.AppSettings["TestTargetURL"].ToString();
            }
            nonce = calculateNonce();
            timeStamp = DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");
            password = getpwd(timeStamp, nonce);
            numberOfRecommendation = Int16.Parse(ConfigurationManager.AppSettings["amadeusRecommendation"].ToString());
        }
        private string calculateNonce()
        {
            var nonceString = new StringBuilder();
            for (int i = 0; i < 12; i++)
            {
                nonceString.Append(validChars[random.Next(0, validChars.Length - 1)]);
            }
            byte[] encbuff = Encoding.UTF8.GetBytes(nonceString.ToString());
            return Convert.ToBase64String(encbuff);
        }
        private string getpwd(string _created, string _nonce)
        {
            string _password = ConfigurationManager.AppSettings["Password"].ToString();
            SHA1Managed shaPwd1 = new SHA1Managed();
            byte[] pwd = shaPwd1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(_password));
            byte[] nonceBytes = Convert.FromBase64String(_nonce);
            byte[] createdBytes = System.Text.Encoding.UTF8.GetBytes(_created);
            byte[] operand = new byte[nonceBytes.Length + createdBytes.Length + pwd.Length];
            Array.Copy(nonceBytes, operand, nonceBytes.Length);
            Array.Copy(createdBytes, 0, operand, nonceBytes.Length, createdBytes.Length);
            Array.Copy(pwd, 0, operand, nonceBytes.Length + createdBytes.Length, pwd.Length);
            SHA1Managed sha1 = new SHA1Managed();
            string trueDigest = Convert.ToBase64String(sha1.ComputeHash(operand));
            SHA1Managed sh = new SHA1Managed();
            return trueDigest;
        }
        public static string GetAmadeusSoapAction(AmadeusSoapActionType actionType)
        {
            string soapAction = string.Empty;
            switch (actionType)
            {
                case AmadeusSoapActionType.Security_Authenticate:
                    soapAction = "VLSSLQ_06_1_1A";
                    break;
                case AmadeusSoapActionType.Fare_MasterPricerTravelBoardSearch:
                    soapAction = "FMPTBQ_17_4_1A";
                    break;
                case AmadeusSoapActionType.Fare_MasterPricerCalendar:
                    soapAction = "FMPCAQ_16_3_1A";
                    break;
                case AmadeusSoapActionType.Air_SellFromRecommendation:
                    soapAction = "ITAREQ_05_2_IA";
                    break;
                case AmadeusSoapActionType.PNR_AddMultiElements:
                    soapAction = "PNRADD_17_1_1A";
                    break;
                case AmadeusSoapActionType.Fare_PricePNRWithBookingClass:
                    soapAction = "TPCBRQ_18_1_1A";
                    break;
                case AmadeusSoapActionType.Ticket_CreateTSTFromPricing:
                    soapAction = "TAUTCQ_04_1_1A";
                    break;
                case AmadeusSoapActionType.Queue_PlacePNR:
                    soapAction = "QUQPCQ_03_1_1A";
                    break;
                case AmadeusSoapActionType.PNR_Retrieve:
                    soapAction = "PNRRET_17_1_1A";
                    break;
                case AmadeusSoapActionType.PNR_Cancel:
                    soapAction = "PNRXCL_17_1_1A";
                    break;
                case AmadeusSoapActionType.PNR_Ignore:
                    soapAction = "CLTREQ_04_1_IA";
                    break;
                case AmadeusSoapActionType.Command_Cryptic:
                    soapAction = "HSFREQ_07_3_1A";
                    break;
                case AmadeusSoapActionType.Air_RetrieveSeatMap:
                    soapAction = "SMPREQ_14_2_1A";
                    break;
                case AmadeusSoapActionType.Security_SignOut:
                    soapAction = "VLSSOQ_04_1_1A";
                    break;
                case AmadeusSoapActionType.Queue_RemoveItem:
                    soapAction = "QUQMDQ_03_1_1A";
                    break;
                case AmadeusSoapActionType.PNR_Split:
                    soapAction = "PNRSPL_11_3_1A";
                    break;
                case AmadeusSoapActionType.Air_FlightInfo:
                    soapAction = "FLIREQ_07_1_1A";
                    break;
                case AmadeusSoapActionType.Ticket_DisplayTST:
                    soapAction = "TTSTRQ_07_1_1A";
                    break;
                case AmadeusSoapActionType.DocIssuance_IssueTicket:
                    soapAction = "TTKTIQ_15_1_1A";
                    break;
                case AmadeusSoapActionType.Ticket_CreditCardCheck:
                    soapAction = "CCVRQT_06_1_1A";
                    break;
                case AmadeusSoapActionType.Fare_InformativePricingWithoutPNR:
                    soapAction = "TIPNRQ_18_1_1A";
                    break;
                case AmadeusSoapActionType.MiniRule_GetFromPricing:
                    soapAction = "TMRCRQ_11_1_1A";
                    break;
                case AmadeusSoapActionType.FOP_CreateFormOfPayment:
                    soapAction = "TFOPCQ_15_4_1A";
                    break;
                case AmadeusSoapActionType.Doc_DisplayItinerary:
                    soapAction = "ITIRQT_01_2_1A";
                    break;
              
                case AmadeusSoapActionType.Fare_PriceUpsellWithoutPNR:
                    soapAction = "TIUNRQ_16_1_1A";
                    break;
              
               
                case AmadeusSoapActionType.Fare_InformativeBestPricingWithoutPNR:
                    soapAction = "TIBNRQ_18_1_1A";
                    break;
                case AmadeusSoapActionType.Ticket_ProcessEDoc:
                    soapAction = "TATREQ_15_2_1A";
                    break;
                case AmadeusSoapActionType.Ticket_CancelDocument:
                    soapAction = "TRCANQ_11_1_1A";
                    break;
              
                case AmadeusSoapActionType.Fare_CheckRules:
                    soapAction = "FARQNQ_07_1_1A";
                    break;
                case AmadeusSoapActionType.Fare_QuoteItinerary:
                    soapAction = "FITQPQ_12_2_1A";
                    break;
            }
            return soapAction;
        }
        //public static string GetAmadeusSoapAction_Old(AmadeusSoapActionType actionType)
        //{
        //    string soapAction = string.Empty;
        //    switch (actionType)
        //    {
        //        case AmadeusSoapActionType.Security_Authenticate:
        //            soapAction = "VLSSLQ_06_1_1A";
        //            break;
        //        case AmadeusSoapActionType.Fare_MasterPricerTravelBoardSearch:
        //            soapAction = "FMPTBQ_17_3_1A";
        //            break;
        //        case AmadeusSoapActionType.Fare_MasterPricerCalendar:
        //            soapAction = "FMPCAQ_14_3_1A";
        //            break;
        //        case AmadeusSoapActionType.Air_SellFromRecommendation:
        //            soapAction = "ITAREQ_05_2_IA";
        //            break;
        //        case AmadeusSoapActionType.PNR_AddMultiElements:
        //            soapAction = "PNRADD_17_1_1A";
        //            break;
        //        case AmadeusSoapActionType.Fare_PricePNRWithBookingClass:
        //            soapAction = "TPCBRQ_18_1_1A";
        //            break;
        //        case AmadeusSoapActionType.Ticket_CreateTSTFromPricing:
        //            soapAction = "TAUTCQ_04_1_1A";
        //            break;
        //        case AmadeusSoapActionType.Queue_PlacePNR:
        //            soapAction = "QUQPCQ_03_1_1A";
        //            break;
        //        case AmadeusSoapActionType.PNR_Retrieve:
        //            soapAction = "PNRRET_17_1_1A";
        //            break;
        //        case AmadeusSoapActionType.PNR_Cancel:
        //            soapAction = "PNRXCL_16_1_1A";
        //            break;
        //        case AmadeusSoapActionType.PNR_Ignore:
        //            soapAction = "CLTREQ_04_1_IA";
        //            break;
        //        case AmadeusSoapActionType.Command_Cryptic:
        //            soapAction = "HSFREQ_07_3_1A";
        //            break;
        //        case AmadeusSoapActionType.Air_RetrieveSeatMap:
        //            soapAction = "SMPREQ_14_2_1A";
        //            break;
        //        case AmadeusSoapActionType.Security_SignOut:
        //            soapAction = "VLSSOQ_04_1_1A";
        //            break;
        //        case AmadeusSoapActionType.Queue_RemoveItem:
        //            soapAction = "QUQMDQ_03_1_1A";
        //            break;
        //        case AmadeusSoapActionType.PNR_Split:
        //            soapAction = "PNRSPL_11_3_1A";
        //            break;
        //        case AmadeusSoapActionType.Air_FlightInfo:
        //            soapAction = "FLIREQ_07_1_1A";
        //            break;
        //        case AmadeusSoapActionType.Ticket_DisplayTST:
        //            soapAction = "TTSTRQ_15_1_1A";
        //            break;
        //        case AmadeusSoapActionType.DocIssuance_IssueTicket:
        //            soapAction = "TTKTIQ_15_1_1A";
        //            break;
        //        case AmadeusSoapActionType.Ticket_CreditCardCheck:
        //            soapAction = "CCVRQT_06_1_1A";
        //            break;
        //        case AmadeusSoapActionType.Fare_InformativePricingWithoutPNR:
        //            soapAction = "TIPNRQ_18_1_1A";
        //            break;
        //        case AmadeusSoapActionType.MiniRule_GetFromPricing:
        //            soapAction = "TMRCRQ_11_1_1A";
        //            break;
        //        case AmadeusSoapActionType.FOP_CreateFormOfPayment:
        //            soapAction = "TFOPCQ_15_4_1A";
        //            break;
        //        case AmadeusSoapActionType.Doc_DisplayItinerary:
        //            soapAction = "ITIRQT_01_2_1A";
        //            break;
        //        case AmadeusSoapActionType.Fare_PriceUpsellWithoutPNR:
        //            soapAction = "TIUNRQ_16_1_1A";
        //            break;
        //        case AmadeusSoapActionType.Ticket_ProcessEDoc:
        //            soapAction = "TATREQ_15_2_1A";
        //            break;
        //        case AmadeusSoapActionType.Ticket_CancelDocument:
        //            soapAction = "TRCANQ_11_1_1A";
        //            break;
        //        case AmadeusSoapActionType.Fare_GetFareFamilyDescription:
        //            soapAction = "TFQFRQ_15_1_1A";
        //            break;
        //        case AmadeusSoapActionType.Fare_InformativeBestPricingWithoutPNR:
        //            soapAction = "TIBNRQ_18_1_1A";
        //            break;
        //        case AmadeusSoapActionType.Air_SellFromAvailability:
        //            soapAction = "ITAREQ_05_1_IA";
        //            break;
        //        case AmadeusSoapActionType.Fare_CheckRules:
        //            soapAction = "FARQNQ_07_1_1A";
        //            break;
        //    }
        //    return soapAction;
        //}

    }
    public enum AmadeusSoapActionType : int
    {
        Security_Authenticate = 1,
        Fare_MasterPricerTravelBoardSearch = 2,
        Fare_MasterPricerCalendar = 3,
        Air_SellFromRecommendation = 4,
        PNR_AddMultiElements = 5,
        Fare_PricePNRWithBookingClass = 6,
        Ticket_CreateTSTFromPricing = 7,
        Queue_PlacePNR = 8,
        PNR_Retrieve = 9,
        PNR_Cancel = 10,
        PNR_Ignore = 11,
        Command_Cryptic = 12,
        Air_RetrieveSeatMap = 13,
        Security_SignOut = 14,
        Queue_RemoveItem = 15,
        PNR_Split = 16,
        Air_FlightInfo = 17,
        Ticket_DisplayTST = 18,
        DocIssuance_IssueTicket = 19,
        Ticket_CreditCardCheck = 20,
        Fare_InformativePricingWithoutPNR = 21,
        MiniRule_GetFromPricing = 22,
        FOP_CreateFormOfPayment = 23,
        Doc_DisplayItinerary = 24,
        Fare_PriceUpsellWithoutPNR = 25,
        Ticket_ProcessEDoc = 26,
        Ticket_CancelDocument = 27,
        Fare_GetFareFamilyDescription = 28,
        Fare_InformativeBestPricingWithoutPNR = 29,
        Air_SellFromAvailability = 30,
        Fare_CheckRules = 31,
        Fare_QuoteItinerary = 32
    }
}
