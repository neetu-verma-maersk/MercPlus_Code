using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Collections.Generic;

namespace MercPlusClient.UtilityClass
{
    public class MercApplicationContant
    {
        public static Dictionary<string, string> GetSTATUSByReviewEstimate(string LoginType)
        {
            Dictionary<string, string> rsQueryStatus = new Dictionary<string, string>();

            if (LoginType == "EMR_APPROVER_COUNTRY" || LoginType == "EMR_APPROVER_SHOP")
            {
                rsQueryStatus.Add("130,200,310,320", "Daily"); // leo
                rsQueryStatus.Add("200", "Pending");
                rsQueryStatus.Add("-390", "Working");
                rsQueryStatus.Add("100,130", "Rejected");
                rsQueryStatus.Add("150", "Total Loss");

                // rsQueryStatus.Add("310,320", "Suspended");
                rsQueryStatus.Add("310", "Suspended To Approver");
                rsQueryStatus.Add("320", "Suspended to Approver then forward to Specialist");
                rsQueryStatus.Add("340", "Suspend to CENEQULOUS /CPH");
                rsQueryStatus.Add("330", "Suspend to EMR Specialist");
                rsQueryStatus.Add("390", "Approved");
                rsQueryStatus.Add("400,500,550,600,900", "Completed");
                rsQueryStatus.Add("500", "RRIS Transmitted");
                rsQueryStatus.Add("550", "RRIS Rejected");
                rsQueryStatus.Add("600", "RRIS Accepted");
                rsQueryStatus.Add("9999", "Deleted");
                rsQueryStatus.Add("100,130,140,150,200,310,320,330,340,390,400,500,550,600,900,9999", "ALL");
            }


            else if (LoginType == "CPH")
            {
                rsQueryStatus.Add("100,130,140,150,200,310,320,330,340,390,400,500,550,600,900,9999,-9000", "ALL"); //leo
                rsQueryStatus.Add("340", "Suspend to CENEQULOS /CPH");

                rsQueryStatus.Add("330", "Suspend to EMR Specialist");

                rsQueryStatus.Add("310", "Suspended To Approver");
                rsQueryStatus.Add("320", "Suspended to Approver then forward to Specialist");
                rsQueryStatus.Add("-390", "Working");

                rsQueryStatus.Add("100,130,140", "Rejected");
                rsQueryStatus.Add("200", "Pending");
                //rsQueryStatus.Add("310,320,330,340", "Suspended");  //pinaki

                rsQueryStatus.Add("150", "Total Loss");
                rsQueryStatus.Add("390", "Approved");
                rsQueryStatus.Add("400,500,550,600,900", "Completed");
                rsQueryStatus.Add("500", "RRIS Transmitted");
                rsQueryStatus.Add("550", "RRIS Rejected");
                rsQueryStatus.Add("600", "RRIS Accepted");
                rsQueryStatus.Add("9999", "Deleted");

            }
            else if (LoginType == "EMR_SPECIALIST_COUNTRY" || LoginType == "EMR_SPECIALIST_SHOP")
            {
                rsQueryStatus.Add("330", "Suspend to EMR Specialist");
                rsQueryStatus.Add("340", "Suspend to CENEQULOS /CPH");
                rsQueryStatus.Add("310", "Suspended To Approver");
                rsQueryStatus.Add("320", "Suspended to Approver then forward to Specialist");
                rsQueryStatus.Add("-390", "Working");
                rsQueryStatus.Add("100,130,140", "Rejected");
                //  rsQueryStatus.Add("140", "Rejected to Specialist"); //Bug_fix_Debadrita
                rsQueryStatus.Add("200", "Pending");
                // rsQueryStatus.Add("330,340,310,320", "Suspended"); //Bug_fix_Debadrita

                rsQueryStatus.Add("150", "Total Loss");
                rsQueryStatus.Add("390", "Approved");
                rsQueryStatus.Add("400,500,550,600,900", "Completed");
                rsQueryStatus.Add("500", "RRIS Transmitted");
                rsQueryStatus.Add("550", "RRIS Rejected");
                rsQueryStatus.Add("600", "RRIS Accepted");
                rsQueryStatus.Add("9999", "Deleted");
                rsQueryStatus.Add("100,130,140,150,200,310,320,330,340,390,400,500,550,600,900,9999,-9001", "ALL");
            }
            else if (LoginType == "ADMIN")
            {
                rsQueryStatus.Add("100,130,140,150,200,310,320,330,340,390,400,500,550,600,900,9999", "ALL");
                rsQueryStatus.Add("000", "Draft");
                rsQueryStatus.Add("-390", "Working");
                rsQueryStatus.Add("100,130,140", "Rejected");
                rsQueryStatus.Add("200", "Pending");
                //rsQueryStatus.Add("310,320,330,340", "Suspended");
                rsQueryStatus.Add("330", "Suspend to EMR Specialist");
                rsQueryStatus.Add("340", "Suspend to CENEQULOS /CPH");
                rsQueryStatus.Add("310", "Suspended To Approver");
                rsQueryStatus.Add("320", "Suspended to Approver then forward to Specialist");
                rsQueryStatus.Add("150", "Total Loss");
                rsQueryStatus.Add("390", "Approved");
                rsQueryStatus.Add("400,500,550,600,900", "Completed");
                rsQueryStatus.Add("500", "RRIS Transmitted");
                rsQueryStatus.Add("550", "RRIS Rejected");
                rsQueryStatus.Add("600", "RRIS Accepted");
                rsQueryStatus.Add("9999", "Deleted");
            }

            else if (LoginType == "SHOP")
            {
                rsQueryStatus.Add("390", "Approved");
                rsQueryStatus.Add("100", "Rejected");
                rsQueryStatus.Add("000,100,390", "Daily");
                rsQueryStatus.Add("-390", "Working");
                rsQueryStatus.Add("130,140,200,310,320,330,340", "Submitted");
                rsQueryStatus.Add("400,500,550,600,900", "Completed");
                rsQueryStatus.Add("9999", "Deleted");
                rsQueryStatus.Add("150", "Total Loss");
                rsQueryStatus.Add("000", "Draft");
                // rsQueryStatus.Add("330", "Suspend to EMR Specialist");//Shreya suspend query type removal
                //rsQueryStatus.Add("340", "Suspend to CENEQULOS /CPH");
                //rsQueryStatus.Add("310", "Suspended To Approver");
                //rsQueryStatus.Add("320", "Suspended to Approver then forward to Specialist");
                rsQueryStatus.Add("000,100,130,140,150,200,310,320,330,340,390,400,500,550,600,900,9999", "ALL");
            }
            //else if (LoginType == "VENDOR")
            //{
            //    rsQueryStatus.Add("000,100,130,140,150,200,310,320,330,340,390,400,500,550,600,900,9999", "ALL");
            //    rsQueryStatus.Add("000,100,390", "Daily");
            //    rsQueryStatus.Add("000", "Draft");
            //    rsQueryStatus.Add("-390", "Working");
            //    rsQueryStatus.Add("100", "Rejected");
            //    rsQueryStatus.Add("130,140,200,310,320,330,340", "Submitted");
            //    rsQueryStatus.Add("150", "Total Loss");
            //    rsQueryStatus.Add("390", "Approved");
            //    rsQueryStatus.Add("400,500,550,600,900", "Completed");
            //    rsQueryStatus.Add("500", "RRIS Transmitted");
            //    rsQueryStatus.Add("550", "RRIS Rejected");
            //    rsQueryStatus.Add("600", "RRIS Accepted");
            //    rsQueryStatus.Add("9999", "Deleted");
            //}
            else if (LoginType == "MPRO_CLUSTER" || LoginType == "MPRO_SHOP" || LoginType == "READONLY")
            {
                rsQueryStatus.Add("000,100,130,140,150,200,310,320,330,340,390,400,500,550,600,900,9999", "ALL");
                rsQueryStatus.Add("000,100,390", "Daily");
                rsQueryStatus.Add("000", "Draft");
                rsQueryStatus.Add("-390", "Working");
                rsQueryStatus.Add("100", "Rejected");
                rsQueryStatus.Add("200", "Pending");//shreya bug fix
                rsQueryStatus.Add("130,140,200,310,320,330,340", "Submitted");
                rsQueryStatus.Add("330", "Suspend to EMR Specialist");
                rsQueryStatus.Add("340", "Suspend to CENEQULOS /CPH");
                rsQueryStatus.Add("310", "Suspended To Approver");
                rsQueryStatus.Add("320", "Suspended to Approver then forward to Specialist");
                rsQueryStatus.Add("150", "Total Loss");//Shreya bug fix
                rsQueryStatus.Add("390", "Approved");
                rsQueryStatus.Add("400,500,550,600,900", "Completed");
                rsQueryStatus.Add("500", "RRIS Transmitted");
                rsQueryStatus.Add("550", "RRIS Rejected");
                rsQueryStatus.Add("600", "RRIS Accepted");
                rsQueryStatus.Add("9999", "Deleted");
            }

            return rsQueryStatus;


        }
        /// <summary>
        /// Work Order Enum Constant for Query Type  and Sort By
        /// </summary>
        /// <param Name>Afroz Khan></param>
        /// <param Created On>01-Sep-2015</param>
        /// <param Topic">Work Order</param>
        /// <returns>Enum</returns>
        /// 
        public enum SortType
        {
            [Description("")]
            noreq = 0,
            [Description("Amount Only")]
            amount = 1,
            [Description("Equipment Number Only")]
            eqptno = 2,
            [Description("Total Hours Only")]
            tothrs = 3,
            [Description("Size/Type Only")]
            mode = 4,
            [Description("Mode Only")]
            sizetype = 5,
            [Description("Vendor Ref. No Only")]
            vendorrefno = 6,
        };

        //public enum QueryTypeMSL
        //{
        // [Description("Daily"), AssemblyKeyFile("130,200,310,320")]
        // DailyMgr = 1,
        ////[Description("Daily CENEQULOS")]
        // //DailyCNQ = 340,
        // [Description("Working"),AssemblyKeyFile("-390")]
        // Working=2,
        // [Description("Rejected"), AssemblyKeyFile("100,130")]
        // Rejected = 3,
        //[Description("Pending")]
        //Pending=200,
        //[Description("Suspended")]
        //Suspended = 310,
        //[Description("Total Loss")]
        //TotalLoss = 150,
        //[Description("Approved")]
        //Approved = 390,
        //[Description("Completed")]
        //Completed = 400,
        //[Description("RRIS Transmitted")]
        //RRISTransmitted = 500,
        //[Description("RRIS Rejected")]
        //RRISRejected = 550,
        //[Description("RRIS Accepted")]
        //RRISAccepted = 600,
        //[Description("Deleted")]
        //Deleted = 9999,
        //};
        //public enum QueryTypeCountry
        //{
        // [Description("Daily")]
        // DailyMgr = "130,200,310,320",
        //[Description("Daily CENEQULOS")]
        //DailyCNQ = 340,
        // [Description("Working")]
        // Working="-390",
        // [Description("Rejected")]
        // Rejected="100,130",
        //[Description("Pending")]
        //Pending=200,
        //[Description("Suspended")]
        //Suspended = 310,
        //[Description("Total Loss")]
        //TotalLoss = 150,
        //[Description("Approved")]
        //Approved = 390,
        //[Description("Completed")]
        //Completed = 400,
        //[Description("RRIS Transmitted")]
        //RRISTransmitted = 500,
        //[Description("RRIS Rejected")]
        //RRISRejected = 550,
        //[Description("RRIS Accepted")]
        //RRISAccepted = 600,
        //[Description("Deleted")]
        //Deleted = 9999,
        //};

        //public enum QueryTypeAdmin
        //{

        //[Description("Draft")]
        //Draft = 0,
        //[Description("Working")]
        //Working = 50,
        //[Description("Rejected")]
        //Rejected =100,130,140,
        //[Description("Pending")]
        //Pending = 200,
        //[Description("Suspended")]
        //Suspended = 310,
        //[Description("Total Loss")]
        //TotalLoss = 150,
        //[Description("Approved")]
        //Approved = 390,
        //[Description("Completed")]
        //Completed = 400,
        //[Description("RRIS Transmitted")]
        //RRISTransmitted = 500,
        //[Description("RRIS Rejected")]
        //RRISRejected = 550,
        //[Description("RRIS Accepted")]
        //RRISAccepted = 600,
        //[Description("Deleted")]
        //Deleted = 9999,
        //};
        //public enum QueryTypeArea
        //{
        //[Description("Daily EMR Mng")]
        //DailyMgr = "330",
        ////[Description("Working")]
        ////Working = 50,
        //[Description("Rejected")]
        //Rejected = "130",
        //[Description("Pending")]
        //Pending = 200,
        //[Description("Suspended")]
        //Suspended = 310,
        //[Description("Total Loss")]
        //TotalLoss = 150,
        //[Description("Approved")]
        //Approved = 390,
        //[Description("Completed")]
        //Completed = 400,
        //[Description("RRIS Transmitted")]
        //RRISTransmitted = 500,
        //[Description("RRIS Rejected")]
        //RRISRejected = 550,
        //[Description("RRIS Accepted")]
        //RRISAccepted = 600,
        //[Description("Deleted")]
        //Deleted = 9999,
        //};
        //public enum QueryTypeCPH
        //{
        //[Description("Daily EMR Mng")]
        //DailyMgr = "330",
        //[Description("Daily CENEQUSAL")]
        //DailyMgr = "340",
        ////[Description("Working")]
        ////Working = 50,
        //[Description("Rejected")]
        //Rejected = 130,
        //[Description("Pending")]
        //Pending = 200,
        //[Description("Suspended")]
        //Suspended = 310,
        //[Description("Total Loss")]
        //TotalLoss = 150,
        //[Description("Approved")]
        //Approved = 390,
        //[Description("Completed")]
        //Completed = 400,
        //[Description("RRIS Transmitted")]
        //RRISTransmitted = 500,
        //[Description("RRIS Rejected")]
        //RRISRejected = 550,
        //[Description("RRIS Accepted")]
        //RRISAccepted = 600,
        //[Description("Deleted")]
        //Deleted = 9999,
        //};
        //public enum QueryTypeThirdParty
        //{
        //[Description("Daily")]
        //Daily = "000,100,390",
        ////[Description("Draft")]
        ////Draft = 0,
        //[Description("Working")]
        //Working="-390",
        //[Description("Rejected")]
        //Rejected = "100",
        //[Description("Under Review")]
        //Pending = 200,
        //[Description("Total Loss")]
        //TotalLoss = 150,
        //[Description("Approved")]
        //Approved = 390,
        //[Description("Completed")]
        //Completed = 400,
        //[Description("Deleted")]
        //Deleted = 9999,
        //};
        //public enum QueryTypeVendor
        //{
        //[Description("Daily")]
        //Daily = "000,100,390",
        ////[Description("Draft")]
        ////Draft = 0,
        //[Description("Working")]
        //Working="-390",
        //[Description("Rejected")]
        //Rejected = "100",
        //[Description("Under Review")]
        //Pending = 200,
        //[Description("Total Loss")]
        //TotalLoss = 150,
        //[Description("Approved")]
        //Approved = 390,
        //[Description("Completed")]
        //Completed = 400,
        //[Description("RRIS Transmitted")]
        //RRISTransmitted = 500,
        //[Description("RRIS Rejected")]
        //RRISRejected = 550,
        //[Description("RRIS Accepted")]
        //RRISAccepted = 600,
        //[Description("Deleted")]
        //Deleted = 9999,
        //};
        //public enum QueryTypeShop
        //{
        //[Description("Daily")]
        //Daily = "000,100,390",
        ////[Description("Draft")]
        ////Draft = 0,
        //[Description("Working")]
        //Working = "-390",
        //[Description("Rejected")]
        //Rejected = "100",
        //[Description("Under Review")]
        //Pending = 200,
        //[Description("Total Loss")]
        //TotalLoss = 150,
        //[Description("Approved")]
        //Approved = 390,
        //[Description("Completed")]
        //Completed = 400,
        //[Description("RRIS Transmitted")]
        //RRISTransmitted = 500,
        //[Description("RRIS Rejected")]
        //RRISRejected = 550,
        //[Description("RRIS Accepted")]
        //RRISAccepted = 600,
        //[Description("Deleted")]
        //Deleted = 9999,
        // };

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

    }
}