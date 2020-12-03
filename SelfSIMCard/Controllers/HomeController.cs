using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SelfSIMCard.Models;
using System.Data.Entity.Validation;

namespace SelfSIMCard.Controllers
{
    public class HomeController : Controller
    {
        private SelfSIMContext db = new SelfSIMContext();
        private CRMContext crm = new CRMContext();

        public ActionResult Index()
        {
            //var ApplePay = JsonConvert.DeserializeObject("{\"id\":\"8ac9a4a56d432c0a016d43481cc85a52\",\"paymentType\":\"DB\",\"paymentBrand\":\"VISA\",\"amount\":\"15.00\",\"currency\":\"SAR\",\"descriptor\":\"0257.9527.5489 6023 - Lebara\",\"merchantTransactionId\":\"c85b23b4 - bc42 - 4bbc - 9c96 - 4ab680be562020190918072936\",\"result\":{\"code\":\"000.000.000\",\"description\":\"Transaction succeeded\"},\"resultDetails\":{\"ExtendedDescription\":\"Transaction Approved.\",\"clearingInstituteName\":\"SAIB MPGS\",\"ConnectorTxID1\":\"8ac9a4a56d432c0a016d43481cc85a52\",\"authorizationResponse.stan\":\"174682\",\"connectorId\":\"8ac9a4a56d432c0a016d43481cc85a52\",\"ConnectorTxID2\":\"0257.9527.5489\",\"transaction.receipt\":\"926107174682\",\"transaction.acquirer.settlementDate\":\"2019 - 09 - 18\",\"AcquirerResponse\":\"APPROVED\",\"reconciliationId\":\"0257.9527.5489\",\"transaction.authorizationCode\":\"079991\"},\"card\":{\"bin\":\"465154\",\"binCountry\":\"SA\",\"last4Digits\":\"0057\",\"expiryMonth\":\"03\",\"expiryYear\":\"2023\"},\"customer\":{\"ip\":\"37.99.129.146\",\"ipCountry\":\"SA\"},\"threeDSecure\":{\"eci\":\"07\",\"verificationId\":\"AeEYJSUAAt9KfFgNRdNNMAACAAA = \"},\"customParameters\":{\"APPLEPAY_Source\":\"Web\",\"orderid\":\"c85b23b4 - bc42 - 4bbc - 9c96 - 4ab680be5620\",\"CTPE_DESCRIPTOR_TEMPLATE\":\"\",\"APPLEPAY_TokenVersion\":\"EC_v1\"},\"risk\":{\"score\":\"0\"},\"buildNumber\":\"7db5c09d97c947847433be5ba406686508c588ba@2019 - 09 - 12 08:03:18 + 0000\",\"timestamp\":\"2019 - 09 - 18 07:31:04 + 0000\",\"ndc\":\"3FAC76D2828C3A9DDF293B5E08BF9E61.prod02 - vm - tx07\"}");
            return View();
        }


        private IEnumerable<SelectListItem> GetNationalities()
        {
            var nationalities = db
                            .Nationalities.OrderBy(a => a.Name_en)
                            .Select(x =>
                                new SelectListItem
                                {
                                    Value = x.Code.ToString(),
                                    Text = x.Name_en
                                }).ToList();

            return new SelectList(nationalities, "Value", "Text");
        }

        [HttpPost]
        public ActionResult Register(SIMOrder order)
        {
            if (ModelState.IsValid) {
                try
                {
                    db.SimOrders.Add(order);
                    db.SaveChanges();

                    Session["orderId"] = order.ID;
                    return RedirectToAction("Payment", new { amount = 32, orderId = order.ID });
                }catch(DbEntityValidationException e)
                {
                    Console.WriteLine(e.EntityValidationErrors);
                    var nationalities = GetNationalities();
                    ViewBag.nationalities = nationalities;
                    return View(order);
                }
            }
            else
            {
                var nationalities = GetNationalities();
                ViewBag.nationalities = nationalities;
                return View(order);
            }
        }

        public ActionResult Register()
        {
            Random rnd = new Random();
            var nationalities = GetNationalities();
            ViewBag.nationalities = nationalities;
            SIMOrder order = new SIMOrder();
            order.OrderID = rnd.Next(1943, 2500).ToString();
            order.PaymentDone = "1";
            order.MVNO_ID = "1";
            order.Result = "Payment is completed";
            order.ChannelId = "2";
            order.UserId = "0";
            order.UserName = "AbdullahAlshehri";
            order.IdNumber = null;

            return View(order);
        }

        public ActionResult Payment(double amount)
        {
            int orderId = Int32.Parse(Session["orderId"].ToString());
            Dictionary<string, dynamic> responseData = this.SubmitPayment(amount);
            ViewBag.CheckoutID = responseData["id"];
            ViewBag.Amount = amount;
            ViewBag.OrderId = orderId;
            return View();
        }

        public ActionResult Verify()
        {
            string resourcePath = Request.QueryString["resourcePath"];
            Dictionary<string, dynamic> paymentStatus = this.PaymentStatus(resourcePath);
            ViewBag.Result = JsonConvert.SerializeObject(paymentStatus);
            ViewBag.orderId = Session["orderId"];
            return View();
        }

        public ActionResult Activate()
        {
            return View();
        }

        public ActionResult GetToken()
        {
            int orderId = Int32.Parse(Session["orderId"].ToString());
            Dictionary<string, dynamic> responseData;
            string url = "https://api.simkomat.com/token-api/token";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.Headers["X-Auth-Token"] = "Le3E1zlG6Tj4X36p";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var s = new JavaScriptSerializer();
                responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
                dataStream.Close();
            }
            SMKToken smktoken = new SMKToken();
            smktoken.Token = responseData["token"];
            smktoken.Orderid = orderId;
            db.SMKTokens.Add(smktoken);
            db.SaveChanges();

            return Content(responseData["token"]);
        }

        public ActionResult CheckOrder()
        {
            int orderId = Int32.Parse(Session["orderId"].ToString());
            var result = from d in db.SMKTokens
                         where d.Orderid == orderId && d.Card_id != null
                         select d;
            return result.Count() > 0 ? Content(result.FirstOrDefault().Card_id) : Content("NO");
        }
        public ActionResult UpdateOrder(string ICCID)
        {
            int orderId = Int32.Parse(Session["orderId"].ToString());
            var result = from d in db.SMKTokens
                         where d.Orderid == orderId && d.Card_id != null
                         select d;

            IAMSession iamsession = new IAMSession();
            DbStock stock = crm.GetStock(ICCID);

            iamsession.SimOrderId = orderId;
            iamsession.ICCID = ICCID;
            iamsession.SessionId = result.FirstOrDefault().Token;
            iamsession.MVNO_ID = 1;
            iamsession.IMSI = stock.IMSI;
            iamsession.SIM_ItemCode = stock.MODEL_ID;

            StoreSIM storeSIM = new StoreSIM();
            storeSIM.ICCID = ICCID;
            storeSIM.MVNO_ID = iamsession.MVNO_ID;
            storeSIM.IMSI = stock.IMSI;
            storeSIM.PUK1 = stock.PUK1;


            try
            {
                db.IAMSessions.Add(iamsession);
                db.StoreSIMs.Add(storeSIM);
                db.SaveChanges();

                return Content("0");
            }catch(DbEntityValidationException e)
            {
                return Content(e.EntityValidationErrors.ToString());
            }
        }

        public Dictionary<string, dynamic> PaymentStatus(string resourcePath)
        {
            Dictionary<string, dynamic> responseData;
            string data = "entityId=8ac7a4c96b55ab97016b5f92c56009ff";
            string url = "https://test.oppwa.com" + resourcePath + "?" + data;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer OGE4Mjk0MTc0ZGMzYjQ5ZTAxNGRjZTU4NDg1YTBiNzR8WVI2dGJzRm4=";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var s = new JavaScriptSerializer();
                responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
                dataStream.Close();
            }
            return responseData;
        }

        public Dictionary<string, dynamic> SubmitPayment(double amount)
        {
            Dictionary<string, dynamic> responseData;
            string data = "entityId=8ac7a4c96b55ab97016b5f92c56009ff" +
                "&amount=" + amount +
                "&currency=SAR" +
                "&paymentType=DB" +
                "&merchantTransactionId=" + DateTime.Now.ToString("yyMMddhhmmssfffff") +
                "&customer.email=simkomat@lebara.sa" +
                "&testMode=EXTERNAL";
            string url = "https://test.oppwa.com/v1/checkouts";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.Headers["Authorization"] = "Bearer OGE4Mjk0MTc0ZGMzYjQ5ZTAxNGRjZTU4NDg1YTBiNzR8WVI2dGJzRm4=";
            request.ContentType = "application/x-www-form-urlencoded";
            Stream PostData = request.GetRequestStream();
            PostData.Write(buffer, 0, buffer.Length);
            PostData.Close();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var s = new JavaScriptSerializer();
                responseData = s.Deserialize<Dictionary<string, dynamic>>(reader.ReadToEnd());
                reader.Close();
                dataStream.Close();
            }
            return responseData;
        }
    }
}