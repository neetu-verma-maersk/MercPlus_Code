using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.Objects;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace ManageMasterDataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class ManageMasterData : IManageMasterData
    {
        //ManageMasterData()
        //{
        //    LogEntry logEntry = new LogEntry();
        //    logEntry.Message = "Starting up the application";
        //    Logger.Write(logEntry, "Debug");
        //}
        ManageMasterDataServiceEntities objContext = new ManageMasterDataServiceEntities();
        public List<PayAgent> GetPayAgent()
        {
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_PAYAGENT> payAgent = new List<MESC1TS_PAYAGENT>();
            payAgent = (from pay in objContext.MESC1TS_PAYAGENT
            select pay).ToList();

//            string esqlQuery = @"SELECT VALUE pay
//                    FROM ManageMasterDataServiceEntities.MESC1TS_PAYAGENT as pay";


//            ObjectQuery<MESC1TS_PAYAGENT> query = new ObjectQuery<MESC1TS_PAYAGENT>(esqlQuery, objContext);
//            payAgent = query.ToList();
            return PrepareDataContract(payAgent);

        }

        private List<PayAgent> PrepareDataContract(List<MESC1TS_PAYAGENT> payAgentFromDB)
        {
            List<PayAgent> payAgentList = new List<PayAgent>();
            for (int count = 0; count < payAgentFromDB.Count; count++)
            {
                PayAgent payAgent = new PayAgent();
                payAgent.PayAgentCode = payAgentFromDB[count].PAYAGENT_CD;
                payAgent.CorpPayAgentCode = payAgentFromDB[count].CORP_PAYAGENT_CD;
                payAgent.ProfitCenter = payAgentFromDB[count].PROFIT_CENTER;
                payAgent.RRISFormat = payAgentFromDB[count].RRIS_FORMAT;
                payAgent.ChangeUser = payAgentFromDB[count].CHUSER;
                payAgent.ChangeTime = payAgentFromDB[count].CHTS;
                payAgentList.Add(payAgent);
            }
            return payAgentList;
        }

        public PayAgent UpdatePayAgent(PayAgent PayAgentToBeUpdated)
        {
            objContext = new ManageMasterDataServiceEntities();

            List<MESC1TS_PAYAGENT> payAgentDBObject = new List<MESC1TS_PAYAGENT>();
            payAgentDBObject = (from pay in objContext.MESC1TS_PAYAGENT
                                where pay.PAYAGENT_CD == PayAgentToBeUpdated.PayAgentCode
                                select pay).ToList();
//            string esqlQuery = @"SELECT VALUE pay
//                FROM ManageMasterDataServiceEntities.MESC1TS_PAYAGENT as pay where pay.PAYAGENT_CD = @ln";
//            ObjectQuery<MESC1TS_PAYAGENT> query = new ObjectQuery<MESC1TS_PAYAGENT>(esqlQuery, objContext);
//            query.Parameters.Add(new ObjectParameter("ln", PayAgentToBeUpdated.PayAgentCode));
//            payAgentDBObject = query.ToList();

            payAgentDBObject[0].CORP_PAYAGENT_CD = PayAgentToBeUpdated.CorpPayAgentCode;
            payAgentDBObject[0].PROFIT_CENTER = PayAgentToBeUpdated.ProfitCenter;
            payAgentDBObject[0].RRIS_FORMAT = PayAgentToBeUpdated.RRISFormat;
            payAgentDBObject[0].CHUSER = PayAgentToBeUpdated.ChangeUser;
            //payAgentDBObject[0].CHTS = PayAgentToBeUpdated.ChangeTime;
            payAgentDBObject[0].CHTS = DateTime.Now;
            payAgentDBObject[0].SUB_PROFIT_CENTER = PayAgentToBeUpdated.ProfitCenter;
            //objContext.MESC1TS_PAYAGENT.AddObject(payAgentDBObject[0]);
            objContext.SaveChanges();
            return PayAgentToBeUpdated;
        }

        public bool DeletePayAgent(string RRISPayAgentCode)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            List<MESC1TS_PAYAGENT> payAgentDBObject = new List<MESC1TS_PAYAGENT>();
            payAgentDBObject = (from pay in objContext.MESC1TS_PAYAGENT
             where pay.PAYAGENT_CD == RRISPayAgentCode
             select pay).ToList();
//            string esqlQuery = @"SELECT VALUE pay
//                FROM ManageMasterDataServiceEntities.MESC1TS_PAYAGENT as pay where pay.PAYAGENT_CD = @ln";
            //ObjectQuery<MESC1TS_PAYAGENT> query = new ObjectQuery<MESC1TS_PAYAGENT>(esqlQuery, objContext);
            //query.Parameters.Add(new ObjectParameter("ln", RRISPayAgentCode));
            //payAgentDBObject = query.ToList();
            objContext.DeleteObject(payAgentDBObject.First());
            objContext.SaveChanges();
            return success;
        }

        public bool CreatePayAgent(PayAgent PayAgentFromClient)
        {
            bool success = false;
            objContext = new ManageMasterDataServiceEntities();
            MESC1TS_PAYAGENT PayAgentToBeInserted = new MESC1TS_PAYAGENT();
            PayAgentToBeInserted.PAYAGENT_CD = PayAgentFromClient.PayAgentCode;
            PayAgentToBeInserted.CORP_PAYAGENT_CD = PayAgentFromClient.CorpPayAgentCode;
            PayAgentToBeInserted.PROFIT_CENTER = PayAgentFromClient.ProfitCenter;
            PayAgentToBeInserted.SUB_PROFIT_CENTER = PayAgentFromClient.ProfitCenter;
            PayAgentToBeInserted.RRIS_FORMAT = PayAgentFromClient.RRISFormat;
            PayAgentToBeInserted.CHTS = DateTime.Now;
            objContext.MESC1TS_PAYAGENT.AddObject(PayAgentToBeInserted);
            try
            {
                objContext.SaveChanges();
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }

            return success;
        }
    }
}
