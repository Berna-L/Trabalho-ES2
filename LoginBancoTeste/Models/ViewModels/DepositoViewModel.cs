using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models.ViewModels
{
    public class DepositoViewModel
    {
        [Required]
        public int? NumeroConta { get; set; }

        [Required]
        [Range(5, Double.MaxValue, ErrorMessage = "O valor minimo para realizar um depósito é de 5 reais!")]
        public double Valor { get; set; }
    }
}