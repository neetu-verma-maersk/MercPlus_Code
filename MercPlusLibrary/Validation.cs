using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Globalization;
using System.ServiceModel;

namespace MercPlusLibrary
{
    [DataContract]
    public static class Validation
    {
        public enum MESSAGETYPE
        {
            SUCCESS, ERROR, CLEAR, WARNING, INFO
        }

        public enum STATE
        {
            NEW = 0,
            DELETED,
            UPDATED,
            DISCARDED,
            EXISTING,
            REFRESHED
        };

        public enum WOSTATUS
        {
            ASDRAFT = -1,
            AS = 0
        };

        public enum EQPOWNTHIRDPARTY
        {
            VSA,
            SO,
            SUB,
            ONE
        }

        public enum REMARKSTYPE
        {
            S, //SYSTEM_REMARK
            E, //EXTERNAL_REMARK
            I, //INTERNAL_REMARK
            V  //VENDOR_REMARK
        }

        public static string FormatTextString(string sValue)
        {

            // Using prefixed column names.

            string sBuild = string.Empty; ;
            sValue = (sValue).Trim();
            if (sValue.Length == 0)
            {
                sBuild = "NULL";
                return (sBuild);
            }
            sBuild += "\'";

            // Correct apostrophes etc.  else will fail in SQL server

            sValue = RepairText(sValue);

            sBuild += sValue;
            sBuild += "\'";

            return (sBuild);


        }

        public static string RepairText(string sStr)
        {

            string tmp = sStr;
            string newstr = "";
            char ctest;

            // fix apostrophe for SQL server.
            for (int i = 0; i < tmp.Count(); i++)
            {
                ctest = tmp[i];
                if (ctest == '\'') newstr += '\'';
                newstr += ctest;
            }

            return (newstr);

        }

        public static string FormatDateString(string sValue)
        {
            // Prefixed names.
            string sBuild = "\'";
            sValue = (sValue).Trim();

            if (sValue.Length == 0)
                return ("NULL");
            //		return( sBuild+= "\'" );

            // YYYY-MM-DD HH:MM:SS.mmm
            // changed to same string format as expected in SQL server.
            // NOTE: RKEM returns 0001 for year as a default date. return 'NULL' for this value.
            string sBadYear = sValue.Substring(0, 4);

            if (sBadYear.Equals("0001") == true)
            {
                return ("NULL");
            }


            sBuild += sValue.Substring(0, 4);
            sBuild += "-";
            sBuild += sValue.Substring(5, 2);
            sBuild += "-";
            sBuild += sValue.Substring(8, 2);

            // original
            //	sBuild+= sValue.substr(4,4);
            //	sBuild+= "-";
            //	sBuild+= sValue.substr(2,2);
            //	sBuild+= "-";
            //	sBuild+= sValue.substr(0,2);

            sBuild += " 00:00:00.000";
            sBuild += "\'";

            return (sBuild);

        }

        public static string FormatIntString(string sValue)
        {
            string sBuild = string.Empty;
            if (sValue.Length == 0)
            { sValue = "NULL"; }
            sBuild += sValue;

            return (sBuild);

        }

        public static string FormatDecimalString(string sValue)
        {
            string result = string.Empty;

            try
            {
                decimal value = decimal.Parse("sValue");


                if (value > 0)
                {
                    result = Convert.ToString(value);
                }
                else
                {
                    result = "0.00";
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;

        }



        public static string GetCurrentTimeString()
        {
            return (DateTime.Now).ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
        }




    }
}
