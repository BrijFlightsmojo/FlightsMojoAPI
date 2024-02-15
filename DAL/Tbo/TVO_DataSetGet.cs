
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Tbo
{
    public class Tbo_DataSetGet
    {
        public DataSet getToken()
        {
            using (SqlConnection conn = DataConnection.GetConnection())
            {
                using (SqlDataAdapter da = new SqlDataAdapter("select id, tokenID, status, creationDateTime, MemberId, AgencyId from tbotoken where status='true' and isLive='"+ (ConfigurationManager.AppSettings["TvoApiMode"].ToString().Equals("live", StringComparison.OrdinalIgnoreCase)).ToString().ToLower() + "' order by creationDateTime desc", conn))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    
                    return ds;
                }
            }
        }

        public void setToken(string TokenId, int MemberId, int AgencyId)
        {
            using (SqlConnection conn = DataConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("insert into tbotoken(tokenID, status, creationDateTime, MemberId, AgencyId, isLive)values( @tokenID, @status, @creationDateTime, @MemberId, @AgencyId, @isLive)", conn))
                {
                    cmd.Parameters.AddWithValue("@tokenID", TokenId);
                    cmd.Parameters.AddWithValue("@status", "true");
                    cmd.Parameters.AddWithValue("@creationDateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@MemberId", MemberId);
                    cmd.Parameters.AddWithValue("@AgencyId", AgencyId);
                    cmd.Parameters.AddWithValue("@isLive", ConfigurationManager.AppSettings["TvoApiMode"].ToString().Equals("live", StringComparison.OrdinalIgnoreCase));
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        public void setLogout( int id)
        {
            using (SqlConnection conn = DataConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("update tbotoken set status='false' where id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        public void setLogoutByTokenID(string tokenID)
        {
            using (SqlConnection conn = DataConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("update tbotoken set status='false' where tokenID=@tokenID", conn))
                {
                    cmd.Parameters.AddWithValue("@tokenID", tokenID);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}
