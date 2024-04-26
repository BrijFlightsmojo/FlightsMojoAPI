using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Booking
{
   public class Get_BookingDetails
    {
        //public void Get_BookingSessionID(int BookingID)
        //{
        //    SqlParameter[] param = new SqlParameter[1];

        //    param[0] = new SqlParameter("@BookingID", SqlDbType.Int);
        //    param[0].Value = BookingID;
        //    try
        //    {
        //        using (SqlConnection con = DataConnection.GetConnection())
        //        {
        //            DataSet ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Get_BookingRequest", param);
        //            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //            {
        //                string SID = ds.Tables[0].Rows[0]["searchID"].ToString();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        public Core.Flight.FlightSearchRequest GetFsr(int BookingID,bool isReturn,ref decimal totPrice,ref string SectorRef,ref Core.Flight.FlightBookingRequest bookingRequest)/*, ref string Paxlst*/
        {
            Core.Flight.FlightSearchRequest objFSR = new Core.Flight.FlightSearchRequest();
            using (SqlConnection con = DataConnection.GetConnection())
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@BookingID", SqlDbType.Int);
                param[0].Value=BookingID;
                param[1] = new SqlParameter("@TripID", SqlDbType.Int);
                param[1].Value = isReturn?2:1;
                DataSet ds = SqlHelper.ExecuteDataset(con, CommandType.StoredProcedure, "Get_BookingRequestForBooking", param);

                if (ds != null && ds.Tables.Count == 4)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        objFSR.userSessionID = ds.Tables[0].Rows[0]["userSessionID"].ToString();
                        objFSR.userSearchID = ds.Tables[0].Rows[0]["searchID"].ToString();
                        objFSR.userLogID = ds.Tables[0].Rows[0]["searchID"].ToString();
                        objFSR.tripType = Core.TripType.OneWay;
                        objFSR.adults = Convert.ToInt32(ds.Tables[0].Rows[0]["adult"]);
                        objFSR.child = Convert.ToInt32(ds.Tables[0].Rows[0]["child"]);
                        objFSR.infants = Convert.ToInt32(ds.Tables[0].Rows[0]["infant"]);
                        objFSR.cabinType = (Core.CabinType)ds.Tables[0].Rows[0]["CabinClass"];
                        objFSR.siteId = (Core.SiteId)ds.Tables[0].Rows[0]["SiteID"];
                        objFSR.sourceMedia = ds.Tables[0].Rows[0]["Source_Media"].ToString();
                        objFSR.userIP = ds.Tables[0].Rows[0]["UserIP"].ToString();
                        objFSR.travelType = (Core.TravelType)ds.Tables[0].Rows[0]["TravelType"];
                        bookingRequest.emailID = ds.Tables[0].Rows[0]["EmailID"].ToString();
                        bookingRequest.phoneNo = ds.Tables[0].Rows[0]["PhoneNo"].ToString();
                        objFSR.segment = new List<Core.Flight.SearchSegment>();
                        if (isReturn)
                        {
                            objFSR.segment.Add(new Core.Flight.SearchSegment()
                            {
                                originAirport = ds.Tables[0].Rows[0]["Destination"].ToString(),
                                orgArp = Core.FlightUtility.GetAirport(ds.Tables[0].Rows[0]["Destination"].ToString()),
                                destinationAirport = ds.Tables[0].Rows[0]["Origin"].ToString(),
                                destArp = Core.FlightUtility.GetAirport(ds.Tables[0].Rows[0]["Origin"].ToString()),
                                travelDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["ReturnDate"].ToString())
                            });
                        }
                        else
                        {
                            objFSR.segment.Add(new Core.Flight.SearchSegment()
                            {
                                originAirport = ds.Tables[0].Rows[0]["Origin"].ToString(),
                                orgArp = Core.FlightUtility.GetAirport(ds.Tables[0].Rows[0]["Origin"].ToString()),
                                destinationAirport = ds.Tables[0].Rows[0]["Destination"].ToString(),
                                destArp = Core.FlightUtility.GetAirport(ds.Tables[0].Rows[0]["Destination"].ToString()),
                                travelDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["TravelDate"].ToString())
                            });
                        }
                       
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            totPrice = Convert.ToDecimal(ds.Tables[1].Rows[0]["TotPrice"]);
                        }
                        if (ds.Tables[2].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[2].Rows)
                            {
                                SectorRef += (dr["Airline"].ToString() + dr["Flight_No"].ToString() + Convert.ToDateTime( dr["From_Date_Time"]).ToString("ddMMHHmm"));
                            }
                            //result.ResultCombination += (segment.Airline + segment.FlightNumber + segment.DepTime.ToString("ddMMHHmm"));
                        }

                        if (ds.Tables[3].Rows.Count > 0)
                        {
                            bookingRequest.passengerDetails = new List<Core.PassengerDetails>();
                            foreach (DataRow dr in ds.Tables[3].Rows)
                            {
                               

                               
                                bookingRequest.passengerDetails.Add(new Core.PassengerDetails()
                                {
                                    title = dr["Title"].ToString(),
                                    firstName = dr["Pax_First_Name"].ToString(),
                                    middleName = dr["Pax_Middle_Name"].ToString(),
                                    lastName = dr["Pax_Last_Name"].ToString(),
                                     dateOfBirth = Convert.ToDateTime(dr["Pax_DOB"]),
                                    gender = (Core.Gender)(dr["Gender"]),
                                    passportNumber = dr["Passport_No"].ToString(),
                                   // passportIssueDate = DateTime.Today(),
                                    passengerType = (Core.PassengerType)(dr["Pax_Type"]),
                                    nationality = "IN",
                                    day = "",
                                    month = "",
                                    year = "",
                                    exDay = "",
                                    exMonth = "",
                                    exYear = "",
                                    expiryDate = Convert.ToDateTime(dr["Expiry_Date"]),
                                    FFMiles = "",
                                    issueCountry = "",
                                    Meal = "",
                                    RedressNumber = "",
                                    Seat = "",
                                    SpecialAssistance = "",
                                    ticketNo = "",
                                   // travelerNo = "",
                                    TSA_Precheck = "",
                                });
                            }
                            
                        }
                    }
                }                   
            }
            return objFSR;
        }

        public static int SaveRemarks(Core.Flight.BookingRemark BookingRemark)
        {
            int isSaved = 0;
            try
            {
                SqlParameter[] param = new SqlParameter[5];
                param[0] = new SqlParameter("@BookingID", SqlDbType.Int);
                param[0].Value = BookingRemark.BookingID;
                param[1] = new SqlParameter("@ProdID", SqlDbType.Int);
                param[1].Value = BookingRemark.ProdID;
                param[2] = new SqlParameter("@Booking_Remarks", SqlDbType.VarChar, 2000);
                param[2].Value = BookingRemark.Booking_Remarks;
                param[3] = new SqlParameter("@ModifiedBy", SqlDbType.VarChar, 100);
                param[3].Value = BookingRemark.ModifiedBy;
                param[4] = new SqlParameter("@ErrorMsg", SqlDbType.VarChar, 2000);
                param[4].Direction = ParameterDirection.Output;
                using (SqlConnection con = DataConnection.GetConnection())
                {
                    isSaved = SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "SaveRemarks", param);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSaved;
        }


        

    }
}
