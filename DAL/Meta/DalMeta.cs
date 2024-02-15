using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Meta
{
    public class DalMeta
    {
        public Core.MetaResponse.MetaRoute getAllMetaRoute()
        {
            Core.MetaResponse.MetaRoute obj = new Core.MetaResponse.MetaRoute() { locale = new List<string>(), routes = new List<Core.MetaResponse.Route>() };
            obj.locale.Add("IN");
            using (SqlConnection con = DataConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("select [Origin],[Destination] from [MetaRouteTable] (NOLOCK)", con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                Core.MetaResponse.Route arp = new Core.MetaResponse.Route();
                                arp.OriginAirportCode = dr["Origin"].ToString().ToUpper();
                                arp.DestinationtAirportCode = dr["Destination"].ToString();                             
                                obj.routes.Add(arp);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return obj;
        }
        public List<Core.MetaResponse.RouteSyc> getAllMetaRouteSyc()
        {
            List<Core.MetaResponse.RouteSyc> obj = new List<Core.MetaResponse.RouteSyc>();
          
            using (SqlConnection con = DataConnection.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("select * from [MetaRouteTable_Skyscanner] (NOLOCK)", con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                Core.MetaResponse.RouteSyc arp = new Core.MetaResponse.RouteSyc();
                                arp.origin = dr["Origin"].ToString().ToUpper();
                                arp.destination = dr["Destination"].ToString();
                                arp.days_of_week = dr["days_of_week"].ToString().ToUpper();
                                arp.effective_start =string.IsNullOrEmpty( dr["effective_start"].ToString())?DateTime.Today.ToString("yyyy-MM-dd"):Convert.ToDateTime(dr["effective_start"]).ToString("yyyy-MM-dd");
                                arp.effective_end = string.IsNullOrEmpty(dr["effective_end"].ToString().ToUpper()) ? DateTime.Today.AddYears(1).ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["effective_end"]).ToString("yyyy-MM-dd");
                              
                                obj.Add(arp);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return obj;
        }
    }
}
