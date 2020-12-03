using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SelfSIMCard.Models
{
    [Table("store_sim_details")]
    public class StoreSIM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string ICCID { get; set; }
        public string IMSI { get; set; }
        public string PUK1 { get; set; }
        public int MVNO_ID { get; set; }
    }
}