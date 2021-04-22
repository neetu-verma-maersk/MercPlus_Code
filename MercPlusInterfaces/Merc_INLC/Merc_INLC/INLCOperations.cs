using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercPlusLibrary;

namespace Merc_INLC
{
    public class INLCOperations
    {
        MESC2DSEntities objContext = null;

        public bool EventLog(string EVENT_NAME, string UNIQUE_ID, string TABLE_NAME, string EVENT_DESC, string CHUSER, DateTime CHTS)
        {
            objContext = new MESC2DSEntities();
            MESC1TS_EVENT_LOG EventLog = new MESC1TS_EVENT_LOG();

            EventLog.TABLE_NAME = TABLE_NAME;
            EventLog.UNIQUE_ID = UNIQUE_ID;
            EventLog.EVENT_DESC = EVENT_DESC;
            EventLog.EVENT_NAME = EVENT_NAME;
            EventLog.CHUSER = CHUSER;
            EventLog.CHTS = DateTime.Now;
            objContext.MESC1TS_EVENT_LOG.Add(EventLog);
            objContext.SaveChanges();

            return true;
        }

        public bool AuditLog(int WO_ID, string AUDIT_TEXT, string CHUSER, DateTime CHTS)
        {
            objContext = new MESC2DSEntities();
            MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
            WOAudit.WO_ID = WO_ID;
            WOAudit.AUDIT_TEXT = AUDIT_TEXT;
            WOAudit.CHUSER = CHUSER;
            WOAudit.CHTS = DateTime.Now;
            objContext.MESC1TS_WOAUDIT.Add(WOAudit);
            objContext.SaveChanges();
            return true;
        }

        public Currency LoadFromDB(string cucdn)
        {
            objContext = new MESC2DSEntities();
            Currency cur = new Currency();
            MESC1TS_CURRENCY mCur = new MESC1TS_CURRENCY();
            mCur = (from curr in objContext.MESC1TS_CURRENCY
                    where curr.CUCDN == cucdn
                    select curr).FirstOrDefault();

            if (mCur != null)
            {
                cur.Cucdn = mCur.CUCDN;
                cur.CurCode = mCur.CURCD;
                cur.CurrName = mCur.CURRNAMC;
                cur.ExtraTdkk = mCur.EXRATDKK;
                cur.ExtratUsd = mCur.EXRATUSD;
                cur.ExtraTyen = mCur.EXRATYEN;
                cur.ExtraTeur = Convert.ToDecimal(mCur.EXRATEUR);
                cur.ChangeUser = mCur.CHUSER;
                cur.QuoteDat = mCur.QUOTEDAT;
            }
            else
                cur = null;

            return cur;
        }

        public bool InsertCurrencyDetails(Currency cur)
        {
            objContext = new MESC2DSEntities();
            MESC1TS_CURRENCY mCur = new MESC1TS_CURRENCY();
            mCur.CUCDN = cur.Cucdn;
            mCur.CURCD = cur.CurCode;
            mCur.CURRNAMC = cur.CurrName;
            mCur.EXRATDKK = cur.ExtraTdkk;
            mCur.EXRATUSD = cur.ExtratUsd;
            mCur.EXRATYEN = cur.ExtraTyen;
            mCur.EXRATEUR = cur.ExtraTeur;
            mCur.CHUSER = "MercINLC_Process";
            mCur.CHTS = DateTime.Now;
            mCur.QUOTEDAT = (cur.QuoteDat == DateTime.MinValue ? null : cur.QuoteDat);
            objContext.MESC1TS_CURRENCY.Add(mCur);
            objContext.SaveChanges();
            return true;
        }
        public bool UpdateCurrencyDetails(Currency cur)
        {
            objContext = new MESC2DSEntities();
            MESC1TS_CURRENCY mCur = new MESC1TS_CURRENCY();
            mCur = (from curr in objContext.MESC1TS_CURRENCY
                    where curr.CUCDN == cur.Cucdn
                    select curr).FirstOrDefault();

            if (mCur != null)
            {
                    mCur.CURCD = cur.CurCode;
                    mCur.CURRNAMC = cur.CurrName;
                    mCur.EXRATDKK = cur.ExtraTdkk;
                    mCur.EXRATUSD = cur.ExtratUsd;
                    mCur.EXRATYEN = cur.ExtraTyen;
                    mCur.EXRATEUR = cur.ExtraTeur;
                    mCur.CHUSER = "MercINLC_Process";
                    mCur.CHTS = DateTime.Now;
                    if (cur.QuoteDat != null && cur.QuoteDat != DateTime.MinValue)
                        mCur.QUOTEDAT = cur.QuoteDat;
                    objContext.SaveChanges();
            }

            return true;
        }
    }
}
