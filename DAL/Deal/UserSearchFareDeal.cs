using Core.Flight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
namespace DAL.Deal
{
    public class UserSearchFareDeal
    {
        public static SqlDataReader Get(Int64 id)
        {
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@id", SqlDbType.BigInt);
            param[0].Value = id;

            using (SqlConnection con = DataConnection.GetConnection())
            {
                return SqlHelper.ExecuteReader(con, CommandType.StoredProcedure, "usp_UserSearchFareDealSelect", param);
            }
        }
        public async System.Threading.Tasks.Task SaveUserSearchFareDeal(FlightSearchRequest fsr, List<List<FlightResult>> result)
        {
            if (result.Count == 1)
            {
                string Origin = fsr.segment[0].originAirport, destination = fsr.segment[0].destinationAirport;
                StringBuilder strCarrier = new StringBuilder();
                using (SqlConnection conn = DataConnection.GetConnection())
                {
                    try
                    {
                        conn.Open();
                        foreach (var item in result[0])
                        {
                            if (strCarrier.ToString().IndexOf(item.FlightSegments[0].Segments[0].Airline, StringComparison.OrdinalIgnoreCase) == -1)
                            {
                                strCarrier.Append(item.FlightSegments[0].Segments[0].Airline + ",");
                                using (SqlCommand cmd = new SqlCommand("usp_UserSearchFareDealInsert", conn))
                                {
                                    if (fsr.client != Core.ClientType.CRM)
                                    {
                                        Origin = item.FlightSegments[0].Segments[0].Origin;
                                        destination = item.FlightSegments[0].Segments[item.FlightSegments[0].Segments.Count - 1].Destination;
                                    }
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@siteID", (int)fsr.siteId);
                                    cmd.Parameters.AddWithValue("@sourchMedia", string.IsNullOrEmpty(fsr.sourceMedia) ? "BulkSearch" : fsr.sourceMedia);
                                    cmd.Parameters.AddWithValue("@origin", Origin.ToUpper());
                                    cmd.Parameters.AddWithValue("@destination", destination.ToUpper());
                                    cmd.Parameters.AddWithValue("@depDate", item.FlightSegments[0].Segments[0].DepTime);
                                    cmd.Parameters.AddWithValue("@retDate", (item.FlightSegments.Count >= 2 ? item.FlightSegments.LastOrDefault().Segments.FirstOrDefault().DepTime : item.FlightSegments[0].Segments[0].DepTime));
                                    cmd.Parameters.AddWithValue("@airline", item.FlightSegments[0].Segments[0].Airline);
                                    cmd.Parameters.AddWithValue("@tripType", (Int16)fsr.tripType);
                                    cmd.Parameters.AddWithValue("@cabinClass", (Int16)fsr.cabinType);
                                    cmd.Parameters.AddWithValue("@baseFare", item.Fare.fareBreakdown[0].BaseFare);
                                    cmd.Parameters.AddWithValue("@tax", item.Fare.fareBreakdown[0].Tax);
                                    cmd.Parameters.AddWithValue("@markup", item.Fare.fareBreakdown[0].Markup);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DalLog.LogCreater.CreateLogFile(ex.ToString(), "Log\\TripJack\\Error", "SaveUserSearchFareDeal1" + ".txt");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            else
            {
                using (SqlConnection conn = DataConnection.GetConnection())
                {
                    try
                    {
                        conn.Open();
                        int ctr = 0;
                        StringBuilder strCarrier = new StringBuilder();
                        for (int i = 0; i < result[0].Count && ctr < 300; i++)
                        {
                            for (int j = 0; j < result[1].Count && ctr < 300; j++)
                            {
                                TimeSpan ts = result[1][j].FlightSegments[0].Segments[0].DepTime - result[0][i].FlightSegments[0].Segments.Last().ArrTime;
                                if (ts.TotalMinutes > 240)
                                {
                                    if (strCarrier.ToString().IndexOf(result[0][i].FlightSegments[0].Segments[0].Airline, StringComparison.OrdinalIgnoreCase) == -1)
                                    {
                                        strCarrier.Append(result[0][i].FlightSegments[0].Segments[0].Airline + ",");
                                        using (SqlCommand cmd = new SqlCommand("usp_UserSearchFareDealInsert", conn))
                                        {
                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@siteID", (int)fsr.siteId);
                                            cmd.Parameters.AddWithValue("@sourchMedia", string.IsNullOrEmpty(fsr.sourceMedia) ? "BulkSearch" : fsr.sourceMedia);
                                            cmd.Parameters.AddWithValue("@origin", fsr.segment[0].originAirport.ToUpper());
                                            cmd.Parameters.AddWithValue("@destination", fsr.segment[0].destinationAirport.ToUpper());
                                            cmd.Parameters.AddWithValue("@depDate", result[0][i].FlightSegments[0].Segments[0].DepTime);
                                            cmd.Parameters.AddWithValue("@retDate", result[1][j].FlightSegments[0].Segments[0].DepTime);
                                            cmd.Parameters.AddWithValue("@airline", result[0][i].FlightSegments[0].Segments[0].Airline);
                                            cmd.Parameters.AddWithValue("@tripType", (Int16)fsr.tripType);
                                            cmd.Parameters.AddWithValue("@cabinClass", (Int16)fsr.cabinType);
                                            cmd.Parameters.AddWithValue("@baseFare", (result[0][i].Fare.PublishedFare + result[1][j].Fare.PublishedFare));
                                            cmd.Parameters.AddWithValue("@tax", 0);
                                            cmd.Parameters.AddWithValue("@markup", 0);

                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                                ctr++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        DalLog.LogCreater.CreateLogFile(ex.ToString(), "Log\\TripJack\\Error", "SaveUserSearchFareDeal2" + ".txt");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        public static int UpdateUserSearchFareDeal(Int64 id, int siteID, string sourchMedia, string origin, string destination, string airline, string tripType, string cabinClass, decimal baseFare, decimal tax, decimal markup, DateTime searchDateTime)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[12];
                param[0] = new SqlParameter("@id", SqlDbType.BigInt);
                param[0].Value = id;
                param[1] = new SqlParameter("@siteID", SqlDbType.Int);
                param[1].Value = siteID;
                param[2] = new SqlParameter("@sourchMedia", SqlDbType.VarChar, 50);
                param[2].Value = sourchMedia;
                param[3] = new SqlParameter("@origin", SqlDbType.VarChar, 3);
                param[3].Value = origin;
                param[4] = new SqlParameter("@destination", SqlDbType.VarChar, 3);
                param[4].Value = destination;
                param[5] = new SqlParameter("@airline", SqlDbType.VarChar, 2);
                param[5].Value = airline;
                param[6] = new SqlParameter("@tripType", SqlDbType.SmallInt);
                param[6].Value = tripType;
                param[7] = new SqlParameter("@cabinClass", SqlDbType.SmallInt);
                param[7].Value = cabinClass;
                param[8] = new SqlParameter("@baseFare", SqlDbType.Decimal);
                param[8].Value = baseFare;
                param[9] = new SqlParameter("@tax", SqlDbType.Decimal);
                param[9].Value = tax;
                param[10] = new SqlParameter("@markup", SqlDbType.Decimal);
                param[10].Value = markup;
                param[11] = new SqlParameter("@searchDateTime", SqlDbType.DateTime);
                param[11].Value = searchDateTime;
                using (SqlConnection con = DataConnection.GetConnection())
                {
                    return SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "usp_UserSearchFareDealUpdate", param);
                }
            }
            catch (Exception ex)
            {

                DalLog.LogCreater.CreateLogFile(ex.ToString(), "Log\\TripJack\\Error", "UpdateUserSearchFareDeal" + ".txt");
                return 0;
            }

        }
        public static int DeleteUserSearchFareDeal(Int64 id)
        {
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@id", SqlDbType.BigInt);
            param[0].Value = id;
            using (SqlConnection con = DataConnection.GetConnection())
            {
                return SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "usp_UserSearchFareDealDelete", param);
            }
        }

    }

}

