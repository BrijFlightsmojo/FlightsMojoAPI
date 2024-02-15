using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Deal
{
    public class FareType
    {
        //public async Task SetFareType(int FMFareType, string Airline, string FareType, int Provider)
        //{
        //    using (SqlConnection con = DataConnection.GetConnection())
        //    {
        //        using (SqlCommand cmd = new SqlCommand())
        //        {
        //            try
        //            {
        //                cmd.Connection = con;
        //                cmd.CommandText = "Set_Faretypes";
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@FMFareType", FMFareType);
        //                cmd.Parameters.AddWithValue("@Airline", Airline.Trim());
        //                cmd.Parameters.AddWithValue("@FareType", FareType.Trim());
        //                cmd.Parameters.AddWithValue("@Provider", Provider);
        //                cmd.Parameters.AddWithValue("@Counter", "Insert");
        //                con.Open();
        //                cmd.ExecuteNonQuery();
        //            }
        //            catch (Exception ex)
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

    }
}
