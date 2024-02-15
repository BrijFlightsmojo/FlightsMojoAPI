using Core.Flight;
using System;
using System.Data;
using System.Data.SqlClient;
namespace DAL.Deal
{
    public class UserSearchHistory
    {

        //public SqlDataReader Get(Int64 id)
        //{
        //    SqlParameter[] param = new SqlParameter[1];
        //    param[0] = new SqlParameter("@id", SqlDbType.BigInt);
        //    param[0].Value = id;
        //    using (SqlConnection con = DataConnection.GetConnection())
        //    {
        //        return SqlHelper.ExecuteReader(con, CommandType.StoredProcedure, "usp_UserSearchHistorySelect", param);
        //    }
        //}
        public async System.Threading.Tasks.Task SaveUserSearchHistory(FlightSearchRequest fsr, bool isCommingMeta, string ip, int totalResult)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[23];

                param[0] = new SqlParameter("@siteID", SqlDbType.Int);
                param[0].Value = (int)fsr.siteId;
                param[1] = new SqlParameter("@sourchMedia", SqlDbType.VarChar, 50);
                param[1].Value = fsr.sourceMedia;
                param[2] = new SqlParameter("@origin", SqlDbType.VarChar, 3);
                param[2].Value = fsr.segment[0].originAirport;
                param[3] = new SqlParameter("@destination", SqlDbType.VarChar, 3);
                param[3].Value = fsr.segment[0].destinationAirport;
                param[4] = new SqlParameter("@depDate", SqlDbType.DateTime);
                param[4].Value = fsr.segment[0].travelDate;
                if (fsr.segment.Count > 1)
                {
                    param[5] = new SqlParameter("@retDate", SqlDbType.DateTime);
                    param[5].Value = fsr.segment[1].travelDate;
                }
                param[6] = new SqlParameter("@tripType", SqlDbType.SmallInt);
                param[6].Value = (int)fsr.tripType;
                param[7] = new SqlParameter("@cabinClass", SqlDbType.SmallInt);
                param[7].Value = (int)fsr.cabinType;
                param[8] = new SqlParameter("@adult", SqlDbType.SmallInt);
                param[8].Value = fsr.adults;
                param[9] = new SqlParameter("@child", SqlDbType.SmallInt);
                param[9].Value = fsr.child;
                param[10] = new SqlParameter("@infatnt", SqlDbType.SmallInt);
                param[10].Value = fsr.infants;
                param[11] = new SqlParameter("@infantWs", SqlDbType.SmallInt);
                param[11].Value = 0;
                param[12] = new SqlParameter("@airline", SqlDbType.VarChar, 3);
                param[12].Value = fsr.airline;
                param[13] = new SqlParameter("@isDirectAirline", SqlDbType.Bit);
                param[13].Value = fsr.searchDirectFlight;
                param[14] = new SqlParameter("@isFlexiableDate", SqlDbType.Bit);
                param[14].Value = fsr.flexibleSearch;
                param[15] = new SqlParameter("@isNearBy", SqlDbType.Bit);
                param[15].Value = fsr.isNearBy;
                param[16] = new SqlParameter("@searchID", SqlDbType.VarChar, 100);
                param[16].Value = fsr.userSearchID;
                param[17] = new SqlParameter("@userSessionID", SqlDbType.VarChar, 100);
                param[17].Value = fsr.userSessionID;
                param[18] = new SqlParameter("@userIP", SqlDbType.VarChar, 20);
                param[18].Value = fsr.userIP;
                param[19] = new SqlParameter("@serverIP", SqlDbType.VarChar, 20);
                param[19].Value = ip;
                param[20] = new SqlParameter("@bookingID", SqlDbType.Int);
                param[20].Value = 0;
                param[21] = new SqlParameter("@totalResult", SqlDbType.Int);
                param[21].Value = totalResult;
                param[22] = new SqlParameter("@isCommingMeta", SqlDbType.Bit);
                param[22].Value = isCommingMeta;

                using (SqlConnection con = DataConnection.GetConnection())
                {
                    SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "usp_UserSearchHistoryInsert", param);
                }
            }
            catch (Exception ex)
            {
                DalLog.LogCreater.CreateLogFile(ex.ToString(), "Log\\TripJack\\Error", "SaveUserSearchHistory" + ".txt");
            }

        }
        private DataTable getSearchSegmentTable(FlightSearchRequest fsr)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("origin", typeof(string));
            dt.Columns.Add("destination", typeof(string));
            dt.Columns.Add("travelDate", typeof(DateTime));

            foreach (var item in fsr.segment)
            {
                DataRow dtSeg = dt.NewRow();
                dtSeg["origin"] = item.originAirport;
                dtSeg["destination"] = item.destinationAirport;
                dtSeg["travelDate"] = item.travelDate;
                dt.Rows.Add(dtSeg);
            }
            return dt;
        }

        public async System.Threading.Tasks.Task SaveUserSearchHistoryMeta(FlightSearchRequest fsr, int totalResult, string Provider, string serverIP)
        {
            try
            {
                SqlParameter[] param = new SqlParameter[13];

                param[0] = new SqlParameter("@siteID", SqlDbType.Int);
                param[0].Value = (int)fsr.siteId;
                param[1] = new SqlParameter("@sourchMedia", SqlDbType.Int);
                param[1].Value = fsr.sourceMedia;
                param[2] = new SqlParameter("@origin", SqlDbType.VarChar, 3);
                param[2].Value = fsr.segment[0].originAirport;
                param[3] = new SqlParameter("@destination", SqlDbType.VarChar, 3);
                param[3].Value = fsr.segment[0].destinationAirport;
                param[4] = new SqlParameter("@tripType", SqlDbType.SmallInt);
                param[4].Value = (int)fsr.tripType;
                param[5] = new SqlParameter("@cabinClass", SqlDbType.SmallInt);
                param[5].Value = (int)fsr.cabinType;
                param[6] = new SqlParameter("@adult", SqlDbType.SmallInt);
                param[6].Value = fsr.adults;
                param[7] = new SqlParameter("@child", SqlDbType.SmallInt);
                param[7].Value = fsr.child;
                param[8] = new SqlParameter("@infatnt", SqlDbType.SmallInt);
                param[8].Value = fsr.infants;
                param[9] = new SqlParameter("@userIP", SqlDbType.VarChar, 20);
                param[9].Value = fsr.userIP;
                param[10] = new SqlParameter("@serverIP", SqlDbType.VarChar, 20);
                param[10].Value = serverIP;
                param[11] = new SqlParameter("@totalResult", SqlDbType.Int);
                param[11].Value = totalResult;
                param[12] = new SqlParameter("@provider", SqlDbType.VarChar, 20);
                param[12].Value = Provider;
                using (SqlConnection con = DataConnection.GetConnection())
                {
                    SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "Set_UserSearchHistory_Meta", param);
                }
            }
            catch (Exception ex)
            {
                DalLog.LogCreater.CreateLogFile(ex.ToString(), "Log\\TripJack\\Error", "SaveUserSearchHistoryMeta" + ".txt");
            }
        }

        public async System.Threading.Tasks.Task DeleteUserSearchHistoryMeta()
        {
            try
            {
                using (SqlConnection con = DataConnection.GetConnectionFareCaching())
                {
                    int a = SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "usp_MetaSearchDetailsDelete", null);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

}

