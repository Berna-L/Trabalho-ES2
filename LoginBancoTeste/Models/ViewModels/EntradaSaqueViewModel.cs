using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models.ViewModels
{
    public class EntradaSaqueViewModel
    {
        public int? NumeroConta { get; set; }

        [Required]
        public double Valor { get; set; }
    }
}