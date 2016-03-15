using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LoginBancoTeste.Models
{
    public class Agencia
    {
        [Key]
        public long numAgencia { get; set; }

        public virtual Banco banco { get; set; }
    }
}