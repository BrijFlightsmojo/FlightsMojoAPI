using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Markup
{
    public class MarkupTransaction
    {
        public List<Core.Markup.FlightMarkupNew> getFlightMarkupNew(int cabinType, int journeyType, string sourceMedia)
        {
            List<Core.Markup.FlightMarkupNew> markList = new List<Core.Markup.FlightMarkupNew>();
            try
            {

                SqlParameter[] prm = new SqlParameter[]
                {
                    new SqlParameter("@cabinType",cabinType),
                    new SqlParameter("@journeyType",journeyType),
                    new SqlParameter("@AffiliateId",sourceMedia)
                };

                DataSet ds = SqlHelper.ExecuteDataset(DataConnection.GetConnection(), CommandType.StoredProcedure, "usp_GetFlightMarkupNew", prm);
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Core.Markup.FlightMarkupNew objMarkup = new Core.Markup.FlightMarkupNew();
                        objMarkup.Airline = (string.IsNullOrEmpty(Convert.ToString(dr["Airline"])) == false ? Convert.ToString(dr["Airline"]).Split('-').ToList() : new List<string>());
                        objMarkup.AirlineNot = (string.IsNullOrEmpty(Convert.ToString(dr["AirlineNot"])) == false ? Convert.ToString(dr["AirlineNot"]).Split('-').ToList() : new List<string>());
                        objMarkup.FmFareType = new List<Core.MojoFareType>();
                        if (!string.IsNullOrEmpty(dr["FMFareType"].ToString()))
                        {
                            foreach (string str in Convert.ToString(dr["FMFareType"]).Trim().Split('-').ToList())
                            {
                                if (!string.IsNullOrEmpty(str) && str != "0")
                                    objMarkup.FmFareType.Add((Core.MojoFareType)Convert.ToInt32(str));
                            }
                        }

                        string amt = dr["Amount"].ToString();
                        if (amt.IndexOf("%") != -1)
                        {
                            objMarkup.Amount = Convert.ToDecimal(amt.Replace("%", ""));
                            objMarkup.AmountType = 2;
                        }
                        else
                        {
                            objMarkup.Amount = Convert.ToDecimal(amt);
                            objMarkup.AmountType = 1;
                        }
                        objMarkup.SubProvider = new List<Core.SubProvider>();
                        if (!string.IsNullOrEmpty(dr["SubProvider"].ToString()))
                        {
                            foreach (string str in Convert.ToString(dr["SubProvider"]).Trim().Split('-').ToList())
                            {
                                if (!string.IsNullOrEmpty(str) && str != "0")
                                    objMarkup.SubProvider.Add((Core.SubProvider)Convert.ToInt32(str));
                            }
                        }
                        objMarkup.RuleName = Convert.ToString(Convert.ToString(dr["RuleName"]));
                        markList.Add(objMarkup);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return markList;
        }
        public List<Core.Markup.FlightMarkupNew> getFlightMarkupWithSkyScanner(int cabinType, int journeyType, string sourceMedia, string Org,
            string Dest, DateTime TravelType,Core.Device device, ref List<Core.Markup.skyScannerMetaRankData> metaData)
        {
            List<Core.Markup.FlightMarkupNew> markList = new List<Core.Markup.FlightMarkupNew>();
            try
            {
                if (sourceMedia == "1015")
                {
                    SqlParameter[] prm1 = new SqlParameter[]
                    {
                        new SqlParameter("@org",Org),
                        new SqlParameter("@dest",Dest),
                        new SqlParameter("@depDate",TravelType)
                    };
                    DataSet ds1 = SqlHelper.ExecuteDataset(DataConnection.GetConnectionMetaRank(), CommandType.StoredProcedure, "get_MetaRankWithAirline", prm1);
                    if (ds1 != null && ds1.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds1.Tables[0].Rows)
                        {
                            Core.Markup.skyScannerMetaRankData objMetaRank = new Core.Markup.skyScannerMetaRankData();
                            objMetaRank.Airline = dr["Airline"].ToString();
                            objMetaRank.flightNo = dr["flightNo"].ToString();
                            objMetaRank.Amount = Convert.ToDecimal(dr["minFare"]);
                            metaData.Add(objMetaRank);
                        }
                    }
                }
                SqlParameter[] prm = new SqlParameter[]
               {
                    new SqlParameter("@cabinType",cabinType),
                    new SqlParameter("@journeyType",journeyType),
                    new SqlParameter("@AffiliateId",sourceMedia),
                    new SqlParameter("@device",(int)device)
               };

                DataSet ds = SqlHelper.ExecuteDataset(DataConnection.GetConnection(), CommandType.StoredProcedure, "usp_GetFlightMarkupNew_V1", prm);
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Core.Markup.FlightMarkupNew objMarkup = new Core.Markup.FlightMarkupNew();
                        objMarkup.Airline = (string.IsNullOrEmpty(Convert.ToString(dr["Airline"])) == false ? Convert.ToString(dr["Airline"]).Split('-').ToList() : new List<string>());
                        objMarkup.AirlineNot = (string.IsNullOrEmpty(Convert.ToString(dr["AirlineNot"])) == false ? Convert.ToString(dr["AirlineNot"]).Split('-').ToList() : new List<string>());
                        objMarkup.FmFareType = new List<Core.MojoFareType>();
                        if (!string.IsNullOrEmpty(dr["FMFareType"].ToString()))
                        {
                            foreach (string str in Convert.ToString(dr["FMFareType"]).Trim().Split('-').ToList())
                            {
                                if (!string.IsNullOrEmpty(str) && str != "0")
                                    objMarkup.FmFareType.Add((Core.MojoFareType)Convert.ToInt32(str));
                            }
                        }

                        string amt = dr["Amount"].ToString();
                        if (amt.IndexOf("%") != -1)
                        {
                            objMarkup.Amount = Convert.ToDecimal(amt.Replace("%", ""));
                            objMarkup.AmountType = 2;
                        }
                        else
                        {
                            objMarkup.Amount = Convert.ToDecimal(amt);
                            objMarkup.AmountType = 1;
                        }
                        objMarkup.SubProvider = new List<Core.SubProvider>();
                        if (!string.IsNullOrEmpty(dr["SubProvider"].ToString()))
                        {
                            foreach (string str in Convert.ToString(dr["SubProvider"]).Trim().Split('-').ToList())
                            {
                                if (!string.IsNullOrEmpty(str) && str != "0")
                                    objMarkup.SubProvider.Add((Core.SubProvider)Convert.ToInt32(str));
                            }
                        }
                        objMarkup.RuleName = Convert.ToString(Convert.ToString(dr["RuleName"]));
                        markList.Add(objMarkup);
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return markList;
        }
        public List<Core.Markup.FlightMarkup> getFlightMarkup(DateTime departureDate, int cabinType, int siteId, string countryFrom,
           string countryTo, string airportFrom, string airportTo, int tripType, int journeyType, string sourceMedia, int totalPax,
           int DayDiffrence)
        {
            List<Core.Markup.FlightMarkup> arpList = new List<Core.Markup.FlightMarkup>();
            try
            {
                SqlParameter[] prm = new SqlParameter[]
                {
                    new SqlParameter("@travelDate",departureDate),
                    new SqlParameter("@cabinType",cabinType),
                    new SqlParameter("@siteId",siteId),
                    new SqlParameter("@countryFrom",countryFrom),
                    new SqlParameter("@countryTo",countryTo),
                    new SqlParameter("@airportFrom",airportFrom),
                    new SqlParameter("@airportTo",airportTo),
                    new SqlParameter("@tripType",tripType),
                    new SqlParameter("@journeyType",journeyType),
                    new SqlParameter("@AffiliateId",sourceMedia),
                    new SqlParameter("@totPax",totalPax),
                    new SqlParameter("@DayDiffrence",DayDiffrence)
                };

                DataSet ds = SqlHelper.ExecuteDataset(DataConnection.GetConnection(), CommandType.StoredProcedure, "usp_GetFlightMarkup", prm);
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Core.Markup.FlightMarkup objMarkup = new Core.Markup.FlightMarkup();
                        objMarkup.Id = Convert.ToInt32(Convert.ToString(dr["Id"]));
                        objMarkup.Airline = (string.IsNullOrEmpty(Convert.ToString(dr["Airline"])) == false ? Convert.ToString(dr["Airline"]).Split('-').ToList() : new List<string>());
                        objMarkup.airlineMatchType = (Core.AirlineMatchType)Convert.ToInt32(dr["AirlineType"]);
                        objMarkup.AirlineClass = (string.IsNullOrEmpty(Convert.ToString(dr["AirlineClass"])) == false ? Convert.ToString(dr["AirlineClass"]).Split('-').ToList() : new List<string>());
                        objMarkup.airlineClassMatchType = (Core.AirlineClassMatchType)Convert.ToInt32(dr["AirlineClassType"]);

                        objMarkup.Stops = new List<int>();
                        if (!string.IsNullOrEmpty(dr["Stops"].ToString()))
                        {
                            foreach (string str in Convert.ToString(dr["Stops"]).Trim().Split('-').ToList())
                            {
                                if (!string.IsNullOrEmpty(str) && str != "0")
                                    objMarkup.Stops.Add(Convert.ToInt32(str));
                            }
                        }

                        objMarkup.Amount = Convert.ToDecimal(Convert.ToString(dr["Amount"]));
                        objMarkup.AmountType = Convert.ToInt16(Convert.ToString(dr["AmountType"]));

                        objMarkup.CalculateOn = Convert.ToInt16(Convert.ToString(dr["CalculateOn"]));
                        objMarkup.RuleType = Convert.ToInt16(Convert.ToString(dr["RuleType"]));
                        objMarkup.RuleName = Convert.ToString(Convert.ToString(dr["RuleName"]));
                        objMarkup.checkOperatedBy = string.IsNullOrEmpty(dr["CheckOperatedBy"].ToString()) ? Core.CheckOperatedBy.None : (Core.CheckOperatedBy)Convert.ToInt32(dr["CheckOperatedBy"]);
                        arpList.Add(objMarkup);
                    }
                }
            }
            catch //(Exception ex)
            {

            }
            return arpList;
        }
    }
}
