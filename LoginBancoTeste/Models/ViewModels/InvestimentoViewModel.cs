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
        [Range(1, long.MaxValue, ErrorMessage ="Valor vazio.")]
        [DisplayFormat(DataFormatString = "{0:c2}")]
        public long valor { get; set; }
        [Required]
        public int? tipo { get; set; }
        [Required]
        public bool confirmado { get; set; }

    }
}