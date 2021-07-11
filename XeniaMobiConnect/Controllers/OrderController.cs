using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Tracing;
using XeniaMobiConnect.Controllers.Resource;
using XeniaMobiConnect.Util;

namespace XeniaMobiConnect.Controllers
{
    public class OrderController : ApiController
    {
        private Entities db = new Entities();
        private readonly ITraceWriter _tracer;

        public OrderController()
        {
            _tracer = GlobalConfiguration.Configuration.Services.GetTraceWriter();

        }

        [HttpGet]
        [Route("api/Order/Recent")]
        public APIResponse GetRecentOrders()
        {
            var headers = Request.Headers;
            int customerId = APIUtil.GetCustomerId(db, headers);


            var orders = db.trsOrders.Where(o => o.lid == customerId && o.invType == "SalesOrder").OrderByDescending(o => o.SSMA_TimeStamp).Take(10).ToList().Select(o => Build(o)).ToList();

            if (orders == null)
            {
                return new APIResponse(APIResponseStatus.success, new List<Order>(), "");
            }

            foreach (Order order in orders)
            {
                var orderDetails = db.trsOrderDetails.Where(d => d.invID == order.invid).ToList().Select(d => BuildOrderItem(d)).ToList();

                order.orderDetails = orderDetails;

            }

            return new APIResponse(APIResponseStatus.success, orders, "");

        }

        [HttpGet]
        [Route("api/Order/info")]
        public APIResponse GetOrderDetails(int orderId)
        {
            var headers = Request.Headers;
            int customerId = APIUtil.GetCustomerId(db, headers);


            var order = db.trsOrders.Where(o => o.invid == orderId && o.invType == "SalesOrder").ToList().Select(o => Build(o)).FirstOrDefault();

            if (order == null)
            {
                return new APIResponse(APIResponseStatus.failed, null, "Order Not Found");
            }

            var orderDetails = db.trsOrderDetails.Where(d => d.invID == order.invid).ToList().Select(d => BuildOrderItem(d)).ToList();

            order.orderDetails = orderDetails;

            return new APIResponse(APIResponseStatus.success, order, "");

        }

        [HttpPost]
        [Route("api/Order/Summary")]
        public APIResponse GetOrderDetails(List<CartItem> cartItems)
        {
            var headers = Request.Headers;

            mtrledger Customer = APIUtil.GetCustomer(db, headers);
            if (Customer == null)
            {
                return new APIResponse(APIResponseStatus.failed, null, "Invalid Customer");
            }

            if (cartItems == null || cartItems.Count == 0)
            {
                return new APIResponse(APIResponseStatus.failed, null, "Invalid Order Items");
            }

            var priceListItems = GetPriceList(Customer.lid);

            List<OrderDetail> orderDetails = new List<OrderDetail>();

            Boolean isBatchEnabled = IsBatchEnabled();
            Boolean isCessEnabled = IsCessEnabled();
            Boolean isAddCessEnabled = IsAddCessEnabled();
            Boolean isKFCessEnabled = IsKFCessEnabled();
            int decimalPlaces = GetDecimalPlaces();

            if (isKFCessEnabled)
            {
                if (Customer.tinno != null && Customer.tinno.Trim() != "")
                    isKFCessEnabled = false;
            }

            Order order = new Order
            {
                invType = "SalesOrder",
                party = Customer.LedgerName,
                lid = Customer.lid,
                taxType = "GST",
                contactno = Customer.contactno,
                remarks = Customer.remarks,
                address = Customer.address,
                SalePersonID = 1,
                blnProcessed = false,
                cancelled = false,
                tinNo = Customer.tinno,
                userid = 1,
                ccID = 1,
                ccName = "Main",
                dccID=1,
                dccName="Main",
                itemDiscount = 0,
                labourCost = 0,
                otherCost = 0,
                serviceTax = 0,
                cessOnTax = 0,
                counterid = 1,
                IsSettled = false,
                CompanyVPA=GetVPAAddress(),
                rndCutoff=1,
                typeID=1,
                AgentId=Customer.AgentID,
                compName="APP"
            };

            double tGrossAmount = 0;
            double tDiscount = 0;
            double tTaxableAmount = 0;
            double tTaxAmount = 0;
            double tExempted = 0;
            double tBillAmount = 0;
            double tCost = 0;
            double tSaleValues = 0;
            double tBillProfit = 0;
            double tCessAmount = 0;
            double tAddCessAmt = 0;
            double tCgstAmount = 0;
            double tSgstAmount = 0;
            double tIgstAmount = 0;
            double tKfcessAmt = 0;

            foreach (CartItem cartItem in cartItems)
            {
                var product = db.mtrProducts.Where(p => p.prodID == cartItem.itemid && p.active == false).FirstOrDefault();
                if (product == null)
                    return new APIResponse(APIResponseStatus.failed, null, "Invalid Order Items");

                if (priceListItems != null && priceListItems.Count() > 0)
                {

                    var priceListPrice = priceListItems.Where(item => item.prodId == product.prodID).FirstOrDefault();
                    if (priceListPrice != null)
                    {
                        product.srate = (decimal)priceListPrice.price;
                    }
                }

                OrderDetail orderDetail = new OrderDetail
                {
                    rOrder = cartItem.rOrder,
                    itemid = cartItem.itemid,
                    quantity = cartItem.quantity,
                    freeQty = cartItem.freeQty,
                    blnAltUnit = cartItem.blnAltUnit,
                    unit = cartItem.unit,
                    Rate = (product.srate ?? 0),
                    sRate = (double)(product.srate??0),
                    pRate = (double)(product.prate ?? 0),
                    taxper = (double)(product.gstPer ?? 0),
                    mrp = product.mrp,
                    unitfactor=1,
                    expDate="NA",
                    extraColumn="",
                    ItemName=product.prodName
                };
                if (isBatchEnabled)
                    orderDetail.batch = product.Batch;

                orderDetail.cgstPer = (double)(product.gstPer / 2);
                orderDetail.sgstPer = (double)(product.gstPer / 2);
                orderDetail.igstPer = 0;

                if (isCessEnabled)
                    orderDetail.cessPer = product.cessPer;
                else
                    orderDetail.cessPer = 0;

                if (isAddCessEnabled)
                    orderDetail.AddCessRate = product.AddCess;
                else
                    orderDetail.AddCessRate = 0;

                if (isKFCessEnabled && product.gstPer > 5)
                    orderDetail.kfcessper = product.kfcessPer;
                else
                    orderDetail.kfcessper = 0;

                double qty = orderDetail.quantity??0;
                double price = orderDetail.sRate ?? 0;
                double pRate = orderDetail.pRate ?? 0;
                double taxPerc = orderDetail.taxper ?? 0;
                double kfCessPerc = orderDetail.kfcessper ?? 0;
                double cessPerc = orderDetail.cessPer ?? 0;
                double additionalCessRate = orderDetail.AddCessRate ?? 0;
                double grossAmount = 0;
                double cost = 0;
                double discountAmount = 0;
                double kfCessAmount = 0;
                double cessAmount = 0;
                double taxAmount = 0;
                double additionalCessAmount = qty * additionalCessRate;
                double amount = 0;
                bool isTaxInclusive = product.staxincl ?? false;
                double exempted = 0;
                if (isTaxInclusive)
                {

                    double subAmount = (qty * price) - additionalCessAmount;
                    double subAmountPRate = (qty * pRate) - additionalCessAmount;
                    double totalTaxPer = taxPerc + kfCessPerc + cessPerc;
                    grossAmount = ProductUtil.GetBasePrice(subAmount, totalTaxPer, true);
                    cost= 0;
                }
                else
                {
                    grossAmount = qty * price;
                    cost= 0;
                }

               


                amount = grossAmount - discountAmount;
                taxAmount = ProductUtil.GetTaxAmount(amount, taxPerc, false);
                kfCessAmount = ProductUtil.GetKFCess(amount, kfCessPerc);
                cessAmount = ProductUtil.GetCessAmount(amount, cessPerc);
                double netAmount = amount + taxAmount + kfCessAmount + cessAmount + additionalCessAmount;

                orderDetail.grossAmount = RoundOffByDecimalPlaces(grossAmount,decimalPlaces);
                orderDetail.Discount = RoundOffByDecimalPlaces(discountAmount,decimalPlaces);
                if (taxPerc > 0)
                {
                    orderDetail.taxable = RoundOffByDecimalPlaces(amount,decimalPlaces);
                    exempted = 0;
                }
                else
                {
                    orderDetail.taxable = 0;
                    exempted = RoundOffByDecimalPlaces(amount,decimalPlaces);
                }
              
                orderDetail.cost = RoundOffByDecimalPlaces(cost,decimalPlaces);
                orderDetail.saleValue = orderDetail.taxable;
                double profit = (orderDetail.saleValue??0)- (orderDetail.cost??0);
                orderDetail.profit = RoundOffByDecimalPlaces(profit,decimalPlaces);

                orderDetail.taxAmount = RoundOffByDecimalPlaces(taxAmount,decimalPlaces);
                orderDetail.cgstAmount = RoundOffByDecimalPlaces(orderDetail.taxAmount / 2,decimalPlaces);
                orderDetail.sgstAmount = RoundOffByDecimalPlaces(orderDetail.taxAmount / 2,decimalPlaces);
                orderDetail.igstAmount = 0;

                orderDetail.kfcessamt = RoundOffByDecimalPlaces(kfCessAmount,decimalPlaces);
                orderDetail.cessAmount = RoundOffByDecimalPlaces(cessAmount,decimalPlaces);
                orderDetail.AddCessAmt = RoundOffByDecimalPlaces(additionalCessAmount,decimalPlaces);
                orderDetail.totalTax = RoundOffByDecimalPlaces((orderDetail.taxAmount+ orderDetail.kfcessamt + orderDetail.cessAmount + orderDetail.AddCessAmt), decimalPlaces);
                orderDetail.netAmount = RoundOffByDecimalPlaces(netAmount,decimalPlaces);
                
                tGrossAmount += orderDetail.grossAmount ?? 0;
                tDiscount += orderDetail.Discount ?? 0;
                tTaxableAmount += orderDetail.taxable ?? 0;
                tExempted += exempted;
                tTaxAmount += orderDetail.taxAmount ?? 0;
                tCgstAmount += orderDetail.cgstAmount ?? 0;
                tSgstAmount += orderDetail.sgstAmount ?? 0;
                tIgstAmount += orderDetail.igstAmount ?? 0;
                tKfcessAmt += orderDetail.kfcessamt ?? 0;
                tCessAmount += orderDetail.cessAmount ?? 0;
                tAddCessAmt += orderDetail.AddCessAmt ?? 0;
                tBillAmount += orderDetail.netAmount ?? 0;
                tCost += orderDetail.cost ?? 0;
                tSaleValues += orderDetail.saleValue ?? 0;
                tBillProfit += orderDetail.profit ?? 0;
                orderDetails.Add(orderDetail);



            }
            order.orderDetails = orderDetails;
            Double totalAmount = Math.Round(tBillAmount);
            Double roundOffAmount = totalAmount-tBillAmount;
            order.grossAmount = RoundOffByDecimalPlaces(tGrossAmount, decimalPlaces);
            order.Discount = RoundOffByDecimalPlaces(tDiscount, decimalPlaces);
            order.taxableAmount = RoundOffByDecimalPlaces(tTaxableAmount, decimalPlaces);
            order.taxAmount = RoundOffByDecimalPlaces(tTaxAmount, decimalPlaces);
            order.Exempted = (decimal)RoundOffByDecimalPlaces(tExempted, decimalPlaces);
            order.cessAmount = RoundOffByDecimalPlaces(tCessAmount, decimalPlaces);
            order.AddCessAmt = RoundOffByDecimalPlaces(tAddCessAmt, decimalPlaces);
            order.cgstAmount = RoundOffByDecimalPlaces(tCgstAmount,decimalPlaces);
            order.sgstAmount = RoundOffByDecimalPlaces(tSgstAmount, decimalPlaces);
            order.igstAmount = RoundOffByDecimalPlaces(tIgstAmount, decimalPlaces);
            order.kfcessAmt = RoundOffByDecimalPlaces(tKfcessAmt, decimalPlaces);
            order.roundOff = RoundOffByDecimalPlaces(roundOffAmount, decimalPlaces); ;
            order.roundOffMode = 0;
            order.cashDiscount = 0;
            order.otherExpense = 0;
            order.freightCoolie = 0;
            order.totalTax = RoundOffByDecimalPlaces((order.taxAmount + order.kfcessAmt + order.cessAmount + order.AddCessAmt), decimalPlaces);
            order.billAmount = RoundOffByDecimalPlaces(totalAmount, decimalPlaces);
            order.cost = tCost;
            order.SaleValues = RoundOffByDecimalPlaces(tSaleValues, decimalPlaces);
            order.BillProfit = RoundOffByDecimalPlaces(tBillProfit, decimalPlaces);
            

            return new APIResponse(APIResponseStatus.success, order, "");

        }

        [HttpPost]
        [Route("api/Order/Create")]
        public APIResponse PlaceOrder(Order order)
        {
            var headers = Request.Headers;
            int customerId = APIUtil.GetCustomerId(db, headers);

            if (order == null || order.orderDetails == null || order.orderDetails.Count() == 0)
            {
                return new APIResponse(APIResponseStatus.failed, null, "Invalid Request");
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var trsOrder = JsonConvert.DeserializeObject<trsOrder>(JsonConvert.SerializeObject(order));
                    long? orderNo = db.trsOrders.Where(o => o.invType == "SalesOrder").Max(o => o.AutoNo);
                    trsOrder.invno = ((orderNo ?? 0) + 1) + "";
                    long? orderId = db.trsOrders.Max(o => (long?)o.invid);
                    trsOrder.invid = ((orderId ?? 0) + 1);
                    trsOrder.invdate = DateTime.Now;
                    trsOrder.AutoNo = ((orderNo ?? 0) + 1);
                    db.trsOrders.Add(trsOrder);
                    db.SaveChanges();
                    order.orderDetails.ForEach(item =>
                    {
                        string itmQry = "INSERT INTO [dbo].[trsOrderDetail] ([invID],[itemid],[taxper],[mrp],[quantity],[freeQty],[grossAmount],[Discount],[taxable],[taxAmount],[netAmount],[batch],[cost],[saleValue]" +
                            ",[profit],[rOrder],[unitfactor],[blnAltUnit],[unit],[expDate],[sRate],[pRate],[modifier],[cgstPer],[cgstAmount],[sgstPer],[sgstAmount],[igstPer],[igstAmount],[cessPer],[cessAmount],[AddCessRate],[AddCessAmt],[Rate],[SerialNo],[kfcessper],[kfcessamt])" +
                            "VALUES(" + trsOrder.invid + "," + item.itemid + "," + GetValue(item.taxper) + "," + GetValue(item.mrp) + "," + GetValue(item.quantity) + "," + GetValue(item.freeQty) + "," + GetValue(item.grossAmount) + "," + GetValue(item.Discount) + "," + GetValue(item.taxable) + "," + GetValue(item.taxAmount) + "," + GetValue(item.netAmount) +
                            ",'" + item.batch + "'," + GetValue(item.cost) + "," + GetValue(item.saleValue) + "," + GetValue(item.profit) + "," + item.rOrder +"," + (item.unitfactor==null?"NULL": item.unitfactor+"") + "," + ((item.blnAltUnit==null || item.blnAltUnit==false)?0:1) + ",'" + item.unit + "','" + item.expDate +
                            "'," + GetValue(item.sRate) + "," + GetValue(item.pRate) + ",'" + item.modifier + "'," + GetValue(item.cgstPer) + "," + GetValue(item.cgstAmount) + "," + GetValue(item.sgstPer) + "," + GetValue(item.sgstAmount) + "," + GetValue(item.igstPer) + "," + GetValue(item.igstAmount) + "," + GetValue(item.cessPer) +
                            "," + GetValue(item.cessAmount) + "," + GetValue(item.AddCessRate) + "," + GetValue(item.AddCessAmt) + "," + GetValue(item.Rate) + ",'" + item.SerialNo + "'," + GetValue(item.kfcessper) + "," + GetValue(item.kfcessamt) + ")";
                       
                        db.Database.ExecuteSqlCommand(itmQry);
                        
                    });
                    var companyVPALedger = GetVPALedger();

                    if (order.mop == "Mixed" && companyVPALedger != "")
                    {
                        trsVoucher trsVoucher = new trsVoucher();
                        long? voucherNo = db.trsVouchers.Where(o => o.vchtype == "Receipt").Max(o => o.invno);
                        trsVoucher.invno = ((voucherNo ?? 0) + 1);
                        long? voucherId = db.trsVouchers.Max(o => (long?)o.InvID);
                        trsVoucher.InvID = ((voucherId ?? 0) + 1);
                        trsVoucher.invdate = DateTime.Now;
                        trsVoucher.vchtype = "Receipt";
                        trsVoucher.drLID = int.Parse(companyVPALedger);//bank
                        trsVoucher.crLID = (int)order.lid;//customer ledger
                        trsVoucher.amount = (decimal)order.cardAmount;
                        trsVoucher.refNo = "";
                        trsVoucher.remarks = order.remarks;//payment ref number
                        trsVoucher.issueingBank = "";
                        trsVoucher.chequeNo = "";
                        trsVoucher.spID = 1;
                        trsVoucher.crAmount = 0;
                        trsVoucher.cancelled = false;
                        trsVoucher.narration = "";
                        trsVoucher.CheqStatus = "No Status";
                        trsVoucher.ChequeDate = new DateTime(1900, 01, 01);
                        trsVoucher.ReconcilDate = new DateTime(1900,01,01);
                        trsVoucher.IsReconcil = false;
                        trsVoucher.compName = "APP";
                        trsVoucher.userid = 1;
                        trsVoucher.taxable =order.cardAmount;
                        trsVoucher.taxAmt = 0;
                        trsVoucher.taxPer = 0;
                        trsVoucher.lid = 0;
                        trsVoucher.taxRefNo = "";
                        trsVoucher.rMode = 0;
                        trsVoucher.ccid = 1;
                        trsVoucher.ccName = "MAIN";
                        trsVoucher.agentid = 1;

                        string vchQuery = "INSERT INTO[dbo].[trsVoucher] ([InvID],[invno],[invdate],[vchtype],[drLID],[crLID],[amount],[refNo],[remarks],[issueingBank],[chequeNo],[spID],[cancelled],[crAmount],[narration],[IsReconcil],[CheqStatus],[ChequeDate],[ReconcilDate],[compName],[userid],[taxable],[taxAmt],[taxPer],[lid],[taxRefNo],[rMode],[ccid],[ccName],[agentid])"+
                        "VALUES("+ trsVoucher.InvID + ","+ trsVoucher.invno + ",'" + trsVoucher.invdate + "','" + trsVoucher.vchtype + "'," + trsVoucher.drLID + "," + trsVoucher.crLID + "," + trsVoucher.amount + ",'" + trsVoucher.refNo + "','" + trsVoucher.remarks + "','" + trsVoucher.issueingBank + "','" + trsVoucher.chequeNo + "'," + trsVoucher.spID + "," + (trsVoucher.cancelled==true?1:0) + "," + trsVoucher.crAmount + ",'" + trsVoucher.narration + "'," + (trsVoucher.IsReconcil == true ? 1 : 0) + ",'" + trsVoucher.CheqStatus + "','" + trsVoucher.ChequeDate + "','" + trsVoucher.ReconcilDate + "','" + trsVoucher.compName + "'," + trsVoucher.userid + "," + trsVoucher.taxable + "," + trsVoucher.taxAmt + "," + trsVoucher.taxPer + "," + trsVoucher.lid + ",'" + trsVoucher.taxRefNo + "'," + trsVoucher.rMode + "," + trsVoucher.ccid + ",'" + trsVoucher.ccName + "'," + trsVoucher.agentid+")";

                        db.Database.ExecuteSqlCommand(vchQuery);

                        string accQuery = "INSERT INTO[dbo].[trsAccounts]([Invid],[InvType],[InvDate],[Drlid],[Crlid],[AmountD],[AmountC],[invNo],[remarks],[IsHold],[ccid],[agentid])" +
                        "VALUES(" + trsVoucher.InvID + ",'" + trsVoucher.vchtype + "','" + trsVoucher.invdate + "'," + trsVoucher.drLID + "," + trsVoucher.crLID + ", " + trsVoucher.amount + ",0," + trsVoucher.invno + ",'" + trsVoucher.remarks + "',0,1,1)";

                        db.Database.ExecuteSqlCommand(accQuery);

                        string accQuery2 = "INSERT INTO[dbo].[trsAccounts]([Invid],[InvType],[InvDate],[Drlid],[Crlid],[AmountD],[AmountC],[invNo],[remarks],[IsHold],[ccid],[agentid])" +
                        "VALUES(" + trsVoucher.InvID + ",'" + trsVoucher.vchtype + "','" + trsVoucher.invdate + "'," + trsVoucher.crLID + "," + trsVoucher.drLID + ",0,"+ trsVoucher.amount +","+ trsVoucher.invno + ",'" + trsVoucher.remarks + "',0,1,1)";

                        db.Database.ExecuteSqlCommand(accQuery2);
                    }

                    transaction.Commit();

                    var savedOrder = db.trsOrders.Where(o => o.invid == trsOrder.invid && o.invType == "SalesOrder").ToList().Select(o => Build(o)).FirstOrDefault();

                    if (savedOrder == null)
                    {
                        return new APIResponse(APIResponseStatus.failed, null, "Order Not Found");
                    }

                    var savedOrderDetails = db.trsOrderDetails.Where(d => d.invID == savedOrder.invid).ToList().Select(d => BuildOrderItem(d)).ToList();

                    savedOrder.orderDetails = savedOrderDetails;

                    var orderTemplate = db.InvoiceSettings.Where(s => s.InvoiceType == "SalesOrder" && s.smsformat != null && s.smsformat != "").FirstOrDefault();
                    if (orderTemplate != null)
                    {
                        Dictionary<string, Object> parameters = GetOrderParams(trsOrder);
                        
                        new NotificationUtil().SendSMS(trsOrder.contactno, orderTemplate.smsformat,orderTemplate.smsid, parameters);
                    }

                    return new APIResponse(APIResponseStatus.success, savedOrder, "Order Placed Successfully");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _tracer.Error(Request, this.ControllerContext.ControllerDescriptor.ControllerType.FullName, "Place Order Failed, Reason:" + ex.Message);
                    return new APIResponse(APIResponseStatus.failed, null, "Place Order Failed");
                }
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public Dictionary<string, object> GetOrderParams(trsOrder trsOrder)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("<VchNo>", trsOrder.invno);
            dict.Add("<VchDate>", trsOrder.invdate.Value.ToShortDateString());
            dict.Add("<VchTime>", trsOrder.invdate.Value.ToShortTimeString());
            dict.Add("<PayMode>", trsOrder.mop ?? "");
            dict.Add("<Party>", trsOrder.party ?? "");
            dict.Add("<Address>", trsOrder.address??"");

            dict.Add("<TGross>", trsOrder.grossAmount ??0);
            dict.Add("<TTaxable>", trsOrder.taxableAmount ?? 0);
            dict.Add("<TCGST>", trsOrder.cgstAmount ?? 0);
            dict.Add("<TSGST>", trsOrder.sgstAmount ?? 0);
            dict.Add("<TIGST>", trsOrder.igstAmount ?? 0);
            dict.Add("<TTax>", trsOrder.taxAmount ?? 0);
            dict.Add("<TCESS>", trsOrder.cessAmount ?? 0);
            dict.Add("<TADCESS>", trsOrder.AddCessAmt ?? 0);
            dict.Add("<Savings>", trsOrder.SaveAmt ?? 0);
            dict.Add("<OtherExp>", trsOrder.otherExpense ?? 0);
            dict.Add("<FreightCoolie>", trsOrder.freightCoolie ?? 0);
            dict.Add("<Remarks>", trsOrder.remarks ?? "");
            dict.Add("<CashDisc>", trsOrder.cashDiscount ?? 0);
            dict.Add("<RoundOFF>", trsOrder.roundOff ?? 0);
            dict.Add("<Discount>", trsOrder.Discount ?? 0);
            dict.Add("<BillAmt>", trsOrder.billAmount ?? 0);
            dict.Add("<TaxAmt>", trsOrder.taxAmount ?? 0);
            dict.Add("<Loyalty>", trsOrder.grossAmount ?? 0);
            dict.Add("<Inwords>", NumberToWordsConverter.ConvertAmount(trsOrder.billAmount??0,"Rupees","Paisa"));

            // var ledgerBalance = db.trsAccounts.Where(a => a.Drlid == customer.lid && a.IsHold==false).Sum(a => ((a.AmountD == null ? 0 : a.AmountD) - (a.AmountC == null ? 0 : a.AmountC)));
            var ledgerBalance = db.Database.SqlQuery<decimal>("Select ISNULL(sum(ISNULL(AmountD,0)-ISNULL(AmountC,0)),0) from [dbo].trsAccounts where Drlid=" + trsOrder.lid + " and IsHold=0").FirstOrDefault();
          
            dict.Add("<CurrentBalance>", RoundOffByDecimalPlaces(ledgerBalance, GetDecimalPlaces()));
            //dict.Add("<SalePerson>", cbSP.Text);
            //dict.Add("<CDRecAmt>", _cdRecAmt.ToString("0.00"));
            //dict.Add("<CDBalAmt>", _cdBalAmt.ToString("0.00"));

            return dict;
        }



        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public Order Build(trsOrder trsOrder)
        {
            int decimalPlaces = GetDecimalPlaces();

            var Order = JsonConvert.DeserializeObject<Order>(JsonConvert.SerializeObject(trsOrder));

            Order.totalTax = RoundOffByDecimalPlaces((Order.taxAmount + Order.kfcessAmt + Order.cessAmount + Order.AddCessAmt), decimalPlaces); 

            return Order;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public OrderDetail BuildOrderItem(trsOrderDetail trsOrderDetail)
        {
            int decimalPlaces = GetDecimalPlaces();
            var item = JsonConvert.DeserializeObject<OrderDetail>(JsonConvert.SerializeObject(trsOrderDetail));
            item.totalTax = RoundOffByDecimalPlaces((item.taxAmount + item.kfcessamt + item.cessAmount + item.AddCessAmt),decimalPlaces);
            if (item!=null && item.itemid != null)
            {
                var Product = db.mtrProducts.Where(p => p.prodID == item.itemid).FirstOrDefault();
                if (Product != null)
                {
                    item.ItemName = Product.prodName;
                }
            }

            return item;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public List<trsPricelist> GetPriceList(long customerId)
        {

            //Check PriceList Enabled Or Not
            var blnPricelist = db.companySettings.Where(e => e.KeyName == "blnPricelist").FirstOrDefault();

            if (blnPricelist == null || blnPricelist.ValueName == null || blnPricelist.ValueName.Trim() == "" && blnPricelist.ValueName == "False")
            {
                return null;
            }

            var customer = db.mtrledgers.Find(customerId);
            if (customer == null || customer.plID == null)
            {
                return null;
            }

            var priceList = db.mtrPricelists.Where(p => p.plId == customer.plID && p.deActive == false).FirstOrDefault();
            if (priceList == null)
            {
                return null;
            }

            var priceListItems = db.trsPricelists.Where(p => p.plId == customer.plID).ToList();
            if (priceListItems == null || priceListItems.Count() == 0)
            {
                return null;
            }

            return priceListItems;


        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public bool IsBatchEnabled()
        {

            //Check PriceList Enabled Or Not
            var blnbarcode = db.companySettings.Where(e => e.KeyName == "blnbarcode").FirstOrDefault();

            if (blnbarcode == null || blnbarcode.ValueName == null || blnbarcode.ValueName.Trim() == "" && blnbarcode.ValueName == "False")
            {
                return false;
            }

            return true;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public bool IsCessEnabled()
        {

            //Check PriceList Enabled Or Not
            var blnCess = db.companySettings.Where(e => e.KeyName == "blnCess").FirstOrDefault();

            if (blnCess == null || blnCess.ValueName == null || blnCess.ValueName.Trim() == "" && blnCess.ValueName == "False")
            {
                return false;
            }

            return true;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public bool IsAddCessEnabled()
        {

            //Check PriceList Enabled Or Not
            var blnAddCess = db.companySettings.Where(e => e.KeyName == "blnAddCess").FirstOrDefault();

            if (blnAddCess == null || blnAddCess.ValueName == null || blnAddCess.ValueName.Trim() == "" && blnAddCess.ValueName == "False")
            {
                return false;
            }

            return true;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public bool IsKFCessEnabled()
        {

            //Check PriceList Enabled Or Not
            var blnKFCess = db.companySettings.Where(e => e.KeyName == "blnKFCess").FirstOrDefault();

            if (blnKFCess == null || blnKFCess.ValueName == null || blnKFCess.ValueName.Trim() == "" && blnKFCess.ValueName == "False")
            {
                return false;
            }

            return true;

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public int GetDecimalPlaces()
        {

            //Check PriceList Enabled Or Not
            var amtDecimal = 2;
            try
            {
                var companySetting = db.companySettings.Where(e => e.KeyName == "amtDecimal").FirstOrDefault();
                amtDecimal = int.Parse((companySetting == null || companySetting.ValueName == null || companySetting.ValueName.Equals("")) ? "2" : companySetting.ValueName);
            }
            catch
            {
                amtDecimal = 2;
            }
            return amtDecimal;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public double RoundOffByDecimalPlaces(Nullable<double> value, int decimalPlaces)
        {

            return (double)Math.Round((value == null ? 0 : (decimal)value), decimalPlaces);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public double RoundOffByDecimalPlaces(Nullable<decimal> value, int decimalPlaces)
        {

            return (double)Math.Round((value == null ? 0 : (decimal)value), decimalPlaces);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public double GetValue(Nullable<double> value)
        {

            return (value == null ? 0 : (double)value);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public decimal GetValue(Nullable<decimal> value)
        {

            return (value == null ? 0 : (decimal)value);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public string GetVPAAddress()
        {

            //Check PriceList Enabled Or Not
            var companyVPA="" ;
            try
            {
                var companySetting = db.companySettings.Where(e => e.KeyName == "companyVPA").FirstOrDefault();
                companyVPA = (companySetting == null || companySetting.ValueName == null || companySetting.ValueName.Equals("")) ? "" : companySetting.ValueName;
            }
            catch
            {
                companyVPA = "";
            }
            return companyVPA;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public string GetVPALedger()
        {

            //Check PriceList Enabled Or Not
            var companyVPALedger = "";
            try
            {
                var companySetting = db.companySettings.Where(e => e.KeyName == "companyVPALedger").FirstOrDefault();
                companyVPALedger = (companySetting == null || companySetting.ValueName == null || companySetting.ValueName.Equals("")) ? "" : companySetting.ValueName;
            }
            catch
            {
                companyVPALedger = "";
            }
            return companyVPALedger;
        }
    }
}
