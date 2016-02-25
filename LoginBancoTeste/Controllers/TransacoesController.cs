using LoginBancoTeste.DAL;
using LoginBancoTeste.Models;
using LoginBancoTeste.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LoginBancoTeste.Controllers
{
    public class TransacoesController : Controller
    {
        private BancoContext db = new BancoContext();

        // HOME: Transacoes
        public ActionResult Index(int? id)
        {
            Cliente cliente;

            if (User.Identity.IsAuthenticated)
            {
                cliente = this.db.Clientes.Where(s => User.Identity.Name == s.Username).SingleOrDefault();
                return View(cliente);
            }

            // neste caso ainda não logamos como cliente
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
                return HttpNotFound();
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
        public ActionResult Investimentos(int? idCliente, int? idConta)
        {
            if (idCliente == null || idConta == null) {
                return HttpNotFound("deu id nulo mermão");
            }
            Cliente cliente = this.db.Clientes.Find(idCliente);
            Conta conta = this.db.Contas.Find(idConta);
            if (cliente == null || conta == null) {
                return HttpNotFound("deu objeto nulo rapá");
            }
            ViewBag.idCliente = idCliente;
            ViewBag.idConta = idConta;
            return View(this.db.Investimentos.Where(i => i.cliente.Id == cliente.Id));
        }

        [HttpPost]
        public ActionResult Investimentos(Investimento invest) {
            if (invest == null) {
                HttpNotFound();
            }
            return View(invest);
        }


        [Authorize]
        public ActionResult InvestimentoCriar(int? idCliente, int? idConta) {
            if (idCliente == null || idConta == null) {
                return HttpNotFound();
            }
            Investimento invest = new Investimento();
            invest.cliente = this.db.Clientes.Find(idCliente);
            invest.data = DateTime.Today;
            ViewBag.idConta = idConta;
            ViewBag.tipos = new SelectList(this.db.TiposInvestimento.ToList(), "Id", "nome", this.db.TiposInvestimento.Find(1));
            return View(invest);
        }

        public PartialViewResult DetalhesNovoInvestimento(int? idTipo, double? valor) {
            TipoInvestimento tipo = this.db.TiposInvestimento.Find(idTipo);
            ViewBag.jurosMes = tipo.jurosDia * 30 * 100;
            ViewBag.jurosAno = tipo.jurosDia * 365 * 100;
            ViewBag.rendMes =TipoInvestimentoAux.CalcularRendimento(valor.GetValueOrDefault(), tipo, DateTime.Today, DateTime.Today.AddMonths(1));
            ViewBag.rendAno = LoginBancoTeste.Models.TipoInvestimentoAux.CalcularRendimento(valor.GetValueOrDefault(), tipo, DateTime.Today, DateTime.Today.AddYears(1));
            return PartialView();
        }

        public ActionResult Deposito(int? numero)
        {
            if (numero == null)
            {
                return View();
            }
            DepositoViewModel deposito = new DepositoViewModel();
            deposito.NumeroConta = numero;

            return View(deposito);
        }

        [HttpPost]
        public ActionResult Deposito(DepositoViewModel deposito)
        {
            if (ModelState.IsValid)
            {
                Conta conta = this.db.Contas.Find(deposito.NumeroConta);
                conta.Saldo += deposito.Valor;
                this.db.SaveChanges();

                TempData["Sucesso"] = "Seu deposito de R$ " + deposito.Valor + " reais foi realizado com sucesso!";

                return RedirectToAction("Opcoes", new { numero = deposito.NumeroConta });
            }

            return View(deposito);
        }

        [Authorize]
        public ActionResult Transferencia(int? numero)
        {
            if (numero == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransferenciaViewModel dados = new TransferenciaViewModel();
            dados.NumeroConta = numero;

            return View(dados);
        }
        
        [HttpPost]
        public ActionResult Transferencia(TransferenciaViewModel dados)
        {
            if (ModelState.IsValid)
            {
                Conta conta = this.db.Contas.Find(dados.NumeroConta);
                if (conta.Saldo < dados.Valor)
                {
                    ViewBag.Error = "Saldo insuficiente para transferencia de R$ " + dados.Valor + " reais!";
                    return View(dados);
                }

                Conta contaDestino = this.db.Contas.Find(dados.NumeroContaDestino);
                if (contaDestino == null)
                {
                    ViewBag.Error = "Conta destino inexistente!";
                    return View(dados);
                }

                dados.ContaDestino = contaDestino;
                return View("TransferenciaConfirmacao", dados);
            }
            return View(dados);
        }

        [HttpPost]
        public ActionResult TransferenciaConfirmacao(TransferenciaViewModel dados)
        {
            if (ModelState.IsValid)
            {
                // descrementa da conta 
                Conta conta = this.db.Contas.Find(dados.NumeroConta);
                conta.Saldo -= dados.Valor;

                Conta contaDestino = this.db.Contas.Find(dados.NumeroContaDestino);
                // incrementa na conta destino
                contaDestino.Saldo += dados.Valor;

                // salva no banco de dados
                this.db.SaveChanges();

                TempData["Sucesso"] = "Você transferiu R$ " + dados.Valor + " reais para " + contaDestino.Cliente.Nome + " com sucesso!";

                return RedirectToAction("Opcoes", new { numero = dados.NumeroConta });
            }

            return View(dados);
        }

    }
}