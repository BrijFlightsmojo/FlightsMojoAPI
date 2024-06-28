using System;
using Core;
using Core.Flight;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL.Booking
{
    public class SaveBookingDetails
    {
        public void SaveFlightBookingDetails(FlightBookingRequest flightBookingRequest)
        {
            string Eft = "";
            int outEft = 0, inEft = 0;
            DateTime depDate = DateTime.Today, arrDate = DateTime.Today;
            decimal TotalAmount = 0;
            SaveFMJ_FlightBookingDetails(ref flightBookingRequest, getAmountTable(ref flightBookingRequest, ref TotalAmount), getpassengerTable(ref flightBookingRequest), getSectorTable(ref flightBookingRequest, ref Eft, ref outEft, ref inEft, ref depDate, ref arrDate), ref Eft, ref outEft, ref inEft, ref depDate, ref arrDate, ref TotalAmount);
        }
        private bool SaveFMJ_FlightBookingDetails(ref FlightBookingRequest flightBookingRequest, DataTable dtAmount, DataTable dtPax, DataTable dtSector, ref string Eft, ref int outEft, ref int inEft, ref DateTime DepDate, ref DateTime arrDate, ref decimal TotalAmount)
        {
            SqlParameter[] param = new SqlParameter[72];

            TripType tripType = TripType.OneWay;
            if (flightBookingRequest.flightResult.Count > 1)
                tripType = TripType.RoundTrip;
            else if (flightBookingRequest.flightResult[0].FlightSegments.Count > 1)
                tripType = TripType.RoundTrip;

            param[0] = new SqlParameter("@BookingID", SqlDbType.Int);
            param[0].Value = flightBookingRequest.bookingID;

            param[1] = new SqlParameter("@Invoice_No", SqlDbType.VarChar, 50);
            param[1].Value = flightBookingRequest.bookingID.ToString();
            param[2] = new SqlParameter("@Booking_Type", SqlDbType.VarChar, 50);
            param[2].Value = "ARF";
            param[3] = new SqlParameter("@Currency_Type", SqlDbType.VarChar, 3);
            param[3].Value = flightBookingRequest.flightResult[0].Fare.Currency;

            param[4] = new SqlParameter("@ProdID", SqlDbType.Int);
            param[4].Value = flightBookingRequest.prodID;
            param[5] = new SqlParameter("@Provider", SqlDbType.Int);
            param[5].Value = (int)flightBookingRequest.flightResult[0].gdsType;
            param[6] = new SqlParameter("@AdminID", SqlDbType.Int);
            param[6].Value = flightBookingRequest.AdminID > 0 ? flightBookingRequest.AdminID : 1000;
            param[7] = new SqlParameter("@Booking_By_Type", SqlDbType.Int);
            param[7].Value = (int)BookingType.WebBooking;
            param[8] = new SqlParameter("@Booking_Status", SqlDbType.Int);
            param[8].Value = (int)BookingStatus.Incomplete;
            param[9] = new SqlParameter("@Payment_Status", SqlDbType.Int);
            param[9].Value = (int)PaymentStatus.PaymentPending;
            param[10] = new SqlParameter("@Booking_Remarks", SqlDbType.VarChar, 500);
            param[10].Value = "";
            param[11] = new SqlParameter("@Total_Amount", SqlDbType.Decimal);
            param[11].Value = TotalAmount;
            param[12] = new SqlParameter("@PNR_OB", SqlDbType.VarChar, 50);
            param[12].Value = "";
            param[13] = new SqlParameter("@PNR_IB", SqlDbType.VarChar, 50);
            param[13].Value = "";
            param[14] = new SqlParameter("@SiteID", SqlDbType.Int);
            param[14].Value = (int)flightBookingRequest.siteID;
            param[15] = new SqlParameter("@Source_Media", SqlDbType.VarChar, 50);
            param[15].Value = flightBookingRequest.sourceMedia;
            param[16] = new SqlParameter("@Product_Type", SqlDbType.Int);
            param[16].Value = ProductType.Flight;
            param[17] = new SqlParameter("@isLocked", SqlDbType.Bit);
            param[17].Value = false;
            param[18] = new SqlParameter("@MobileNo", SqlDbType.VarChar, 50);
            param[18].Value = flightBookingRequest.mobileNo;
            param[19] = new SqlParameter("@PhoneNo", SqlDbType.VarChar, 50);
            param[19].Value = flightBookingRequest.phoneNo;
            param[20] = new SqlParameter("@EmailID", SqlDbType.VarChar, 50);
            param[20].Value = flightBookingRequest.emailID;
            param[21] = new SqlParameter("@TripType", SqlDbType.Int);
            param[21].Value = (int)tripType;
            param[22] = new SqlParameter("@CabinClass", SqlDbType.Int);
            param[22].Value = flightBookingRequest.flightResult[0].cabinClass;
            param[23] = new SqlParameter("@Origin", SqlDbType.VarChar, 50);
            param[23].Value = flightBookingRequest.flightResult[0].FlightSegments.FirstOrDefault().Segments.FirstOrDefault().Origin;
            param[24] = new SqlParameter("@Destination", SqlDbType.VarChar, 50);
            param[24].Value = flightBookingRequest.flightResult[0].FlightSegments.FirstOrDefault().Segments.LastOrDefault().Destination;
            param[25] = new SqlParameter("@ValCarrier", SqlDbType.VarChar, 50);
            param[25].Value = flightBookingRequest.flightResult[0].valCarrier;
            param[26] = new SqlParameter("@TravelDate", SqlDbType.DateTime);
            param[26].Value = DepDate;
            param[27] = new SqlParameter("@PaxFirstName", SqlDbType.VarChar, 50);
            param[27].Value = flightBookingRequest.passengerDetails.FirstOrDefault().firstName;
            param[28] = new SqlParameter("@PaxMiddleName", SqlDbType.VarChar, 50);
            param[28].Value = flightBookingRequest.passengerDetails.FirstOrDefault().middleName;
            param[29] = new SqlParameter("@PaxLastName", SqlDbType.VarChar, 50);
            param[29].Value = flightBookingRequest.passengerDetails.FirstOrDefault().lastName;
            param[30] = new SqlParameter("@adult", SqlDbType.Int);
            param[30].Value = flightBookingRequest.adults;
            param[31] = new SqlParameter("@child", SqlDbType.Int);
            param[31].Value = flightBookingRequest.child;
            param[32] = new SqlParameter("@infant", SqlDbType.Int);
            param[32].Value = flightBookingRequest.infants;
            param[33] = new SqlParameter("@infantWs", SqlDbType.Int);
            param[33].Value = flightBookingRequest.infantsWs;
            param[34] = new SqlParameter("@AirlineLocator", SqlDbType.VarChar, 100);
            param[34].Value = "";
            param[35] = new SqlParameter("@TicketionPCC", SqlDbType.VarChar, 50);
            param[35].Value = "";
            param[36] = new SqlParameter("@SubStatus", SqlDbType.VarChar, 200);
            param[36].Value = "";
            param[37] = new SqlParameter("@outEft", SqlDbType.Int);
            param[37].Value = outEft;
            param[38] = new SqlParameter("@InEft", SqlDbType.Int);
            param[38].Value = inEft;
            param[39] = new SqlParameter("@MarkupID", SqlDbType.Int);
            param[39].Value = 0;
            param[40] = new SqlParameter("@FareType", SqlDbType.VarChar, 50);
            param[40].Value = flightBookingRequest.flightResult[0].Fare.FareType;
            param[41] = new SqlParameter("@TravelType", SqlDbType.Int);
            param[41].Value = new FlightUtility().isDomestic(flightBookingRequest.flightResult[0].FlightSegments.FirstOrDefault().Segments.FirstOrDefault().Origin, flightBookingRequest.flightResult[0].FlightSegments.FirstOrDefault().Segments.LastOrDefault().Destination) ? (int)TravelType.Domestic : (int)TravelType.International;
            param[42] = new SqlParameter("@ModifyBy", SqlDbType.Int);
            param[42].Value = 1000;

            //param[43] = new SqlParameter("@billingPhoneNo", SqlDbType.VarChar, 15);
            //param[43].Value = flightBookingRequest.phoneNo;
            param[44] = new SqlParameter("@EFT", SqlDbType.VarChar, 50);
            param[44].Value = Eft;

            param[45] = new SqlParameter("@Amount_Details", dtAmount);
            param[46] = new SqlParameter("@Passenger_Details", dtPax);
            param[47] = new SqlParameter("@Sector_Details", dtSector);

            param[48] = new SqlParameter("@returnMessage", SqlDbType.NVarChar, 200);
            param[48].Direction = ParameterDirection.Output;

            param[49] = new SqlParameter("@searchID", SqlDbType.VarChar, 100);
            param[49].Value = flightBookingRequest.userSearchID;
            param[50] = new SqlParameter("@userSessionID", SqlDbType.VarChar, 100);
            param[50].Value = flightBookingRequest.userSessionID;

            if (flightBookingRequest.flightResult.Count >= 2 || flightBookingRequest.flightResult[0].FlightSegments.Count > 1)
            {
                param[51] = new SqlParameter("@ReturnDate", SqlDbType.DateTime);
                param[51].Value = arrDate;
            }

            //param[52] = new SqlParameter("@InsuranceID", SqlDbType.VarChar, 50);
            //param[52].Value = flightBookingRequest.InsuranceID;
            //param[53] = new SqlParameter("@TravelAssistance", SqlDbType.Bit);
            //param[53].Value = flightBookingRequest.isTravelAssistanceBye;
            //param[54] = new SqlParameter("@CancellaionPolicy", SqlDbType.Bit);
            //param[54].Value = flightBookingRequest.isCancellaionPolicyBye;
            //param[55] = new SqlParameter("@FlexibleTicket", SqlDbType.Bit);
            //param[55].Value = flightBookingRequest.isFlexibleTicket;
            if (!string.IsNullOrEmpty(flightBookingRequest.CouponCode) && flightBookingRequest.CouponAmount > 0)
            {
                param[56] = new SqlParameter("@CouponCode", SqlDbType.VarChar, 50);
                param[56].Value = flightBookingRequest.CouponCode;
            }


            param[57] = new SqlParameter("@userIp", SqlDbType.VarChar, 50);
            param[57].Value = flightBookingRequest.userIP;
            //param[58] = new SqlParameter("@RequestedSeat", SqlDbType.VarChar, 50);
            //param[58].Value = flightBookingRequest.RequestedSeat;

            param[59] = new SqlParameter("@BrowserDetails", SqlDbType.VarChar, 200);
            param[59].Value = flightBookingRequest.BrowserDetails;

            if (flightBookingRequest.paymentDetails != null)
            {
                param[60] = new SqlParameter("@Address", SqlDbType.VarChar, 500);
                param[60].Value = (flightBookingRequest.paymentDetails.address1 + " " + flightBookingRequest.paymentDetails.address2).Trim();
                param[61] = new SqlParameter("@Country", SqlDbType.VarChar, 50);
                param[61].Value = flightBookingRequest.paymentDetails.country;
                param[62] = new SqlParameter("@State", SqlDbType.VarChar, 50);
                param[62].Value = flightBookingRequest.paymentDetails.state;
                param[63] = new SqlParameter("@City", SqlDbType.VarChar, 50);
                param[63].Value = flightBookingRequest.paymentDetails.city;
                param[64] = new SqlParameter("@PinCode", SqlDbType.VarChar, 10);
                param[64].Value = flightBookingRequest.paymentDetails.postalCode;
            }
            param[65] = new SqlParameter("@TraceID", SqlDbType.VarChar, 200);
            param[65].Value = flightBookingRequest.TvoTraceId;

            param[66] = new SqlParameter("@OBIndexNo", SqlDbType.VarChar, 50);
            param[66].Value = flightBookingRequest.flightResult[0].ResultIndex;
            if (flightBookingRequest.flightResult.Count > 1)
            {
                param[67] = new SqlParameter("@IBIndexNo", SqlDbType.VarChar, 50);
                param[67].Value = flightBookingRequest.flightResult[1].ResultIndex;
            }
            if (flightBookingRequest.TvoBookingID > 0)
            {
                param[68] = new SqlParameter("@OBBookingID", SqlDbType.Int);
                param[68].Value = flightBookingRequest.TvoBookingID;
            }
            if (flightBookingRequest.TvoReturnBookingID > 0)
            {
                param[69] = new SqlParameter("@IBBookingID", SqlDbType.Int);
                param[69].Value = flightBookingRequest.TvoReturnBookingID;
            }
            if (!string.IsNullOrEmpty(flightBookingRequest.PNR))
            {
                param[70] = new SqlParameter("@OBPnr", SqlDbType.VarChar, 50);
                param[70].Value = flightBookingRequest.PNR;
            }
            if (flightBookingRequest.TvoReturnBookingID > 0)
            {
                param[71] = new SqlParameter("@IBPnr", SqlDbType.VarChar, 50);
                param[71].Value = flightBookingRequest.ReturnPNR;
            }
            
            using (SqlConnection con = DataConnection.GetConnection())
            {

                SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "Save_FlightBookingDetails", param);
                if (param[48].Value.ToString().ToUpper() == "SUCCESS")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public void SaveFlightBookingDetails_WithOutPax(FlightBookingRequest flightBookingRequest)
        {
            string Eft = "";
            int outEft = 0, inEft = 0;
            DateTime depDate = DateTime.Today, arrDate = DateTime.Today;
            decimal TotalAmount = 0;
            SaveFMJ_FlightBookingDetails_WithOutPax(ref flightBookingRequest, getAmountTable(ref flightBookingRequest, ref TotalAmount), getSectorTable(ref flightBookingRequest, ref Eft, ref outEft, ref inEft, ref depDate, ref arrDate), ref Eft, ref outEft, ref inEft, ref depDate, ref arrDate, ref TotalAmount);
        }
        private bool SaveFMJ_FlightBookingDetails_WithOutPax(ref FlightBookingRequest flightBookingRequest, DataTable dtAmount, DataTable dtSector, ref string Eft, ref int outEft, ref int inEft, ref DateTime DepDate, ref DateTime arrDate, ref decimal TotalAmount)
        {
            //SqlParameter[] param = new SqlParameter[86];
            SqlParameter[] param = new SqlParameter[84];
            TripType tripType = TripType.OneWay;
            if (flightBookingRequest.flightResult.Count > 1)
                tripType = TripType.RoundTrip;
            else if (flightBookingRequest.flightResult[0].FlightSegments.Count > 1)
                tripType = TripType.RoundTrip;

            param[0] = new SqlParameter("@BookingID", SqlDbType.Int);
            param[0].Value = flightBookingRequest.bookingID;

            param[1] = new SqlParameter("@Invoice_No", SqlDbType.VarChar, 50);
            param[1].Value = flightBookingRequest.bookingID.ToString();
            param[2] = new SqlParameter("@Booking_Type", SqlDbType.VarChar, 50);
            param[2].Value = "ARF";
            param[3] = new SqlParameter("@Currency_Type", SqlDbType.VarChar, 3);
            param[3].Value = flightBookingRequest.flightResult[0].Fare.Currency;

            param[4] = new SqlParameter("@ProdID", SqlDbType.Int);
            param[4].Value = flightBookingRequest.prodID;
            param[5] = new SqlParameter("@Provider", SqlDbType.Int);
            //param[5].Value = (int)flightBookingRequest.flightResult[0].gdsType;
            param[5].Value = (int)flightBookingRequest.flightResult[0].Fare.gdsType;
            param[6] = new SqlParameter("@AdminID", SqlDbType.Int);
            param[6].Value = flightBookingRequest.AdminID > 0 ? flightBookingRequest.AdminID : 1000;
            param[7] = new SqlParameter("@Booking_By_Type", SqlDbType.Int);
            param[7].Value = (int)BookingType.WebBooking;
            param[8] = new SqlParameter("@Booking_Status", SqlDbType.Int);
            param[8].Value = (int)BookingStatus.Incomplete;
            param[9] = new SqlParameter("@Payment_Status", SqlDbType.Int);
            param[9].Value = (int)PaymentStatus.PaymentPending;
            param[10] = new SqlParameter("@Booking_Remarks", SqlDbType.VarChar, 500);
            param[10].Value = "";
            param[11] = new SqlParameter("@Total_Amount", SqlDbType.Decimal);
            param[11].Value = TotalAmount;
            param[12] = new SqlParameter("@PNR_OB", SqlDbType.VarChar, 50);
            param[12].Value = "";
            param[13] = new SqlParameter("@PNR_IB", SqlDbType.VarChar, 50);
            param[13].Value = "";
            param[14] = new SqlParameter("@SiteID", SqlDbType.Int);
            param[14].Value = (int)flightBookingRequest.siteID;
            param[15] = new SqlParameter("@Source_Media", SqlDbType.VarChar, 50);
            param[15].Value = flightBookingRequest.sourceMedia;
            param[16] = new SqlParameter("@Product_Type", SqlDbType.Int);
            param[16].Value = ProductType.Flight;
            param[17] = new SqlParameter("@isLocked", SqlDbType.Bit);
            param[17].Value = false;
            param[18] = new SqlParameter("@MobileNo", SqlDbType.VarChar, 50);
            param[18].Value = flightBookingRequest.mobileNo;
            param[19] = new SqlParameter("@PhoneNo", SqlDbType.VarChar, 50);
            param[19].Value = flightBookingRequest.phoneNo;
            param[20] = new SqlParameter("@EmailID", SqlDbType.VarChar, 50);
            param[20].Value = flightBookingRequest.emailID;
            param[21] = new SqlParameter("@TripType", SqlDbType.Int);
            param[21].Value = (int)tripType;
            param[22] = new SqlParameter("@CabinClass", SqlDbType.Int);
            param[22].Value = flightBookingRequest.flightResult[0].cabinClass;
            param[23] = new SqlParameter("@Origin", SqlDbType.VarChar, 50);
            param[23].Value = flightBookingRequest.flightResult[0].FlightSegments.FirstOrDefault().Segments.FirstOrDefault().Origin;
            param[24] = new SqlParameter("@Destination", SqlDbType.VarChar, 50);
            param[24].Value = flightBookingRequest.flightResult[0].FlightSegments.FirstOrDefault().Segments.LastOrDefault().Destination;
            param[25] = new SqlParameter("@ValCarrier", SqlDbType.VarChar, 50);
            param[25].Value = flightBookingRequest.flightResult[0].valCarrier;
            param[26] = new SqlParameter("@TravelDate", SqlDbType.DateTime);
            param[26].Value = DepDate;

            param[30] = new SqlParameter("@adult", SqlDbType.Int);
            param[30].Value = flightBookingRequest.adults;
            param[31] = new SqlParameter("@child", SqlDbType.Int);
            param[31].Value = flightBookingRequest.child;
            param[32] = new SqlParameter("@infant", SqlDbType.Int);
            param[32].Value = flightBookingRequest.infants;
            param[33] = new SqlParameter("@infantWs", SqlDbType.Int);
            param[33].Value = flightBookingRequest.infantsWs;
            param[34] = new SqlParameter("@AirlineLocator", SqlDbType.VarChar, 100);
            param[34].Value = "";
            param[35] = new SqlParameter("@TicketionPCC", SqlDbType.VarChar, 50);
            param[35].Value = "";
            param[36] = new SqlParameter("@SubStatus", SqlDbType.VarChar, 200);
            param[36].Value = "";
            param[37] = new SqlParameter("@outEft", SqlDbType.Int);
            param[37].Value = outEft;
            param[38] = new SqlParameter("@InEft", SqlDbType.Int);
            param[38].Value = inEft;
            param[39] = new SqlParameter("@MarkupID", SqlDbType.Int);
            param[39].Value = 0;
            param[40] = new SqlParameter("@FareType", SqlDbType.VarChar, 50);
            param[40].Value = flightBookingRequest.flightResult[0].Fare.FareType;
            param[41] = new SqlParameter("@TravelType", SqlDbType.Int);
            param[41].Value = new FlightUtility().isDomestic(flightBookingRequest.flightResult[0].FlightSegments.FirstOrDefault().Segments.FirstOrDefault().Origin, flightBookingRequest.flightResult[0].FlightSegments.FirstOrDefault().Segments.LastOrDefault().Destination) ? (int)TravelType.Domestic : (int)TravelType.International;
            param[42] = new SqlParameter("@ModifyBy", SqlDbType.Int);
            param[42].Value = 1000;

            //param[43] = new SqlParameter("@billingPhoneNo", SqlDbType.VarChar, 15);
            //param[43].Value = flightBookingRequest.phoneNo;
            param[44] = new SqlParameter("@EFT", SqlDbType.VarChar, 50);
            param[44].Value = Eft;

            param[45] = new SqlParameter("@Amount_Details", dtAmount);

            param[47] = new SqlParameter("@Sector_Details", dtSector);

            param[48] = new SqlParameter("@returnMessage", SqlDbType.NVarChar, 200);
            param[48].Direction = ParameterDirection.Output;

            param[49] = new SqlParameter("@searchID", SqlDbType.VarChar, 100);
            param[49].Value = flightBookingRequest.userSearchID;
            param[50] = new SqlParameter("@userSessionID", SqlDbType.VarChar, 100);
            param[50].Value = flightBookingRequest.userSessionID;

            if (flightBookingRequest.flightResult.Count >= 2 || flightBookingRequest.flightResult[0].FlightSegments.Count > 1)
            {
                param[51] = new SqlParameter("@ReturnDate", SqlDbType.DateTime);
                param[51].Value = arrDate;
            }

            //param[52] = new SqlParameter("@InsuranceID", SqlDbType.VarChar, 50);
            //param[52].Value = flightBookingRequest.InsuranceID;
            //param[53] = new SqlParameter("@TravelAssistance", SqlDbType.Bit);
            //param[53].Value = flightBookingRequest.isTravelAssistanceBye;
            //param[54] = new SqlParameter("@CancellaionPolicy", SqlDbType.Bit);
            //param[54].Value = flightBookingRequest.isCancellaionPolicyBye;
            //param[55] = new SqlParameter("@FlexibleTicket", SqlDbType.Bit);
            //param[55].Value = flightBookingRequest.isFlexibleTicket;
            if (!string.IsNullOrEmpty(flightBookingRequest.CouponCode) && flightBookingRequest.CouponAmount > 0)
            {
                param[56] = new SqlParameter("@CouponCode", SqlDbType.VarChar, 50);
                param[56].Value = flightBookingRequest.CouponCode;
            }


            param[57] = new SqlParameter("@userIp", SqlDbType.VarChar, 50);
            param[57].Value = flightBookingRequest.userIP;
            //param[58] = new SqlParameter("@RequestedSeat", SqlDbType.VarChar, 50);
            //param[58].Value = flightBookingRequest.RequestedSeat;

            param[59] = new SqlParameter("@BrowserDetails", SqlDbType.VarChar, 200);
            param[59].Value = flightBookingRequest.BrowserDetails;

            if (flightBookingRequest.paymentDetails != null)
            {
                param[60] = new SqlParameter("@Address", SqlDbType.VarChar, 500);
                param[60].Value = (flightBookingRequest.paymentDetails.address1 + " " + flightBookingRequest.paymentDetails.address2).Trim();
                param[61] = new SqlParameter("@Country", SqlDbType.VarChar, 50);
                param[61].Value = flightBookingRequest.paymentDetails.country;
                param[62] = new SqlParameter("@State", SqlDbType.VarChar, 50);
                param[62].Value = flightBookingRequest.paymentDetails.state;
                param[63] = new SqlParameter("@City", SqlDbType.VarChar, 50);
                param[63].Value = flightBookingRequest.paymentDetails.city;
                param[64] = new SqlParameter("@PinCode", SqlDbType.VarChar, 10);
                param[64].Value = flightBookingRequest.paymentDetails.postalCode;
            }
            param[65] = new SqlParameter("@TraceID", SqlDbType.VarChar, 200);
            param[65].Value = flightBookingRequest.TvoTraceId;

            param[66] = new SqlParameter("@OBIndexNo", SqlDbType.VarChar, 50);
            param[66].Value = flightBookingRequest.flightResult[0].ResultIndex;
            if (flightBookingRequest.flightResult.Count > 1)
            {
                param[67] = new SqlParameter("@IBIndexNo", SqlDbType.VarChar, 50);
                param[67].Value = flightBookingRequest.flightResult[1].ResultIndex;
            }
            if (flightBookingRequest.TvoBookingID > 0)
            {
                param[68] = new SqlParameter("@OBBookingID", SqlDbType.Int);
                param[68].Value = flightBookingRequest.TvoBookingID;
            }
            if (flightBookingRequest.TvoReturnBookingID > 0)
            {
                param[69] = new SqlParameter("@IBBookingID", SqlDbType.Int);
                param[69].Value = flightBookingRequest.TvoReturnBookingID;
            }
            if (!string.IsNullOrEmpty(flightBookingRequest.PNR))
            {
                param[70] = new SqlParameter("@OBPnr", SqlDbType.VarChar, 50);
                param[70].Value = flightBookingRequest.PNR;
            }
            if (flightBookingRequest.TvoReturnBookingID > 0)
            {
                param[71] = new SqlParameter("@IBPnr", SqlDbType.VarChar, 50);
                param[71].Value = flightBookingRequest.ReturnPNR;
            }
            if (flightBookingRequest.flightResult.Count > 1)
            {
                param[72] = new SqlParameter("@FareTypeReturn", SqlDbType.VarChar, 50);
                param[72].Value = flightBookingRequest.flightResult[1].Fare.FareType;
            }
            if (!string.IsNullOrEmpty(flightBookingRequest.GSTNo))
            {
                param[73] = new SqlParameter("@GstNo", SqlDbType.VarChar, 50);
                param[73].Value = flightBookingRequest.GSTNo;
            }
            if (!string.IsNullOrEmpty(flightBookingRequest.GSTCompany))
            {
                param[74] = new SqlParameter("@GstCompany", SqlDbType.VarChar, 150);
                param[74].Value = flightBookingRequest.GSTCompany;
            }

            param[75] = new SqlParameter("@IsWhatsapp", SqlDbType.Bit);
            param[75].Value = flightBookingRequest.isWhatsapp;

            param[76] = new SqlParameter("@OnlineStatus", SqlDbType.Int);
            param[76].Value = (int)BookingStatus.Incomplete;

            param[77] = new SqlParameter("@MojoFareType", SqlDbType.Int);
            param[77].Value = (int)flightBookingRequest.flightResult[0].Fare.mojoFareType;

            if (flightBookingRequest.flightResult.Count > 1)
            {
                param[78] = new SqlParameter("@MojofareTypeReturn", SqlDbType.Int);
                param[78].Value = (int)flightBookingRequest.flightResult[1].Fare.mojoFareType;
            }

            param[79] = new SqlParameter("@FB_Supplier", SqlDbType.Int);
            param[79].Value = (int)flightBookingRequest.flightResult[0].Fare.subProvider;


            param[80] = new SqlParameter("@MarkupRule", SqlDbType.VarChar, 500);
            param[80].Value = flightBookingRequest.flightResult[0].Fare.markupID;

            param[81] = new SqlParameter("@isBuyCancellaionPolicy", SqlDbType.Bit);
            param[81].Value = flightBookingRequest.isBuyCancellaionPolicy;

            param[82] = new SqlParameter("@isBuyRefundPolicy", SqlDbType.VarChar, 10);
            param[82].Value = flightBookingRequest.isBuyRefundPolicy;

            if (flightBookingRequest.flightResult.Count > 1)
            {
                param[83] = new SqlParameter("@ProviderIB", SqlDbType.Int);
                param[83].Value = (int)flightBookingRequest.flightResult[1].Fare.gdsType;
            }

            //param[84] = new SqlParameter("@utm_campaign", SqlDbType.VarChar,50);
            //param[84].Value = flightBookingRequest.utm_campaign;

            //param[85] = new SqlParameter("@utm_medium", SqlDbType.VarChar, 50);
            //param[85].Value = flightBookingRequest.utm_medium;

            using (SqlConnection con = DataConnection.GetConnection())
            {
                SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "Save_FlightBookingDetails_WithOutPax_V2", param);
                if (param[48].Value.ToString().ToUpper() == "SUCCESS")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }


        public void UpdateBookingPaxDetail(FlightBookingRequest flightBookingRequest)
        {

            UpdateFMJ_BookingPaxDetail(ref flightBookingRequest, getpassengerTable(ref flightBookingRequest));
        }
        private bool UpdateFMJ_BookingPaxDetail(ref FlightBookingRequest flightBookingRequest, DataTable dtPax)
        {
            SqlParameter[] param = new SqlParameter[72];

            param[0] = new SqlParameter("@BookingID", SqlDbType.Int);
            param[0].Value = flightBookingRequest.bookingID;

            param[1] = new SqlParameter("@PaxFirstName", SqlDbType.VarChar, 50);
            param[1].Value = flightBookingRequest.passengerDetails.FirstOrDefault().firstName;
            param[2] = new SqlParameter("@PaxMiddleName", SqlDbType.VarChar, 50);
            param[2].Value = flightBookingRequest.passengerDetails.FirstOrDefault().middleName;
            param[3] = new SqlParameter("@PaxLastName", SqlDbType.VarChar, 50);
            param[3].Value = flightBookingRequest.passengerDetails.FirstOrDefault().lastName;

            param[4] = new SqlParameter("@Passenger_Details", dtPax);

            param[5] = new SqlParameter("@returnMessage", SqlDbType.NVarChar, 200);
            param[5].Direction = ParameterDirection.Output;

            using (SqlConnection con = DataConnection.GetConnection())
            {

                SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "Save_UpdateBookingPaxDetail", param);
                if (param[5].Value.ToString().ToUpper() == "SUCCESS")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        private void PaxExists(FlightBookingRequest flightBookingRequest)
        {
            SqlParameter[] param = new SqlParameter[10];

            param[0] = new SqlParameter("@PhoneNo", SqlDbType.NVarChar, 50);
            param[0].Value = flightBookingRequest.phoneNo;

            param[1] = new SqlParameter("@EmailID", SqlDbType.VarChar, 50);
            param[1].Value = flightBookingRequest.emailID;

            param[2] = new SqlParameter("@TripType", SqlDbType.Int);
            param[2].Value = flightBookingRequest.flightResult.Count;

            param[3] = new SqlParameter("@CabinClass", SqlDbType.Int);
            param[3].Value = (int)flightBookingRequest.flightResult[0].cabinClass;

            param[4] = new SqlParameter("@Origin", SqlDbType.VarChar, 50);
            param[4].Value = flightBookingRequest.flightResult[0].FlightSegments.FirstOrDefault().Segments.FirstOrDefault().Origin;

            param[5] = new SqlParameter("@Destination", SqlDbType.VarChar, 50);
            param[5].Value = flightBookingRequest.flightResult[0].FlightSegments.FirstOrDefault().Segments.LastOrDefault().Destination;

            param[6] = new SqlParameter("@TravelDate", SqlDbType.VarChar, 50);
            param[6].Value = flightBookingRequest.flightResult[0].FlightSegments.FirstOrDefault().Segments.FirstOrDefault().DepTime;

            param[7] = new SqlParameter("@PaxFirstName", SqlDbType.VarChar, 50);
            param[7].Value = flightBookingRequest.passengerDetails.FirstOrDefault().firstName;


            param[8] = new SqlParameter("@PaxLastName", SqlDbType.VarChar, 50);
            param[8].Value = flightBookingRequest.passengerDetails.FirstOrDefault().lastName;

            param[9] = new SqlParameter("@returnMessage", SqlDbType.NVarChar, 200);
            param[9].Direction = ParameterDirection.Output;

            using (SqlConnection con = DataConnection.GetConnection())
            {
                SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "DuplicateBookingPaxDetail_V1", param);
                if (param[9].Value.ToString() == "Pax already exist")
                {
                    flightBookingRequest.tgy_Flight_Key = null;
                    flightBookingRequest.tgy_Booking_RefNo = null;
                    flightBookingRequest.tgy_Request_id = null;

                }
                else
                {
                    //  return false;
                }

            }
        }

        public ResponseStatus SaveFMJ_FlightBookingTransactionDetails(ref FlightBookingResponse flightBookingResponse)
        {
            SqlParameter[] param = new SqlParameter[24];

            param[0] = new SqlParameter("@BookingID", SqlDbType.Int);
            param[0].Value = flightBookingResponse.bookingID;

            param[1] = new SqlParameter("@Trns_No", SqlDbType.Int);
            param[1].Value = flightBookingResponse.transactionID;
            param[2] = new SqlParameter("@Amount", SqlDbType.Money);
            param[2].Value = flightBookingResponse.flightResult[0].Fare.PublishedFare;

            param[3] = new SqlParameter("@Card_No", SqlDbType.VarChar, 20);
            param[3].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.cardNumber : "";
            param[4] = new SqlParameter("@Holder_Name", SqlDbType.VarChar, 100);
            param[4].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.cardHolderName : "";
            param[5] = new SqlParameter("@Exp_Date", SqlDbType.VarChar, 10);
            param[5].Value = flightBookingResponse.paymentDetails != null ? ((flightBookingResponse.paymentDetails.expiryMonth) + "_" + (flightBookingResponse.paymentDetails.expiryYear)) : "";
            param[6] = new SqlParameter("@Valid_From", SqlDbType.VarChar, 10);
            param[6].Value = "";
            param[7] = new SqlParameter("@Issue_No", SqlDbType.VarChar, 10);
            param[7].Value = "";
            param[8] = new SqlParameter("@Security_Code", SqlDbType.VarChar, 5);
            param[8].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.cvvNo : "";
            param[9] = new SqlParameter("@Card_Type", SqlDbType.Int);
            param[9].Value = flightBookingResponse.paymentDetails != null ? ((int)getCardType(flightBookingResponse.paymentDetails.cardCode, flightBookingResponse.paymentDetails.cardNumber)) : 0;
            param[10] = new SqlParameter("@Card_Category", SqlDbType.VarChar, 50);
            param[10].Value = flightBookingResponse.paymentDetails.cardType;
            param[11] = new SqlParameter("@Country", SqlDbType.VarChar, 50);
            param[11].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.country : "";
            param[12] = new SqlParameter("@Couty_State", SqlDbType.VarChar, 50);
            param[12].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.state : "";
            param[13] = new SqlParameter("@City", SqlDbType.VarChar, 50);
            param[13].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.city : "";
            param[14] = new SqlParameter("@Post_Code", SqlDbType.VarChar, 10);
            param[14].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.postalCode : "";
            param[15] = new SqlParameter("@Address", SqlDbType.VarChar, 200);
            param[15].Value = flightBookingResponse.paymentDetails != null ? (flightBookingResponse.paymentDetails.address1 + (string.IsNullOrEmpty(flightBookingResponse.paymentDetails.address2) ? "" : (" " + flightBookingResponse.paymentDetails.address2))) : "";
            param[16] = new SqlParameter("@Card_Charges", SqlDbType.Decimal);
            param[16].Value = 0;
            param[17] = new SqlParameter("@Marchent", SqlDbType.VarChar, 50);
            param[17].Value = "";
            param[18] = new SqlParameter("@billingPhoneNo", SqlDbType.VarChar, 15);
            param[18].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.billingPhoneNo : flightBookingResponse.phoneNo;

            if (flightBookingResponse.TvoBookingID > 0)
            {
                param[19] = new SqlParameter("@OBBookingID", SqlDbType.Int);
                param[19].Value = flightBookingResponse.TvoBookingID;
            }
            if (flightBookingResponse.TvoReturnBookingID > 0)
            {
                param[20] = new SqlParameter("@IBBookingID", SqlDbType.Int);
                param[20].Value = flightBookingResponse.TvoReturnBookingID;
            }
            if (!string.IsNullOrEmpty(flightBookingResponse.PNR))
            {
                param[21] = new SqlParameter("@OBPnr", SqlDbType.VarChar, 50);
                param[21].Value = flightBookingResponse.PNR;
            }
            if (!string.IsNullOrEmpty(flightBookingResponse.ReturnPNR))
            {
                param[22] = new SqlParameter("@IBPnr", SqlDbType.VarChar, 50);
                param[22].Value = flightBookingResponse.ReturnPNR;
            }

            param[23] = new SqlParameter("@returnMessage", SqlDbType.NVarChar, 200);
            param[23].Direction = ParameterDirection.Output;

            //param[24] = new SqlParameter("@PaymentStatus", SqlDbType.VarChar, 50);
            //param[24].Value = flightBookingResponse.paymentDetails.OnlinePaymentStauts;
            //param[25] = new SqlParameter("@Hash", SqlDbType.VarChar, 500);
            //param[25].Value = flightBookingResponse.paymentDetails.Hash;
            //param[26] = new SqlParameter("@RetursHashMatched", SqlDbType.Bit);
            //param[26].Value = flightBookingResponse.paymentDetails.IsReturnHashMatched;



            using (SqlConnection con = DataConnection.GetConnection())
            {
                SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "Save_FlightBookingTransactionDetails", param);
                if (param[23].Value.ToString().ToUpper() == "SUCCESS")
                {
                    return new ResponseStatus() { status = TransactionStatus.Success, message = "SUCCESS" };
                }
                else
                {
                    return new ResponseStatus() { status = TransactionStatus.Error, message = "Transaction details not save successfully!" }; ;
                }
            }
        }
        public ResponseStatus SaveFMJ_FlightBookingTransactionDetailsWithOutCard(ref FlightBookingRequest bookRequest, ref FlightBookingResponse flightBookingResponse)
        {
            SqlParameter[] param = new SqlParameter[26];
            //BookingStatus bs = BookingStatus.Incomplete;
            //PaymentStatus ps = PaymentStatus.PaymentPending;
            //if (bookRequest.paymentDetails.OnlinePaymentStauts.Equals("success", StringComparison.OrdinalIgnoreCase) &&
            //    bookRequest.paymentDetails.IsReturnHashMatched)
            //{
            //    ps = PaymentStatus.PaymentDone;
            //    //bs = BookingStatus.Pending;
            //}
            //else
            //{
            //    ps = PaymentStatus.CardDecline;
            //}

            param[0] = new SqlParameter("@BookingID", SqlDbType.Int);
            param[0].Value = flightBookingResponse.bookingID;

            param[1] = new SqlParameter("@Trns_No", SqlDbType.Int);
            param[1].Value = flightBookingResponse.transactionID;
            param[2] = new SqlParameter("@Amount", SqlDbType.Money);
            param[2].Value = flightBookingResponse.flightResult[0].Fare.PublishedFare;


            param[3] = new SqlParameter("@Country", SqlDbType.VarChar, 50);
            param[3].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.country : "";
            param[4] = new SqlParameter("@Couty_State", SqlDbType.VarChar, 50);
            param[4].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.state : "";
            param[5] = new SqlParameter("@City", SqlDbType.VarChar, 50);
            param[5].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.city : "";
            param[6] = new SqlParameter("@Post_Code", SqlDbType.VarChar, 10);
            param[6].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.postalCode : "";
            param[7] = new SqlParameter("@Address", SqlDbType.VarChar, 200);
            param[7].Value = flightBookingResponse.paymentDetails != null ? (flightBookingResponse.paymentDetails.address1 + (string.IsNullOrEmpty(flightBookingResponse.paymentDetails.address2) ? "" : (" " + flightBookingResponse.paymentDetails.address2))) : "";
            param[8] = new SqlParameter("@Card_Charges", SqlDbType.Decimal);
            param[8].Value = 0;
            param[9] = new SqlParameter("@Marchent", SqlDbType.VarChar, 50);
            param[9].Value = "";
            param[10] = new SqlParameter("@billingPhoneNo", SqlDbType.VarChar, 15);
            param[10].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.billingPhoneNo : flightBookingResponse.phoneNo;

            if (flightBookingResponse.TvoBookingID > 0)
            {
                param[11] = new SqlParameter("@OBBookingID", SqlDbType.Int);
                param[11].Value = flightBookingResponse.TvoBookingID;
            }
            if (flightBookingResponse.TvoReturnBookingID > 0)
            {
                param[12] = new SqlParameter("@IBBookingID", SqlDbType.Int);
                param[12].Value = flightBookingResponse.TvoReturnBookingID;
            }
            if (!string.IsNullOrEmpty(flightBookingResponse.PNR))
            {
                param[13] = new SqlParameter("@OBPnr", SqlDbType.VarChar, 50);
                param[13].Value = flightBookingResponse.PNR;
            }
            if (!string.IsNullOrEmpty(flightBookingResponse.ReturnPNR))
            {
                param[14] = new SqlParameter("@IBPnr", SqlDbType.VarChar, 50);
                param[14].Value = flightBookingResponse.ReturnPNR;
            }

            param[15] = new SqlParameter("@returnMessage", SqlDbType.NVarChar, 200);
            param[15].Direction = ParameterDirection.Output;

            param[16] = new SqlParameter("@PaymentStatus", SqlDbType.VarChar, 50);
            param[16].Value = flightBookingResponse.paymentDetails.OnlinePaymentStauts;
            param[17] = new SqlParameter("@Hash", SqlDbType.VarChar, 500);
            param[17].Value = flightBookingResponse.paymentDetails.Hash;
            param[18] = new SqlParameter("@RetursHashMatched", SqlDbType.Bit);
            param[18].Value = flightBookingResponse.paymentDetails.IsReturnHashMatched;

            param[19] = new SqlParameter("@Booking_Status", SqlDbType.Int);
            param[19].Value = (int)flightBookingResponse.bookingStatus;
            param[20] = new SqlParameter("@Payment_Status", SqlDbType.Int);
            param[20].Value = (int)flightBookingResponse.paymentStatus;
            if (!string.IsNullOrEmpty(flightBookingResponse.responseStatus.message))
            {
                param[21] = new SqlParameter("@BookingRemarks", SqlDbType.VarChar, 500);
                param[21].Value = flightBookingResponse.responseStatus.message;
            }
            if (flightBookingResponse.invoice != null && flightBookingResponse.invoice.Count > 0)
            {
                if (flightBookingResponse.invoice[0].InvoiceAmount > 0)
                {
                    param[22] = new SqlParameter("@InvoiceAmount_OB", SqlDbType.Decimal);
                    param[22].Value = flightBookingResponse.invoice[0].InvoiceAmount;
                }
                //if (!string.IsNullOrEmpty(flightBookingResponse.invoice[0].InvoiceNo))
                //{
                //    param[23] = new SqlParameter("@Invoice_OB", SqlDbType.VarChar, 50);
                //    param[23].Value = flightBookingResponse.invoice[0].InvoiceNo;
                //}
            }
            if (flightBookingResponse.invoice != null && flightBookingResponse.invoice.Count > 1)
            {
                if (flightBookingResponse.invoice[1].InvoiceAmount > 0)
                {
                    param[24] = new SqlParameter("@InvoiceAmount_IB", SqlDbType.Decimal);
                    param[24].Value = flightBookingResponse.invoice[1].InvoiceAmount;
                }
                if (!string.IsNullOrEmpty(flightBookingResponse.invoice[1].InvoiceNo))
                {
                    param[25] = new SqlParameter("@Invoice_IB", SqlDbType.VarChar, 50);
                    param[25].Value = flightBookingResponse.invoice[1].InvoiceNo;
                }
            }

            //   if(flightBookingResponse.sourceMedia == "1000" || flightBookingResponse.sourceMedia == "1016" || flightBookingResponse.sourceMedia == "1013" || flightBookingResponse.sourceMedia == "1001")
            //if (flightBookingResponse.sourceMedia == "1000" || flightBookingResponse.sourceMedia == "1016" || flightBookingResponse.sourceMedia == "1037")
            //{
            param[26] = new SqlParameter("@Invoice_OB", SqlDbType.VarChar, 50);
            param[26].Value = flightBookingResponse.TjBookingID;
            //}
            //else
            //{
            //    param[26] = new SqlParameter("@Invoice_OB", SqlDbType.VarChar, 50);
            //    param[26].Value = flightBookingResponse.tgy_Booking_RefNo;
            //}


            using (SqlConnection con = DataConnection.GetConnection())
            {
                SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "Save_FlightBookingTransactionDetails_V3", param);
                if (param[15].Value.ToString().ToUpper() == "SUCCESS")
                {
                    return new ResponseStatus() { status = TransactionStatus.Success, message = "SUCCESS" };
                }
                else
                {
                    return new ResponseStatus() { status = TransactionStatus.Error, message = "Transaction details not save successfully!" }; ;
                }
            }
        }
        public ResponseStatus SaveFMJ_FlightBookingTransactionDetailsWithTickNo(ref FlightBookingRequest bookRequest, ref FlightBookingResponse flightBookingResponse)
        {
            SqlParameter[] param = new SqlParameter[39];

            param[0] = new SqlParameter("@BookingID", SqlDbType.Int);
            param[0].Value = flightBookingResponse.bookingID;

            param[1] = new SqlParameter("@Trns_No", SqlDbType.Int);
            param[1].Value = flightBookingResponse.transactionID;
            param[2] = new SqlParameter("@Amount", SqlDbType.Money);
            param[2].Value = flightBookingResponse.flightResult[0].Fare.PublishedFare;


            param[3] = new SqlParameter("@Country", SqlDbType.VarChar, 50);
            param[3].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.country : "";
            param[4] = new SqlParameter("@Couty_State", SqlDbType.VarChar, 50);
            param[4].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.state : "";
            param[5] = new SqlParameter("@City", SqlDbType.VarChar, 50);
            param[5].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.city : "";
            param[6] = new SqlParameter("@Post_Code", SqlDbType.VarChar, 10);
            param[6].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.postalCode : "";
            param[7] = new SqlParameter("@Address", SqlDbType.VarChar, 200);
            param[7].Value = flightBookingResponse.paymentDetails != null ? (flightBookingResponse.paymentDetails.address1 + (string.IsNullOrEmpty(flightBookingResponse.paymentDetails.address2) ? "" : (" " + flightBookingResponse.paymentDetails.address2))) : "";
            param[8] = new SqlParameter("@Card_Charges", SqlDbType.Decimal);
            param[8].Value = 0;
            param[9] = new SqlParameter("@Marchent", SqlDbType.VarChar, 50);
            param[9].Value = "";
            param[10] = new SqlParameter("@billingPhoneNo", SqlDbType.VarChar, 15);
            param[10].Value = flightBookingResponse.paymentDetails != null ? flightBookingResponse.paymentDetails.billingPhoneNo : flightBookingResponse.phoneNo;

            if (flightBookingResponse.TvoBookingID > 0)
            {
                param[11] = new SqlParameter("@OBBookingID", SqlDbType.Int);
                param[11].Value = flightBookingResponse.TvoBookingID;
            }
            if (flightBookingResponse.TvoReturnBookingID > 0)
            {
                param[12] = new SqlParameter("@IBBookingID", SqlDbType.Int);
                param[12].Value = flightBookingResponse.TvoReturnBookingID;
            }
            if (!string.IsNullOrEmpty(flightBookingResponse.PNR))
            {
                param[13] = new SqlParameter("@OBPnr", SqlDbType.VarChar, 50);
                param[13].Value = flightBookingResponse.PNR;
            }
            if (!string.IsNullOrEmpty(flightBookingResponse.ReturnPNR))
            {
                param[14] = new SqlParameter("@IBPnr", SqlDbType.VarChar, 50);
                param[14].Value = flightBookingResponse.ReturnPNR;
            }

            param[15] = new SqlParameter("@returnMessage", SqlDbType.NVarChar, 200);
            param[15].Direction = ParameterDirection.Output;

            param[16] = new SqlParameter("@PaymentStatus", SqlDbType.VarChar, 50);
            param[16].Value = flightBookingResponse.paymentDetails.OnlinePaymentStauts;
            param[17] = new SqlParameter("@Hash", SqlDbType.VarChar, 500);
            param[17].Value = flightBookingResponse.paymentDetails.Hash;
            param[18] = new SqlParameter("@RetursHashMatched", SqlDbType.Bit);
            param[18].Value = flightBookingResponse.paymentDetails.IsReturnHashMatched;

            if (flightBookingResponse.bookingStatus == 0)
            {
                param[19] = new SqlParameter("@Booking_Status", SqlDbType.Int);
                param[19].Value = 7;
            }
            else
            {
                param[19] = new SqlParameter("@Booking_Status", SqlDbType.Int);
                param[19].Value = (int)flightBookingResponse.bookingStatus;
            }

            param[20] = new SqlParameter("@Payment_Status", SqlDbType.Int);
            param[20].Value = (int)flightBookingResponse.paymentStatus;
            if (!string.IsNullOrEmpty(flightBookingResponse.responseStatus.message))
            {
                param[21] = new SqlParameter("@BookingRemarks", SqlDbType.VarChar, 500);
                param[21].Value = flightBookingResponse.responseStatus.message;
            }
            if (flightBookingResponse.invoice != null && flightBookingResponse.invoice.Count > 0)
            {
                if (flightBookingResponse.invoice[0].InvoiceAmount > 0)
                {
                    param[22] = new SqlParameter("@InvoiceAmount_OB", SqlDbType.Decimal);
                    param[22].Value = flightBookingResponse.invoice[0].InvoiceAmount;
                }
                if (!string.IsNullOrEmpty(flightBookingResponse.invoice[0].InvoiceNo))
                {
                    //param[23] = new SqlParameter("@Invoice_OB", SqlDbType.VarChar, 50);
                    //param[23].Value = flightBookingResponse.invoice[0].InvoiceNo;
                }
            }
            if (flightBookingResponse.invoice != null && flightBookingResponse.invoice.Count > 1)
            {
                if (flightBookingResponse.invoice[1].InvoiceAmount > 0)
                {
                    param[24] = new SqlParameter("@InvoiceAmount_IB", SqlDbType.Decimal);
                    param[24].Value = flightBookingResponse.invoice[1].InvoiceAmount;
                }
                if (!string.IsNullOrEmpty(flightBookingResponse.invoice[1].InvoiceNo))
                {
                    param[25] = new SqlParameter("@Invoice_IB", SqlDbType.VarChar, 50);
                    param[25].Value = flightBookingResponse.invoice[1].InvoiceNo;
                }
            }
            param[26] = new SqlParameter("@PassengerTktNo", getpassengerTableWithTickNo(ref flightBookingResponse));

            param[27] = new SqlParameter("@GatewayTransectionID", SqlDbType.VarChar, 50);
            param[27].Value = flightBookingResponse.razorpayTransectionID;

            param[28] = new SqlParameter("@ConvenienceFee", SqlDbType.Decimal);
            param[28].Value = bookRequest.convenienceFee;

            param[29] = new SqlParameter("@GatewayOrderID", SqlDbType.VarChar, 50);
            param[29].Value = flightBookingResponse.razorpayOrderID;

            param[30] = new SqlParameter("@PaymentMode", SqlDbType.Int);
            param[30].Value = flightBookingResponse.paymentMode;

            param[31] = new SqlParameter("@IssueDate", SqlDbType.DateTime);
            param[31].Value = DateTime.Now;

            if (flightBookingResponse.bookingStatus == Core.BookingStatus.Ticketed)
            {
                param[32] = new SqlParameter("@InvoiceNo", SqlDbType.Int);
                param[32].Value = DAL.IdGenrator.Get("InvoiceNo");
            }

            if (flightBookingResponse.flightResult[0].Fare.gdsType == Core.GdsType.TripJack)
            {
                param[33] = new SqlParameter("@Invoice_OB", SqlDbType.VarChar, 50);
                param[33].Value = flightBookingResponse.TjBookingID;
            }
            else if (flightBookingResponse.flightResult[0].Fare.gdsType == Core.GdsType.Travelogy)
            {
                param[33] = new SqlParameter("@Invoice_OB", SqlDbType.VarChar, 50);
                param[33].Value = flightBookingResponse.tgy_Booking_RefNo;
            }
            else if (flightBookingResponse.flightResult[0].Fare.gdsType == Core.GdsType.Tbo)
            {
                param[33] = new SqlParameter("@Invoice_OB", SqlDbType.VarChar, 50);
                param[33].Value = flightBookingResponse.TvoInvoiceNo;
            }
            param[34] = new SqlParameter("@OnlineStatus", SqlDbType.Int);
            param[34].Value = (int)BookingStatus.Ticketed;

            param[35] = new SqlParameter("@isBuyCancellaionPolicy", SqlDbType.Bit);
            param[35].Value = bookRequest.isBuyCancellaionPolicy;

            param[36] = new SqlParameter("@isBuyRefundPolicy", SqlDbType.Bit);
            param[36].Value = bookRequest.isBuyRefundPolicy;

            param[37] = new SqlParameter("@isBuyCancellaionPolicyAmt", SqlDbType.Decimal);
            param[37].Value = bookRequest.CancellaionPolicyAmt;

            param[38] = new SqlParameter("@isBuyRefundPolicyAmt", SqlDbType.Decimal);
            param[38].Value = bookRequest.RefundPolicyAmt;

            using (SqlConnection con = DataConnection.GetConnection())
            {
                SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "Save_FlightBookingTransactionDetails_WithTicketNo_V2", param);
                if (param[15].Value.ToString().ToUpper() == "SUCCESS")
                {
                    return new ResponseStatus() { status = TransactionStatus.Success, message = "SUCCESS" };
                }
                else
                {
                    return new ResponseStatus() { status = TransactionStatus.Error, message = "Transaction details not save successfully!" }; ;
                }
            }
        }
        public void update_mailStatus(long BookingID, int ProdID, string MailType)
        {
            if (BookingID > 0 && ProdID > 0 && string.IsNullOrEmpty(MailType) == false)
            {
                SqlParameter[] param = new SqlParameter[4];

                param[0] = new SqlParameter("@BookingID", SqlDbType.Int);
                param[0].Value = BookingID;
                param[1] = new SqlParameter("@ProdID", SqlDbType.Int);
                param[1].Value = ProdID;
                if (MailType.ToString().ToLower() == "confirmation")
                {
                    param[2] = new SqlParameter("@isConfirmationMailSent", SqlDbType.Bit);
                    param[2].Value = true;
                    param[3] = new SqlParameter("@ConfirmationMailSentOn", SqlDbType.DateTime);
                    param[3].Value = DateTime.Now;
                }
                if (MailType.ToString().ToLower() == "issue")
                {
                    param[2] = new SqlParameter("@isIssuedMailSent", SqlDbType.Bit);
                    param[2].Value = true;
                    param[3] = new SqlParameter("@IssuedMailSentOn", SqlDbType.DateTime);
                    param[3].Value = DateTime.Now;
                }
                if (MailType.ToString().ToLower() == "cancel")
                {
                    param[2] = new SqlParameter("@isCancelMailSent", SqlDbType.Bit);
                    param[2].Value = true;
                    param[3] = new SqlParameter("@CancelMailSentOn", SqlDbType.DateTime);
                    param[3].Value = DateTime.Now;
                }

                using (SqlConnection con = DataConnection.GetConnection())
                {
                    try
                    {
                        SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "Save_MailStatus", param);
                    }
                    catch
                    {

                    }
                }
            }
        }

        public void Save_PNRandBookingStatus(long BookingID, int ProdID, string PNR, int BookingStatus, string Remarks)
        {
            if (BookingID > 0 && ProdID > 0)
            {
                SqlParameter[] param = new SqlParameter[5];

                param[0] = new SqlParameter("@BookingID", SqlDbType.Int);
                param[0].Value = BookingID;
                param[1] = new SqlParameter("@ProdID", SqlDbType.Int);
                param[1].Value = ProdID;
                param[2] = new SqlParameter("@PNR_Confirmation", SqlDbType.VarChar, 50);
                param[2].Value = PNR;
                param[3] = new SqlParameter("@Booking_Status", SqlDbType.Int);
                param[3].Value = BookingStatus;
                param[4] = new SqlParameter("@Booking_Remarks", SqlDbType.VarChar, 500);
                param[4].Value = Remarks;
                using (SqlConnection con = DataConnection.GetConnection())
                {
                    try
                    {
                        SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "Save_PNRandBookingStatus", param);
                    }
                    catch
                    {

                    }
                }
            }
        }
        public void Save_PNRandBookingStatus_V1(long BookingID, int ProdID, string PNR, int BookingStatus, string Remarks)
        {
            if (BookingID > 0 && ProdID > 0)
            {
                SqlParameter[] param = new SqlParameter[5];

                param[0] = new SqlParameter("@BookingID", SqlDbType.Int);
                param[0].Value = BookingID;
                param[1] = new SqlParameter("@ProdID", SqlDbType.Int);
                param[1].Value = ProdID;
                if (!string.IsNullOrEmpty(PNR))
                {
                    param[2] = new SqlParameter("@PNR_Confirmation", SqlDbType.VarChar, 50);
                    param[2].Value = PNR;
                }
                if (BookingStatus > 0)
                {
                    param[3] = new SqlParameter("@Booking_Status", SqlDbType.Int);
                    param[3].Value = BookingStatus;
                }
                if (!string.IsNullOrEmpty(Remarks))
                {
                    param[4] = new SqlParameter("@Booking_Remarks", SqlDbType.VarChar, 500);
                    param[4].Value = Remarks;
                }
                using (SqlConnection con = DataConnection.GetConnection())
                {
                    try
                    {
                        SqlHelper.ExecuteNonQuery(con, CommandType.StoredProcedure, "Save_PNRandBookingStatus_V1", param);
                    }
                    catch
                    {

                    }
                }
            }
        }

        private CardType getCardType(string CardCode, string cardNo)
        {
            if (string.IsNullOrEmpty(CardCode))
            {
                CardCode = creditCardTypeFromNumber(cardNo);
            }
            if (CardCode.ToUpper() == "VI" || CardCode.ToUpper() == "VISA")
            {
                return CardType.Visa;
            }
            else if (CardCode.ToUpper() == "CA" || CardCode.ToUpper() == "MASTER CARD")
            {
                return CardType.MasterCard;
            }
            else if (CardCode.ToUpper() == "AX" || CardCode.ToUpper() == "AMERICAN EXPRESS")
            {
                return CardType.AmericanExpress;
            }
            else if (CardCode.ToUpper() == "DC" || CardCode.ToUpper() == "DINERS CLUB")
            {
                return CardType.DinersClub;
            }
            else if (CardCode.ToUpper() == "DS" || CardCode.ToUpper() == "DISCOVER")
            {
                return CardType.Discover;
            }
            else if (CardCode.ToUpper() == "CB" || CardCode.ToUpper() == "CARTE BLANCHE")
            {
                return CardType.CarteBlanche;
            }
            else if (CardCode.ToUpper() == "TO" || CardCode.ToUpper() == "MAESTRO")
            {
                return CardType.Maestro;
            }
            else if (CardCode.ToUpper() == "BC" || CardCode.ToUpper() == "BC CARD")
            {
                return CardType.BCCard;
            }
            else if (CardCode.ToUpper() == "JC" || CardCode.ToUpper() == "JAPAN CREDIT BUREAU")
            {
                return CardType.JapanCreditBureau;
            }
            else if (CardCode.ToUpper() == "T" || CardCode.ToUpper() == "BC CARD")
            {
                return CardType.CartaSi;
            }
            else if (CardCode.ToUpper() == "R" || CardCode.ToUpper() == "CARTE BLEUE")
            {
                return CardType.CarteBleue;
            }
            else if (CardCode.ToUpper() == "E" || CardCode.ToUpper() == "VISA ELECTRON")
            {
                return CardType.VisaElectron;
            }
            else
            {
                return CardType.None;
            }
        }
        private DataTable getAmountTable(ref FlightBookingRequest fsr, ref decimal TotalAmount)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("BookingID", typeof(int));
            dt.Columns.Add("ProdID", typeof(int));
            dt.Columns.Add("ItinID", typeof(int));
            dt.Columns.Add("ChargeID", typeof(int));
            dt.Columns.Add("ChargesFor", typeof(int));
            dt.Columns.Add("TotalUnit", typeof(int));
            dt.Columns.Add("CostPrice", typeof(decimal));
            dt.Columns.Add("SellPrice", typeof(decimal));
            dt.Columns.Add("ChargesStatus", typeof(int));
            dt.Columns.Add("SupplierID", typeof(int));
            dt.Columns.Add("ChargesRemarks", typeof(string));
            dt.Columns.Add("ModifyBy", typeof(int));
            int itinID = 0;
            foreach (FlightResult fResult in fsr.flightResult)
            {
                string tjID = fsr.PriceID[itinID];
                Fare fare = fResult.Fare;


                TotalAmount += fare.grandTotal;
                var adtFare = fare.fareBreakdown.Where(k => k.PassengerType == PassengerType.Adult).FirstOrDefault();
                var chdFare = fare.fareBreakdown.Where(k => k.PassengerType == PassengerType.Child).FirstOrDefault();
                var infFare = fare.fareBreakdown.Where(k => k.PassengerType == PassengerType.Infant).FirstOrDefault();

                DataRow adtBFare = dt.NewRow();
                adtBFare["BookingID"] = fsr.bookingID;
                adtBFare["ProdID"] = fsr.prodID;
                adtBFare["ItinID"] = itinID;
                adtBFare["ChargeID"] = ChargeID.BaseFare;
                adtBFare["ChargesFor"] = ChargeFor.Adult;
                adtBFare["TotalUnit"] = fsr.adults;
                if (fResult.Fare.gdsType == GdsType.Tbo)/* || fResult.Fare.gdsType == GdsType.SatkarTravel*/
                {
                    adtBFare["CostPrice"] = (adtFare.BaseFare / fsr.adults);
                    adtBFare["SellPrice"] = (adtFare.BaseFare / fsr.adults);
                }
                else
                {
                    adtBFare["CostPrice"] = adtFare.BaseFare;
                    adtBFare["SellPrice"] = adtFare.BaseFare;
                }

                adtBFare["ChargesStatus"] = 0;
                adtBFare["SupplierID"] = 0;
                adtBFare["ChargesRemarks"] = "";
                adtBFare["ModifyBy"] = 1000;
                dt.Rows.Add(adtBFare);

                DataRow adtBTax = dt.NewRow();
                adtBTax["BookingID"] = fsr.bookingID;
                adtBTax["ProdID"] = fsr.prodID;
                adtBTax["ItinID"] = itinID;
                adtBTax["ChargeID"] = ChargeID.Tax;
                adtBTax["ChargesFor"] = ChargeFor.Adult;
                adtBTax["TotalUnit"] = fsr.adults;
                if (fResult.Fare.gdsType == GdsType.Tbo)/* || fResult.Fare.gdsType == GdsType.SatkarTravel*/
                {
                    adtBTax["CostPrice"] = (adtFare.Tax / fsr.adults);
                    adtBTax["SellPrice"] = (adtFare.Tax / fsr.adults);
                }
                else
                {
                    adtBTax["CostPrice"] = adtFare.Tax;
                    adtBTax["SellPrice"] = adtFare.Tax;
                }


                adtBTax["ChargesStatus"] = 0;
                adtBTax["SupplierID"] = 0;
                adtBTax["ChargesRemarks"] = "";
                adtBTax["ModifyBy"] = 1000;
                dt.Rows.Add(adtBTax);

                //DataRow adtBMarkup = dt.NewRow();
                //adtBMarkup["BookingID"] = fsr.bookingID;
                //adtBMarkup["ProdID"] = fsr.prodID;
                //adtBMarkup["ItinID"] = itinID;
                //adtBMarkup["ChargeID"] = ChargeID.Markup;
                //adtBMarkup["ChargesFor"] = ChargeFor.Adult;
                //adtBMarkup["TotalUnit"] = 1;
                //adtBMarkup["CostPrice"] = 0;
                //adtBMarkup["SellPrice"] = adtFare.Markup;
                //adtBMarkup["ChargesStatus"] = 0;
                //adtBMarkup["SupplierID"] = 0;
                //adtBMarkup["ChargesRemarks"] = "";
                //adtBMarkup["ModifyBy"] = 1000;
                //dt.Rows.Add(adtBMarkup);

                if (fsr.child > 0 && chdFare != null)
                {
                    DataRow chdBFare = dt.NewRow();
                    chdBFare["BookingID"] = fsr.bookingID;
                    chdBFare["ProdID"] = fsr.prodID;
                    chdBFare["ItinID"] = itinID;
                    chdBFare["ChargeID"] = ChargeID.BaseFare;
                    chdBFare["ChargesFor"] = ChargeFor.Child;
                    chdBFare["TotalUnit"] = fsr.child;
                    if (fResult.Fare.gdsType == GdsType.Tbo)/* || fResult.Fare.gdsType == GdsType.SatkarTravel*/
                    {
                        chdBFare["CostPrice"] = (chdFare.BaseFare / fsr.child);
                        chdBFare["SellPrice"] = (chdFare.BaseFare / fsr.child);
                    }
                    else
                    {
                        chdBFare["CostPrice"] = chdFare.BaseFare;
                        chdBFare["SellPrice"] = chdFare.BaseFare;
                    }


                    chdBFare["ChargesStatus"] = 0;
                    chdBFare["SupplierID"] = 0;
                    chdBFare["ChargesRemarks"] = "";
                    chdBFare["ModifyBy"] = 1000;
                    dt.Rows.Add(chdBFare);

                    DataRow chdBTax = dt.NewRow();
                    chdBTax["BookingID"] = fsr.bookingID;
                    chdBTax["ProdID"] = fsr.prodID;
                    chdBTax["ItinID"] = itinID;
                    chdBTax["ChargeID"] = ChargeID.Tax;
                    chdBTax["ChargesFor"] = ChargeFor.Child;
                    chdBTax["TotalUnit"] = fsr.child;
                    if (fResult.Fare.gdsType == GdsType.Tbo)/* || fResult.Fare.gdsType == GdsType.SatkarTravel*/
                    {
                        chdBTax["CostPrice"] = (chdFare.Tax / fsr.child);
                        chdBTax["SellPrice"] = (chdFare.Tax / fsr.child);
                    }
                    else
                    {
                        chdBTax["CostPrice"] = chdFare.Tax;
                        chdBTax["SellPrice"] = chdFare.Tax;
                    }

                    chdBTax["ChargesStatus"] = 0;
                    chdBTax["SupplierID"] = 0;
                    chdBTax["ChargesRemarks"] = "";
                    chdBTax["ModifyBy"] = 1000;
                    dt.Rows.Add(chdBTax);

                    //DataRow chdBMarkup = dt.NewRow();
                    //chdBMarkup["BookingID"] = fsr.bookingID;
                    //chdBMarkup["ProdID"] = fsr.prodID;
                    //chdBMarkup["ItinID"] = itinID;
                    //chdBMarkup["ChargeID"] = ChargeID.Markup;
                    //chdBMarkup["ChargesFor"] = ChargeFor.Child;
                    //chdBMarkup["TotalUnit"] = 1;
                    //chdBMarkup["CostPrice"] = 0;
                    //chdBMarkup["SellPrice"] = chdFare.Markup;
                    //chdBMarkup["ChargesStatus"] = 0;
                    //chdBMarkup["SupplierID"] = 0;
                    //chdBMarkup["ChargesRemarks"] = "";
                    //chdBMarkup["ModifyBy"] = 1000;
                    //dt.Rows.Add(chdBMarkup);
                }
                if (fsr.infants > 0 && infFare != null)
                {
                    DataRow infBFare = dt.NewRow();
                    infBFare["BookingID"] = fsr.bookingID;
                    infBFare["ProdID"] = fsr.prodID;
                    infBFare["ItinID"] = itinID;
                    infBFare["ChargeID"] = ChargeID.BaseFare;
                    infBFare["ChargesFor"] = ChargeFor.Infant;
                    infBFare["TotalUnit"] = fsr.infants;
                    if (fResult.Fare.gdsType == GdsType.Tbo)/* || fResult.Fare.gdsType == GdsType.SatkarTravel*/
                    {
                        infBFare["CostPrice"] = (infFare.BaseFare / fsr.infants);
                        infBFare["SellPrice"] = (infFare.BaseFare / fsr.infants);
                    }
                    else
                    {
                        infBFare["CostPrice"] = infFare.BaseFare;
                        infBFare["SellPrice"] = infFare.BaseFare;
                    }
                    infBFare["ChargesStatus"] = 0;
                    infBFare["SupplierID"] = 0;
                    infBFare["ChargesRemarks"] = "";
                    infBFare["ModifyBy"] = 1000;
                    dt.Rows.Add(infBFare);

                    DataRow infBTax = dt.NewRow();
                    infBTax["BookingID"] = fsr.bookingID;
                    infBTax["ProdID"] = fsr.prodID;
                    infBTax["ItinID"] = itinID;
                    infBTax["ChargeID"] = ChargeID.Tax;
                    infBTax["ChargesFor"] = ChargeFor.Infant;
                    infBTax["TotalUnit"] = fsr.infants;
                    if (fResult.Fare.gdsType == GdsType.Tbo)/* || fResult.Fare.gdsType == GdsType.SatkarTravel*/
                    {
                        infBTax["CostPrice"] = (infFare.Tax / fsr.infants);
                        infBTax["SellPrice"] = (infFare.Tax / fsr.infants);
                    }
                    else
                    {
                        infBTax["CostPrice"] = infFare.Tax;
                        infBTax["SellPrice"] = infFare.Tax;
                    }
                    infBTax["ChargesStatus"] = 0;
                    infBTax["SupplierID"] = 0;
                    infBTax["ChargesRemarks"] = "";
                    infBTax["ModifyBy"] = 1000;
                    dt.Rows.Add(infBTax);

                    //DataRow infBMarkup = dt.NewRow();
                    //infBMarkup["BookingID"] = fsr.bookingID;
                    //infBMarkup["ProdID"] = fsr.prodID;
                    //infBMarkup["ItinID"] = itinID;
                    //infBMarkup["ChargeID"] = ChargeID.Markup;
                    //infBMarkup["ChargesFor"] = ChargeFor.Infant;
                    //infBMarkup["TotalUnit"] = fsr.infants;
                    //infBMarkup["CostPrice"] = 0;
                    //infBMarkup["SellPrice"] = infFare.Markup;
                    //infBMarkup["ChargesStatus"] = 0;
                    //infBMarkup["SupplierID"] = 0;
                    //infBMarkup["ChargesRemarks"] = "";
                    //infBMarkup["ModifyBy"] = 1000;
                    //dt.Rows.Add(infBMarkup);
                }
                DataRow Markup = dt.NewRow();
                Markup["BookingID"] = fsr.bookingID;
                Markup["ProdID"] = fsr.prodID;
                Markup["ItinID"] = itinID;
                Markup["ChargeID"] = ChargeID.Markup;
                Markup["ChargesFor"] = ChargeFor.NA;
                Markup["TotalUnit"] = 1;
                Markup["CostPrice"] = 0;
                Markup["SellPrice"] = fare.Markup;
                Markup["ChargesStatus"] = 0;
                Markup["SupplierID"] = 0;
                Markup["ChargesRemarks"] = "";
                Markup["ModifyBy"] = 1000;
                dt.Rows.Add(Markup);

                if (fare.OtherCharges > 0)
                {
                    DataRow othCharge = dt.NewRow();
                    othCharge["BookingID"] = fsr.bookingID;
                    othCharge["ProdID"] = fsr.prodID;
                    othCharge["ItinID"] = itinID;
                    othCharge["ChargeID"] = ChargeID.OtherCharges;
                    othCharge["ChargesFor"] = ChargeFor.NA;
                    othCharge["TotalUnit"] = 1;
                    othCharge["CostPrice"] = fare.OtherCharges;
                    othCharge["SellPrice"] = fare.OtherCharges;
                    othCharge["ChargesStatus"] = 0;
                    othCharge["SupplierID"] = 0;
                    othCharge["ChargesRemarks"] = "";
                    othCharge["ModifyBy"] = 1000;
                    dt.Rows.Add(othCharge);
                }

                // TripJack Case
                //if (fResult.gdsType == Core.GdsType.TripJack)
                //{
                //    if (fare.CommissionEarned > 0)
                //    {
                //        DataRow othCharge = dt.NewRow();
                //        othCharge["BookingID"] = fsr.bookingID;
                //        othCharge["ProdID"] = fsr.prodID;
                //        othCharge["ItinID"] = itinID;
                //        othCharge["ChargeID"] = ChargeID.CommissionEarned;
                //        othCharge["ChargesFor"] = ChargeFor.NA;
                //        othCharge["TotalUnit"] = 1;
                //        othCharge["CostPrice"] = (-1) * fare.CommissionEarned;
                //        othCharge["SellPrice"] = (-1) * fare.CommissionEarned;
                //        //othCharge["CostPrice"] = fare.CommissionEarned;
                //        //othCharge["SellPrice"] = fare.CommissionEarned;
                //        othCharge["ChargesStatus"] = 0;
                //        othCharge["SupplierID"] = 0;
                //        othCharge["ChargesRemarks"] = "";
                //        othCharge["ModifyBy"] = 1000;
                //        dt.Rows.Add(othCharge);
                //    }
                //}
                // Travelogy Case

                //if (fsr.adults > 0)
                //{
                //    fare.ServiceFee = adtFare.ServiceFee;
                //}


                if ( fare.gdsType == Core.GdsType.Travelopedia)/*fResult.gdsType == Core.GdsType.Travelogy ||*/
                {
                    if (fare.ServiceFee > 0)
                    {
                        DataRow ServiceFeeAmt = dt.NewRow();
                        ServiceFeeAmt["BookingID"] = fsr.bookingID;
                        ServiceFeeAmt["ProdID"] = fsr.prodID;
                        ServiceFeeAmt["ItinID"] = itinID;
                        ServiceFeeAmt["ChargeID"] = ChargeID.ServiceFee;
                        ServiceFeeAmt["ChargesFor"] = ChargeFor.NA;
                        ServiceFeeAmt["TotalUnit"] = 1;
                        ServiceFeeAmt["CostPrice"] = fare.ServiceFee;
                        ServiceFeeAmt["SellPrice"] = fare.ServiceFee;
                        ServiceFeeAmt["ChargesStatus"] = 0;
                        ServiceFeeAmt["SupplierID"] = 0;
                        ServiceFeeAmt["ChargesRemarks"] = "";
                        ServiceFeeAmt["ModifyBy"] = 1000;
                        dt.Rows.Add(ServiceFeeAmt);
                    }
                }
                itinID++;
            }
            if (fsr.fareIncreaseAmount > 0)
            {
                DataRow Coupon = dt.NewRow();
                Coupon["BookingID"] = fsr.bookingID;
                Coupon["ProdID"] = fsr.prodID;
                Coupon["ItinID"] = 0;
                Coupon["ChargeID"] = ChargeID.FareIncreaseAmount;
                Coupon["ChargesFor"] = ChargeFor.NA;
                Coupon["TotalUnit"] = 1;
                Coupon["CostPrice"] = 0;
                Coupon["SellPrice"] = fsr.fareIncreaseAmount;
                Coupon["ChargesStatus"] = 0;
                Coupon["SupplierID"] = 0;
                Coupon["ChargesRemarks"] = "";
                Coupon["ModifyBy"] = 1000;
                dt.Rows.Add(Coupon);

                TotalAmount += fsr.fareIncreaseAmount;
            }
            if (!string.IsNullOrEmpty(fsr.CouponCode) && fsr.CouponAmount > 0)
            {
                DataRow Coupon = dt.NewRow();
                Coupon["BookingID"] = fsr.bookingID;
                Coupon["ProdID"] = fsr.prodID;
                Coupon["ItinID"] = 0;
                Coupon["ChargeID"] = ChargeID.Coupon;
                Coupon["ChargesFor"] = ChargeFor.NA;
                Coupon["TotalUnit"] = 1;
                Coupon["CostPrice"] = 0;
                Coupon["SellPrice"] = (-1) * fsr.CouponAmount;
                Coupon["ChargesStatus"] = 0;
                Coupon["SupplierID"] = 0;
                Coupon["ChargesRemarks"] = fsr.CouponCode;
                Coupon["ModifyBy"] = 1000;
                dt.Rows.Add(Coupon);
                TotalAmount = TotalAmount - fsr.CouponAmount;
            }


            //if (fsr.sourceMedia == "1000" || fsr.sourceMedia == "1001" || fsr.sourceMedia == "1004" || fsr.sourceMedia == "1016" || fsr.sourceMedia == "1015")
            //{
            if (!string.IsNullOrEmpty(fsr.CouponCode) && fsr.CouponIncreaseAmount > 0)
            {
                DataRow CouponIncreaseAmount = dt.NewRow();
                CouponIncreaseAmount["BookingID"] = fsr.bookingID;
                CouponIncreaseAmount["ProdID"] = fsr.prodID;
                CouponIncreaseAmount["ItinID"] = 0;
                CouponIncreaseAmount["ChargeID"] = ChargeID.CouponIncreaseAmount;
                CouponIncreaseAmount["ChargesFor"] = ChargeFor.NA;
                CouponIncreaseAmount["TotalUnit"] = 1;
                CouponIncreaseAmount["CostPrice"] = 0;
                CouponIncreaseAmount["SellPrice"] = (1) * fsr.CouponIncreaseAmount;
                CouponIncreaseAmount["ChargesStatus"] = 0;
                CouponIncreaseAmount["SupplierID"] = 0;
                CouponIncreaseAmount["ChargesRemarks"] = 100;
                CouponIncreaseAmount["ModifyBy"] = 1000;
                dt.Rows.Add(CouponIncreaseAmount);
                TotalAmount = TotalAmount + fsr.CouponIncreaseAmount;
            }
            //}




            if (false && fsr.convenienceFee > 0)
            {
                DataRow Coupon = dt.NewRow();
                Coupon["BookingID"] = fsr.bookingID;
                Coupon["ProdID"] = fsr.prodID;
                Coupon["ItinID"] = 0;
                Coupon["ChargeID"] = ChargeID.ConvenienceFee;
                Coupon["ChargesFor"] = ChargeFor.NA;
                Coupon["TotalUnit"] = 1;
                Coupon["CostPrice"] = 0;
                Coupon["SellPrice"] = fsr.convenienceFee;
                Coupon["ChargesStatus"] = 0;
                Coupon["SupplierID"] = 0;
                Coupon["ChargesRemarks"] = "";
                Coupon["ModifyBy"] = 1000;
                dt.Rows.Add(Coupon);

                TotalAmount = TotalAmount + fsr.convenienceFee;
            }

            if (false && fsr.CancellaionPolicyAmt > 0)
            {
                DataRow CPAmt = dt.NewRow();
                CPAmt["BookingID"] = fsr.bookingID;
                CPAmt["ProdID"] = fsr.prodID;
                CPAmt["ItinID"] = 0;
                CPAmt["ChargeID"] = ChargeID.CancellationPolicyAmount;
                CPAmt["ChargesFor"] = ChargeFor.NA;
                CPAmt["TotalUnit"] = 1;
                CPAmt["CostPrice"] = 0;
                CPAmt["SellPrice"] = fsr.CancellaionPolicyAmt;
                CPAmt["ChargesStatus"] = 0;
                CPAmt["SupplierID"] = 0;
                CPAmt["ChargesRemarks"] = "";
                CPAmt["ModifyBy"] = 1000;
                dt.Rows.Add(CPAmt);
                TotalAmount = TotalAmount + fsr.CancellaionPolicyAmt;
            }

            if (false && fsr.RefundPolicyAmt > 0)
            {
                DataRow RPAmt = dt.NewRow();
                RPAmt["BookingID"] = fsr.bookingID;
                RPAmt["ProdID"] = fsr.prodID;
                RPAmt["ItinID"] = 0;
                RPAmt["ChargeID"] = ChargeID.RefundPolicyAmount;
                RPAmt["ChargesFor"] = ChargeFor.NA;
                RPAmt["TotalUnit"] = 1;
                RPAmt["CostPrice"] = 0;
                RPAmt["SellPrice"] = fsr.RefundPolicyAmt;
                RPAmt["ChargesStatus"] = 0;
                RPAmt["SupplierID"] = 0;
                RPAmt["ChargesRemarks"] = "";
                RPAmt["ModifyBy"] = 1000;
                dt.Rows.Add(RPAmt);

                TotalAmount = TotalAmount + fsr.RefundPolicyAmt;
            }



            return dt;
        }
        private DataTable getpassengerTable(ref FlightBookingRequest fsr)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("BookingID", typeof(int));
            dt.Columns.Add("ProdID", typeof(int));
            dt.Columns.Add("PaxID", typeof(int));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Pax_First_Name", typeof(string));
            dt.Columns.Add("Pax_Middle_Name", typeof(string));
            dt.Columns.Add("Pax_Last_Name", typeof(string));
            dt.Columns.Add("Frequent_Flyer_No", typeof(string));
            dt.Columns.Add("Passport_No", typeof(string));
            dt.Columns.Add("Nationality", typeof(string));
            dt.Columns.Add("Expiry_Date", typeof(DateTime));
            dt.Columns.Add("Place_of_Issue", typeof(string));
            dt.Columns.Add("Place_of_Birth", typeof(string));
            dt.Columns.Add("Pax_DOB", typeof(DateTime));
            dt.Columns.Add("Pax_Type", typeof(int));
            dt.Columns.Add("Gender", typeof(int));
            dt.Columns.Add("TicketNo", typeof(string));
            dt.Columns.Add("Meal", typeof(string));
            dt.Columns.Add("Seat", typeof(string));
            dt.Columns.Add("SpecialAssistance", typeof(string));
            dt.Columns.Add("FFMiles", typeof(string));
            dt.Columns.Add("TSA_Precheck", typeof(string));
            dt.Columns.Add("RedressNumber", typeof(string));
            dt.Columns.Add("ModifyBy", typeof(int));

            int i = 1;
            foreach (PassengerDetails pax in fsr.passengerDetails)
            {
                DataRow p = dt.NewRow();
                p["BookingID"] = fsr.bookingID;
                p["ProdID"] = fsr.prodID;
                p["PaxID"] = i++;
                p["Title"] = pax.title;
                p["Pax_First_Name"] = pax.firstName;
                p["Pax_Middle_Name"] = pax.middleName;
                p["Pax_Last_Name"] = pax.lastName;
                p["Frequent_Flyer_No"] = "";
                p["Passport_No"] = pax.passportNumber;
                p["Nationality"] = pax.nationality;
                p["Expiry_Date"] = (pax.expiryDate != null && pax.expiryDate.HasValue) ? pax.expiryDate : new DateTime(1900, 1, 1);
                p["Place_of_Issue"] = pax.issueCountry;
                p["Place_of_Birth"] = "";
                p["Pax_DOB"] = pax.dateOfBirth;
                p["Pax_Type"] = (int)pax.passengerType;
                p["Gender"] = (int)pax.gender;
                p["TicketNo"] = "";
                p["Meal"] = pax.Meal;
                p["Seat"] = pax.Seat;
                p["SpecialAssistance"] = pax.SpecialAssistance;
                p["FFMiles"] = pax.FFMiles;
                p["TSA_Precheck"] = pax.TSA_Precheck;
                p["RedressNumber"] = pax.RedressNumber;
                p["ModifyBy"] = 1000;
                dt.Rows.Add(p);
            }
            return dt;
        }
        private DataTable getpassengerTableWithTickNo(ref FlightBookingResponse fsr)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("BookingID", typeof(int));
            dt.Columns.Add("ProdID", typeof(int));
            dt.Columns.Add("PaxID", typeof(int));
            dt.Columns.Add("TicketNo", typeof(string));

            int i = 1;
            foreach (PassengerDetails pax in fsr.passengerDetails)
            {
                DataRow p = dt.NewRow();
                p["BookingID"] = fsr.bookingID;
                p["ProdID"] = fsr.prodID;
                p["PaxID"] = i++;
                p["TicketNo"] = pax.ticketNo;

                dt.Rows.Add(p);
            }
            return dt;
        }
        private DataTable getSectorTable(ref FlightBookingRequest fsr, ref string eft, ref int outEft, ref int inEft, ref DateTime DepDate, ref DateTime arrDate)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("BookingID", typeof(int));
            dt.Columns.Add("ProdID", typeof(int));
            dt.Columns.Add("ItinID", typeof(int));
            dt.Columns.Add("SegID", typeof(int));
            dt.Columns.Add("TripID", typeof(int));
            dt.Columns.Add("Airline", typeof(string));
            dt.Columns.Add("OperatedBy", typeof(string));
            dt.Columns.Add("From_Destination", typeof(string));
            dt.Columns.Add("From_Date_Time", typeof(DateTime));
            dt.Columns.Add("To_Destination", typeof(string));
            dt.Columns.Add("To_Date_Time", typeof(DateTime));
            dt.Columns.Add("Flight_No", typeof(string));
            dt.Columns.Add("EqupmentType", typeof(string));
            dt.Columns.Add("CabinClass", typeof(int));
            dt.Columns.Add("AClass", typeof(string));
            dt.Columns.Add("Terminal_From", typeof(string));
            dt.Columns.Add("Terminal_To", typeof(string));
            dt.Columns.Add("Eft", typeof(int));
            dt.Columns.Add("Seg_Remarks", typeof(string));
            dt.Columns.Add("ModifyBy", typeof(int));

            int itinID = 0, j = 1;
            foreach (FlightResult fResult in fsr.flightResult)
            {

                foreach (FlightSegment fSeg in fResult.FlightSegments)
                {
                    if (itinID == 0 && j == 1)
                    {
                        outEft = fSeg.Duration;
                        DepDate = fSeg.Segments[0].DepTime;
                    }
                    else
                    {
                        inEft = fSeg.Duration;
                        arrDate = fSeg.Segments[0].DepTime;
                    }
                    eft += (string.IsNullOrEmpty(eft) ? fSeg.Duration.ToString() : ("-" + fSeg.Duration.ToString()));
                    int i = 1;
                    foreach (var seg in fSeg.Segments)
                    {
                        DataRow segment = dt.NewRow();
                        segment["BookingID"] = fsr.bookingID;
                        segment["ProdID"] = fsr.prodID;
                        segment["ItinID"] = itinID;
                        segment["SegID"] = i++;
                        segment["TripID"] = j;
                        segment["Airline"] = seg.Airline;
                        segment["OperatedBy"] = seg.OperatingCarrier;
                        segment["From_Destination"] = seg.Origin;
                        segment["From_Date_Time"] = seg.DepTime;
                        segment["To_Destination"] = seg.Destination;
                        segment["To_Date_Time"] = seg.ArrTime;

                        //if (seg.FlightNumber.Length > 3)
                        //{
                        //    segment["Seg_Remarks"]= seg.FlightNumber;
                        //    segment["Flight_No"] = "";
                        //}
                        //else
                        //{
                        //    segment["Seg_Remarks"] = "";
                        //    segment["Flight_No"] = seg.FlightNumber;
                        //}

                        segment["Flight_No"] = seg.FlightNumber;

                        segment["EqupmentType"] = seg.equipmentType;
                        segment["CabinClass"] = (int)seg.CabinClass;
                        //if (fsr.sourceMedia == "1000" || fsr.sourceMedia == "1016")
                        //{
                        segment["AClass"] = seg.FareClass;
                        //}
                        //else
                        //{
                        //    if ((int)seg.CabinClass == 1)
                        //    {
                        //        //
                        //        segment["AClass"] = "Y";
                        //    }
                        //    if ((int)seg.CabinClass == 2)
                        //    {
                        //        segment["AClass"] = "M";
                        //    }
                        //    if ((int)seg.CabinClass == 3)
                        //    {
                        //        segment["AClass"] = "C";
                        //    }
                        //    if ((int)seg.CabinClass == 4)
                        //    {
                        //        segment["AClass"] = "F";
                        //    }
                        //}
                        segment["Terminal_From"] = seg.FromTerminal;
                        segment["Terminal_To"] = seg.ToTerminal;
                        segment["Eft"] = seg.Duration;
                        segment["Seg_Remarks"] = "";
                        segment["ModifyBy"] = 1000;
                        dt.Rows.Add(segment);
                    }
                    j++;
                }
                itinID++;
            }
            return dt;
        }

        private string creditCardTypeFromNumber(string num)
        {
            if (num.Substring(0, 1) == "4")
            {
                return "VI";
            }
            else if (num.Length >= 2 && (num.Substring(0, 2) == "34" || num.Substring(0, 2) == "37"))
            {
                return "AX";
            }
            else if (num.Substring(0, 1) == "5")
            {
                return "CA";
            }
            else if (num.Substring(0, 1) == "6")
            {
                return "DS";
            }
            else if (num.Length >= 2 && (num.Substring(0, 2) == "35"))
            {
                return "JC";
            }
            else if (num.Length >= 2 && (num.Substring(0, 2) == "36" || num.Substring(0, 2) == "38"))
            {
                return "DC";
            }
            else
            {
                return "None";
            }
        }
    }
}
