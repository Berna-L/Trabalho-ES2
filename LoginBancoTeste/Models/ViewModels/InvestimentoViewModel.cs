using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models.ViewModels {
    public class InvestimentoViewModel {

        [Required]
        public int? numCliente { get; set; }
        [Required]
        public int? contaADebitar { get; set; }
        [Required]
        public DateTime data { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage ="Valor vazio.")]
        public int valor { get; set; }
        [Required]
        public int? tipo { get; set; }

    }
}