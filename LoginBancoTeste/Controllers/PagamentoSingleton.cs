using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Controllers
{
    public class PagamentoSingleton
    {
        public long cod_boleto { get; set; }
        public double valor { get; set; }
        public String descricao { get; set; }
        public DateTime data_venc { get; set; }
        public DateTime data_pagam { get; set; }
        public DateTime data_realiza { get; set; }
        public int numConta { get; set; }

        private static PagamentoSingleton instance;
        private PagamentoSingleton() { }

        public static PagamentoSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PagamentoSingleton();
                }
                return instance;
            }
        }
    }
}