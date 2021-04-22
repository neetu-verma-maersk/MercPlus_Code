using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercPlusLibrary;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Merc_INLC
{
    public class INLCService
    {
        public Currency NewRec, OldRec;
        INLCOperations Iop = new INLCOperations();
        int iInserts = 0;
        int iUpdates = 0;
        int iNoChanges = 0;
        const int SKIPCHAR = 8;//skipping the starting 8 characters

        // offset/lenght of values in currency record MQ string.
        const int CUCDNOFF = 26;
        const int CUCDNLEN = 3;
        const int CURCDOFF = 0;
        const int CURCDLEN = 3;
        const int CURRNAMCOFF = 6;
        const int CURRNAMCLEN = 20;

        const int EXRATDKKOFF = 30;
        const int EXRATDKKLEN = 13;
        const int EXRATEUROFF = 45;
        const int EXRATEURLEN = 13;
        const int EXRATUSDOFF = 58;
        const int EXRATUSDLEN = 13;
        const int EXRATYENOFF = 72;
        const int EXRATYENLEN = 13;

        const int QUOTEDATOFF = 85;
        const int QUOTEDATLEN = 10;

        public int doServiceWork(string sFileNameWithPath)
        {
            try
            {
                string line = string.Empty;
                using (FileStream fs = File.Open(sFileNameWithPath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (System.IO.StreamReader file = new System.IO.StreamReader(fs))
                    {
                        while ((line = file.ReadLine()) != null)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                try
                                {
                                    UpdateTable(line);
                                }
                                catch (Exception ex)
                                {
                                    Merc_INLC.Program.logEntry.Message = "MercINLC : UpdateTable : throws exception :" + ex.Message;
                                    Logger.Write(Merc_INLC.Program.logEntry);
                                }
                            }
                        }
                    }
                }
                // rename file to .complete
                string newFileNameWithPath = sFileNameWithPath + ".complete";
                FileInfo fileI = new FileInfo(sFileNameWithPath);
                FileInfo fileComplete = new FileInfo(newFileNameWithPath);
                if (fileComplete.Exists)
                {
                    fileComplete.Delete();
                    //if exists delete complete file
                }
                fileI.CopyTo(newFileNameWithPath, false);

                //FileInfo OriginalFile = new FileInfo(sFileNameWithPath);
                fileI.Delete();
                //original exrt.txt delete
            }
            catch (Exception ex)
            {
                Merc_INLC.Program.logEntry.Message = "MercINLC : doServiceWork : throws exception :" + ex.Message;
                Logger.Write(Merc_INLC.Program.logEntry);
            }

            // Send results to the merc event log.
            SendEventTableAudit();
            return 0;
        }

        private void SendEventTableAudit()
        {
            try
            {
                string msg;
                msg = "Batch Currency Feed process - Records inserted: ";
                msg += iInserts.ToString();
                msg += ". Records updated: ";
                msg += iUpdates.ToString();
                msg += ". Records unchanged: ";
                msg += iNoChanges.ToString();
                Iop.EventLog("CURRENCY_FEED", "Not Applicable", "MESC1TS_CURRENCY", msg, "MercINLC Process", DateTime.Now);
            }
            catch (Exception ex)
            {
                Merc_INLC.Program.logEntry.Message = "MercINLC : SendEventTableAudit : throws exception :" + ex.Message;
                Logger.Write(Merc_INLC.Program.logEntry);
            }
        }

        public Currency LoadFromText(string sRecord)
        {
            string sData = string.Empty;
            NewRec = new Currency();
            sData = CheckBadData(sRecord.Substring(CUCDNOFF, CUCDNLEN));
            if (string.IsNullOrEmpty(sData)) return null; else NewRec.Cucdn = sData;

            sData = CheckBadData(sRecord.Substring(CURCDOFF, CURCDLEN));
            if (string.IsNullOrEmpty(sData)) return null; else NewRec.CurCode = sData;

            sData = CheckBadData(sRecord.Substring(CURRNAMCOFF, CURRNAMCLEN));
            if (string.IsNullOrEmpty(sData)) return null; else NewRec.CurrName = sData;

            sData = CheckBadData(sRecord.Substring(EXRATDKKOFF, EXRATDKKLEN));
            if (string.IsNullOrEmpty(sData)) return null; else NewRec.ExtraTdkk = Convert.ToDecimal(sData);

            sData = CheckBadData(sRecord.Substring(EXRATUSDOFF, EXRATUSDLEN));
            if (string.IsNullOrEmpty(sData)) return null; else NewRec.ExtratUsd = Convert.ToDecimal(sData);

            sData = CheckBadData(sRecord.Substring(EXRATYENOFF, EXRATYENLEN));
            if (string.IsNullOrEmpty(sData)) return null; else NewRec.ExtraTyen = Convert.ToDecimal(sData);

            sData = CheckBadData(sRecord.Substring(EXRATEUROFF, EXRATEURLEN));
            if (string.IsNullOrEmpty(sData)) return null; else NewRec.ExtraTeur = Convert.ToDecimal(sData);

            NewRec.ChangeUser = "MERCINLC_Process";

            sData = CheckBadData(sRecord.Substring(QUOTEDATOFF, QUOTEDATLEN));
            if (string.IsNullOrEmpty(sData)) return null; else NewRec.QuoteDat = Convert.ToDateTime(sData);

            return NewRec;
        }

        private string CheckBadData(string data)
        {
            if (string.IsNullOrEmpty(data) || data.Equals("\t") || data.Equals("\n") || string.IsNullOrWhiteSpace(data))
                return null;
            else
                return data;
        }

        public void UpdateTable(string sRecord)
        {
            // load record from MQ message and from database if record exists
            NewRec = LoadFromText(sRecord.Substring(SKIPCHAR, (sRecord.Length - SKIPCHAR)));
            if (NewRec == null)
                return;

            // check if cucdn code exists - many records received do not have this code
            // This is a primary key in the mercplus currency table
            NewRec.Cucdn = NewRec.Cucdn.Trim();
            if (string.IsNullOrEmpty(NewRec.Cucdn))
                return;

            OldRec = Iop.LoadFromDB(NewRec.Cucdn);
            if (OldRec == null)
            {
                // not exist in DB, then is a new currency record.
                if (Iop.InsertCurrencyDetails(NewRec))
                    iInserts++;
            }
            else
            {
                if (!IsRecordsEqual(NewRec, OldRec))
                {
                    if (Iop.UpdateCurrencyDetails(NewRec))
                        iUpdates++;
                }
                else
                    iNoChanges++;
            }
        }

        private bool IsRecordsEqual(Currency NewRec, Currency OldRec)
        {
            bool match = true;

            if (string.Equals(NewRec.Cucdn, OldRec.Cucdn)) match = true; else return false;
            if (string.Equals(NewRec.CurCode, OldRec.CurCode)) match = true; else return false;
            if (string.Equals(NewRec.CurrName.TrimEnd(), OldRec.CurrName)) match = true; else return false;
            if (Decimal.Equals(NewRec.ExtraTdkk, OldRec.ExtraTdkk)) match = true; else return false;
            if (Decimal.Equals(NewRec.ExtratUsd, OldRec.ExtratUsd)) match = true; else return false;
            if (Decimal.Equals(NewRec.ExtraTyen, OldRec.ExtraTyen)) match = true; else return false;
            if (Decimal.Equals(NewRec.ExtraTeur, OldRec.ExtraTeur)) match = true; else return false;
            if (DateTime.Equals(NewRec.QuoteDat, OldRec.QuoteDat)) match = true; else return false;

            return match;
        }
    }
}
