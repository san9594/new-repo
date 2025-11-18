using DGP.Filters;
using EntityLayer;
using EntityLayer.DGPTransaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MasterEntity = EntityLayer.DGPTransaction.RequestAgencyEntity;
using MasterEntityClient = EntityLayer.DGPTransaction.RequestEntity;
namespace WebAPILayer.Transaction
{
    public class BIBudgetController : ApiController
    {


        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionStrings"].ToString();
        public static string AdminConnectionString = ConfigurationManager.ConnectionStrings["AdminConnectionString"].ToString();
        MemoryCache globalSessionHandlerForApis;

        String ExceptionReport = String.Empty;
        StringBuilder sb;

        public BIBudgetController()
        {
            globalSessionHandlerForApis = MemoryCache.Default;
            sb = new StringBuilder();
        }

        [HttpGet]
        [ActionName("CheckAPI")]
        public string CheckAPI()
        {
            return "API Working";
        }
        [DontValidate]
        [HttpGet]
        [ActionName("BudgetFlag")]
        public object BudgetFlag()
        {
            string strMsg = "";
            try
            {
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 7;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                        strMsg = ds.Rows[0][0].ToString();
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            ArrayList obj = new ArrayList();
            obj.Add(new
            {
                isBudgetMandatory = strMsg,
            });

            return obj;
        }

        [DontValidate]
        [HttpGet]
        [ActionName("getBudgetBucketList")]
        public object getBudgetBucketList(string userCode, string Target, string AreaCode)
        {
            var strMsg = "";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            //OperationResult obj = new OperationResult();
            List<BudgetBucketTable> data = new List<BudgetBucketTable>();
            List<CurrentMonthBuffer> data1 = new List<CurrentMonthBuffer>();
            List<LastMonthBuffer> data2 = new List<LastMonthBuffer>();
            try
            {
                string SapFrnFlag = "";
                try
                {
                    using (SqlConnection con = new SqlConnection(AdminConnectionString))
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 17;
                            SqlDataAdapter ada = new SqlDataAdapter(cmd);
                            DataTable ds1 = new DataTable();
                            ada.Fill(ds1);
                            SapFrnFlag = ds1.Rows[0][0].ToString();
                        }

                    }
                }
                catch (SqlException e)
                {
                    SapFrnFlag = e.Message.ToString();
                }
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 8;
                        cmd.Parameters.Add("@UserCode", SqlDbType.VarChar).Value = userCode;
                        cmd.Parameters.Add("@Target", SqlDbType.VarChar).Value = Target;
                        cmd.Parameters.Add("@AreaCode", SqlDbType.VarChar).Value = AreaCode;
                        //cmd.Parameters.Add("@AreaOfficeCode", SqlDbType.VarChar).Value = AreaOfficeCode;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        DataTable Dt = ds.Tables[0];
                        if (SapFrnFlag == "False")
                        {
                            if (Dt != null)
                            {
                                data = (from DataRow dr in Dt.Rows
                                        select new BudgetBucketTable()
                                        {
                                            BucketTitle = dr["BucketTitle"].ToString(),
                                            BudgetBucketHeadCode = Convert.ToInt32(dr["BudgetBucketHeadCode"]),
                                            Period = dr["Period"].ToString(),
                                            Branch1 = dr["Branch1"].ToString(),
                                            Branch2 = dr["Branch2"].ToString(),
                                            Amount = Convert.ToDecimal(dr["Amount"]),
                                            FRNNo = dr["FRNNo"].ToString(),
                                            GLCode = dr["GLCode"].ToString(),
                                            CcCode = dr["CcCode"].ToString(),
                                            Branch1Code = dr["Branch1Code"].ToString(),
                                            Branch2Code = dr["Branch2Code"].ToString(),
                                            FRNSrNo = Convert.ToInt32(dr["FRNSrNo"]),
                                            BranchDetailsCode = Convert.ToInt32(dr["BranchDetailsCode"]),
                                            Description = dr["Description"].ToString(),
                                            //TotalBudget = dr["TotalBudget"].ToString(),
                                            //ActualBudget = dr["ActualBudget"].ToString()
                                        }).ToList();
                            }
                        }
                        else
                        {
                            if (Dt != null)
                            {
                                data = (from DataRow dr in Dt.Rows
                                        select new BudgetBucketTable()
                                        {
                                            BucketTitle = dr["BucketTitle"].ToString(),
                                            BudgetBucketHeadCode = Convert.ToInt32(dr["BudgetBucketHeadCode"]),
                                            Period = dr["Period"].ToString(),
                                            Branch1 = dr["Branch1"].ToString(),
                                            Branch2 = dr["Branch2"].ToString(),
                                            Amount = Convert.ToDecimal(dr["Amount"]),
                                            //FRNNo = dr["FRNNo"].ToString(),
                                            GLCode = dr["GLCode"].ToString(),
                                            CcCode = dr["CcCode"].ToString(),
                                            Branch1Code = dr["Branch1Code"].ToString(),
                                            Branch2Code = dr["Branch2Code"].ToString(),
                                            //FRNSrNo = Convert.ToInt32(dr["FRNSrNo"]),
                                            BranchDetailsCode = Convert.ToInt32(dr["BranchDetailsCode"]),
                                            //Description = dr["Description"].ToString(),
                                            //TotalBudget = dr["TotalBudget"].ToString(),
                                            //ActualBudget = dr["ActualBudget"].ToString()
                                        }).ToList();
                            }
                        }
                        if (Target == "Regional Sales")
                        {
                            if (ds.Tables[1].Rows.Count > 0)
                            {
                                DataTable CurrentBuffer = ds.Tables[1];
                                if (CurrentBuffer != null)
                                {
                                    data1 = (from DataRow dr in CurrentBuffer.Rows
                                             select new CurrentMonthBuffer()
                                             {
                                                 CurrentTotalBudget = dr["CurrentTotalBudget"].ToString(),
                                                 CurrentActualBudget = dr["CurrentActualBudget"].ToString()
                                             }).ToList();
                                }
                            }
                            if (ds.Tables[2].Rows.Count > 0)
                            {
                                DataTable LastBuffer = ds.Tables[2];
                                if (LastBuffer != null)
                                {
                                    data2 = (from DataRow dr in LastBuffer.Rows
                                             select new LastMonthBuffer()
                                             {
                                                 LastMonthTotalBudget = dr["LastMonthTotalBudget"].ToString(),
                                                 LastMonthActualBudget = dr["LastMonthActualBudget"].ToString()
                                             }).ToList();
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            dict.Add("BudgetBucketTable", data);
            dict.Add("Buffer", data1);
            dict.Add("PreBuffer", data2);
            return dict;
        }
        [DontValidate]
        [HttpGet]
        [ActionName("DGPBudgetBuckeConsumption")]
        public object DGPBudgetBuckeConsumption(string BucketHeadCode, string Target, string AreaOfficeID, string BrandList, string POTypes)
        {
            string strMsg = "";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            List<BudgetBucketTable> data = new List<BudgetBucketTable>();
            List<CurrentMonthBuffer> data1 = new List<CurrentMonthBuffer>();
            List<LastMonthBuffer> data2 = new List<LastMonthBuffer>();
            try
            {
                string SapFrnFlag = "";
                try
                {
                    using (SqlConnection con = new SqlConnection(AdminConnectionString))
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 17;
                            SqlDataAdapter ada = new SqlDataAdapter(cmd);
                            DataTable ds1 = new DataTable();
                            ada.Fill(ds1);
                            SapFrnFlag = ds1.Rows[0][0].ToString();
                        }

                    }
                }
                catch (SqlException e)
                {
                    SapFrnFlag = e.Message.ToString();
                }
                if (BucketHeadCode == null || BucketHeadCode == "undefined" || Target == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(AdminConnectionString))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 12;
                            cmd.Parameters.Add("@BucketCods", SqlDbType.VarChar).Value = (BucketHeadCode).ToString();
                            cmd.Parameters.Add("@Target", SqlDbType.VarChar).Value = Target.ToString();
                            cmd.Parameters.Add("@AreaOfficeID", SqlDbType.VarChar).Value = AreaOfficeID.ToString();
                            cmd.Parameters.Add("@BrandList", SqlDbType.VarChar).Value = BrandList.ToString();
                            cmd.Parameters.Add("@POTypes", SqlDbType.VarChar).Value = POTypes.ToString();
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            DataTable Dt = ds.Tables[0];
                            if (SapFrnFlag == "False")
                            {
                                if (Dt != null)
                                {
                                    data = (from DataRow dr in Dt.Rows
                                            select new BudgetBucketTable()
                                            {
                                                BucketTitle = dr["BucketTitle"].ToString(),
                                                BudgetBucketHeadCode = Convert.ToInt32(dr["BudgetBucketHeadCode"]),
                                                Period = dr["Period"].ToString(),
                                                Branch1 = dr["Branch1"].ToString(),
                                                Branch2 = dr["Branch2"].ToString(),
                                                Amount = Convert.ToDecimal(dr["Amount"]),
                                                FRNNo = dr["FRNNo"].ToString(),
                                                GLCode = dr["GLCode"].ToString(),
                                                CcCode = dr["CcCode"].ToString(),
                                                Branch1Code = dr["Branch1Code"].ToString(),
                                                Branch2Code = dr["Branch2Code"].ToString(),
                                                FRNSrNo = Convert.ToInt32(dr["FRNSrNo"]),
                                                BranchDetailsCode = Convert.ToInt32(dr["BranchDetailsCode"]),
                                                IsClosed = Convert.ToBoolean(dr["IsClosed"]),
                                                IsBlocked = Convert.ToBoolean(dr["IsBlocked"])
                                            }).ToList();
                                }
                            }
                            else
                            {
                                if (Dt != null)
                                {
                                    data = (from DataRow dr in Dt.Rows
                                            select new BudgetBucketTable()
                                            {
                                                BucketTitle = dr["BucketTitle"].ToString(),
                                                BudgetBucketHeadCode = Convert.ToInt32(dr["BudgetBucketHeadCode"]),
                                                Period = dr["Period"].ToString(),
                                                Branch1 = dr["Branch1"].ToString(),
                                                Branch2 = dr["Branch2"].ToString(),
                                                Amount = Convert.ToDecimal(dr["Amount"]),
                                                //FRNNo = dr["FRNNo"].ToString(),
                                                GLCode = dr["GLCode"].ToString(),
                                                CcCode = dr["CcCode"].ToString(),
                                                Branch1Code = dr["Branch1Code"].ToString(),
                                                Branch2Code = dr["Branch2Code"].ToString(),
                                                //FRNSrNo = Convert.ToInt32(dr["FRNSrNo"]),
                                                BranchDetailsCode = Convert.ToInt32(dr["BranchDetailsCode"]),
                                                IsClosed = Convert.ToBoolean(dr["IsClosed"]),
                                                IsBlocked = Convert.ToBoolean(dr["IsBlocked"])
                                            }).ToList();
                                }
                            }
                            if (Target == "Regional Sales")
                            {
                                if (ds.Tables[1].Rows.Count > 0)
                                {
                                    DataTable CurrentBuffer = ds.Tables[1];
                                    if (CurrentBuffer != null)
                                    {
                                        data1 = (from DataRow dr in CurrentBuffer.Rows
                                                 select new CurrentMonthBuffer()
                                                 {
                                                     CurrentTotalBudget = dr["CurrentTotalBudget"].ToString(),
                                                     CurrentActualBudget = dr["CurrentActualBudget"].ToString()
                                                 }).ToList();
                                    }
                                }
                                if (ds.Tables[2].Rows.Count > 0)
                                {
                                    DataTable LastBuffer = ds.Tables[2];
                                    if (LastBuffer != null)
                                    {
                                        data2 = (from DataRow dr in LastBuffer.Rows
                                                 select new LastMonthBuffer()
                                                 {
                                                     LastMonthTotalBudget = dr["LastMonthTotalBudget"].ToString(),
                                                     LastMonthActualBudget = dr["LastMonthActualBudget"].ToString()
                                                 }).ToList();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }

            dict.Add("BudgetBucketTable", data);
            dict.Add("Buffer", data1);
            dict.Add("PreBuffer", data2);
            return dict;
        }

        [DontValidate]
        [HttpPost]
        [ActionName("SaveBudgetBucket")]
        public object SaveBudgetBucket(string RequestID, string RequestNumber, int RequestDetailID, [FromBody] List<BucketFields> BucketFields)
        {
            OperationResult obj = new OperationResult();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            string strMsg = "";
            obj.Result = true;
            var BucketTitle = "";
            var RequestDetail = "";
            try
            {
                if (BucketFields == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[DGP].[DealerBillNoCheck]", con))
                    {
                        foreach (var oEntity in BucketFields)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 3;
                            cmd.Parameters.Add("@RequestNumber", SqlDbType.VarChar).Value = RequestNumber;
                            cmd.Parameters.Add("@RequestID", SqlDbType.VarChar).Value = RequestID;
                            cmd.Parameters.Add("@PONO", SqlDbType.VarChar).Value = oEntity.PONO;
                            cmd.Parameters.Add("@BudgetLinkedCode", SqlDbType.VarChar).Value = oEntity.Code;
                            cmd.Parameters.Add("@Ratio", SqlDbType.VarChar).Value = oEntity.Ratio;
                            cmd.Parameters.Add("@BudgetType", SqlDbType.VarChar).Value = oEntity.BudgetType;
                            cmd.Parameters.Add("@CcCode", SqlDbType.VarChar).Value = oEntity.CcCode;
                            cmd.Parameters.Add("@GLCode", SqlDbType.VarChar).Value = oEntity.GLCode;
                            cmd.Parameters.Add("@BucketTitle", SqlDbType.VarChar).Value = oEntity.BucketTitle;
                            cmd.Parameters.Add("@FrnNo", SqlDbType.VarChar).Value = oEntity.FrnNo;
                            cmd.Parameters.Add("@FrnSrNo", SqlDbType.VarChar).Value = oEntity.FrnSrNo;
                            cmd.Parameters.Add("@BucketHeadCode", SqlDbType.VarChar).Value = oEntity.BucketHeadCode;
                            cmd.Parameters.Add("@FrnDescription", SqlDbType.VarChar).Value = oEntity.FrnDescription;
                            cmd.Parameters.Add("@RequestDetailID", SqlDbType.VarChar).Value = RequestDetailID;
                            cmd.Parameters.Add("@BrandName", SqlDbType.VarChar).Value = oEntity.BrandName;
                            cmd.Parameters.Add("@Period", SqlDbType.VarChar).Value = oEntity.Period;
                            cmd.Parameters.Add("@IsBlocked", SqlDbType.VarChar).Value = oEntity.IsBlocked;
                            cmd.Parameters.Add("@IsClosed", SqlDbType.VarChar).Value = oEntity.IsClosed;
                            cmd.Parameters.Add("@Amount", SqlDbType.VarChar).Value = oEntity.Amount;
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable ds = new DataTable();
                            adapter.Fill(ds);
                            BucketTitle = ds.Rows[0][0].ToString();
                            RequestDetail = ds.Rows[0][1].ToString();
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
                obj.Result = false;
                obj.Message = strMsg;
            }
            dict.Add("Data", obj);
            dict.Add("BucketTitle", BucketTitle);
            dict.Add("ReqDetailID", RequestDetail);
            //obj.Data = JsonSerializer.Deserialize<OperationResult>(BucketTutle);
            return dict;
        }

        [DontValidate]
        [HttpPost]
        [ActionName("DeleteBudgetBucket")]
        public object DeleteBudgetBucket(string RequestID, int RequestDetailID)
        {
            OperationResult obj = new OperationResult();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            string strMsg = "";
            //obj.Result = true;
            var BucketStatus = "";
            try
            {

                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[DGP].[DealerBillNoCheck]", con))
                    {

                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 13;
                        cmd.Parameters.Add("@RequestID", SqlDbType.VarChar).Value = RequestID;
                        cmd.Parameters.Add("@RequestDetailID", SqlDbType.VarChar).Value = RequestDetailID;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                        BucketStatus = ds.Rows[0][0].ToString();
                        //BucketTitle = ds.Rows[0][0].ToString();
                        //RequestDetail = ds.Rows[0][1].ToString();

                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
                obj.Result = false;
                obj.Message = strMsg;
            }
            if (BucketStatus == "True")
            {
                obj.Result = true;
            }
            else
            {
                obj.Result = false;
            }

            //dict.Add("Data", obj);
            //dict.Add("BucketStatus", BucketStatus);
            //dict.Add("ReqDetailID", RequestDetail);
            //obj.Data = JsonSerializer.Deserialize<OperationResult>(BucketTutle);
            return obj;
        }

        [DontValidate]
        [HttpPut]
        [ActionName("UpdatePoNoInLinked")]
        public object UpdatePoNoInLinked(string RequestID, string PONO)
        {
            string strMsg = "";
            //var Bucket = data.code;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[DGP].[DealerBillNoCheck]", con))
                    {

                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 9;
                        cmd.Parameters.Add("@RequestID", SqlDbType.VarChar).Value = RequestID;
                        cmd.Parameters.Add("@PONO", SqlDbType.VarChar).Value = PONO;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }

            return strMsg;
        }

        [DontValidate]
        [HttpDelete]
        [ActionName("DeleteBucketOnResubmit")]
        public object DeleteBucketOnResubmit(string RequestID, string RequestNumber)
        {
            string strMsg = "";
            //var Bucket = data.code;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[DGP].[DealerBillNoCheck]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 7;
                        cmd.Parameters.Add("@RequestNumber", SqlDbType.VarChar).Value = RequestNumber;
                        cmd.Parameters.Add("@RequestID", SqlDbType.VarChar).Value = RequestID;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }

            return strMsg;
        }

        [DontValidate]
        [HttpPut]
        [ActionName("UpdatePONumber")]
        public object UpdatePONumber(string RequestID, string RequestNumber, string PONO)
        {

            string strMsg = "";
            //var Bucket = data.code;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[DGP].[DealerBillNoCheck]", con))
                    {

                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 7;
                        cmd.Parameters.Add("@RequestNumber", SqlDbType.VarChar).Value = RequestNumber;
                        cmd.Parameters.Add("@RequestID", SqlDbType.VarChar).Value = RequestID;
                        cmd.Parameters.Add("@PONO", SqlDbType.VarChar).Value = PONO;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);


                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [DontValidate]
        [HttpPost]
        [ActionName("BudgetBucketConsumption")]
        public object BudgetBucketConsumption(string PONO, string BillNo, string Medium, string HeadCode, string RequestID, string Amount, string userCode, string DocType, string Action,
        string TotalBillableAmt, string MultiDealerAmt, string ActivityID, bool IsCompletionApp, bool PartsCompletion, string CompParentActivityID,string BucketRatio)
        {
            string strMsg = "";
            OperationResult obj = new OperationResult();
            using (SqlConnection con = new SqlConnection(AdminConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 9;
                        cmd.Parameters.Add("@DocType", SqlDbType.VarChar).Value = DocType;
                        cmd.Parameters.Add("@Medium", SqlDbType.VarChar).Value = Medium;
                        cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = Action;
                        cmd.Parameters.Add("@PoNumber", SqlDbType.VarChar).Value = PONO;
                        cmd.Parameters.Add("@DocID", SqlDbType.VarChar).Value = RequestID;
                        cmd.Parameters.Add("@BudgetLinkedCode", SqlDbType.VarChar).Value = HeadCode;
                        cmd.Parameters.Add("@ConsumedAmount", SqlDbType.VarChar).Value = Amount;
                        cmd.Parameters.Add("@UserCode", SqlDbType.VarChar).Value = userCode;
                        cmd.Parameters.Add("@BillNo", SqlDbType.VarChar).Value = BillNo;
                        cmd.Parameters.Add("@DealerAmount", SqlDbType.VarChar).Value = MultiDealerAmt;
                        cmd.Parameters.Add("@TotalPOAmount", SqlDbType.VarChar).Value = TotalBillableAmt;
                        cmd.Parameters.Add("@ActivityID", SqlDbType.VarChar).Value = ActivityID;
                        cmd.Parameters.Add("@IsCompletionApp", SqlDbType.VarChar).Value = IsCompletionApp;
                        cmd.Parameters.Add("@PartsCompletion", SqlDbType.VarChar).Value = PartsCompletion;
                        cmd.Parameters.Add("@CompParentActivityID", SqlDbType.VarChar).Value = CompParentActivityID;
                        cmd.Parameters.Add("@BucketRatio", SqlDbType.VarChar).Value = BucketRatio;
                        //cmd.Parameters.Add("@BudgetLinkedCode", SqlDbType.VarChar).Value = ConsumptioParams;
                        //cmd.Parameters.Add("@BudgetLinkedCode", SqlDbType.VarChar).Value = ConsumptioParams;
                        //cmd.Parameters.Add("@AreaOfficeCode", SqlDbType.VarChar).Value = AreaOfficeCode;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                    }
                }
                catch (SqlException ex)
                {
                    strMsg = ex.Message.ToString();
                }
            }
            return strMsg;
        }

        [DontValidate]
        [HttpPost]
        [ActionName("ResubmitBucketDetails")]
        public object ResubmitBucketDetails(string HeadCode, string RequestID, string Amount, string DocType, string PONO, string BillNo, string TotalBillableAmt, string MultiDealerAmt, string ActivityID, bool IsCompletionApp, bool PartsCompletion)
        {
            string strMsg = "";
            try
            {
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 13;
                        cmd.Parameters.Add("@DocType", SqlDbType.VarChar).Value = DocType;
                        cmd.Parameters.Add("@PoNumber", SqlDbType.VarChar).Value = PONO;
                        cmd.Parameters.Add("@DocID", SqlDbType.VarChar).Value = RequestID;
                        cmd.Parameters.Add("@BudgetLinkedCode", SqlDbType.VarChar).Value = HeadCode;
                        cmd.Parameters.Add("@ConsumedAmount", SqlDbType.VarChar).Value = Amount;
                        cmd.Parameters.Add("@BillNo", SqlDbType.VarChar).Value = BillNo;
                        cmd.Parameters.Add("@DealerAmount", SqlDbType.VarChar).Value = MultiDealerAmt;
                        cmd.Parameters.Add("@TotalPOAmount", SqlDbType.VarChar).Value = TotalBillableAmt;
                        cmd.Parameters.Add("@ActivityID", SqlDbType.VarChar).Value = ActivityID;
                        cmd.Parameters.Add("@IsCompletionApp", SqlDbType.VarChar).Value = IsCompletionApp;
                        cmd.Parameters.Add("@PartsCompletion", SqlDbType.VarChar).Value = PartsCompletion;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                String ErrorCategory = String.Empty;
                String ErrorMessage = String.Empty;
                ErrorMessage = ex.Message.ToString();
                ExceptionReport = "<style>" +
                                  "table {font-size:15px}" +
                                  "p {font-size:16px}" +
                                  "</style>" +
                                  "<p style=\"color:red;font-size:16px;\">" + ErrorCategory + "</p><hr>" +
                                  "<table style=\"width:100%\"><tr><th>Error Code</th><th>Description</th></tr>" +
                                  ErrorMessage +
                                  "</table>";
                strMsg = ex.Message.ToString();
            }
            return strMsg;
        }

        [DontValidate]
        [HttpGet]
        [ActionName("GetBrandWiseBucket")]
        public object GetBrandWiseBucket(string userCode, string Target, string AreaCode, string BrandName, string ExpenseType)
        {
            var strMsg = "";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            //OperationResult obj = new OperationResult();
            List<BudgetBucketTable> data = new List<BudgetBucketTable>();
            List<CurrentMonthBuffer> data1 = new List<CurrentMonthBuffer>();
            List<LastMonthBuffer> data2 = new List<LastMonthBuffer>();
            try
            {
                string SapFrnFlag = "";
                try
                {
                    using (SqlConnection con = new SqlConnection(AdminConnectionString))
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 17;
                            SqlDataAdapter ada = new SqlDataAdapter(cmd);
                            DataTable ds1 = new DataTable();
                            ada.Fill(ds1);
                            SapFrnFlag = ds1.Rows[0][0].ToString();
                        }
                    }
                }
                catch (SqlException e)
                {
                    SapFrnFlag = e.Message.ToString();
                }
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 18;
                        cmd.Parameters.Add("@UserCode", SqlDbType.VarChar).Value = userCode;
                        cmd.Parameters.Add("@Target", SqlDbType.VarChar).Value = Target;
                        cmd.Parameters.Add("@AreaCode", SqlDbType.VarChar).Value = AreaCode;
                        cmd.Parameters.Add("@BrandName", SqlDbType.VarChar).Value = BrandName;
                        cmd.Parameters.Add("@ExpenseType", SqlDbType.VarChar).Value = ExpenseType;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        DataTable Dt = ds.Tables[0];
                        if (SapFrnFlag == "False")
                        {
                            if (Dt != null)
                            {
                                data = (from DataRow dr in Dt.Rows
                                        select new BudgetBucketTable()
                                        {
                                                                        BucketTitle = dr["BucketTitle"].ToString(),
                            BudgetBucketHeadCode = Convert.ToInt32(dr["BudgetBucketHeadCode"]),
                            Period = dr["Period"].ToString(),
                            Branch1 = dr["Branch1"].ToString(),
                            Branch2 = dr["Branch2"].ToString(),
                            Amount = Convert.ToDecimal(dr["Amount"]),
                            FRNNo = dr["FRNNo"].ToString(),
                                            GLCode = dr["GLCode"].ToString(),
                                            CcCode = dr["CcCode"].ToString(),
                                            Branch1Code = dr["Branch1Code"].ToString(),
                                            Branch2Code = dr["Branch2Code"].ToString(),
                                            FRNSrNo = Convert.ToInt32(dr["FRNSrNo"]),
                                            BranchDetailsCode = Convert.ToInt32(dr["BranchDetailsCode"]),
                                            Description = dr["Description"].ToString(),
                                            //TotalBudget = dr["TotalBudget"].ToString(),
                                            //ActualBudget = dr["ActualBudget"].ToString()
                                        }).ToList();
                            }
                        }
                        else
                        {
                            if (Dt != null)
                            {
                                data = (from DataRow dr in Dt.Rows
                                        select new BudgetBucketTable()
                                        {
                                                                        BucketTitle = dr["BucketTitle"].ToString(),
                            BudgetBucketHeadCode = Convert.ToInt32(dr["BudgetBucketHeadCode"]),
                            Period = dr["Period"].ToString(),
                            Branch1 = dr["Branch1"].ToString(),
                            Branch2 = dr["Branch2"].ToString(),
                            Amount = Convert.ToDecimal(dr["Amount"]),
                            //FRNNo = dr["FRNNo"].ToString(),
                                            GLCode = dr["GLCode"].ToString(),
                                            CcCode = dr["CcCode"].ToString(),
                                            Branch1Code = dr["Branch1Code"].ToString(),
                                            Branch2Code = dr["Branch2Code"].ToString(),
                                            //FRNSrNo = Convert.ToInt32(dr["FRNSrNo"]),
                                            BranchDetailsCode = Convert.ToInt32(dr["BranchDetailsCode"]),
                                            //Description = dr["Description"].ToString(),
                                            //TotalBudget = dr["TotalBudget"].ToString(),
                                            BudgetType = dr["BudgetType"].ToString()
                                        }).ToList();
                            }
                        }
                        if (Target == "Regional Sales")
                        {
                            if (ds.Tables[1].Rows.Count > 0)
                            {
                                DataTable CurrentBuffer = ds.Tables[1];
                                if (CurrentBuffer != null)
                                {
                                    data1 = (from DataRow dr in CurrentBuffer.Rows
                                             select new CurrentMonthBuffer()
                                             {
                                                 CurrentTotalBudget = dr["CurrentTotalBudget"].ToString(),
                                                 CurrentActualBudget = dr["CurrentActualBudget"].ToString()
                                             }).ToList();
                                }
                            }
                            if (ds.Tables[2].Rows.Count > 0)
                            {
                                DataTable LastBuffer = ds.Tables[2];
                                if (LastBuffer != null)
                                {
                                    data2 = (from DataRow dr in LastBuffer.Rows
                                             select new LastMonthBuffer()
                                             {
                                                 LastMonthTotalBudget = dr["LastMonthTotalBudget"].ToString(),
                                                 LastMonthActualBudget = dr["LastMonthActualBudget"].ToString()
                                             }).ToList();
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            dict.Add("BudgetBucketTable", data);
            dict.Add("Buffer", data1);
            dict.Add("PreBuffer", data2);
            return dict;
        }

        [DontValidate]
        [HttpPost]
        [ActionName("POCloseRevertRemBill")]
        public object POCloseRevertRemBill(string HeadCode, string RequestID, string Amount, string DocType, string PONO, string BillNo)
        {
            string strMsg = "";
            try
            {
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 16;
                        cmd.Parameters.Add("@DocType", SqlDbType.VarChar).Value = DocType;
                        cmd.Parameters.Add("@PoNumber", SqlDbType.VarChar).Value = PONO;
                        cmd.Parameters.Add("@DocID", SqlDbType.VarChar).Value = RequestID;
                        cmd.Parameters.Add("@BudgetLinkedCode", SqlDbType.VarChar).Value = HeadCode;
                        cmd.Parameters.Add("@RemainingBillAmt", SqlDbType.VarChar).Value = Amount;
                        cmd.Parameters.Add("@BillNo", SqlDbType.VarChar).Value = BillNo;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                String ErrorCategory = String.Empty;
                String ErrorMessage = String.Empty;
                ErrorMessage = ex.Message.ToString();

                ExceptionReport = "<style>" +
                                  "table {font-size:15px}" +
                                  "p {font-size:16px}" +
                                  "</style>" +
                                  "<p style=\"color:red;font-size:16px;\">" + ErrorCategory + "</p><hr>" +
                                  "<table style=\"width:100%\"><tr><th>Error Code</th><th>Description</th></tr>" +
                                  ErrorMessage +
                                  "</table>";
                strMsg = ex.Message.ToString();
            }
            return strMsg;
        }
        //TML
        [DontValidate]
        [HttpGet]
        [ActionName("HideSapFrnFlag")] //EInvoice //9DaysPoApproval //KPIMaster //GeoTagging
        public object HideSapFrnFlag()
        {
            string strMsg = "";
            string InvoicePdfValidation = "";
            string IsDealerSplit = "";
            string HideApproverName = "";
            string PO_Approval_PeriodFrom_Date = "";
            string No_Of_Days = "";
            string DGP_MandatoryPerformanceEntry = "";
            string RegionalERPTaxCodeMandatory = ""; //ERPTaxCode
            string DOA_Mandatory = ""; //pFirst
            string IS_Geo_Tagging = ""; //GeoTagging
            string AllowNewActivity = ""; //GeoTagging
            string MSME = ""; //MSMS
            string IsAutoVerifyEnabled = ""; //MSMS
            string IsAutoApproveEnabled = ""; //MSME Auto Approval
            string NoOfDaystoAutoVerifyReq = ""; //MSMS
            string NoOfDaystoAutoApproveReq = ""; //MSMS
            string billdueDateCalculationOn = ""; //MSMS
            string IsInvoiceDateValidation = ""; //MSMS
            string RegionalRemarksMandatory = ""; //MSMS

            try
            {
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con)) //CompAvailableHMCLAmt
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 17;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                        strMsg = ds.Rows[0][0].ToString();
                        InvoicePdfValidation = ds.Rows[0][1].ToString();
                        IsDealerSplit = ds.Rows[0][2].ToString();
                        HideApproverName = ds.Rows[0][3].ToString();
                        PO_Approval_PeriodFrom_Date = ds.Rows[0][4].ToString();
                        No_Of_Days = ds.Rows[0][5].ToString();
                        DGP_MandatoryPerformanceEntry = ds.Rows[0][6].ToString();
                        RegionalERPTaxCodeMandatory = ds.Rows[0][7].ToString();
                        DOA_Mandatory = ds.Rows[0][8].ToString();
                        IS_Geo_Tagging = ds.Rows[0][10].ToString();
                        AllowNewActivity = ds.Rows[0][11].ToString();
                        MSME = ds.Rows[0][12].ToString();
                        IsAutoVerifyEnabled = ds.Rows[0][13].ToString();
                        IsAutoApproveEnabled = ds.Rows[0][14].ToString();
                        NoOfDaystoAutoVerifyReq = ds.Rows[0][15].ToString();
                        NoOfDaystoAutoApproveReq = ds.Rows[0][16].ToString();
                        billdueDateCalculationOn = ds.Rows[0][17].ToString();
                        IsInvoiceDateValidation = ds.Rows[0][18].ToString();
                        RegionalRemarksMandatory = ds.Rows[0][19].ToString();
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            ArrayList obj = new ArrayList();
            obj.Add(new
            {
                HideSapFRN = strMsg,
                InvoicePdfValidation = InvoicePdfValidation,
                IsDealerSplit = IsDealerSplit, //IsDealerSplit
                HideApproverName = HideApproverName, //Verify/Approvar
                PO_Approval_PeriodFrom_Date = PO_Approval_PeriodFrom_Date,
                No_Of_Days = No_Of_Days,
                DGP_MandatoryPerformanceEntry = DGP_MandatoryPerformanceEntry,
                RegionalERPTaxCodeMandatory = RegionalERPTaxCodeMandatory,
                DOA_Mandatory = DOA_Mandatory,
                IS_Geo_Tagging = IS_Geo_Tagging,
                AllowNewActivity = AllowNewActivity,
                MSME = MSME,
                IsAutoVerifyEnabled = IsAutoVerifyEnabled,
                IsAutoApproveEnabled = IsAutoApproveEnabled,
                NoOfDaystoAutoVerifyReq = NoOfDaystoAutoVerifyReq,
                NoOfDaystoAutoApproveReq = NoOfDaystoAutoApproveReq,
                billdueDateCalculationOn = billdueDateCalculationOn,
                IsInvoiceDateValidation = IsInvoiceDateValidation,
                RegionalRemarksMandatory = RegionalRemarksMandatory

            });

            return obj;
        }
        [DontValidate]
        [HttpPost]
        [ActionName("SaveEInvoiceValidation")] //EInvoice
        public async Task<bool> SaveEInvoiceValidation(string RequestID, string EInvoiceTitle, [FromBody] List<ValidationResponse> Response ) //EInvoice
        {
            //bool Result = false;
            //_logger.Information(Response + " : Save Validation Responses.");
            //var billDetails = con.NonMediaBillHeader.Where(x => x.BillNo == Response.billNo).FirstOrDefault();
            //var user = _context.Sec_UserMaster.Where(x => x.UserId == billDetails.CreatedBy).FirstOrDefault();
            var UserId = HttpContext.Current.Request.RequestContext.RouteData.Values["UserID"].ToString();
            try
            {
                if (Response == null)
                {
                    return false;
                }
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[DGP].[DealerBillNoCheck]", conn))
                    {
                        foreach (var oEntity in Response)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("Mode", 10);
                            cmd.Parameters.AddWithValue("BillNo", oEntity.billNo);
                            cmd.Parameters.AddWithValue("PONO", oEntity.PoNo);
                            cmd.Parameters.AddWithValue("Action", oEntity.Action);
                            cmd.Parameters.AddWithValue("FileName", oEntity.FileName);
                            cmd.Parameters.AddWithValue("PoValidation", oEntity.PoValidation);
                            cmd.Parameters.AddWithValue("Type", oEntity.Type);
                            cmd.Parameters.AddWithValue("DataBaseValues", oEntity.DataBaseValue);
                            cmd.Parameters.AddWithValue("InvoicePdfValues", oEntity.InvoicePdfValue);
                            cmd.Parameters.AddWithValue("InvoiceManualInput", oEntity.InvoiceManualInput);
                            cmd.Parameters.AddWithValue("CreatedBy", UserId);
                            cmd.Parameters.AddWithValue("RequestID", RequestID);
                            cmd.Parameters.AddWithValue("EInvoiceTitle", EInvoiceTitle);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable ds = new DataTable();
                            adapter.Fill(ds);
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return true;
        }
        [DontValidate]
        [HttpGet]
        [ActionName("GetValidationData")] //EInvoice
        public object GetValidationData(string RequestId, string BillNo)
        {
            var strMsg = "";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            //OperationResult obj = new OperationResult();
            List<ValidationResponse> data = new List<ValidationResponse>();
            try
            {
                string SapFrnFlag = "";
                try
                {
                    using (SqlConnection con = new SqlConnection(AdminConnectionString))
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 17;
                            SqlDataAdapter ada = new SqlDataAdapter(cmd);
                            DataTable ds1 = new DataTable();
                            ada.Fill(ds1);
                            SapFrnFlag = ds1.Rows[0][0].ToString();
                        }
                    }
                }
                catch (SqlException e)
                {
                    SapFrnFlag = e.Message.ToString();
                }
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[DGP].[DealerBillNoCheck]", conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 11;
                        cmd.Parameters.Add("@RequestId", SqlDbType.VarChar).Value = RequestId;
                        cmd.Parameters.Add("@BillNo", SqlDbType.VarChar).Value = BillNo;
                        //cmd.Parameters.Add("@AreaOfficeCode", SqlDbType.VarChar).Value = AreaOfficeCode;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        DataTable Dt = ds.Tables[0];
                        if (SapFrnFlag == "False" || SapFrnFlag == "True")
                        {
                            if (Dt != null)
                            {
                                data = (from DataRow dr in Dt.Rows
                                        select new ValidationResponse()
                                        {
                                            PoNo = dr["PONO"].ToString(),
                                            PoValidation = dr["Status"].ToString(),
                                            billNo = dr["BillNo"].ToString(),
                                            FileName = dr["FileName"].ToString(),
                                            Action = dr["Remarks"].ToString(),
                                            Type = dr["ValidatedData"].ToString(),
                                            DataBaseValue = dr["DataBaseValues"].ToString(),
                                            pdfValues = dr["InvoicePdfValues"].ToString(),
                                            InvoicePdfValue = dr["InvoicePdfValues"].ToString(),
                                            AmountTitle = dr["AmountTitle"].ToString(),
                                            InvoiceManualInput = dr["InvoiceManualInput"].ToString(),
                                        }).ToList();
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            dict.Add("IvValidationTable", data);
            return dict;
        }


        [DontValidate]
        [HttpGet]
        [ActionName("GetExpenseType")]
        public object GetExpenseType(int id)
        {
            string strMsg = "";
            List<ExpenseTypeTable> data = new List<ExpenseTypeTable>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 19;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        DataTable Dt = ds.Tables[0];
                        if (Dt != null)
                        {
                            data = (from DataRow dr in Dt.Rows
                                    select new ExpenseTypeTable()
                                    {
                                        Name = dr["Name"].ToString(),
                                        POTypeCode = dr["POTypeCode"].ToString(),
                                        Mediums = dr["Mediums"].ToString(),
                                        GLCode = dr["GLCODE"].ToString()
                                    }).ToList();
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            return data;
        }

        [DontValidate]
        [HttpGet]
        [ActionName("GetExpenseTypeByID")]
        public object GetExpenseTypeByID(int id)
        {
            string strMsg = "";
            List<ExpenseTypeTable> data = new List<ExpenseTypeTable>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 23;
                        cmd.Parameters.Add("@ExpenseCode", SqlDbType.VarChar).Value = id;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        DataTable Dt = ds.Tables[0];
                        if (Dt != null)
                        {
                            data = (from DataRow dr in Dt.Rows
                                    select new ExpenseTypeTable()
                                    {
                                        Name = dr["Name"].ToString(),
                                    }).ToList();
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            return data;
        }

        [DontValidate]
        [HttpGet]
        [ActionName("GetBUPlant")]
        public object GetBUPlant()
        {
            string strMsg = "";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            List<GetPlantBU> data = new List<GetPlantBU>();
            List<GetBUUnit> data1 = new List<GetBUUnit>();
            List<GetBURegion> data2 = new List<GetBURegion>();
            List<GetPOGroup> data3 = new List<GetPOGroup>();
            List<GetPOOrganization> data4 = new List<GetPOOrganization>();
            List<GetStateMaster> data5 = new List<GetStateMaster>();
            List<GetServiceType> data6 = new List<GetServiceType>();
            try
            {
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 20;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        DataTable Dt = ds.Tables[0];
                        DataTable Dt1 = ds.Tables[1];
                        DataTable Dt2 = ds.Tables[2];
                        DataTable Dt3 = ds.Tables[3];
                        DataTable Dt4 = ds.Tables[4];
                        DataTable Dt5 = ds.Tables[5];
                        DataTable Dt6 = ds.Tables[6];

                        if (Dt != null)
                        {
                            data = (from DataRow dr in Dt.Rows
                                    select new GetPlantBU()
                                    {
                                        PlantName = dr["PlantName"].ToString(),
                                        PlantCode = dr["PlantCode"].ToString(),
                                        GSTStateCode = Convert.ToInt32(dr["GSTStateCode"].ToString()),
                                    }).ToList();
                        }
                        if (Dt1 != null)
                        {
                            data1 = (from DataRow dr in Dt1.Rows
                                     select new GetBUUnit()
                                     {
                                         BusinessUnitName = dr["BusinessUnitName"].ToString(),
                                         BusinessUnitCode = dr["BusinessUnitCode"].ToString(),
                                     }).ToList();
                        }
                        if (Dt2 != null)
                        {
                            data2 = (from DataRow dr in Dt2.Rows
                                     select new GetBURegion()
                                     {
                                         BusinessRegionName = dr["BusinessRegionName"].ToString(),
                                         BusinessRegionCode = dr["BusinessRegionCode"].ToString(),
                                         BusinessUnitID = dr["BusinessUnitID"].ToString(),
                                     }).ToList();
                        }
                        if (Dt3 != null)
                        {
                            data3 = (from DataRow dr in Dt3.Rows
                                     select new GetPOGroup()
                                     {
                                         PurchaseGroupName = dr["PurchaseGroupName"].ToString(),
                                         PurchaseGroupCode = dr["PurchaseGroupCode"].ToString(),
                                         BusinessRegionId = dr["BusinessRegionId"].ToString(),
                                     }).ToList();
                        }
                        if (Dt4 != null)
                        {
                            data4 = (from DataRow dr in Dt4.Rows
                                     select new GetPOOrganization()
                                     {
                                         purchaseorganizationName = dr["purchaseorganizationName"].ToString(),
                                         purchaseorganizationCode = dr["purchaseorganizationCode"].ToString(),
                                         BusinessRegionId = dr["BusinessRegionId"].ToString(),
                                     }).ToList();
                        }
                        if (Dt5 != null)
                        {
                            data5 = (from DataRow dr in Dt5.Rows
                                     select new GetStateMaster()
                                     {
                                         ID = Convert.ToInt32(dr["ID"]),
                                         Name = dr["Name"].ToString(),
                                         IsIsdState = Convert.ToBoolean(dr["IsIsdState"].ToString()),
                                     }).ToList();
                        }
                        if (Dt6 != null)
                        {
                            data6 = (from DataRow dr in Dt6.Rows
                                     select new GetServiceType()
                                     {
                                         Id = Convert.ToInt32(dr["Id"]),
                                         ServiceType = dr["ServiceType"].ToString(),
                                         TaxType = dr["TaxType"].ToString(),
                                         DisplayRCMTaxCode = Convert.ToBoolean(dr["DisplayRCMTaxCode"].ToString()),
                                     }).ToList();
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            dict.Add("BUPlant", data);
            dict.Add("BUUnit", data1);
            dict.Add("BURegion", data2);
            dict.Add("POGroup", data3);
            dict.Add("POOrganization", data4);
            dict.Add("StateMaster", data5);
            dict.Add("ServiceTypes", data6);
            return dict;
        }

        [DontValidate]
        [HttpGet]
        [ActionName("GetApprovalDocStage")]
        public object GetApprovalDocStage(string ActivityOwnerCode, string RequestType, int? CurrentTransactionStageID = null, int? NextTransactionStageID = null)
        {
            string strMsg = "";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            List<GetApprovalDocStage> data = new List<GetApprovalDocStage>();
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[DGP].[DealerBillNoCheck]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 22; // You can change this mode as needed
                        cmd.Parameters.Add("@ActivityOwnerCode", SqlDbType.VarChar).Value = ActivityOwnerCode ?? "";
                        cmd.Parameters.Add("@RequestType", SqlDbType.VarChar).Value = RequestType ?? "";
                        cmd.Parameters.Add("@CurrentTransactionStageID", SqlDbType.Int).Value = CurrentTransactionStageID ?? (object)DBNull.Value;
                        cmd.Parameters.Add("@NextTransactionStageID", SqlDbType.Int).Value = NextTransactionStageID ?? (object)DBNull.Value;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        
                        // Check if dataset has tables and the first table exists
                        if (ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0] != null)
                        {
                            DataTable Dt = ds.Tables[0];
                            
                            if (Dt.Rows != null && Dt.Rows.Count > 0)
                            {
                                data = (from DataRow dr in Dt.Rows
                                        select new GetApprovalDocStage()
                                        {
                                            ID = Convert.ToInt32(dr["ID"]),
                                            NextProfileID = Convert.ToInt32(dr["NextProfileID"]),
                                            NextProfileName = dr["NextProfileName"].ToString(),
                                            NextSequence = Convert.ToInt32(dr["NextSequence"]),
                                            NextTransactionStageID = Convert.ToInt32(dr["NextTransactionStageID"]),
                                            NextTransactionStageName = dr["NextTransactionStageName"].ToString(),
                                            IsStage = Convert.ToBoolean(dr["IsStage"]),
                                            ActivityOwnerCode = dr["ActivityOwnerCode"].ToString(),
                                            CreatedOn = dr["CreatedOn"].ToString(),
                                            CreatedBy = dr["CreatedBy"] != DBNull.Value ? dr["CreatedBy"].ToString() : null,
                                            RequestType = dr["RequestType"].ToString(),
                                            UserID = Convert.ToInt32(dr["UserID"])
                                        }).ToList();
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            dict.Add("ApprovalDocStage", data);
            return dict;
        }


        [DontValidate]
        [HttpGet]
        [ActionName("GetERPTaxCode")] //ERPTaxCode
        public object GetERPTaxCode(int id)
        {
            string strMsg = "";
            List<ERPTaxCodeTempate> data = new List<ERPTaxCodeTempate>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 22;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        DataTable Dt = ds.Tables[0];
                        if (Dt != null)
                        {
                            data = (from DataRow dr in Dt.Rows
                                    select new ERPTaxCodeTempate()
                                    {
                                        ERPCode = dr["ERPCode"].ToString(),
                                        Description = dr["Description"].ToString(),
                                        TaxPer = dr["TaxPer"].ToString(),
                                        IsRCM = Convert.ToBoolean(dr["IsRCM"].ToString()),
                                    }).ToList();
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            return data;
        }

        [DontValidate]
        [HttpGet]
        [ActionName("FetchKPIMaster")] //ERPTaxCode
        public object FetchKPIMaster()
        {
            string strMsg = "";
            List<KPIMasterTemplate> data = new List<KPIMasterTemplate>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 24;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        DataTable Dt = ds.Tables[0];
                        if (Dt != null)
                        {
                            data = (from DataRow dr in Dt.Rows
                                    select new KPIMasterTemplate()
                                    {
                                        Code = dr["Code"].ToString(),
                                        KPIName = dr["KPI"].ToString(),
                                        MediumCode = dr["MediumCode"].ToString(),
                                        IsActive = Convert.ToBoolean(dr["IsActive"].ToString()),
                                        KPIShortName = dr["KPIShortName"].ToString(),
                                        IsBudgetKPI = Convert.ToBoolean(dr["IsBudgetKPI"].ToString()),
                                        UnitCode = dr["UnitCode"].ToString(),
                                        UnitName = dr["UnitName"].ToString(),
                                    }).ToList();
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            return data;
        }

        [DontValidate]
        [HttpPost]
        [ActionName("SaveDOADetails")] //pFirst
        public async Task<bool> SaveDOADetails(string DOANo, [FromBody] List<MasterEntity.ListOFDOAs> doaParams) //pFirst
        {
            //bool Result = false;
            //_logger.Information(Response + " : Save Validation Responses.");
            //var billDetails = con.NonMediaBillHeader.Where(x => x.BillNo == Response.billNo).FirstOrDefault();
            //var user = _context.Sec_UserMaster.Where(x => x.UserId == billDetails.CreatedBy).FirstOrDefault();
            var UserId = HttpContext.Current.Request.RequestContext.RouteData.Values["UserID"].ToString();
            try
            {
                if (doaParams == null)
                {
                    return false;
                }
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[DGP].[DealerBillNoCheck]", conn))
                    {
                        foreach (var oEntity in doaParams)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("Mode", 16);
                            cmd.Parameters.AddWithValue("DOA_NO", oEntity.DOA_NO);
                            cmd.Parameters.AddWithValue("DOA_COMPLETION_DATE", oEntity.DOA_COMPLETION_DATE);
                            cmd.Parameters.AddWithValue("SPRS_INDENT_NO", oEntity.SPRS_INDENT_NO);
                            cmd.Parameters.AddWithValue("SAP_PR_NO", oEntity.SAP_PR_NO);
                            cmd.Parameters.AddWithValue("DOA_VALUE", oEntity.DOA_VALUE);
                            cmd.Parameters.AddWithValue("FIN_VENDOR_CODE", oEntity.FIN_VENDOR_CODE);
                            cmd.Parameters.AddWithValue("FIN_SOB_VALUE", oEntity.FIN_SOB_VALUE);
                            cmd.Parameters.AddWithValue("PAYMENT_TERM", oEntity.PAYMENT_TERM);
                            cmd.Parameters.AddWithValue("ADDITIONAL_DETAIL", oEntity.ADDITIONAL_DETAIL);
                            cmd.Parameters.AddWithValue("NEW_CONTRACT_VALIDITY_FROM", oEntity.NEW_CONTRACT_VALIDITY_FROM);
                            cmd.Parameters.AddWithValue("NEW_CONTRACT_VALIDITY_TO", oEntity.NEW_CONTRACT_VALIDITY_TO);
                            cmd.Parameters.AddWithValue("DOA_RAISING_DATE", oEntity.DOA_RAISING_DATE);
                            cmd.Parameters.AddWithValue("GREEN_ISSUE_FLAG", oEntity.GREEN_ISSUE_FLAG);
                            cmd.Parameters.AddWithValue("CONTRACT_DESC", oEntity.CONTRACT_DESC);
                            cmd.Parameters.AddWithValue("ISSUE_ID", oEntity.ISSUE_ID);
                            cmd.Parameters.AddWithValue("PR_BU", oEntity.PR_BU);
                            cmd.Parameters.AddWithValue("INDENT_RAISING_DATE", oEntity.INDENT_RAISING_DATE);
                            cmd.Parameters.AddWithValue("FIN_RECOMMENDED", oEntity.FIN_RECOMMENDED);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable ds = new DataTable();
                            adapter.Fill(ds);
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return true;
        }

        [DontValidate]
        [HttpPost]
        [ActionName("SaveDOADetailsClient")] //pFirst
        public async Task<bool> SaveDOADetailsClient(string DOANo, [FromBody] List<MasterEntityClient.ListOFDOAs> doaParams) //pFirst
        {
            //bool Result = false;
            //_logger.Information(Response + " : Save Validation Responses.");
            //var billDetails = con.NonMediaBillHeader.Where(x => x.BillNo == Response.billNo).FirstOrDefault();
            //var user = _context.Sec_UserMaster.Where(x => x.UserId == billDetails.CreatedBy).FirstOrDefault();
            var UserId = HttpContext.Current.Request.RequestContext.RouteData.Values["UserID"].ToString();
            try
            {
                if (doaParams == null)
                {
                    return false;
                }
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[DGP].[DealerBillNoCheck]", conn))
                    {
                        foreach (var oEntity in doaParams)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("Mode", 16);
                            cmd.Parameters.AddWithValue("DOA_NO", oEntity.DOA_NO);
                            cmd.Parameters.AddWithValue("DOA_COMPLETION_DATE", oEntity.DOA_COMPLETION_DATE);
                            cmd.Parameters.AddWithValue("SPRS_INDENT_NO", oEntity.SPRS_INDENT_NO);
                            cmd.Parameters.AddWithValue("SAP_PR_NO", oEntity.SAP_PR_NO);
                            cmd.Parameters.AddWithValue("DOA_VALUE", oEntity.DOA_VALUE);
                            cmd.Parameters.AddWithValue("FIN_VENDOR_CODE", oEntity.FIN_VENDOR_CODE);
                            cmd.Parameters.AddWithValue("FIN_SOB_VALUE", oEntity.FIN_SOB_VALUE);
                            cmd.Parameters.AddWithValue("PAYMENT_TERM", oEntity.PAYMENT_TERM);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable ds = new DataTable();
                            adapter.Fill(ds);
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return true;
        }
        //Commas,Decimal
        [DontValidate]
        [HttpGet]
        [ActionName("GetCurrencyFormat")] //Comma
        public object GetCurrencyFormat()
        {
            List<object> CurrencyDetails = new List<object>();
            var ShortName = "";
            var Symbol = "";
            var Symbol1 = "";
            var AmountFormat = "";
            DataTable dtResult = new DataTable();
            using (SqlConnection conn = new SqlConnection(AdminConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("MWSP_CurrencyMaster", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Mode", 2);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.SelectCommand = cmd;
                da.Fill(dtResult);
                if (dtResult.Rows.Count > 0)
                {
                    ShortName = dtResult.Rows[0][0].ToString();
                    Symbol = dtResult.Rows[0][1].ToString();
                    Symbol1 = dtResult.Rows[0][2].ToString();
                    AmountFormat = dtResult.Rows[0][3].ToString();
                }
            }
            var dic = new
            {
                ShortName = ShortName,
                Symbol = Symbol,
                Symbol1 = Symbol1,
                AmountFormat = AmountFormat
            };
            CurrencyDetails.Add(dic);
            return CurrencyDetails;
        }
        // Payment terms change
        [DontValidate]
        [HttpGet]
        [ActionName("GetPaymentTerms")] //ERPTaxCode
        public object GetPaymentTerms()
        {
            string strMsg = "";
            List<PaymentTerms> data = new List<PaymentTerms>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 26;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        DataTable Dt = ds.Tables[0];
                        if (Dt != null)
                        {
                            data = (from DataRow dr in Dt.Rows
                                    select new PaymentTerms()
                                    {
                                        PrintTempId = Convert.ToInt32(dr["PrintTempId"]),
                                        PrintTempName = dr["PrintTempName"].ToString(),
                                        PrintDetails = dr["PrintDetails"].ToString(),
                                        PrintPayTerms = dr["PrintPayTerms"].ToString(),
                                        BillDueAfter = Convert.ToInt32(dr["BillDueAfter"]),
                                        MSMEApplicable = Convert.ToBoolean(dr["MSMEApplicable"]),
                                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                                    }).ToList();
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            return data;
        }

        [DontValidate]
        [HttpGet]
        [ActionName("spDueDateMailsDGP")]
        public async Task<object> spDueDateMailsAsync()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[spDueDateMailsDGP]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Emails sent successfully.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [DontValidate]
        [HttpGet]
        [ActionName("CheckBillStatusByPO")]
        public IHttpActionResult CheckBillStatusByPO(string poNo)
        {
            try
            {
                int billCount = 0;
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 28;
                        cmd.Parameters.Add("@PONO", SqlDbType.VarChar).Value = poNo;

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            billCount = dt.AsEnumerable()
                                          .Count(row => row["Status"] != DBNull.Value && Convert.ToInt32(row["Status"]) > 1);
                        }
                    }
                }

                return Ok(billCount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [DontValidate]
        [HttpPost]
        [ActionName("SaveProformaDetailsInBI")] //2804
        public async Task<bool> SaveProformaDetailsInBI(ProformaDetails oEntity) //2804
        {
            if (oEntity == null)
            {
                return false;
            }
            using (SqlConnection con = new SqlConnection(AdminConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 27;
                        cmd.Parameters.Add("@ProformaInvoiceNo", SqlDbType.VarChar).Value = oEntity.ProformaInvoiceNo;
                        cmd.Parameters.Add("@ProformaInvoiceDate", SqlDbType.DateTime).Value = oEntity.ProformaInvoiceDate;
                        cmd.Parameters.Add("@AdvancePaymentReferenceNo", SqlDbType.VarChar).Value = oEntity.AdvancePaymentRefernceNo;
                        cmd.Parameters.Add("@AdvancePaymentDate", SqlDbType.DateTime).Value = oEntity.AdvancePaymentDate;
                        cmd.Parameters.Add("@AdvancePaymentGLCode", SqlDbType.VarChar).Value = oEntity.AdvancePaymentGLCode;
                        cmd.Parameters.Add("@PONO", SqlDbType.VarChar).Value = oEntity.VPONo;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }

                return true;
            }
        }

        [DontValidate]
        [HttpPost]
        [ActionName("RegionalBucketConsumption")]
        public object RegionalBucketConsumption(string PONO, string BillNo, string Medium, string HeadCode, string RequestID, string Amount, string userCode, string DocType, string Action,
        string TotalBillableAmt, string MultiDealerAmt, string ActivityID, bool IsCompletionApp, bool PartsCompletion, string CompParentActivityID, string BucketRatio, string ParentDocID) //string ParentRequestID
        {
            string strMsg = "";
            OperationResult obj = new OperationResult();
            using (SqlConnection con = new SqlConnection(AdminConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 29;
                        cmd.Parameters.Add("@DocType", SqlDbType.VarChar).Value = DocType;
                        cmd.Parameters.Add("@Medium", SqlDbType.VarChar).Value = Medium;
                        cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = Action;
                        cmd.Parameters.Add("@PoNumber", SqlDbType.VarChar).Value = PONO;
                        cmd.Parameters.Add("@DocID", SqlDbType.VarChar).Value = RequestID;
                        cmd.Parameters.Add("@BudgetLinkedCode", SqlDbType.VarChar).Value = HeadCode;
                        cmd.Parameters.Add("@ConsumedAmount", SqlDbType.VarChar).Value = Amount;
                        cmd.Parameters.Add("@UserCode", SqlDbType.VarChar).Value = userCode;
                        cmd.Parameters.Add("@BillNo", SqlDbType.VarChar).Value = BillNo;
                        cmd.Parameters.Add("@DealerAmount", SqlDbType.VarChar).Value = MultiDealerAmt;
                        cmd.Parameters.Add("@TotalPOAmount", SqlDbType.VarChar).Value = TotalBillableAmt;
                        cmd.Parameters.Add("@ActivityID", SqlDbType.VarChar).Value = ActivityID;
                        cmd.Parameters.Add("@IsCompletionApp", SqlDbType.VarChar).Value = IsCompletionApp;
                        cmd.Parameters.Add("@PartsCompletion", SqlDbType.VarChar).Value = PartsCompletion;
                        cmd.Parameters.Add("@CompParentActivityID", SqlDbType.VarChar).Value = CompParentActivityID;
                        cmd.Parameters.Add("@BucketRatio", SqlDbType.VarChar).Value = BucketRatio;
                        cmd.Parameters.Add("@ParentDocID", SqlDbType.VarChar).Value = ParentDocID;
                        //cmd.Parameters.Add("@BudgetLinkedCode", SqlDbType.VarChar).Value = ConsumptioParams;
                        //cmd.Parameters.Add("@BudgetLinkedCode", SqlDbType.VarChar).Value = ConsumptioParams;
                        //cmd.Parameters.Add("@AreaOfficeCode", SqlDbType.VarChar).Value = AreaOfficeCode;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                    }
                }
                catch (SqlException ex)
                {
                    strMsg = ex.Message.ToString();
                }
            }
            return strMsg;
        }


        [DontValidate]
        [HttpPost]
        [ActionName("CompletionBucketConsumption")]
        public object CompletionBucketConsumption(string RequestID)
        {
            string strMsg = "";
            OperationResult obj = new OperationResult();
            using (SqlConnection con = new SqlConnection(AdminConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 30;
                        cmd.Parameters.Add("@DocType", SqlDbType.VarChar).Value = "PO";
                        cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = "PO_APPROVAL_DGP_Regional";
                        cmd.Parameters.Add("@DocID", SqlDbType.VarChar).Value = RequestID;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                    }
                }
                catch (SqlException ex)
                {
                    strMsg = ex.Message.ToString();
                }
            }
            return strMsg;
        }
        [DontValidate]
        [HttpPost]
        [ActionName("RevertCompAmtInOrgBucket")]
        public object RevertCompAmtinOrgBucket(string RequestID, string CompClientAmt,string CompDealerAmt,string CompTotalAmt,string ActivityID)
        {
            string strMsg = "";
            OperationResult obj = new OperationResult();
            using (SqlConnection con = new SqlConnection(AdminConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 31;
                        cmd.Parameters.Add("@DocType", SqlDbType.VarChar).Value = "PO";
                        cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = "PO_APPROVAL_DGP_Regional";
                        cmd.Parameters.Add("@DocID", SqlDbType.VarChar).Value = RequestID;
                        cmd.Parameters.Add("@CompClientAmt", SqlDbType.VarChar).Value = CompClientAmt;
                        cmd.Parameters.Add("@CompDealerAmt", SqlDbType.VarChar).Value = CompDealerAmt;
                        cmd.Parameters.Add("@CompTotalAmt", SqlDbType.VarChar).Value = CompTotalAmt;
                        cmd.Parameters.Add("@ActivityID", SqlDbType.VarChar).Value = ActivityID;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                    }
                }
                catch (SqlException ex)
                {
                    strMsg = ex.Message.ToString();
                }
            }
            return strMsg;
        }

        [DontValidate]
        [HttpPost]
        [ActionName("BudgetBucketConsumptionNew")]
        public object BudgetBucketConsumptionNew(string PONO, string BillNo, string Medium, string HeadCode, string RequestID, string Amount, string userCode, string DocType, string Action,
        string TotalBillableAmt, string MultiDealerAmt, string ActivityID, bool IsCompletionApp, bool PartsCompletion, string CompParentActivityID, string BucketRatio)
        {
            string strMsg = "";
            OperationResult obj = new OperationResult();
            using (SqlConnection con = new SqlConnection(AdminConnectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 32;
                        cmd.Parameters.Add("@DocType", SqlDbType.VarChar).Value = DocType;
                        cmd.Parameters.Add("@Medium", SqlDbType.VarChar).Value = Medium;
                        cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = Action;
                        cmd.Parameters.Add("@PoNumber", SqlDbType.VarChar).Value = PONO;
                        cmd.Parameters.Add("@DocID", SqlDbType.VarChar).Value = RequestID;
                        cmd.Parameters.Add("@BudgetLinkedCode", SqlDbType.VarChar).Value = HeadCode;
                        cmd.Parameters.Add("@ConsumedAmount", SqlDbType.VarChar).Value = Amount;
                        cmd.Parameters.Add("@UserCode", SqlDbType.VarChar).Value = userCode;
                        cmd.Parameters.Add("@BillNo", SqlDbType.VarChar).Value = BillNo;
                        cmd.Parameters.Add("@DealerAmount", SqlDbType.VarChar).Value = MultiDealerAmt;
                        cmd.Parameters.Add("@TotalPOAmount", SqlDbType.VarChar).Value = TotalBillableAmt;
                        cmd.Parameters.Add("@ActivityID", SqlDbType.VarChar).Value = ActivityID;
                        cmd.Parameters.Add("@IsCompletionApp", SqlDbType.VarChar).Value = IsCompletionApp;
                        cmd.Parameters.Add("@PartsCompletion", SqlDbType.VarChar).Value = PartsCompletion;
                        cmd.Parameters.Add("@CompParentActivityID", SqlDbType.VarChar).Value = CompParentActivityID;
                        cmd.Parameters.Add("@BucketRatio", SqlDbType.VarChar).Value = BucketRatio;
                        //cmd.Parameters.Add("@BudgetLinkedCode", SqlDbType.VarChar).Value = ConsumptioParams;
                        //cmd.Parameters.Add("@BudgetLinkedCode", SqlDbType.VarChar).Value = ConsumptioParams;
                        //cmd.Parameters.Add("@AreaOfficeCode", SqlDbType.VarChar).Value = AreaOfficeCode;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable ds = new DataTable();
                        adapter.Fill(ds);
                    }
                }
                catch (SqlException ex)
                {
                    strMsg = ex.Message.ToString();
                }
            }
            return strMsg;
        }

        [DontValidate]
        [HttpGet]
        [ActionName("GetVendorCategories")] //MSME
        public object GetVendorCategories(string VendorCategory)
        {
            string strMsg = "";
            List<VendorCategories> data = new List<VendorCategories>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 33;
                        cmd.Parameters.Add("@VendorCategory", SqlDbType.VarChar).Value = VendorCategory;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        DataTable Dt = ds.Tables[0];
                        if (Dt != null)
                        {
                            data = (from DataRow dr in Dt.Rows
                                    select new VendorCategories()
                                    {
                                        Name = dr["Name"].ToString(),
                                        ShortName = dr["ShortName"].ToString(),
                                        InvoiceLimitDays =Convert.ToInt32(dr["InvoiceLimitDays"]),
                                        IsMSME = Convert.ToBoolean(dr["IsMSME"])
                                    }).ToList();
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            return data;
        }

        [DontValidate]
        [HttpGet]
        [ActionName("GetBrandWiseBucketWithLinq")]
        public object GetBrandWiseBucketWithLinq(string userCode, string Target, string AreaCode, string BrandName, string ExpenseType, decimal? HmclSplitRatio = null)
        {
            var strMsg = "";
            Dictionary<string, object> dict = new Dictionary<string, object>();
            List<BudgetBucketTable> data = new List<BudgetBucketTable>();
            List<CurrentMonthBuffer> data1 = new List<CurrentMonthBuffer>();
            List<LastMonthBuffer> data2 = new List<LastMonthBuffer>();
            
            try
            {
                string SapFrnFlag = "";
                
                // Get SAP FRN Flag
                try
                {
                    using (SqlConnection con = new SqlConnection(AdminConnectionString))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 17;
                            SqlDataAdapter ada = new SqlDataAdapter(cmd);
                            DataTable ds1 = new DataTable();
                            ada.Fill(ds1);
                            SapFrnFlag = ds1.Rows[0][0].ToString();
                        }
                    }
                }
                catch (SqlException e)
                {
                    SapFrnFlag = e.Message.ToString();
                }

                // Get Budget Bucket data from stored procedure (keeping this as it contains complex business logic)
                DataSet budgetDataSet = new DataSet();
                using (SqlConnection con = new SqlConnection(AdminConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[dbo].[MWSP_DGPMaster]", con))
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@Mode", SqlDbType.Int).Value = 18;
                        cmd.Parameters.Add("@UserCode", SqlDbType.VarChar).Value = userCode;
                        cmd.Parameters.Add("@Target", SqlDbType.VarChar).Value = Target;
                        cmd.Parameters.Add("@AreaCode", SqlDbType.VarChar).Value = AreaCode;
                        cmd.Parameters.Add("@BrandName", SqlDbType.VarChar).Value = BrandName;
                        cmd.Parameters.Add("@ExpenseType", SqlDbType.VarChar).Value = ExpenseType;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(budgetDataSet);
                    }
                }

                // Convert DataTable to List for LINQ operations
                var budgetBucketData = budgetDataSet.Tables[0].AsEnumerable()
                    .Select(dr => new
                    {
                        BucketTitle = dr["BucketTitle"].ToString(),
                        BudgetBucketHeadCode = Convert.ToInt32(dr["BudgetBucketHeadCode"]),
                        Period = dr["Period"].ToString(),
                        Branch1 = dr["Branch1"].ToString(),
                        Branch2 = dr["Branch2"].ToString(),
                        Amount = Convert.ToDecimal(dr["Amount"]),
                        FRNNo = SapFrnFlag == "False" ? dr["FRNNo"].ToString() : "",
                        GLCode = dr["GLCode"].ToString(),
                        CcCode = dr["CcCode"].ToString(),
                        Branch1Code = dr["Branch1Code"].ToString(),
                        Branch2Code = dr["Branch2Code"].ToString(),
                        FRNSrNo = SapFrnFlag == "False" ? Convert.ToInt32(dr["FRNSrNo"]) : 0,
                        BranchDetailsCode = Convert.ToInt32(dr["BranchDetailsCode"]),
                        Description = SapFrnFlag == "False" ? dr["Description"].ToString() : "",
                        BudgetType =  dr["BudgetType"].ToString() 
                    }).ToList();

                // Now use LINQ to calculate Approved PO Amount for each bucket
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    
                    // Get all approved and in-process requests with their budget bucket links and medium details
                    // Based on MediumID: 1,2=TV/Radio, 3=Print, 4=Outdoor, 5=Digital, 6=BTL
                    string query = @"
                        SELECT 
                            rbl.BudgetLinkedCode,
                            -- Approved PO Amounts (multiplied by Ratio percentage, based on MediumID)
                            SUM(CASE WHEN r.POStatus = 'Approved' THEN 
                                (CASE 
                                    WHEN r.MediumID IN (1, 2) THEN COALESCE(tv.EstGrandAmt, 0) - COALESCE(tv.EstTaxAmt, 0)
                                    WHEN r.MediumID = 3 THEN COALESCE(pr.EstGrandAmt, 0) - COALESCE(pr.EstTaxAmt, 0)
                                    WHEN r.MediumID = 4 THEN COALESCE(out.EstGrandAmt, 0) - COALESCE(out.EstTaxAmt, 0)
                                    WHEN r.MediumID = 5 THEN COALESCE(dig.EstGrandAmt, 0) - COALESCE(dig.EstTaxAmt, 0)
                                    WHEN r.MediumID = 6 THEN COALESCE(btl.EstGrandAmt, 0) - COALESCE(btl.EstTaxAmt, 0)
                                    ELSE 0 END) * 
                                COALESCE(rbl.Ratio, 100) / 100.0
                                ELSE 0 END) as ApprovedAmount,
                            -- PO In Process Amounts (not approved, not rejected, not cancelled, multiplied by Ratio percentage, based on MediumID)
                            SUM(CASE WHEN ISNULL(r.POStatus, '') != 'Approved' AND (r.Status != 'Rejected' AND r.Status != 'Cancelled') THEN 
                                (CASE 
                                    WHEN r.MediumID IN (1, 2) THEN COALESCE(tv.EstGrandAmt, 0) - COALESCE(tv.EstTaxAmt, 0)
                                    WHEN r.MediumID = 3 THEN COALESCE(pr.EstGrandAmt, 0) - COALESCE(pr.EstTaxAmt, 0)
                                    WHEN r.MediumID = 4 THEN COALESCE(out.EstGrandAmt, 0) - COALESCE(out.EstTaxAmt, 0)
                                    WHEN r.MediumID = 5 THEN COALESCE(dig.EstGrandAmt, 0) - COALESCE(dig.EstTaxAmt, 0)
                                    WHEN r.MediumID = 6 THEN COALESCE(btl.EstGrandAmt, 0) - COALESCE(btl.EstTaxAmt, 0)
                                    ELSE 0 END) * 
                                COALESCE(rbl.Ratio, 100) / 100.0
                                ELSE 0 END) as InProcessAmount
                        FROM [DGPTrans].[RequestBudgetBucketLink] rbl
                        INNER JOIN [DGPTrans].[Request] r ON rbl.RequestID = r.RequestID
                        LEFT JOIN [DGPTrans].[RequestDetailForTVorRadio] tv ON rbl.RequestDetailID = tv.RequestDetailID AND r.MediumID IN (1, 2)
                        LEFT JOIN [DGPTrans].[RequestDetailForPrint] pr ON rbl.RequestDetailID = pr.RequestDetailID AND r.MediumID = 3
                        LEFT JOIN [DGPTrans].[RequestDetailForOutdoor] out ON rbl.RequestDetailID = out.RequestDetailID AND r.MediumID = 4
                        LEFT JOIN [DGPTrans].[RequestDetailForDigital] dig ON rbl.RequestDetailID = dig.RequestDetailID AND r.MediumID = 5
                        LEFT JOIN [DGPTrans].[RequestDetailForBTL] btl ON rbl.RequestDetailID = btl.RequestDetailID AND r.MediumID = 6
                        WHERE (r.Status != 'Rejected' AND r.Status != 'Cancelled')
                        GROUP BY rbl.BudgetLinkedCode";

                    var approvedAmounts = new Dictionary<string, decimal>();
                    var inProcessAmounts = new Dictionary<string, decimal>();
                    
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string budgetLinkedCode = reader["BudgetLinkedCode"].ToString();
                                decimal approvedAmount = Convert.ToDecimal(reader["ApprovedAmount"]);
                                decimal inProcessAmount = Convert.ToDecimal(reader["InProcessAmount"]);
                                
                                approvedAmounts[budgetLinkedCode] = approvedAmount;
                                inProcessAmounts[budgetLinkedCode] = inProcessAmount;
                            }
                        }
                    }

                    // Use LINQ to join budget bucket data with approved and in-process amounts
                    data = budgetBucketData
                        .Select(bucket => 
                        {
                            // Determine which code to use for matching (priority: Branch2Code -> Branch1Code -> BranchDetailsCode)
                            string matchingCode = bucket.Branch2Code != "0" && bucket.Branch2Code != "-" ? bucket.Branch2Code :
                                                 bucket.Branch1Code != "0" && bucket.Branch1Code != "-" ? bucket.Branch1Code :
                                                 bucket.BranchDetailsCode.ToString();
                            
                            // Get approved and in-process amounts for this bucket
                            decimal approvedAmount = approvedAmounts.ContainsKey(matchingCode) ? approvedAmounts[matchingCode] : 0;
                            decimal inProcessAmount = inProcessAmounts.ContainsKey(matchingCode) ? inProcessAmounts[matchingCode] : 0;
                            
                            // Apply HMCL Split Ratio if provided (percentage calculation)
                            if (HmclSplitRatio.HasValue && HmclSplitRatio.Value > 0)
                            {
                                decimal splitPercentage = HmclSplitRatio.Value / 100m; // Convert percentage to decimal
                                approvedAmount = approvedAmount * splitPercentage;
                                inProcessAmount = inProcessAmount * splitPercentage;
                            }
                            
                            // Calculate Available Budget = Total Budget - PO In Process
                            decimal availableBudget = bucket.Amount - inProcessAmount;
                            
                            return new BudgetBucketTable()
                            {
                                BucketTitle = bucket.BucketTitle,
                                BudgetBucketHeadCode = bucket.BudgetBucketHeadCode,
                                Period = bucket.Period,
                                Branch1 = bucket.Branch1,
                                Branch2 = bucket.Branch2,
                                Amount = availableBudget,
                                FRNNo = bucket.FRNNo,
                                GLCode = bucket.GLCode,
                                CcCode = bucket.CcCode,
                                Branch1Code = bucket.Branch1Code,
                                Branch2Code = bucket.Branch2Code,
                                FRNSrNo = bucket.FRNSrNo,
                                BranchDetailsCode = bucket.BranchDetailsCode,
                                Description = bucket.Description,
                                BudgetType = bucket.BudgetType,
                                // Enhanced properties with new columns
                                ActualBudget = approvedAmount.ToString("F2"), // Approved PO Amount (for backward compatibility)
                                TotalBudget = $"{inProcessAmount:F2}|{availableBudget:F2}", // PO In Process | Available Budget (for backward compatibility)
                                // New separate properties for cleaner access
                                ApprovedPOAmount = approvedAmount,
                                POInProcessAmount = inProcessAmount,
                                AvailableBudget = availableBudget
                            };
                        })
                        .OrderBy(x => x.BucketTitle)
                        .ToList();
                }

                // Handle Regional Sales buffer data (keeping original logic)
                if (Target == "Regional Sales")
                {
                    if (budgetDataSet.Tables[1].Rows.Count > 0)
                    {
                        DataTable CurrentBuffer = budgetDataSet.Tables[1];
                        if (CurrentBuffer != null)
                        {
                            data1 = (from DataRow dr in CurrentBuffer.Rows
                                     select new CurrentMonthBuffer()
                                     {
                                         CurrentTotalBudget = dr["CurrentTotalBudget"].ToString(),
                                         CurrentActualBudget = dr["CurrentActualBudget"].ToString()
                                     }).ToList();
                        }
                    }
                    if (budgetDataSet.Tables[2].Rows.Count > 0)
                    {
                        DataTable LastBuffer = budgetDataSet.Tables[2];
                        if (LastBuffer != null)
                        {
                            data2 = (from DataRow dr in LastBuffer.Rows
                                     select new LastMonthBuffer()
                                     {
                                         LastMonthTotalBudget = dr["LastMonthTotalBudget"].ToString(),
                                         LastMonthActualBudget = dr["LastMonthActualBudget"].ToString()
                                     }).ToList();
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                strMsg = e.Message.ToString();
            }
            
            dict.Add("BudgetBucketTable", data);
            dict.Add("Buffer", data1);
            dict.Add("PreBuffer", data2);
            dict.Add("HmclSplitRatio", HmclSplitRatio);
            dict.Add("HmclSplitApplied", HmclSplitRatio.HasValue && HmclSplitRatio.Value > 0);
            dict.Add("Message", strMsg);
            return dict;
        }
    }
}

public class BudgetBucketTable
{
    public string BucketTitle { get; set; }
    public int BudgetBucketHeadCode { get; set; }
    public int BranchDetailsCode { get; set; }
    public string Period { get; set; }
    public decimal Amount { get; set; }
    public string Branch1 { get; set; }
    public string Branch1Code { get; set; }
    public decimal? Branch1Amount { get; set; }
    public string Branch2 { get; set; }
    public int? FRNSrNo { get; set; }
    public int? FRNId { get; set; }
    public string FRNNo { get; set; }
    public string Branch2Code { get; set; }
    public decimal? Branch2Amount { get; set; }
    public string GLCode { get; set; }
    public string CcCode { get; set; }
    public string TotalBudget { get; set; }
    public string ActualBudget { get; set; } // Approved PO Amount
    public bool IsClosed { get; set; }
    public bool IsBlocked { get; set; }
    public string Description { get; set; }
    public string BudgetType { get; set; } //BudgetType 
    
    // New properties for enhanced functionality
    public decimal? ApprovedPOAmount { get; set; } // Approved PO Amount (separate property)
    public decimal? POInProcessAmount { get; set; } // PO In Process Amount
    public decimal? AvailableBudget { get; set; } // Available Budget (Amount - POInProcessAmount)
    public int? ApprovedPOCount { get; set; } // Number of approved POs
    public int? InProcessPOCount { get; set; } // Number of in-process POs
}

public class BucketFields
{
    //List<BucketFields> data = new List<BucketFields>();
    public string Ratio { get; set; }
    public string Code { get; set; }
    public string GLCode { get; set; }
    public string CcCode { get; set; }
    public string PONO { get; set; }
    public string BudgetType { get; set; }
    public string BucketTitle { get; set; }
    public string FrnNo { get; set; }
    public int FrnSrNo { get; set; }
    public string BucketHeadCode { get; set; }
    public string FrnDescription { get; set; }
    public string BrandName { get; set; }
    public string Period { get; set; }
    public bool? IsBlocked { get; set; }
    public bool? IsClosed { get; set; }
    public decimal? Amount { get; set; }


}

public class BucketHeadCodeList
{
    public int HeadCode { get; set; }
}

public class ConsumptionParams
{
    public string DocType { get; set; }
    public string DocID { get; set; }
    public string Medium { get; set; }
    public string ConsumptionAmount { get; set; }
    public string ConsumedBy { get; set; }
    public string PONO { get; set; }
    public string POID { get; set; }
    //public string Action { get; set; }
    public string ExpenceTypeCode { get; set; }
    public string BudgetBucketHeadCode { get; set; }
    public string Ratio { get; set; }
    public string RangeBrandCode { get; set; }
}

public class DetailForResubmit
{
    public string HeadCode { get; set; }
    public string DocID { get; set; }
    public string BillAmount { get; set; }
    public string ConsumptionAmount { get; set; }
    public string PONO { get; set; }
    public string Ratio { get; set; }
    public string ActivityID { get; set; }
}

public class CurrentMonthBuffer
{
    public string CurrentActualBudget { get; set; }
    public string CurrentTotalBudget { get; set; }

}
public class LastMonthBuffer
{
    public string LastMonthTotalBudget { get; set; }
    public string LastMonthActualBudget { get; set; }

}
public class ValidationResponse //EInvoice
{
    public string FileName { get; set; }
    public string billNo { get; set; }
    public string PoNo { get; set; }
    public string Type { get; set; }
    public string Action { get; set; }
    public string PoValidation { get; set; }
    public string InvoicePdfValue { get; set; }
    public string DataBaseValue { get; set; }
    public string pdfValues { get; set; }
    public string AmountTitle { get; set; }
    public string InvoiceManualInput { get; set; }
}
public class KPIMasterTemplate //KPIMaster
{

    public string Code { get; set; }
    public string KPIName { get; set; }
    public string MediumCode { get; set; }
    public string UnitCode { get; set; }
    public string UnitName { get; set; }
    public bool? IsActive { get; set; }
    public string KPIShortName { get; set; }
    public bool? IsBudgetKPI { get; set; }
    //public string GLCode { get; set; }
}
public class PaymentTerms
{
    public int? PrintTempId { get; set; }
    public string PrintDetails { get; set; }
    public string PrintTempName { get; set; }
    public string PrintPayTerms { get; set; }
    public int? BillDueAfter { get; set; }
    public bool? MSMEApplicable { get; set; }
    public bool? IsActive { get; set; }
}
public class DOAParams //pFirst
{
    public string ADDITIONAL_DETAIL { get; set; }
    public string GREEN_ISSUE_FLAG { get; set; }
    public string NEW_CONTRACT_VALIDITY_FROM { get; set; }
    public string WF_ID { get; set; }
    public string DOA_VALUE { get; set; }
    public string CONTRACT_DESC { get; set; }
    public string FIN_RECOMMENDED { get; set; }
    public string ISSUE_ID { get; set; }
    public string SAP_PR_NO { get; set; }
    public string FIN_SERVICE_PROVIDER { get; set; }
    public string PAYMENT_TERM { get; set; }
    public string NEW_CONTRACT_VALIDITY_TO { get; set; }
    public string DOA_RAISING_DATE { get; set; }
    public string INDENT_RAISING_DATE { get; set; }
    public string INDENT_COMPLETION_DATE { get; set; }
    public string DOA_COMPLETION_DATE { get; set; }
    public string PR_BU { get; set; }
    public string FIN_SOB_VALUE { get; set; }
    public string DOA_PDF { get; set; }
    public string SPRS_INDENT_NO { get; set; }
    public string DOA_NO { get; set; }
    public string FIN_VENDOR_CODE { get; set; }
    public string PR_PDF { get; set; }
}
public class GetStateMaster
{
    public int ID { get; set; }
    public string Name { get; set; }
    public bool IsIsdState { get; set; }
}
public class GetServiceType
{
    public int Id { get; set; }
    public string ServiceType { get; set; }
    public string TaxType { get; set; }
    public bool DisplayRCMTaxCode { get; set; }
}

public class VendorCategories
{
    public string Name { get; set; }
    public int? InvoiceLimitDays { get; set; }
    public bool? IsMSME { get; set; }
    public string ShortName { get; set; }
}
