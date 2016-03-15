using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models.ViewModels
{
    public class ExtratoModelView
    {
        public int? NumeroConta { get; set; }
        public string NomeCliente { get; set; }
        public List<Extrato> Extrato { get; set; }
    }
}