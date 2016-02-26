using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models
{
    public class Estoque
    {
        public int EstoqueID { get; set; }
        public int QtdNotas10 { get; set; }
        public int QtdNotas20 { get; set; }
        public int QtdNotas50 { get; set; }
        public int QtdNotas100 { get; set; }
        public int QtdCheques { get; set; }
        
    }
}