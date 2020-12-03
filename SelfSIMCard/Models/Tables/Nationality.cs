using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SelfSIMCard.Models
{
    [Table("tcc_nationality")]
    public class Nationality
    {
        [Key]
        public int Code { get; set; }
        public string Name_ar { get; set; }
        public string Name_en { get; set; }
    }
}