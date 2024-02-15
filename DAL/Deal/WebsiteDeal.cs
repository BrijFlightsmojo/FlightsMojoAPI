using System;
using System.Data;
using System.Data.SqlClient;
namespace DAL.Deal
{
    public class WebsiteDeal
    {
        public static SqlDataReader Get(int id)
        {
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@id", SqlDbType.Int);
            param[0].Value = id;

            using (SqlConnection con = DataConnection.GetConnection())
            {
                return SqlHelper.ExecuteReader(con, CommandType.StoredProcedure, "usp_WebsiteDealSelect", param);
            }
        }
        public static int SaveWebsiteDeal(int id, int dealType, string origin, string destination, string airline, string tripType, string cabinClass, int modifyBy, DateTime modifyOn)
        {
            SqlParameter[] param = new SqlParameter[9];
            param[0] = new SqlParameter("@id", SqlDbType.Int);
            param[0].Value = id;
            param[1] = new SqlParameter("@dealType", SqlDbType.Int);
            param[1].Value = dealType;
            param[2] = new SqlParameter("@origin", SqlDbType.VarChar, 3);
            param[2].Value = origin;
            param[3] = new SqlParameter("@destination", SqlDbType.VarChar, 3);
            param[3].Value = destination;
            param[4] = new SqlParameter("@airline", SqlDbType.VarChar, 3);
            param[4].Value = airline;
            param[5] = new SqlParameter("@tripType", SqlDbType.SmallInt);
            param[5].Value = tripType;
            param[6] = new SqlParameter("@cabinClass", SqlDbType.SmallInt);
            param[6].Value = cabinClass;
            param[7] = new SqlParameter("@modifyBy", SqlDbType.Int);
            param[7].Value = modifyBy;
            param[8] = new SqlParameter("@modifyOn", SqlDbType.DateTime);
            param[8].Value = modifyOn;
            using (SqlConnection con = DataConnection.GetConnection())
            {
                return SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "usp_WebsiteDealInsert", param);
            }
        }
        public static int UpdateWebsiteDeal(int id, int dealType, string origin, string destination, string airline, string tripType, string cabinClass, int modifyBy, DateTime modifyOn)
        {
            SqlParameter[] param = new SqlParameter[9];
            param[0] = new SqlParameter("@id", SqlDbType.Int);
            param[0].Value = id;
            param[1] = new SqlParameter("@dealType", SqlDbType.Int);
            param[1].Value = dealType;
            param[2] = new SqlParameter("@origin", SqlDbType.VarChar, 3);
            param[2].Value = origin;
            param[3] = new SqlParameter("@destination", SqlDbType.VarChar, 3);
            param[3].Value = destination;
            param[4] = new SqlParameter("@airline", SqlDbType.VarChar, 3);
            param[4].Value = airline;
            param[5] = new SqlParameter("@tripType", SqlDbType.SmallInt);
            param[5].Value = tripType;
            param[6] = new SqlParameter("@cabinClass", SqlDbType.SmallInt);
            param[6].Value = cabinClass;
            param[7] = new SqlParameter("@modifyBy", SqlDbType.Int);
            param[7].Value = modifyBy;
            param[8] = new SqlParameter("@modifyOn", SqlDbType.DateTime);
            param[8].Value = modifyOn;
            using (SqlConnection con = DataConnection.GetConnection())
            {
                return SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "usp_WebsiteDealUpdate", param);
            }
        }
        public static int DeleteWebsiteDeal(int id)
        {
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@id", SqlDbType.Int);
            param[0].Value = id;
            using (SqlConnection con = DataConnection.GetConnection())
            {
                return SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "usp_WebsiteDealDelete", param);
            }
        }
    }

}

