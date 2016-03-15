using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models
{
    public enum TipoDeConta
    {
        Corrente, Poupanca
    }

    public class Conta
    {   
        [Key]
        public int Numero { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Saldo { get; set; }

        [Required]
        [Display(Name = "Tipo de conta")]
        public TipoDeConta TipoDeConta { get; set; }

        public virtual List<Extrato> Extrato { get; set; }

        public virtual Cliente Cliente { get; set; }

        public virtual Agencia agencia { get; set; }

    }
}