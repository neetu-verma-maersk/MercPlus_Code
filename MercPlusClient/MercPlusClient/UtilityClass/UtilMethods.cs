using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Xml.Linq;
using MercPlusClient.ManageWorkOrderServiceReference;
using System.Text;

namespace MercPlusClient.UtilityClass
{
    public static class UtilMethods
    {
        public static string[] SPLITBYPIPE = { "|*|" };
        public static char[] SPLITBYCOMMA = { ',' };
        public static decimal? ApprovalAmt = 0;

        /// <summary>
        /// USERTYPE ENUM
        /// </summary>
        /// <returns></returns>

        public enum USERROLE
        {
            ADMIN,
            CPH,
            EMR_SPECIALIST_COUNTRY,
            EMR_SPECIALIST_SHOP,
            EMR_APPROVER_COUNTRY,
            EMR_APPROVER_SHOP,
            SHOP,
            MPRO_CLUSTER,
            MPRO_SHOP,
            READONLY
        }



        public static List<SelectListItem> GetCauseItems()
        {
            List<SelectListItem> CauseItems = new List<SelectListItem>();

            CauseItems.Add(new SelectListItem
            {
                Text = "1 - Wear and Tear",
                Value = "1"
            });
            CauseItems.Add(new SelectListItem
            {
                Text = "2 - Handling Damage",
                Value = "2"
            });
            CauseItems.Add(new SelectListItem
            {
                Text = "3 - Confirmed 3rd Party",
                Value = "3"
            });
            CauseItems.Add(new SelectListItem
            {
                Text = "4 - Unconfirmed 3rd Party",
                Value = "4"
            });

            return CauseItems;
        }

        public static List<SelectListItem> GetTPIItems() //Debadrita_TPI_Indicator-19-07-19
        {
            List<SelectListItem> TPIItems = new List<SelectListItem>();

            TPIItems.Add(new SelectListItem
            {
                //Text = "O",
                Text = "O - Owner",
                Value = "1"
            });
            TPIItems.Add(new SelectListItem
            {
                Text = "T - Third Party",
                //Text = "T",
                Value = "2"
            });
            TPIItems.Add(new SelectListItem
            {
                Text = "W - Wear & Tear",
                //Text = "W",
                Value = "3"
            });

            return TPIItems;
        }


        public static string ReadSetting(string key)
        {
            try
            {
                return Properties.Settings.Default[key].ToString() ?? "Not Found";
            }
            catch (ConfigurationErrorsException)
            {
                throw;
            }
        }


        public static DataTable XElementToDataTable(XElement x)
        {
            DataTable dtable = new DataTable();

            XElement setup = (from p in x.Descendants() select p).First();
            // build your DataTable
            foreach (XElement xe in setup.Descendants())
            {
                dtable.Columns.Add(new DataColumn(xe.Name.ToString(), typeof(string))); // add columns to your dt
            }
            var all = from p in x.Descendants(setup.Name.ToString()) select p;
            foreach (XElement xe in all)
            {
                DataRow dr = dtable.NewRow();
                foreach (XElement xe2 in xe.Descendants())
                    dr[xe2.Name.ToString()] = xe2.Value; //add in the values
                dtable.Rows.Add(dr);
            }
            return dtable;
        }
        public static DataTable BuildDataTableFromXml(string Name, string XMLString)
        {

            XElement xele = XElement.Load(XMLString);//get your file
            // declare a new DataTable and pass your XElement to it
            DataTable objDataTable = new DataTable();
            objDataTable = XElementToDataTable(xele);
            return objDataTable;
        }

        public static String CreateMessageString(List<ErrMessage> MsgList)
        {
            StringBuilder MsgContent = new StringBuilder();
            MsgContent.Append("");

            foreach (ErrMessage msgItem in MsgList)
            {
                if (msgItem.ErrorType == Validation.MESSAGETYPE.ERROR.ToString())
                    MsgContent.Append(ResourceMerc.sERROR + msgItem.Message);
                else if (msgItem.ErrorType == Validation.MESSAGETYPE.SUCCESS.ToString())
                    MsgContent.Append(ResourceMerc.sSUCCESS + msgItem.Message);
                else if (msgItem.ErrorType == Validation.MESSAGETYPE.WARNING.ToString())
                    MsgContent.Append(ResourceMerc.sWARNING + msgItem.Message);//Warning
                else
                    MsgContent.Append(ResourceMerc.sINFO + msgItem.Message);//Info

                if (MsgList.IndexOf(msgItem) + 1 < MsgList.Count)
                    MsgContent.Append("</br>");
            }
            return MsgContent.ToString();
        }

        public static string ReplaceInMsg(string msgStr, string toReplaceWith)
        {
            return msgStr.Replace("%%", toReplaceWith);
        }

        public static bool ToBool(this string s)
        {
            bool i;
            if (bool.TryParse(s, out i)) return i;
            return i;
        }

        public static int? ToNullableInt32(this string s)
        {
            int i;
            if (Int32.TryParse(s, out i)) return i;
            return null;
        }

        //public static bool ToBool(this string s)
        //{
        //    bool i;
        //    if (bool.TryParse(s, out i)) return i;
        //    return i;
        //} 

        public static float? ToNullableFloat(this string s)
        {
            float i;
            if (float.TryParse(s, out i)) return i;
            return null;
        }

        public static DateTime? ToNullableDateTime(this string s)
        {
            DateTime i;
            if (DateTime.TryParse(s, out i)) return i;
            return null;
        }

        public static Decimal? ToNullableDecimal(this string s)
        {
            Decimal i;
            if (Decimal.TryParse(s, out i)) return i;
            return null;
        }

        public static Double? ToNullableDouble(this string s)
        {
            Double i;
            if (Double.TryParse(s, out i)) return i;
            return null;
        }
        public enum ERRORTYPE
        {
            Warning,
            Success
        }
        public static string GenErrorMessage(string errorMessage, string errorType)
        {
            string htmlError = "";
            if (errorType == ERRORTYPE.Success.ToString())
            {
                htmlError = "</br><div class=\"alert alert-success\" style=\"width: 750px; vertical-align: text-top;\"> <strong>Success!</strong> " + errorMessage + "</div>";
            }
            else if (errorType == ERRORTYPE.Warning.ToString())
            {
                htmlError = "</br><div class=\"alert alert-warning\" style=\"width: 750px; vertical-align: text-top;\"> <strong>Warning!</strong> " + errorMessage + "</div>";
            }
            return htmlError;
        }




    }
}