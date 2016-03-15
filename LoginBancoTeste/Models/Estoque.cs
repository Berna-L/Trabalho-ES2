using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models
{
    public class Estoque
    {
        public int EstoqueID { get; set; }
        [Display(Name = "Notas de 10")]
        public int QtdNotas10 { get; set; }
        [Display(Name = "Notas de 20")]
        public int QtdNotas20 { get; set; }
        [Display(Name = "Notas de 50")]
        public int QtdNotas50 { get; set; }
        [Display(Name = "Notas de 100")]
        public int QtdNotas100 { get; set; }
        [Display(Name = "Cheques")]
        public int QtdCheques { get; set; }
        
    }
}