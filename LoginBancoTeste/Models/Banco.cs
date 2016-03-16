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
        public long numBanco { get; set; }
        public String nome { get; set; }
    }
}