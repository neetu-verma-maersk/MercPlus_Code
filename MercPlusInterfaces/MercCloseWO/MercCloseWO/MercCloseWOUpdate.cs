using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging;


namespace MercCloseWO
{
    class MercCloseWOUpdate
    {
        public void MercWorkOrderUpdate()
        {

            
            List<MESC1TS_WO> WOList = new List<MESC1TS_WO>();
            try
            {
                MercCloseWOEntities objContext = new MercCloseWOEntities();

                WOList = (from W in objContext.MESC1TS_WO
                          join S in objContext.MESC1TS_SHOP on W.SHOP_CD equals S.SHOP_CD
                          where W.STATUS_CODE == 400 && S.RRIS_XMIT_SW == "N"
                          select W).ToList();

                foreach (var obj in WOList)
                {
                    obj.STATUS_CODE = 900;
                    MercCloseWO.MercCloseWOService.logEntry.Message = "MercWorkOrderUpdate start for WorkOrderId:" + obj.WO_ID;
                    Logger.Write(MercCloseWO.MercCloseWOService.logEntry);
                    int m=objContext.SaveChanges();
                    if (m == 1)
                    {
                        MercCloseWO.MercCloseWOService.logEntry.Message = "MercWorkOrderUpdate end for WorkOrderId:" + obj.WO_ID;
                    }

                    else
                    {
                        MercCloseWO.MercCloseWOService.logEntry.Message = "MercWorkOrderUpdate failed for WorkOrderId:" + obj.WO_ID;
                    }
                }
               
            }
            catch (Exception ex)
            {
                MercCloseWO.MercCloseWOService.logEntry.Message = "MercWorkOrderUpdate ERROR:" + ex.Message;
                Logger.Write(MercCloseWO.MercCloseWOService.logEntry);
            }
        }
    }
}
