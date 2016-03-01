using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models.ViewModels {
    public class InvestimentoViewModel {

        [Required]
        public Cliente cliente { get; set; }
        [Required]
        public Conta contaADebitar { get; set; }
        [Required]
        public DateTime data { get; set; }
        [Required]
        public int valor { get; set; }
        [Required]
        public TipoInvestimento tipo { get; set; }

    }
}