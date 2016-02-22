using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.Models.ViewModels
{
    public class TransferenciaViewModel
    {
        // numero da conta 
        public int? NumeroConta { get; set; }

        // numero da conta do destinatário
        [Required]
        [Display(Name = "conta destino")]
        public int? NumeroContaDestino { get; set; }
        
        // conta na qual iremos mandar os dados
        public Conta ContaDestino { get; set; }

        // valor a ser transferio de uma conta para outra, para realizar transferencia de ser de no minimo 5 reais
        [Required]
        [Range(5, Double.MaxValue, ErrorMessage = "O valor minimo para realizar transferencia é de 5 reais!")]
        public double Valor { get; set; }
    }

}