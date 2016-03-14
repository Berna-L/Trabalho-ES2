using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models
{
    public class Extrato
    {
        public int ExtratoId { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Data { get; set; }
        public string Lancamento { get; set; }
        public double Valor { get; set; }
        public double SaldoAtual { get; set; }

        public  virtual Conta Conta { get; set; }
    }
}