﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoginBancoTeste.Models.ViewModels
{
    public class EstoqueViewModel
    {
        public int QtdNotas100 { get; set; }
        public int QtdNotas50 { get; set; }
        public int QtdNotas20 { get; set; }
        public int QtdNotas10 { get; set; }

        public bool Sucesso { get; set; }

    }
}
