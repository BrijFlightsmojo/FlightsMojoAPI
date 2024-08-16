using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System;
namespace DAL
{
    public class DALFlightCacheGF
    {
        #region Normal Search
        //public async System.Threading.Tasks.Task SaveFlightData(string SearchKey, string Data, int ExpireHour)
        //{
        //    using (SqlConnection con = DataConnection.GetConnectionFareCaching())
        //    {
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            try
        //            {
        //                cmd.Connection = con;
        //                cmd.CommandText = "FlightSearchData_Insert";
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                cmd.Parameters.AddWithValue("@SearchKey", SearchKey);
        //                cmd.Parameters.AddWithValue("@SearchData", System.Text.Encoding.UTF8.GetBytes(Data));
        //                cmd.Parameters.AddWithValue("@ExpireHour", ExpireHour);
        //                con.Open();

        //                cmd.ExecuteNonQuery();

        //            }
        //            catch(Exception ex)
        //            {

        //            }
        //            finally
        //            {
        //                if (con.State == ConnectionState.Open)
        //                    con.Close();
        //            }

        //        }
        //    }
        //}
        //public string getFlightData(string SearchKey)
        //{
        //    string base64String = "";
        //    using (SqlConnection con = DataConnection.GetConnectionFareCaching())
        //    {
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            try
        //            {
        //                cmd.Connection = con;
        //                cmd.CommandText = "FlightSearchData_Select";
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                cmd.Parameters.AddWithValue("@SearchKey", SearchKey);
        //                con.Open();

        //                Byte[] content = cmd.ExecuteScalar() as Byte[];
        //                // base64String = Convert.ToBase64String(content, 0, content.Length);
        //                if(content!=null)
        //                base64String = System.Text.Encoding.UTF8.GetString(content);
        //            }
        //            catch
        //            {

        //            }
        //            finally
        //            {
        //                if (con.State == ConnectionState.Open)
        //                    con.Close();
        //            }

        //        }
        //    }
        //    return base64String;
        //}
        #endregion

        #region Meta Search 
        public async System.Threading.Tasks.Task SaveMetaSearchDetailsGF(string SearchID, string resultData)
        {
            using (SqlConnection con = DataConnection.GetConnectionFareCachingGF())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "usp_MetaSearchDetailsInsert";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@SearchID", SearchID);                      
                        cmd.Parameters.AddWithValue("@resultData", System.Text.Encoding.UTF8.GetBytes(resultData));
                        con.Open();

                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                            con.Close();
                    }
                }
            }
        }
        public string getMetaSearchDetails(string SearchID)
        {
            string base64String = "";
            using (SqlConnection con = DataConnection.GetConnectionFareCachingGF())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "usp_MetaSearchDetailsSelectGF";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@SearchID", SearchID);
                        con.Open();

                        Byte[] content = cmd.ExecuteScalar() as Byte[];
                        // base64String = Convert.ToBase64String(content, 0, content.Length);
                        if (content != null)
                            base64String = System.Text.Encoding.UTF8.GetString(content);
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                            con.Close();
                    }

                }
            }
            return base64String;          
        }
        #endregion
    }
}
