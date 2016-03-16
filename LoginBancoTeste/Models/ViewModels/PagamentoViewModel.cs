using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LoginBancoTeste.Models.ViewModels
{
    public class PagamentoViewModel
    {
        [Required]
        public long cod_boleto { get; set; }

        public int numeroPgmto { get; set; }

        public double valor { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_pagam { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_realiza { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime data_venc { get; set; }

        public int cod_fatvenc { get; set; } //Apenas para tratar boletos sem vencimento

        public String desc_adicional { get; set; } //Descricao adicional de um pagamento
        public int numConta { get; set; }
        public Conta conta { get; set; }

        public String errorMsg { get; set; } //Para tratamento de erros

        public PagamentoViewModel() { }
        public PagamentoViewModel(PagamentoViewModel other)
        {
            cod_boleto = other.cod_boleto;
            numeroPgmto = other.numeroPgmto;
            valor = other.valor;
            data_pagam = other.data_pagam;
            data_realiza = other.data_realiza;
            data_venc = other.data_venc;
            cod_fatvenc = other.cod_fatvenc;
            desc_adicional = other.desc_adicional;
            numConta = other.numConta;
            conta = other.conta;
            errorMsg = other.errorMsg;
        }
    }
}