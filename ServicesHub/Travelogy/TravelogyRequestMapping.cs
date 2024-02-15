using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelogy
{

    public class TravelogyRequestMapping
    {
        public static string TravelogyUserId = ConfigurationManager.AppSettings["TravelogyUserId"].ToString();
        public static string TravelogyPassword = ConfigurationManager.AppSettings["TravelogyPassword"].ToString();
        public static string TravelogyIMEI_Number = ConfigurationManager.AppSettings["TravelogyIMEI_Number"].ToString();
        //public static string TravelogySearchUrl = ConfigurationManager.AppSettings["TravelogySearchUrl"].ToString();
        public string getFlightSearchRequest(Core.Flight.FlightSearchRequest fsr)
        {
            Travelogy.TravelogyClass.TravelogyFlightSearchRequest request = new TravelogyClass.TravelogyFlightSearchRequest()
            {
                Adult_Count = fsr.adults.ToString(),
                Child_Count = fsr.child.ToString(),
                Infant_Count = fsr.infants.ToString(),
                Auth_Header = new TravelogyClass.AuthHeader() { UserId = TravelogyUserId, Password = TravelogyPassword, IP_Address = fsr.userIP, Request_Id = fsr.tgy_Request_id, IMEI_Number = TravelogyIMEI_Number },
                Booking_Type = fsr.tripType == Core.TripType.OneWay ? 0 : 1,
                Travel_Type = fsr.travelType == Core.TravelType.Domestic ? 0 : 1,
                Class_Of_Travel = getCabinType(fsr.cabinType),
                Filtered_Airline = new List<TravelogyClass.FilteredAirline>(),
                InventoryType = 0,
                TripInfo = new List<TravelogyClass.TripInfo>()
            };

            request.Filtered_Airline.Add(new TravelogyClass.FilteredAirline { Airline_Code = "" });

            for (int i = 0; i < fsr.segment.Count; i++)
            {
                request.TripInfo.Add(new TravelogyClass.TripInfo() { Origin = fsr.segment[i].originAirport, Destination = fsr.segment[i].destinationAirport, Trip_Id = i, TravelDate = fsr.segment[i].travelDate.ToString("MM/dd/yyyy") });
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(request);
        }

        public string getFlightReviewRequest(Core.Flight.PriceVerificationRequest pvr)
        {
            Travelogy.TravelogyClass.RePrice.TravelogyRePriceRequest request = new TravelogyClass.RePrice.TravelogyRePriceRequest()
            {
                Auth_Header = new TravelogyClass.RePrice.AuthHeader() { UserId = TravelogyUserId, Password = TravelogyPassword, IP_Address = pvr.userIP, Request_Id = pvr.tgy_Request_id, IMEI_Number = TravelogyIMEI_Number },
                Search_Key = pvr.tgy_Search_Key,
                Customer_Mobile = pvr.PhoneNo,
                GST_Input = false,
                SinglePricing = true,
                AirRepriceRequests = new List<TravelogyClass.RePrice.AirRepriceRequest>()
            };
            foreach (var fresult in pvr.flightResult)
            {
                TravelogyClass.RePrice.AirRepriceRequest obj = new TravelogyClass.RePrice.AirRepriceRequest();
                obj.Flight_Key = fresult.tgy_Flight_Key;
                obj.Fare_Id = fresult.Fare.Tgy_FareID;
                request.AirRepriceRequests.Add(obj);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(request);
        }
        //public string getFlightTemp_BookingRequest(Core.Flight.FlightBookingRequest fbr)
        //{
        //    Travelogy.TravelogyClass.BookingRequest.TravelogyFlightBookingRequest request = new TravelogyClass.BookingRequest.TravelogyFlightBookingRequest()
        //    {
        //        bookingId = fbr.TjBookingID,
        //        deliveryInfo = new TravelogyClass.BookingRequest.DeliveryInfo() { contacts = new List<string>(), emails = new List<string>() },
        //        gstInfo = new TravelogyClass.BookingRequest.GstInfo(),// { gstNumber = "07AACCF6706H1ZQ", registeredName = "FLIGHTS MOJO BOOKINGS PRIVATE LIMITED", address = "Plot No 83 Sector-28 Gurugram Haryana 122001", email = "SUPPORT@FLIGHTSMOJO.IN", mobile = "9876543265" },
        //        paymentInfos = new List<TravelogyClass.BookingRequest.PaymentInfo>(),
        //        travellerInfo = new List<TravelogyClass.BookingRequest.TravellerInfo>()
        //    };
        //    if (!string.IsNullOrEmpty(fbr.GSTNo) && !string.IsNullOrEmpty(fbr.GSTCompany))
        //    {
        //        request.gstInfo.gstNumber = fbr.GSTNo;
        //        request.gstInfo.registeredName = fbr.GSTCompany;
        //        request.gstInfo.address = fbr.GSTCompany;
        //        request.gstInfo.mobile = fbr.phoneNo;
        //        request.gstInfo.email = fbr.emailID;
        //    }

        //    //request.deliveryInfo.emails.Add(fbr.emailID);
        //    request.deliveryInfo.emails.Add("flightsmojollc@gmail.com");
        //    request.deliveryInfo.contacts.Add("+91" + fbr.phoneNo);
        //    TravelogyClass.BookingRequest.PaymentInfo pi = new TravelogyClass.BookingRequest.PaymentInfo() { amount = fbr.VerifiedTotalPrice };
        //    request.paymentInfos.Add(pi);
        //    foreach (Core.PassengerDetails pax in fbr.passengerDetails)
        //    {
        //        TravelogyClass.BookingRequest.TravellerInfo tinfo = new TravelogyClass.BookingRequest.TravellerInfo { dob = pax.dateOfBirth.ToString("yyyy-MM-dd"), fN = pax.firstName, lN = pax.lastName, ti = pax.title, pt = getPaxType(pax.passengerType) };
        //        request.travellerInfo.Add(tinfo);
        //    }
        //    return Newtonsoft.Json.JsonConvert.SerializeObject(request);
        //}

        public string getFlightTemp_BookingRequest(Core.Flight.FlightBookingRequest fbr)
        {
            try
            {

            ServicesHub.Travelogy.TravelogyClass.TempBookingRequest.Travelogy_TempBookingRequest request = new TravelogyClass.TempBookingRequest.Travelogy_TempBookingRequest()
            {
                Customer_Mobile = fbr.phoneNo,
                WhatsAPP_Mobile = fbr.phoneNo,
                GST_Number = fbr.GSTNo,
                GST_HolderName = fbr.GSTCompany,
                GST = fbr.isGST,
                GST_Address = fbr.GSTAddress,
                Auth_Header = new TravelogyClass.TempBookingRequest.AuthHeader() { UserId = TravelogyUserId, Password = TravelogyPassword, IP_Address = fbr.userIP, Request_Id = fbr.tgy_Request_id, IMEI_Number = TravelogyIMEI_Number },
                BookingAlertIds = "",
                BookingFlightDetails = new List<TravelogyClass.TempBookingRequest.BookingFlightDetail>(),
                BookingRemark = "",
                CorporatePaymentMode = 0,
                CorporateStatus = 0,
                CorpTripSubType = "",
                CorpTripType = "",
                CostCenterId = 0,
               
                MissedSavingReason = "",
                Passenger_Email = fbr.emailID,
                Passenger_Mobile = fbr.phoneNo,
                ProjectId = 0,
                TripRequestId = "",
                PAX_Details = new List<TravelogyClass.TempBookingRequest.PAXDetail>(),
            };
            int ctr = 1;
            foreach (var pax in fbr.passengerDetails)
            {
                TravelogyClass.TempBookingRequest.PAXDetail paxDtl = new TravelogyClass.TempBookingRequest.PAXDetail()
                {
                    First_Name = pax.firstName,
                    Last_Name = pax.lastName,
                    Nationality = pax.nationality,
                    Gender = pax.gender == Core.Gender.Male ? 0 : 1,
                    Title = pax.title,
                    Pax_type = getPaxTypeInt(pax.passengerType),
                    Pax_Id = ctr++,
                    DOB= (pax.passengerType==Core.PassengerType.Child || pax.passengerType == Core.PassengerType.Infant?pax.dateOfBirth.ToString("MM/dd/yyyy"):"")
                };
                request.PAX_Details.Add(paxDtl);
            }
            int ctrFkey = 0;
            foreach (var flt in fbr.flightResult)
            {
                TravelogyClass.TempBookingRequest.BookingFlightDetail fltDtl = new TravelogyClass.TempBookingRequest.BookingFlightDetail()
                {
                    Flight_Key = fbr.tgy_Flight_Key[ctrFkey++],
                    Search_Key = fbr.tgy_Search_Key,
                    BookingSSRDetails = new List<string>()

                };
                request.BookingFlightDetails.Add(fltDtl);
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(request);

            }
            catch (Exception ex)
            {
                ex.ToString();

                throw;
            }
        }

        public string getFlight_Ticketing(Core.Flight.FlightBookingRequest fbr)
        {
            ServicesHub.Travelogy.TravelogyClass.BookingDetails.TravelogyBookingDetailsRequest request = new ServicesHub.Travelogy.TravelogyClass.BookingDetails.TravelogyBookingDetailsRequest()
            {
                Booking_RefNo = fbr.tgy_Booking_RefNo,
                //Ticketing_Type = fbr.tgy_Block_Ticket_Allowed[0] ? "1" : "0",
                Ticketing_Type = "1",
                Auth_Header = new TravelogyClass.BookingDetails.AuthHeader() { UserId = TravelogyUserId, Password = TravelogyPassword, IP_Address = fbr.userIP, Request_Id = fbr.tgy_Request_id, IMEI_Number = TravelogyIMEI_Number }
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(request);

        }



        public string getFlight_TicketingInvoice(Core.Flight.FlightBookingRequest fbr)
        {
            ServicesHub.Travelogy.TravelogyClass.Ticket_Invoice.TravelogyTicketInvoiceRequest request = new ServicesHub.Travelogy.TravelogyClass.Ticket_Invoice.TravelogyTicketInvoiceRequest()
            {
                Booking_RefNo = fbr.tgy_Booking_RefNo,
                Auth_Header = new TravelogyClass.Ticket_Invoice.AuthHeader() { UserId = TravelogyUserId, Password = TravelogyPassword, IP_Address = fbr.userIP, Request_Id = fbr.tgy_Request_id, IMEI_Number = TravelogyIMEI_Number }
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(request);
        }







        public string getAddPayment(Core.Flight.FlightBookingRequest fbr)
        {
            ServicesHub.Travelogy.TravelogyClass.AddPayment.AddPaymentRequest request = new ServicesHub.Travelogy.TravelogyClass.AddPayment.AddPaymentRequest()
            {
                Auth_Header = new TravelogyClass.AddPayment.AuthHeader() { UserId = TravelogyUserId, Password = TravelogyPassword, IP_Address = fbr.userIP, Request_Id = fbr.tgy_Request_id, IMEI_Number = TravelogyIMEI_Number },
                ClientRefNo = "Testing Team",
                RefNo = fbr.tgy_Booking_RefNo,
                TransactionType = 0,
                ProductId = "1"
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(request);

        }
        public string getCabinType(Core.CabinType ct)
        {
            string CabinName = "0";
            if (ct == Core.CabinType.Economy)
            {
                CabinName = "0";
            }
            else if (ct == Core.CabinType.PremiumEconomy)
            {
                CabinName = "3";
            }
            else if (ct == Core.CabinType.Business)
            {
                CabinName = "1";
            }
            else if (ct == Core.CabinType.First)
            {
                CabinName = "2";
            }
            else
            {
                CabinName = "0";
            }
            return CabinName;
        }
        public string getPaxType(Core.PassengerType ct)
        {
            string paxtype = string.Empty;
            if (ct == Core.PassengerType.Adult)
            {
                paxtype = "ADULT";
            }
            else if (ct == Core.PassengerType.Child)
            {
                paxtype = "CHILD";
            }
            else if (ct == Core.PassengerType.Infant)
            {
                paxtype = "INFANT";
            }

            return paxtype;
        }
        public int getPaxTypeInt(Core.PassengerType ct)
        {

            if (ct == Core.PassengerType.Adult)
            {
                return 0;
            }
            else if (ct == Core.PassengerType.Child)
            {
                return 1;
            }
            else if (ct == Core.PassengerType.Infant)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
    }
}
