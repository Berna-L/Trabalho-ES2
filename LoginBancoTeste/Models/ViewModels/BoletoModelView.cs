using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models.ViewModels
{
    public class BoletoModelView
    {

        public long cod_boleto { get; set; }
        public double valor { get; set; }
        public DateTime data_venc { get; set; }
    }
}