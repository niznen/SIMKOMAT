using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SelfSIMCard.Models
{
    [Table("simkomat_dispense_result")]
    public class SMKToken
    {
        [Key]
        public string Token { get; set; }
        public int Orderid { get; set; }
        public string Card_id { get; set; }
    }
}