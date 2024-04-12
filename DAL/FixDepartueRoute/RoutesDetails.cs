
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.FixDepartueRoute
{
    public class RoutesDetails
    {
        public void SaveSatkarRouteswithDate(string org, string dest, string date, int supplier)
        {
            using (SqlConnection conn = DataConnection.GetConFlightsmojoindia_RDS())
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("usp_SaveSectorWithDate", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@origin", org);
                        cmd.Parameters.AddWithValue("@destination", dest);
                        cmd.Parameters.AddWithValue("@supplier", supplier);
                        cmd.Parameters.AddWithValue("@sectorDate",date);
                        cmd.ExecuteNonQuery();
                   
                    }
                }

                catch (Exception ex)
                {
                    ex.ToString();
                }
                finally
                {
                    conn.Close();
                }

            }
        }
        public void DeleteSatkarRouteswithDate( int supplier)
        {
            using (SqlConnection conn = DataConnection.GetConFlightsmojoindia_RDS())
            {
                try
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("usp_DeleteSectorWithDate", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;                     
                        cmd.Parameters.AddWithValue("@supplier", supplier);
                        cmd.ExecuteNonQuery();
                    }
                }

                catch (Exception ex)
                {
                    ex.ToString();
                }
                finally
                {
                    conn.Close();
                }

            }
        }
        public List<Core.GdsType> GetAvailableProvider(string org, string dest, string date)
        {
            List < Core.GdsType > lst = new List<Core.GdsType>();
            using (SqlConnection conn = DataConnection.GetConFlightsmojoindia_RDS())
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("get_SupplierSectorDate", conn))
                    {                     
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Origin", org);
                        cmd.Parameters.AddWithValue("@Destination", dest);
                        cmd.Parameters.AddWithValue("@availabledate", date);
                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                lst.Add((Core.GdsType)Convert.ToInt32(dr["supplier"]));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                finally
                {
                    conn.Close();
                }
            }
            return lst;
        }

        public List<Core.GdsType> GetAvailableProvider(string org, string dest, string date,int tType, string ReturnDate)
        {
            List<Core.GdsType> lst = new List<Core.GdsType>();
            using (SqlConnection conn = DataConnection.GetConFlightsmojoindia_RDS())
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("get_SupplierSectorDate_V2", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Origin", org);
                        cmd.Parameters.AddWithValue("@Destination", dest);
                        cmd.Parameters.AddWithValue("@availabledate", date);
                        cmd.Parameters.AddWithValue("@tripType", tType);
                        if (tType == 2)
                        {
                            cmd.Parameters.AddWithValue("@ReturnDate", ReturnDate);
                        }
                       
                        conn.Open();
                        SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                lst.Add((Core.GdsType)Convert.ToInt32(dr["supplier"]));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                finally
                {
                    conn.Close();
                }
            }
            return lst;
        }
    }
}
