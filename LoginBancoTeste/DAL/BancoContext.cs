﻿using LoginBancoTeste.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace LoginBancoTeste.DAL
{
    public class BancoContext : DbContext
    {
        public BancoContext() : base("BancoContext") { }

        public DbSet<Conta> Contas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Estoque> Estoque { get; set; }

    }
}