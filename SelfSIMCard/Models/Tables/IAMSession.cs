using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SelfSIMCard.Models
{
    [Table("iam_sales_session")]
    public class IAMSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string SessionId { get; set; }
        public string ICCID { get; set; }
        public string IMSI { get; set; }
        public int SimOrderId { get; set; }
        public int MVNO_ID { get; set; }
        public string SIM_ItemCode { get; set; }
    }
}