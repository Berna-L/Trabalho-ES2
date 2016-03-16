using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models
{
    public class Cheque
    {
        [Key]
        public long numCheque { get; set; }

        //[Required]
        //public virtual Agencia agencia { get; set; }

        //[Required]
        //public virtual Banco banco{ get; set; }
    }
}