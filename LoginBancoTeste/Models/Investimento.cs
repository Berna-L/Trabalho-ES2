using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models {
    public class Investimento {

        public int Id { get; set; }

        [Required]
        public Cliente cliente { get; set; }

        [Required]
        [Display(Name = "Tipo de investimento")]
        public virtual TipoInvestimento tipo_invest { get; set; }

        [Required]
        [Display(Name = "Valor inicial")]
        [DisplayFormat(DataFormatString = "{0:c2}")]
        public long valor_ini { get; set; }

        [Required]
        [Display(Name = "Valor acumulado até a data")]
        [DisplayFormat(DataFormatString = "{0:c2}")]
        public long valor_acc { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data de início")]
        public DateTime data{ get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de cancelamento")]
        public DateTime? data_canc { get; set; }

    }
}