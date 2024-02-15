using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.OneDFare
{
    public class DalOneDFare
    {
        public DataSet getOneDResult(string Origin, string Destination, DateTime DepartureDate, int totAdtChd)
        {
            try
            {
                SqlParameter[] prm = new SqlParameter[]
                {
                    new SqlParameter("@Origin",Origin),
                    new SqlParameter("@Destination",Destination),
                    new SqlParameter("@DepartureDate",DepartureDate),
                    new SqlParameter("@totAdtChd",totAdtChd)
                };
                return SqlHelper.ExecuteDataset(DataConnection.GetConnection(), CommandType.StoredProcedure, "GetOneDFare", prm);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataSet getVerify(int id, int totAdtChd)
        {
            try
            {
                SqlParameter[] prm = new SqlParameter[]
                {
                    new SqlParameter("@id",id),                 
                    new SqlParameter("@totAdtChd",totAdtChd),
                     new SqlParameter("@Counter","PriceVerify")
                };
                return SqlHelper.ExecuteDataset(DataConnection.GetConnection(), CommandType.StoredProcedure, "GetOneDFare", prm);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataSet getTicketed(int id, int totAdtChd)
        {
            try
            {
                SqlParameter[] prm = new SqlParameter[]
                {
                    new SqlParameter("@id",id),
                    new SqlParameter("@totAdtChd",totAdtChd),
                     new SqlParameter("@Counter","Ticket")
                };
                return SqlHelper.ExecuteDataset(DataConnection.GetConnection(), CommandType.StoredProcedure, "GetOneDFare", prm);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
