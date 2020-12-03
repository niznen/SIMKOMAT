using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SelfSIMCard.Models
{
    public class DbStock
    {
        [Key]
        public string ICCID { get; set; }
        public string IMSI { get; set; }
        public string ORG_NAME { get; set; }
        public string MODEL_ID { get; set; }
        public int? RES_STATUS_ID { get; set; }
        public string MSISDN { get; set; }
        public long? SUB_ID { get; set; }
        public long? CUST_ID { get; set; }
        public string SUB_ICCID { get; set; }
        public string PUK1 { get; set; }
    }
}