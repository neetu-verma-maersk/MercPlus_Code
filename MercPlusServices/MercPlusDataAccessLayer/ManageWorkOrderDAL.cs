using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercPlusLibrary;
using System.Data.Objects;

namespace MercPlusDataAccessLayer
{   
    public class ManageWorkOrderDAL
    {
        MESC2DSEntities objContext = new MESC2DSEntities();
        private const short APPROVEDSTATUS = 390;
        private const short COMPLETEDWO = 400;

        public List<Damage> GetDamageCode()
        {
            List<Damage> DamageList = new List<Damage>();
            List<MESC1TS_DAMAGE> DamageFromDB = new List<MESC1TS_DAMAGE>();
            try
            {
                DamageFromDB = (from damage in objContext.MESC1TS_DAMAGE
                                select damage).ToList();

                foreach (var obj in DamageFromDB)
                {
                    Damage Damage = new Damage();
                    Damage.DamageCedexCode = obj.cedex_code;
                    Damage.DamageDescription = obj.description;
                    Damage.DamageName = obj.name;
                    DamageList.Add(Damage);
                }
            }
            catch(Exception ex)
            {
            }
            return DamageList;
        }

        public WorkOrderDetail LoadHeaderDetails(int WOID)
        {
            WorkOrderDetail WorkOrder = new WorkOrderDetail();
            WorkOrder.Shop = new Shop();
            WorkOrder.Shop.Customer = new Customer();
            WorkOrder.Shop.Currency = new Currency();
            WorkOrder.EquipmentList = new List<Equipment>();
            double d = 0;
            try
            {
                List<MESC1TS_WO> WorkOrderList = GetWorkOrderHeaderDetails(WOID);
                #region Data Mapper
                if (WorkOrderList != null && WorkOrderList.Count > 0)
                {
                    WorkOrder.WorkOrderID = WorkOrderList[0].WO_ID;
                    WorkOrder.Shop.Customer.CustomerCode = WorkOrderList[0].CUSTOMER_CD;
                    WorkOrder.Shop.Customer.ManualCode = WorkOrderList[0].MANUAL_CD;
                    WorkOrder.Mode = WorkOrderList[0].MODE;
                    WorkOrder.WorkOrderType = WorkOrderList[0].WOTYPE;
                    WorkOrder.VendorCode = WorkOrderList[0].VENDOR_CD;
                    WorkOrder.Shop.ShopCode = WorkOrderList[0].SHOP_CD;
                    WorkOrder.RepairDate = WorkOrderList[0].REPAIR_DTE;
                    WorkOrder.Status = WorkOrderList[0].STATUS_CODE.ToString();
                    WorkOrder.Cause = WorkOrderList[0].CAUSE;

                    WorkOrder.ThirdPartyPort = WorkOrderList[0].THIRD_PARTY;
                    //WorkOrder.THIRD_PARTY= " ";

                    WorkOrder.ManHourRate = WorkOrderList[0].MANH_RATE;
                    WorkOrder.ManHourRateCPH = WorkOrderList[0].MANH_RATE_CPH;
                    WorkOrder.ExchangeRate = WorkOrderList[0].EXCHANGE_RATE;
                    WorkOrder.Shop.CUCDN = WorkOrderList[0].CUCDN;

                    if(WorkOrderList[0].TOT_REPAIR_MANH == null)
                    {
                        d = (double)WorkOrderList[0].TOT_REPAIR_MANH;
                        WorkOrder.TotalRepairManHour = Math.Round(d,4);
                    }
                    else
                    {
                        WorkOrder.TotalRepairManHour = 0.0;
                    }


                    if(WorkOrderList[0].TOT_MANH_REG == null)
                    {
                        d = (double)WorkOrderList[0].TOT_MANH_REG;
                        WorkOrder.TotalManHourReg= Math.Round(d,4);
                    }
                    else
                    {
                        WorkOrder.TotalManHourReg= 0.0;
                    }

        //	   		WorkOrder.TOT_MANH_OT=  WorkOrderList[0].TOT_MANH_OT;
                    if(WorkOrderList[0].TOT_MANH_OT == null)
                    {
                        d = (double)WorkOrderList[0].TOT_MANH_OT;
				        
                        WorkOrder.TotalManHourOverTime= Math.Round(d,4);;
                    }
                    else
                    {
                        WorkOrder.TotalManHourOverTime= 0.0;
                    }

        //	   		WorkOrder.TOT_MANH_DT=  WorkOrderList[0].TOT_MANH_DT;
                    if(WorkOrderList[0].TOT_MANH_DT == null)
                    {
                        d = (double)WorkOrderList[0].TOT_MANH_DT;
        
				        
                        WorkOrder.TotalManHourDoubleTime= Math.Round(d,4);;
                    }
                    else
                    {
                        WorkOrder.TotalManHourDoubleTime= 0.0;
                    }

        //	   		WorkOrder.TOT_MANH_MISC=  WorkOrderList[0].TOT_MANH_MISC;
                    if(WorkOrderList[0].TOT_MANH_MISC == null)
                    {
                        d = (double)WorkOrderList[0].TOT_MANH_MISC;
        
				        
                        WorkOrder.TotalManHourMisc= Math.Round(d,4);;
                    }
                    else
                    {
                        WorkOrder.TotalManHourMisc= 0.0;
                    }


        //	   		WorkOrder.TOT_PREP_HRS=  WorkOrderList[0].TOT_PREP_HRS;
                    if(WorkOrderList[0].TOT_PREP_HRS == null)
                    {
                        d = (double)WorkOrderList[0].TOT_PREP_HRS;
        
				        
                        WorkOrder.TotalPrepHours= Math.Round(d,4);;
                    }
                    else
                    {
                        WorkOrder.TotalPrepHours= 0.0;
                    }

        //	   		WorkOrder.TOT_LABOR_HRS=  WorkOrderList[0].TOT_LABOR_HRS;
                    if(WorkOrderList[0].TOT_LABOR_HRS == null)
                    {
                        d = (double)WorkOrderList[0].TOT_LABOR_HRS;
        
				        
                        WorkOrder.TotalLaborHours= Math.Round(d,4);;
                    }
                    else
                    {
                        WorkOrder.TotalLaborHours= 0.0;
                    }

        //	   		WorkOrder.TOT_LABOR_COST=  WorkOrderList[0].TOT_LABOR_COST;
                    if(WorkOrderList[0].TOT_LABOR_COST == null)
                    {
                        d = (double)WorkOrderList[0].TOT_LABOR_COST;
                        d = Math.Round(d,4);
                        WorkOrder.TotalLabourCost = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalLabourCost= 0;
                    }

        //	   		WorkOrder.TOT_LABOR_COST_CPH=  WorkOrderList[0].TOT_LABOR_COST_CPH;
                    if(WorkOrderList[0].TOT_LABOR_COST_CPH == null)
                    {
                        d = (double)WorkOrderList[0].TOT_LABOR_COST_CPH;
                        d = Math.Round(d, 4);
                        WorkOrder.TotalLabourCostCPH = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalLabourCostCPH= 0;
                    }

        //	   		WorkOrder.TOT_SHOP_AMT=  WorkOrderList[0].TOT_SHOP_AMT;
                    if(WorkOrderList[0].TOT_SHOP_AMT == null)
                    {
                        d = (double)WorkOrderList[0].TOT_SHOP_AMT;
                        d = Math.Round(d, 4);
                        WorkOrder.TotalShopAmount = (decimal)d;

                    }
                    else
                    {
                        WorkOrder.TotalShopAmount= 0;
                    }

                    //Mangal Release 3 RQ6344 retrieving the field value

                    if(WorkOrderList[0].TOT_W_MATERIAL_AMT == null)
                    {
                        d = (double)WorkOrderList[0].TOT_W_MATERIAL_AMT;
                        d = Math.Round(d, 4);

                        WorkOrder.TotalWMaterialAmount = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalWMaterialAmount= 0;
                    }

                    if(WorkOrderList[0].TOT_T_MATERIAL_AMT == null)
                    {
                        d = (double)WorkOrderList[0].TOT_T_MATERIAL_AMT;
                        d = Math.Round(d, 4);

                        WorkOrder.TotalTMaterialAmount = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalTMaterialAmount= 0;
                    }

                    if(WorkOrderList[0].TOT_W_MATERIAL_AMT_CPH == null)
                    {
                        d = (double)WorkOrderList[0].TOT_W_MATERIAL_AMT_CPH;
                        d = Math.Round(d, 4);

                        WorkOrder.TotalWMaterialAmountCPH = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalWMaterialAmountCPH= 0;
                    }

                    if(WorkOrderList[0].TOT_T_MATERIAL_AMT_CPH == null)
                    {
                        d = (double)WorkOrderList[0].TOT_T_MATERIAL_AMT_CPH;
                        d = Math.Round(d, 4);

                        WorkOrder.TotalWMaterialAmountCPH = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalWMaterialAmountCPH= 0;
                    }

                    if(WorkOrderList[0].TOT_W_MATERIAL_AMT_USD == null)
                    {
                        d = (double)WorkOrderList[0].TOT_W_MATERIAL_AMT_USD;
                        d = Math.Round(d, 4);

                        WorkOrder.TotalWMaterialAmountUSD = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalWMaterialAmountUSD= 0;
                    }

                    if(WorkOrderList[0].TOT_T_MATERIAL_AMT_USD == null)
                    {
                        d = (double)WorkOrderList[0].TOT_T_MATERIAL_AMT_USD;

                        d = Math.Round(d, 4);
                        WorkOrder.TotalTMaterialAmountUSD = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalTMaterialAmountUSD= 0;
                    }

                    if(WorkOrderList[0].TOT_W_MATERIAL_AMT_CPH_USD == null)
                    {
                        d = (double)WorkOrderList[0].TOT_W_MATERIAL_AMT_CPH_USD;

                        d = Math.Round(d, 4);
                        WorkOrder.TotalWMaterialAmountCPHUSD = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalWMaterialAmountCPHUSD= 0;
                    }

                    if(WorkOrderList[0].TOT_T_MATERIAL_AMT_CPH_USD == null)
                    {
                        d = (double)WorkOrderList[0].TOT_T_MATERIAL_AMT_CPH_USD;

                        d = Math.Round(d, 4);
                        WorkOrder.TotalTMaterialAmountCPHUSD = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalTMaterialAmountCPHUSD= 0;
                    }

		

			
                    //mangal

        //	   		WorkOrder.TOT_SHOP_AMT_CPH=  WorkOrderList[0].TOT_SHOP_AMT_CPH;
                    if(WorkOrderList[0].TOT_SHOP_AMT_CPH == null)
                    {
                        d = (double)WorkOrderList[0].TOT_SHOP_AMT_CPH;

                        d = Math.Round(d, 4);
                        WorkOrder.TotalShopAmountCPH = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalShopAmountCPH= 0;
                    }

        //	   		WorkOrder.TOT_COST_LOCAL=  WorkOrderList[0].TOT_COST_LOCAL;
                    if(WorkOrderList[0].TOT_COST_LOCAL == null)
                    {
                        d = (double)WorkOrderList[0].TOT_COST_LOCAL;

                        d = Math.Round(d, 4);
                        WorkOrder.TotalCostLocal = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalCostLocal= 0;
                    }

        //	   		WorkOrder.TOT_COST_CPH=  WorkOrderList[0].TOT_COST_CPH;
                    if(WorkOrderList[0].TOT_COST_CPH == null)
                    {
                        d = (double)WorkOrderList[0].TOT_COST_CPH;

                        d = Math.Round(d, 4);
                        WorkOrder.TotalCostCPH = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.TotalCostCPH= 0;
                    }


                    WorkOrder.OverTimeRate=  WorkOrderList[0].OT_RATE;
                    WorkOrder.OverTimeRateCPH = WorkOrderList[0].OT_RATE_CPH;
                    WorkOrder.DoubleTimeRate = WorkOrderList[0].DT_RATE;
                    WorkOrder.DoubleTimeRateCPH = WorkOrderList[0].DT_RATE_CPH;
                    WorkOrder.MiscRate = WorkOrderList[0].MISC_RATE;
                    WorkOrder.MiscRateCPH = WorkOrderList[0].MISC_RATE_CPH;
                    WorkOrder.TotalCostLocalUSD = WorkOrderList[0].TOTAL_COST_LOCAL_USD;
                    WorkOrder.TotalCostOfRepair = WorkOrderList[0].TOT_COST_REPAIR;
                    WorkOrder.TotalCostOfRepairCPH = WorkOrderList[0].TOT_COST_REPAIR_CPH;
                    WorkOrder.SalesTaxLabour = WorkOrderList[0].SALES_TAX_LABOR;
                    WorkOrder.SalesTaxLabourCPH = WorkOrderList[0].SALES_TAX_LABOR_CPH;
                    WorkOrder.SalesTaxParts = WorkOrderList[0].SALES_TAX_PARTS;
                    WorkOrder.SalesTaxPartsCPH = WorkOrderList[0].SALES_TAX_PARTS_CPH;
                    WorkOrder.TotalMaerksParts = WorkOrderList[0].TOT_MAERSK_PARTS;
                    WorkOrder.TotalMaerksPartsCPH=  WorkOrderList[0].TOT_MAERSK_PARTS_CPH;
                    WorkOrder.TotalManParts = WorkOrderList[0].TOT_MAN_PARTS;

                    WorkOrder.TotalManPartsCPH=  WorkOrderList[0].TOT_MAN_PARTS_CPH;
                    //WorkOrder.VendorRefNo=  WorkOrderList[0].VENDOR_REF_NO;
                    WorkOrder.VoucherNumber=  WorkOrderList[0].VOUCHER_NO;

        //	   		WorkOrder.INVOICE_DTE=  WorkOrderList[0].INVOICE_DTE;
                    if (WorkOrderList[0].INVOICE_DTE == null)
                    {
                        WorkOrder.InvoiceDate = WorkOrderList[0].INVOICE_DTE;
                    }
                    //else
                    //{
                    //    WorkOrder.InvoiceDate = "";
                    //}


                    WorkOrder.CheckNo=  WorkOrderList[0].CHECK_NO;

        //	   		WorkOrder.PAID_DTE=  WorkOrderList[0].PAID_DTE;
                    if (WorkOrderList[0].PAID_DTE == null)
                    {
                        WorkOrder.PaidDate = WorkOrderList[0].PAID_DTE;
                    }
                    //else
                    //{
                    //    WorkOrder.PAID_DTE= "";
                    //}


                    WorkOrder.AmountPaid=  WorkOrderList[0].AMOUNT_PAID;

        //	   		WorkOrder.RKRP_REPAIR_DTE=  WorkOrderList[0].RKRP_REPAIR_DTE;
                    if(WorkOrderList[0].RKRP_REPAIR_DTE == null)
                    {
                        WorkOrder.RKRPRepairDate = WorkOrderList[0].RKRP_REPAIR_DTE;
                    }
			        
        //	   		WorkOrder.RKRP_XMIT_DTE=  WorkOrderList[0].RKRP_XMIT_DTE;
                    if(WorkOrderList[0].RKRP_XMIT_DTE == null)
                    {
                        WorkOrder.RKRPXMITDate = WorkOrderList[0].RKRP_XMIT_DTE;
                    }
                    //else
                    //{
                    //    WorkOrder.RKRP_XMIT_DTE= "";
                    //}


                    WorkOrder.RKRPXMITSW=  WorkOrderList[0].RKRP_XMIT_SW;

        //	   		WorkOrder.WO_RECV_DTE=  WorkOrderList[0].WO_RECV_DTE;
                    if(WorkOrderList[0].WO_RECV_DTE == null)
                    {
                        WorkOrder.WorkOrderReceiveDate = WorkOrderList[0].WO_RECV_DTE;
                    }
                    //else
                    //{
                    //    WorkOrder.WO_RECV_DTE= "";
                    //}


                    WorkOrder.ApprovedBy=  WorkOrderList[0].APPROVED_BY;
                    WorkOrder.ShopWorkingSW=  WorkOrderList[0].SHOP_WORKING_SW;

        //	   		WorkOrder.APPROVAL_DTE=  WorkOrderList[0].APPROVAL_DTE;
                    if(WorkOrderList[0].APPROVAL_DTE == null)
                    {
                        WorkOrder.ApprovalDate = WorkOrderList[0].APPROVAL_DTE;
                    }
                    //else
                    //{
                    //    WorkOrder.APPROVAL_DTE= "";
                    //}


                    WorkOrder.ImportTax=  WorkOrderList[0].IMPORT_TAX;
                    WorkOrder.ImportTaxCPH=  WorkOrderList[0].IMPORT_TAX_CPH;
                    WorkOrder.ChangeUser=  WorkOrderList[0].CHUSER;

	   		
                    // !FIX - not getting CHTS from DB.
        //			WorkOrder.CHTS=  WorkOrderList[0].CHTS;
                    if(WorkOrderList[0].CHTS == null)
                    {
                        WorkOrder.ChangeTime = WorkOrderList[0].CHTS;
                    }
                    //else
                    //{
                    //    WorkOrder.CHTS= "";
                    //}


                    WorkOrder.EquipmentList[0].EquipmentNo=  WorkOrderList[0].EQPNO;
                    WorkOrder.EquipmentList[0].Size=  WorkOrderList[0].EQSIZE;
                    WorkOrder.EquipmentList[0].Type=  WorkOrderList[0].EQTYPE;

                    WorkOrder.EquipmentList[0].Eqouthgu=  WorkOrderList[0].EQOUTHGU;

                    WorkOrder.EquipmentList[0].COType=  WorkOrderList[0].COTYPE;
                    WorkOrder.EquipmentList[0].SubType=  WorkOrderList[0].EQSTYPE;
                    WorkOrder.EquipmentList[0].Eqowntp=  WorkOrderList[0].EQOWNTP;
                    WorkOrder.EquipmentList[0].Eqmatr=  WorkOrderList[0].EQMATR;

                    if(WorkOrderList[0].DELDATSH == null)
                    {
                        WorkOrder.Deldatsh = WorkOrderList[0].DELDATSH;
                    }
                    //else
                    //{
                    //    WorkOrder.DELDATSH= "";
                    //}

                    WorkOrder.StEmptyFullInd=  WorkOrderList[0].STEMPTY;

                    WorkOrder.Strefurb=  WorkOrderList[0].STREFURB;

        //WorkOrder.STREFURB= " ";


        //	   		WorkOrder.REFRBDAT=  WorkOrderList[0].REFRBDAT;
                    if(WorkOrderList[0].REFRBDAT == null)
                    {
                        WorkOrder.RefurbishmnentDate = WorkOrderList[0].REFRBDAT;
                    }
                    //else
                    //{
                    //    WorkOrder.REFRBDAT= "";
                    //}



                    WorkOrder.Stredel=  WorkOrderList[0].STREDEL;

        //	   		WorkOrder.FIXCOVER=  WorkOrderList[0].FIXCOVER;
                    if(WorkOrderList[0].FIXCOVER == null)
                    {
                        d = (double)WorkOrderList[0].FIXCOVER;
        
				        
                        WorkOrder.Fixcover= Math.Round(d,4);;
                    }
                    else
                    {
                        WorkOrder.Fixcover= 0.0;
                    }


        //	   		WorkOrder.DPP=  WorkOrderList[0].DPP;
                    if(WorkOrderList[0].TOT_REPAIR_MANH == null)
                    {
                        d = (double)WorkOrderList[0].DPP;
        
				        
                        WorkOrder.Dpp= Math.Round(d,4);;
                    }
                    else
                    {
                        WorkOrder.Dpp= 0.0;
                    }


                    WorkOrder.OffhirLocationSW=  WorkOrderList[0].OFFHIR_LOCATION_SW;
                    WorkOrder.STSELSCR=  WorkOrderList[0].STSELSCR;
                    WorkOrder.EquipmentList[0].EqMancd=  WorkOrderList[0].EQMANCD;
                    WorkOrder.EquipmentList[0].EQProfile=  WorkOrderList[0].EQPROFIL;
                    WorkOrder.EquipmentList[0].EQInDate=  WorkOrderList[0].EQINDAT;

                    WorkOrder.EquipmentList[0].EQRuman=  WorkOrderList[0].EQRUMAN;
                    WorkOrder.EquipmentList[0].EQRutyp=  WorkOrderList[0].EQRUTYP;
                    WorkOrder.EquipmentList[0].EQIoflt=  WorkOrderList[0].EQIOFLT;
                    WorkOrder.EquipmentList[0].ReqRemarkSW=  WorkOrderList[0].REQ_REMARK_SW;
        // ADO tweak...This is not working from the string utility call. 

        //	   		WorkOrder.SALES_TAX_LABOR_PCT=  WorkOrderList[0].SALES_TAX_LABOR_PCT;
                    if(WorkOrderList[0].SALES_TAX_LABOR_PCT == null)
                    {
                        d = (double)WorkOrderList[0].SALES_TAX_LABOR_PCT;
        
				        
                        WorkOrder.SalesTaxLaborPCT= Math.Round(d,4);;
                    }
                    else
                    {
                        WorkOrder.SalesTaxLaborPCT= 0.0;
                    }


        //	   		WorkOrder.SALES_TAX_PARTS_PCT=  WorkOrderList[0].SALES_TAX_PARTS_PCT;
                    if(WorkOrderList[0].SALES_TAX_PARTS_PCT == null)
                    {
                        d = (double)WorkOrderList[0].SALES_TAX_PARTS_PCT;
        
				        
                        WorkOrder.SalesTaxPartsPCT= Math.Round(d,4);;
                    }
                    else
                    {
                        WorkOrder.SalesTaxPartsPCT = 0.0;
                    }


        //			WorkOrder.IMPORT_TAX_PCT=  WorkOrderList[0].IMPORT_TAX_PCT;

        //			d = pRs->Fields->GetItem("IMPORT_TAX_PCT")->Value.fltVal;
        // ADO tweak...This is not working from the string utility call. 
                    if(WorkOrderList[0].IMPORT_TAX_PCT == null)
                    {
                        d = (double)WorkOrderList[0].IMPORT_TAX_PCT;
        
				        
                        WorkOrder.ImportTaxPCT= Math.Round(d,4);;
                    }
                    else
                    {
                        WorkOrder.ImportTaxPCT= 0.0;
                    }

                    // Agent tax on parts
                    if(WorkOrderList[0].AGENT_PARTS_TAX == null)
                    {
                        d = (double)WorkOrderList[0].AGENT_PARTS_TAX;

                        d = Math.Round(d, 4);
                        WorkOrder.AgentPartsTax = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.AgentPartsTax= 0;
                    }

                    if(WorkOrderList[0].AGENT_PARTS_TAX_CPH == null)
                    {
                        d = (double)WorkOrderList[0].AGENT_PARTS_TAX_CPH;

                        d = Math.Round(d, 4);
                        WorkOrder.AgentPartsTaxCPH = (decimal)d;
                    }
                    else
                    {
                        WorkOrder.AgentPartsTaxCPH = 0;
                    }

                    // Get FACT interface values (2005-09-12)
                    WorkOrder.CountryCUCDN=  WorkOrderList[0].COUNTRY_CUCDN;

                    // call below failing.
                    WorkOrder.CountryExchangeRate=  WorkOrderList[0].COUNTRY_EXCHANGE_RATE;
        
                    WorkOrder.CountryExchangeDate=  WorkOrderList[0].COUNTRY_EXCHANGE_DTE;

                    // New RKEM damage value - VJP 2006-07-17
                    //WorkOrder.DAMAGE=  WorkOrderList[0].DAMAGE;
                    // VJP - added RKEM fields. 2006-07-25
                    WorkOrder.LeaseComp=  WorkOrderList[0].LSCOMP;
                    WorkOrder.LeaseContract=  WorkOrderList[0].LSCONTR;

                    //RQ 6361 & RQ 6362 (satadal)
                    // Added for two new RKEM fields
		
                    WorkOrder.PresentLoc=  WorkOrderList[0].PRESENTLOC;
		
                    if(WorkOrderList[0].GATEINDTE == null)
                    {
                        //COleDateTime t = pRs->Fields->GetItem("GATEINDTE")->Value;
                        WorkOrder.GateInDate = WorkOrderList[0].GATEINDTE;
                    }
                    else
                    {
                        //WorkOrder.GateInDate= "";
                    }


                    if(WorkOrderList[0].prev_wo_id == null)
                    {
                        WorkOrder.PrevWorkOrderID=  WorkOrderList[0].prev_wo_id;
                    }
                    //else
                    //{
                    //    WorkOrder.PrevWorkOrderID = "";
                    //}
                    if(WorkOrderList[0].PREV_STATUS == null)
                    {
                        WorkOrder.PrevStatus=  WorkOrderList[0].PREV_STATUS;
                        if (WorkOrder.PrevStatus == null)
                            WorkOrder.PrevStatus = null;
                    }
                    else
                    {
                        WorkOrder.PrevStatus = null;
                    }

                    if(WorkOrderList[0].PREV_LOC_CD == null)
                    {
                        WorkOrder.PrevLocCode=   WorkOrderList[0].PREV_LOC_CD;
                    }
                    else
                    {
                        WorkOrder.PrevLocCode = string.Empty;
                    }

                    if(WorkOrderList[0].PREV_DATE == null)
                    {
                        WorkOrder.PrevDate= WorkOrderList[0].PREV_DATE;
                    }
                    else
                    {
                        //WorkOrder.PrevDate = "";
                    }

                }
            #endregion Data Mapper
            }
            catch
            {
                //errorMessage = "MercWorkOrderTable", "Database error encountered in GetWorkOrder call.";
                //err
            }
            return WorkOrder;
        }

        public List<RepairsView> LoadRepairsDetails(int WOID)
        {
            List<RepairsView>  RepairsViewList = new List<RepairsView>();
            int nQty = 0;
            double d = 0.0;
            try
            {
                List<MESC1TS_WOREPAIR> RepairListFromDB = GetRepairListDetails(WOID);
                #region Data Mapper
                if (RepairsViewList != null && RepairsViewList.Count > 0)
                {
                    foreach (var item in RepairListFromDB)
                    {
                        RepairsView Repairs = new RepairsView();
                        Repairs.rState = (int)Validation.STATE.EXISTING;
                        Repairs.WorkOrderID = item.WO_ID;
                        Repairs.RepairCode.RepairCod = item.REPAIR_CD;
                        Repairs.RepairCode.ManualCode = item.MANUAL_CD;
                        Repairs.RepairCode.Mode.ModeCode = item.MODE;
                        //			Repairs.Pieces = item.QTY_REPAIRS;
                        if (item.QTY_REPAIRS != null)
                        {
                            Repairs.Pieces = (int)item.QTY_REPAIRS; ;
                        }
                        else
                        {
                            Repairs.Pieces = 0;
                        }


                        //			Repairs.MaterialAmt = item.SHOP_MATERIAL_AMT;
                        if (item.SHOP_MATERIAL_AMT != null)
                        {
                            d = (double)item.SHOP_MATERIAL_AMT;
                            //				sprintf(cNum,"%.2f", d);
                            //sprintf(cNum,"%.4f", d);
                            //Repairs.MaterialCost = Math.Round(d, 4);
                            Repairs.MaterialCost = item.SHOP_MATERIAL_AMT;
                        }
                        else
                        {
                            Repairs.MaterialCost = 0;
                        }


                        //			Repairs.MaterialAmtCPH = item.CPH_MATERIAL_AMT;
                        if (item.CPH_MATERIAL_AMT != null)
                        {
                            d = (double)item.CPH_MATERIAL_AMT;
                            //				sprintf(cNum,"%.2f", d);
                            //sprintf(cNum,"%.4f", d);
                            //Repairs.MaterialCostCPH = Math.Round(d, 4);
                        }
                        else
                        {
                            Repairs.MaterialCostCPH = 0;
                        }

                        //			Repairs.ManHrs = item.ACTUAL_MANH;
                        if (item.ACTUAL_MANH != null)
                        {
                            d = (double)item.ACTUAL_MANH;
                            //Repairs.ManHoursPerPiece = Math.Round(d, 4);
                        }
                        else
                        {
                            Repairs.ManHoursPerPiece = 0;
                        }


                        //Mangal Release 3 Loading damage code, repair location code and Tpi code
                        Repairs.Damage.DamageCedexCode = item.DAMAGE_CD;
                        Repairs.RepairLocationCode.CedexCode = item.REPAIR_LOC_CD;
                        Repairs.Tpi.CedexCode = item.TPI_CD;

                        //Mangal
                        Repairs.NonsCode.NonsCd = item.NONS_CD;
                        Repairs.RepairCode.ChangeUser = item.CHUSER;
                        RepairsViewList.Add(Repairs);
                    }
                }
                #endregion Data Mapper
            }

            catch
            {
            }

            return RepairsViewList;
        }

        public List<SparePartsView> LoadPartsDetails(int WOID)
        {
            List<SparePartsView> SparePartsList = new List<SparePartsView>();

            try
            {
                List<MESC1TS_WOPART> SparePartsListFromDB = GetSparePartListDetails(WOID);
                #region DataMapper
                if (SparePartsListFromDB != null && SparePartsListFromDB.Count > 0)
                {
                    foreach (var item in SparePartsListFromDB)
                    {
                        SparePartsView SpareParts = new SparePartsView();
                        SpareParts.pState = (int)Validation.STATE.EXISTING;
                        SpareParts.WorkOrderID = item.WO_ID;
                        SpareParts.RepairCode.RepairCod = item.REPAIR_CD;
                        SpareParts.OwnerSuppliedPartsNumber = item.PART_CD;
                        //SpareParts.CostLocal =Convert.ToDecimal(item.COST_LOCAL);
                        //SpareParts.CostLocalCPH = item.COST_CPH;
                        if (string.IsNullOrEmpty(item.SERIAL_NUMBER))
                        {
                            SpareParts.SerialNumber = item.SERIAL_NUMBER;
                        }
                        else
                        {
                            SpareParts.SerialNumber = string.Empty;
                        }
                        if (item.QTY_PARTS != null)
                        {
                            //d = (double)item.QTY_PARTS;
                            //sprintf(cNum,"%.2f", d);
				            SpareParts.Pieces= (int)item.QTY_PARTS;
                        }
                        else
                        {
                            SpareParts.Pieces= 0;
                        }
                        //SpareParts.CHUser = item.CHUSER;

                        SparePartsList.Add(SpareParts);
                    }
                }
                #endregion DataMapper
            }

            catch
            {
            }

            return SparePartsList;
        }

        public List<RemarkEntry> LoadRemarksDetails(int WOID)
        {
            List<RemarkEntry> RemarksList = new List<RemarkEntry>();
            try
            {
                List<MESC1TS_WOREMARK> RemarksListFromDB = GetRemarksListDetails(WOID);
                #region DataMapper
                if (RemarksListFromDB != null && RemarksListFromDB.Count > 0)
                {
                    foreach (var item in RemarksListFromDB)
                    {
                        RemarkEntry RemarkEntry = new RemarkEntry();
                        RemarkEntry.RemarkID = item.WOREMARK_ID;
                        RemarkEntry.WorkOrderID = item.WO_ID;
                        RemarkEntry.RemarkType = item.REMARK_TYPE;
                        RemarkEntry.SuspendCatID = item.SUSPCAT_ID;
                        RemarkEntry.Remark = item.REMARK;

                        if(item.XMIT_DTE != null)
			            {
				            RemarkEntry.XMITDate = item.XMIT_DTE;
			            }
			            else
			            {
                            RemarkEntry.XMITDate = DateTime.MinValue;
			            }


			            RemarkEntry.ChangeUser = item.CHUSER;

                        RemarkEntry.rState = (int)Validation.STATE.EXISTING;
                    }
                }
                #endregion DataMapper
            }
            catch
            {
            }

            return RemarksList;
        }

        private List<MESC1TS_WO> GetWorkOrderHeaderDetails(int WOID)
        {
            List<MESC1TS_WO> WorkOrder = new List<MESC1TS_WO>();

            WorkOrder = (from w in objContext.MESC1TS_WO
                         where w.WO_ID == WOID
                         select w).ToList();

            return WorkOrder;
        }

        private List<MESC1TS_WOREPAIR> GetRepairListDetails(int WOID)
        {
            List<MESC1TS_WOREPAIR> RepairList = new List<MESC1TS_WOREPAIR>();

            RepairList = (from R in objContext.MESC1TS_WOREPAIR
                         where R.WO_ID == WOID
                         select R).ToList();

            return RepairList;
        }

        private List<MESC1TS_WOPART> GetSparePartListDetails(int WOID)
        {
            List<MESC1TS_WOPART> PartsList = new List<MESC1TS_WOPART>();

            PartsList = (from P in objContext.MESC1TS_WOPART
                          where P.WO_ID == WOID
                          select P).ToList();

            return PartsList;
        }

        private List<MESC1TS_WOREMARK> GetRemarksListDetails(int WOID)
        {
            List<MESC1TS_WOREMARK> RemarksList = new List<MESC1TS_WOREMARK>();

            RemarksList = (from R in objContext.MESC1TS_WOREMARK
                           where R.WO_ID == WOID
                           select R).OrderByDescending(remarks => remarks.CRTS).ToList();

            return RemarksList;
        }

        public short? GetRevisionNo(int WOID)
        {
            List<WorkOrderDetail> WorkOrderDetails = new List<WorkOrderDetail>();
            List<MESC1TS_WO> WorkOrder = new List<MESC1TS_WO>();
            WorkOrderDetail WOD = new WorkOrderDetail();
            short? RevNo = 0;
            try
            {
                WorkOrder = (from wo in objContext.MESC1TS_WO
                             where wo.WO_ID == WOID
                             select wo).ToList();

                if ((WorkOrder != null && WorkOrder.Count > 0) && (WorkOrder[0].REVISION_NO != null))
                {

                    RevNo = WorkOrder[0].REVISION_NO;
                    //WOD.RevisionNo = Math.Round(WOD.RevisionNo)
                }
                else
                {
                    //WOD.RevisionNo = -1;
                }
                
            }
            catch
            {
            }
            return RevNo;
        }

        public short? GetWorkOrderStatus(int WOID)
        {
            short? ID = 0;

            try
            {
                var WorkOrderStatus = (from wo in objContext.MESC1TS_WO
                                       where wo.WO_ID == WOID
                                       select wo).ToList();
                if (WorkOrderStatus != null && WorkOrderStatus.Count > 0)
                {
                    ID = WorkOrderStatus[0].STATUS_CODE;
                }
            }

            catch
            {
            }

            return ID;
        }

        public bool RemoveDraft(int WOID)
        {
            bool success = false;
            try
            {
                var DeleteRemark = (from rem in objContext.MESC1TS_WOREMARK
                                    where rem.WO_ID == WOID
                                    select rem).ToList();

                foreach (MESC1TS_WOREMARK remark in DeleteRemark)
                {
                    objContext.MESC1TS_WOREMARK.Remove(remark);
                    objContext.SaveChanges();
                }
                //objContext.DeleteObject(DeleteRemark.First());
                

                var DeletePart = (from part in objContext.MESC1TS_WOPART
                                  where part.WO_ID == WOID
                                  select part).ToList();

                foreach (MESC1TS_WOPART part in DeletePart)
                {
                    objContext.MESC1TS_WOPART.Remove(part);
                    objContext.SaveChanges();
                }

                var DeleteRepair = (from rep in objContext.MESC1TS_WOREPAIR
                                    where rep.WO_ID == WOID
                                    select rep).ToList();

                foreach (MESC1TS_WOREPAIR repair in DeleteRepair)
                {
                    objContext.MESC1TS_WOREPAIR.Remove(repair);
                    objContext.SaveChanges();
                }

                var DeleteAudit = (from audit in objContext.MESC1TS_WOAUDIT
                                   where audit.WO_ID == WOID
                                   select audit).ToList();

                foreach (MESC1TS_WOAUDIT woAudit in DeleteAudit)
                {
                    objContext.MESC1TS_WOAUDIT.Remove(woAudit);
                    objContext.SaveChanges();
                }

                var DeleteWO = (from wo in objContext.MESC1TS_WO
                                where wo.WO_ID == WOID
                                select wo).ToList();

                foreach (MESC1TS_WO wo in DeleteWO)
                {
                    objContext.MESC1TS_WO.Remove(wo);
                    objContext.SaveChanges();
                }
                success = true;
            }
            catch
            {
                success = false;
            }

            return success;
        }

        public bool UpdateStatus(int WOID, short? Status, string ChangeUser, short? oldStatus)
        {
            bool success = false;
            int? prevWOID = 0;
            string loc = string.Empty;
            short? statusCode = null;
            DateTime date;
            List<MESC1TS_WO> workOrderDB = new List<MESC1TS_WO>();
            MESC1TS_WOAUDIT WOAudit = new MESC1TS_WOAUDIT();
            try
            {
                if (Status == APPROVEDSTATUS)
                {
                    //Pick up previous WO ID
                    workOrderDB = (from wo in objContext.MESC1TS_WO
                                  where wo.WO_ID == WOID
                                  select wo).ToList();

                    prevWOID = workOrderDB[0].prev_wo_id;

                    //For unbuffed(before prev id was introduced), unapproved WO's
                    if (prevWOID == null || prevWOID == 0)
                    {
                        workOrderDB = (from wo1 in objContext.MESC1TS_WO
                                       join wo2 in objContext.MESC1TS_WO on new { WO1 = wo1.EQPNO }
                                       equals new { WO1 = wo2.EQPNO } into INNER
                                       from O in INNER.DefaultIfEmpty()
                                       where wo1.WO_ID == WOID &&
                                       wo1.CRTS > O.CRTS
                                       select O).Take(1).OrderByDescending(crts => crts.CRTS).ToList();
                    }

                    //if previous WO does not exist, set prev_id to -1(means no prev id)
                    if (prevWOID == null || prevWOID == 0)
                    {
                        prevWOID = -1;
                    }

                    //Prev id is -1, i.e., no previous WO exist
                    if (prevWOID == -1)
                    {
                        loc = string.Empty;
                        date = DateTime.MinValue;
                        statusCode = null;
                    }
                    else
                    {
                        var WO = (from wo in objContext.MESC1TS_WO
                                       from s in objContext.MESC1TS_SHOP
                                       where wo.WO_ID == WOID &&
                                       wo.SHOP_CD == s.SHOP_CD
                                       select new { wo.WO_ID ,
                                                    wo.STATUS_CODE,
                                                    wo.CHTS,
                                                    s.LOC_CD 
                                       }).ToList();

                        statusCode = WO[0].STATUS_CODE;
                        date = WO[0].CHTS;
                        loc = WO[0].LOC_CD;
                    }

                    //sSQL += "if exists (select WO_ID from mesc1ts_wo where wo_id = ";
                    //sSQL += result;
                    //sSQL += ") begin ";
                    //sSQL += "if 390 > (select status_code from mesc1ts_wo where wo_id = ";
                    //sSQL += result;
                    //sSQL += ") begin ";

                    workOrderDB = (from wo in objContext.MESC1TS_WO
                                   where wo.WO_ID == WOID
                                   select wo).ToList();

                    workOrderDB[0].STATUS_CODE = 390;
                    workOrderDB[0].CHUSER = ChangeUser;
                    workOrderDB[0].CHTS = DateTime.Now;
                    workOrderDB[0].APPROVAL_DTE = DateTime.Now;
                    workOrderDB[0].PREV_STATUS = statusCode;
                    workOrderDB[0].PREV_DATE = date;
                    workOrderDB[0].PREV_LOC_CD = loc;

                    WOAudit.WO_ID = WOID;
                    WOAudit.CHTS = DateTime.Now;
                    WOAudit.AUDIT_TEXT = "Approved by: " + ChangeUser;
                    WOAudit.CHUSER = ChangeUser;
                    objContext.MESC1TS_WOAUDIT.Add(WOAudit);
                    objContext.SaveChanges();
                }
            }

            catch
            {
            }

            return success;
        }

        public bool InsertWORepair(RepairsView RepairsView, string ModeCode, string ManualCode)
        {
            bool success = false;
            MESC1TS_WOREPAIR WORepair = new MESC1TS_WOREPAIR();

            try
            {
                //WORepair = (from wor in objContext.MESC1TS_WOREPAIR
                //            where wor.REPAIR_CD == RepairsView.RepairCode.RepairCod
                WORepair.WO_ID = RepairsView.WorkOrderID;
                WORepair.REPAIR_CD = RepairsView.RepairCode.RepairCod;
                WORepair.MANUAL_CD = RepairsView.RepairCode.ManualCode;
                WORepair.MODE = RepairsView.RepairCode.ModeCode;
                WORepair.QTY_REPAIRS = (short)RepairsView.Pieces;
                WORepair.SHOP_MATERIAL_AMT = RepairsView.MaterialCost;
                WORepair.CPH_MATERIAL_AMT = RepairsView.MaterialCostCPH;
                WORepair.ACTUAL_MANH = RepairsView.ManHoursPerPiece;
                WORepair.NONS_CD = RepairsView.NonsCode.NonsCd;
                WORepair.CHUSER = RepairsView.RepairCode.ChangeUser;
                WORepair.CHTS = DateTime.Now;
                WORepair.DAMAGE_CD = RepairsView.Damage.DamageCedexCode;
                WORepair.REPAIR_LOC_CD = RepairsView.RepairLocationCode.CedexCode;
                WORepair.TPI_CD = RepairsView.Tpi.CedexCode;
                objContext.MESC1TS_WOREPAIR.Add(WORepair);
                objContext.SaveChanges();
            }

            catch(Exception ex)
            {
            }
            return success;
        }

        public bool UpdateWORepair(RepairsView RepairsView)
        {
            bool success = false;
            MESC1TS_WOREPAIR WORepair = new MESC1TS_WOREPAIR();

            try
            {
                WORepair = (from wor in objContext.MESC1TS_WOREPAIR
                            where wor.WO_ID == RepairsView.WorkOrderID &&
                            wor.REPAIR_CD == RepairsView.RepairCode.RepairCod &&
                            wor.REPAIR_LOC_CD == RepairsView.RepairLocationCode.CedexCode
                            select wor).First();
                
                WORepair.WO_ID = RepairsView.WorkOrderID;
                WORepair.REPAIR_CD = RepairsView.RepairCode.RepairCod;
                WORepair.MANUAL_CD = RepairsView.RepairCode.ManualCode;
                WORepair.MODE = RepairsView.RepairCode.ModeCode;
                WORepair.QTY_REPAIRS = (short)RepairsView.Pieces;
                WORepair.SHOP_MATERIAL_AMT = RepairsView.MaterialCost;
                WORepair.CPH_MATERIAL_AMT = RepairsView.MaterialCostCPH;
                WORepair.ACTUAL_MANH = RepairsView.ManHoursPerPiece;
                WORepair.NONS_CD = RepairsView.NonsCode.NonsCd;
                WORepair.CHUSER = RepairsView.RepairCode.ChangeUser;
                WORepair.CHTS = DateTime.Now;
                WORepair.DAMAGE_CD = RepairsView.Damage.DamageCedexCode;
                WORepair.REPAIR_LOC_CD = RepairsView.RepairLocationCode.CedexCode;
                WORepair.TPI_CD = RepairsView.Tpi.CedexCode;
                //objContext.MESC1TS_WOREPAIR.Add(WORepair);
                objContext.SaveChanges();
            }

            catch (Exception ex)
            {
            }
            return success;
        }

        public bool DeleteWORepair(RepairsView RepairsView)
        {
            bool success = false;
            MESC1TS_WOREPAIR WORepair = new MESC1TS_WOREPAIR();

            try
            {
                WORepair = (from wor in objContext.MESC1TS_WOREPAIR
                            where wor.WO_ID == RepairsView.WorkOrderID &&
                            wor.REPAIR_CD == RepairsView.RepairCode.RepairCod &&
                            wor.REPAIR_LOC_CD == RepairsView.RepairLocationCode.CedexCode
                            select wor).First();

                objContext.MESC1TS_WOREPAIR.Remove(WORepair);
                objContext.SaveChanges();
            }

            catch (Exception ex)
            {
            }
            return success;
        }

        public bool InsertWOPart(SparePartsView SparePartsView)
        {
            bool success = false;
            MESC1TS_WOPART WOPart = new MESC1TS_WOPART();

            try
            {
                //WORepair = (from wor in objContext.MESC1TS_WOREPAIR
                //            where wor.REPAIR_CD == RepairsView.RepairCode.RepairCod
                WOPart.WO_ID = SparePartsView.WorkOrderID;
                WOPart.REPAIR_CD = SparePartsView.RepairCode.RepairCod;
                WOPart.PART_CD = SparePartsView.PartCode.ToString();
                WOPart.COST_LOCAL = (decimal)SparePartsView.CostLocal;
                WOPart.COST_CPH = (decimal)SparePartsView.CostLocalCPH;
                WOPart.SERIAL_NUMBER = SparePartsView.SerialNumber;
                WOPart.QTY_PARTS = SparePartsView.Pieces;
                WOPart.CHUSER = SparePartsView.RepairCode.ChangeUser;
                WOPart.CHTS = DateTime.Now;
                objContext.MESC1TS_WOPART.Add(WOPart);
                objContext.SaveChanges();
            }

            catch (Exception ex)
            {
            }
            return success;
        }

        public bool UpdateWOPart(SparePartsView SparePartsView)
        {
            bool success = false;
            MESC1TS_WOPART WOPart = new MESC1TS_WOPART();

            try
            {
                WOPart = (from wop in objContext.MESC1TS_WOPART
                            where wop.WO_ID == SparePartsView.WorkOrderID &&
                            wop.REPAIR_CD == SparePartsView.RepairCode.RepairCod &&
                            wop.PART_CD == SparePartsView.PartCode.ToString()
                            select wop).First();

                WOPart.WO_ID = SparePartsView.WorkOrderID;
                WOPart.REPAIR_CD = SparePartsView.RepairCode.RepairCod;
                WOPart.PART_CD = SparePartsView.PartCode.ToString();
                WOPart.COST_LOCAL = (decimal)SparePartsView.CostLocal;
                WOPart.COST_CPH = (decimal)SparePartsView.CostLocalCPH;
                WOPart.SERIAL_NUMBER = SparePartsView.SerialNumber;
                WOPart.QTY_PARTS = SparePartsView.Pieces;
                WOPart.CHUSER = SparePartsView.RepairCode.ChangeUser;
                WOPart.CHTS = DateTime.Now;
                objContext.MESC1TS_WOPART.Add(WOPart);
                objContext.SaveChanges();
            }

            catch (Exception ex)
            {
            }
            return success;
        }

        public bool DeleteWOPart(SparePartsView SparePartsView)
        {
            bool success = false;
            MESC1TS_WOPART WOPart = new MESC1TS_WOPART();

            try
            {
                WOPart = (from wor in objContext.MESC1TS_WOPART
                            where wor.WO_ID == SparePartsView.WorkOrderID &&
                            wor.REPAIR_CD == SparePartsView.RepairCode.RepairCod &&
                            wor.PART_CD == SparePartsView.PartCode.ToString()
                            select wor).First();

                objContext.MESC1TS_WOPART.Remove(WOPart);
                objContext.SaveChanges();
            }

            catch (Exception ex)
            {
            }
            return success;
        }
    }
}
