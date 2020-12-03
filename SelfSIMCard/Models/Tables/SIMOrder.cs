using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SelfSIMCard.Models
{
    [Table("store_sim_orders")]
    public class SIMOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string PaymentDone { get; set; }

        [Display(Name = "ID Number")]
        public string IdNumber { get; set; }
        
        [Display(Name = "Nationality")]
        public string Nationality { get; set; }

        public string Result { get; set; }
        public string OrderID { get; set; }
        public string MVNO_ID { get; set; }
        public string ChannelId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}