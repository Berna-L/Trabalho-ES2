﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CaixaATM.Models
{
    // flag para facilitar nas views quais dados exibir de tipo de conta
    public enum TipoDeConta
    {
        CORRENTE, POUPANCA
    }

    public class Conta
    {        
        public int ContaID { get; set; }

        [Required]
        public double Saldo { get; set; }

        public double SaldoProv { get; set; }

        public TipoDeConta TipoDeConta { get; set; }

        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }
    }
}