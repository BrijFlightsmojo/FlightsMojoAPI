using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
namespace IndiaAPI
{
    public class GlobalData
    {
        public static string LogPrefix { get; set; }
        public static string ServerID { get; set; }
        public static string ServerIP { get; set; }
        public static bool isB2B { get; set; }
        public static bool isMakeBooking { get; set; }
        public static bool isUseCaching { get; set; }
        static GlobalData()
        {
            LogPrefix = ConfigurationManager.AppSettings["LogPrefix"] != null ? ConfigurationManager.AppSettings["LogPrefix"] : "Live";
            ServerID = ConfigurationManager.AppSettings["ServerID"] != null ? ConfigurationManager.AppSettings["ServerID"] : "None";
            isB2B = ConfigurationManager.AppSettings["isB2B"] != null ? Convert.ToBoolean(ConfigurationManager.AppSettings["isB2B"]) : false;
            isMakeBooking = ConfigurationManager.AppSettings["isMakeBooking"] != null ? Convert.ToBoolean(ConfigurationManager.AppSettings["isMakeBooking"]) : false;
            ServerIP = ConfigurationManager.AppSettings["ServerIP"] != null ? ConfigurationManager.AppSettings["ServerIP"] : "54.214.158.214";
            isUseCaching = ConfigurationManager.AppSettings["isUseCaching"] != null ? Convert.ToBoolean(ConfigurationManager.AppSettings["isUseCaching"]) : false;
        }
    }
}