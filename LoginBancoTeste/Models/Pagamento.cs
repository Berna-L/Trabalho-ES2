using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LoginBancoTeste.Models
{
    public class Pagamento
    {
        [Key]
        public int id { get; set; }

        public long cod_boleto { get; set; }
        public double valor { get; set; }
        public char status { get; set; }
        public String descricao { get; set; }
        public DateTime data_venc { get; set; }
        public DateTime data_pagam { get; set; }
        public DateTime data_realiza { get; set; }

        public Conta conta { get; set; }
    }
}