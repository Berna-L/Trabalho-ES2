using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LoginBancoTeste.Models
{
    public class Banco
    {
        [Key]
        public int numBanco { get; set; }
        public int codBanco { get; set; }
        public String nome { get; set; }
    }
}