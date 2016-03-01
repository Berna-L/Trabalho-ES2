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
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public virtual Conta conta { get; set; }

        //[Required]
        //public virtual Agencia agencia { get; set; }

        //[Required]
        //public virtual Banco banco{ get; set; }
    }
}