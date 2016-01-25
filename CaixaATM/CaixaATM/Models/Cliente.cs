﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CaixaATM.Models
{
    // flag para ajudar a decidir que tipo de conta estamos lidando
    public enum TipoDeCliente
    {
        PESSOA_FISCA, PESSOA_JURIDICA
    }

    public class Cliente
    {
        public int ClienteID { get; set; }

        [Required]
        public string Nome { get; set; }
                
        [Required]
        public string Telefone1 { get; set; }
        
        public string Telefone2 { get; set; }

        [Required]
        public string Email { get; set; }

        public TipoDeCliente TipoDeCliente { get; set; }

        public virtual Endereco Endereco { get; set; }
        public virtual List<Conta> Contas { get; set; }
    }
}