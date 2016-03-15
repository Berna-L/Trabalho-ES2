using LoginBancoTeste.DAL;
using LoginBancoTeste.Models;
using LoginBancoTeste.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoginBancoTeste.Controllers
{
    public class TransacoesController : Controller
    {
        private BancoContext db = new BancoContext();

        // GET: Transacoes
        public ActionResult Index(int? id)
        {
            Cliente cliente;

            if (User.Identity.IsAuthenticated)
            {
                cliente = this.db.Clientes.Where(s => User.Identity.Name == s.Username).SingleOrDefault();
                return View(cliente);
            }

            // neste caso aind não logamos como cliente
            if (id == null)
            {
                return View();
            }

            // neste caso logamos como cliente
            cliente = this.db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        [Authorize]
        public ActionResult Opcoes(int? numero)
        {
            if (numero == null)
            {
                return View();
            }
            Conta conta = this.db.Contas.Find(numero);
            if (conta == null)
            {
                return HttpNotFound();
            }
            return View(conta);
        }

        [Authorize]
        public ActionResult Saldo(int? numero)
        {
            if (numero == null)
            {
                return View();
            }
            Conta conta = this.db.Contas.Find(numero);
            if (conta == null)
            {
                return HttpNotFound();
            }
            return View(conta);
        }

        [Authorize]
        public ActionResult Saque()
        {
            return View();
        }

        [Authorize]
        public ActionResult Investimentos()
        {
            return View();
        }

        public ActionResult Deposito()
        {
            return View();
        }

        public ActionResult Transferencia()
        {
            return View();
        }

        public ActionResult Pagamento(int numero, String errorMsg)
        {
            PagamentoViewModel pvm = new PagamentoViewModel();
            pvm.numConta = numero;
            pvm.errorMsg = errorMsg;
            return View(pvm);
        }

        public ActionResult PgmtoConfirma(PagamentoViewModel pvm)
        {
            PagamentoSingleton pgmto = PagamentoSingleton.Instance;
            Conta conta = this.db.Contas.Find(pgmto.numConta);

            if (conta == null)
            {
                return HttpNotFound();
            }

            pvm.cod_boleto = pgmto.cod_boleto;
            pvm.conta = conta;
            pvm.data_pagam = pgmto.data_pagam;
            pvm.data_realiza = pgmto.data_realiza;
            pvm.data_venc = pgmto.data_venc;
            pvm.desc_adicional = pgmto.descricao;
            pvm.valor = pgmto.valor;

            Pagamento pagamento = new Pagamento();
            pagamento.cod_boleto = pvm.cod_boleto;
            pagamento.conta = pvm.conta;
            pagamento.data_pagam = pvm.data_pagam.ToUniversalTime();
            pagamento.data_realiza = pvm.data_realiza.ToUniversalTime();
            pagamento.data_venc = pvm.data_venc.ToUniversalTime();
            pagamento.descricao = pvm.desc_adicional;
            pagamento.valor = pvm.valor;

            if (ModelState.IsValid)
            {
                db.Pagamentos.Add(pagamento);
                db.SaveChanges();
                pvm.numeroPgmto = pagamento.id;
            }

            return View(pvm);
        }

        public ActionResult PgmtoVencido()
        {
            return View();
        }

        public ActionResult PgmtoVerifica(PagamentoViewModel pvm, int? numero)
        {
            PagamentoSingleton pgmto = PagamentoSingleton.Instance;
            if (numero == 0)
            {
                pgmto.valor = pvm.valor;
                pvm.cod_boleto = pgmto.cod_boleto;
                pvm.data_pagam = pgmto.data_pagam;
                pvm.data_realiza = pgmto.data_realiza;
                pvm.data_venc = pgmto.data_venc;
                pvm.desc_adicional = pgmto.descricao;
                pvm.numConta = pgmto.numConta;
            }
            else {
                pgmto = null;
                pgmto = PagamentoSingleton.Instance;
            }
            if (numero == null)
            {
                return View();
            }

            Conta conta;
            if (numero != 0)
            {
                conta = this.db.Contas.Find(numero);
            }
            else
            {
                conta = this.db.Contas.Find(pgmto.numConta);
            }

            if (conta == null)
            {
                return HttpNotFound();
            }
            pvm.conta = conta;
            pvm.numConta = conta.Numero;
            if(numero != 0) pgmto.numConta = conta.Numero;
            
            // codigo do boleto completo
            long cod_boleto = pvm.cod_boleto;
            if (numero != 0) pgmto.cod_boleto = cod_boleto;
            int cod_banco = (int)(cod_boleto / Math.Pow(10,15));

            //codigo do boleto sem 3 digitos
            cod_boleto = cod_boleto - cod_banco * (long)Math.Pow(10, 15);
            int cod_moeda = (int)(cod_boleto / Math.Pow(10, 14));

            //codigo do boleto sem 4 digitos
            cod_boleto = cod_boleto - cod_moeda * (long)Math.Pow(10, 14);
            int cod_fatvenc = (int)(cod_boleto / Math.Pow(10, 10));
            pvm.cod_fatvenc = cod_fatvenc; //Apenas para tratar boletos sem vencimento

            //codigo do boleto sem 8 digitos
            cod_boleto = cod_boleto - cod_fatvenc * (long)Math.Pow(10, 10);
            double valor = (double)cod_boleto / 100;

            Boolean erro_boleto = false;
            String msgErro = "";

            Banco banco = this.db.Bancos.Where(ban => cod_banco == ban.numBanco).SingleOrDefault();

            if (pvm.conta.Saldo < valor)
            {
                erro_boleto = true;
                msgErro = "Saldo insuficiente para este pagamento.";
            }
            if (cod_moeda != 4)
            {
                erro_boleto = true;
                msgErro = "Código de boleto inválido. Esse caixa só aceita pagamentos em Real.";
            }
            if (banco == null)
            {
                erro_boleto = true;
                msgErro = "Código de boleto inválido. Banco não existe.";
            }

            if (erro_boleto)
                return RedirectToAction("Pagamento", new { numero = pvm.numConta, errorMsg = msgErro });

            DateTime hoje = DateTime.Today;
            DateTime base_venc = new DateTime(1997, 10, 07);
            DateTime venc = base_venc.AddDays(cod_fatvenc);
            pvm.data_realiza = pvm.data_pagam;
            pvm.valor = valor;
            if (numero != 0)
            {
                pgmto.data_venc = venc;
                pgmto.data_pagam = pvm.data_pagam;
                pgmto.valor = valor;
            }

            Boolean menor = (venc < hoje);
            if (menor){
                return RedirectToAction("PgmtoVencido");
            }
            else
            {
                if((venc == hoje) && (hoje.Hour >= 16))
                {
                    pvm.data_realiza = hoje.AddDays(1);
                }
            }
            if (numero != 0) pgmto.data_realiza = pvm.data_realiza;

            if (valor == 0 && pgmto.valor == 0)
            {
                return RedirectToAction("PgmtoVlrVenc");
            }

            if (numero != 0) pgmto.descricao = pvm.desc_adicional;
            pvm.valor = pgmto.valor;

            return View(pvm);
        }

        public ActionResult PgmtoVlrVenc()
        {
            PagamentoViewModel pvm = new PagamentoViewModel();
            return View(pvm);
        }

    }
}