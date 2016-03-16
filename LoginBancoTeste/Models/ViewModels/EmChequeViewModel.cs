using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models.ViewModels
{
    public class EmChequeViewModel
    {
        public int numConta { get; set; }
        public int qtdCheque { get; set; }
        public String dirSaida { get; set; }
        public String msgControle { get; set; }
    }
}