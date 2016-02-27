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
        public ActionResult Investimentos()
        {
            return View();
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
            // verifica se os campos digitados são validos de acordo com o modelo da classe
            if (ModelState.IsValid)
            {
                Conta conta = this.db.Contas.Find(deposito.NumeroConta);
                conta.Saldo += deposito.Valor;

                // faz o reigistro no extrato
                Extrato linhaExtrato = new Extrato();
                linhaExtrato.Data = DateTime.Now;
                linhaExtrato.Valor = deposito.Valor;
                linhaExtrato.SaldoAtual = conta.Saldo;
                linhaExtrato.Lancamento = "Deposito";

                // associa a nova instancia no extrato
                conta.Extrato.Add(linhaExtrato);

                // salva no banco de dados
                this.db.SaveChanges();

                // guarda a mensagem de sucesso para realização da operação, para exibir na view 
                TempData["Sucesso"] = "Seu deposito de R$ " + deposito.Valor + " reais foi realizado com sucesso!";

                // redireciona para o menu de opções
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
            //instancia o ViewModel para que o cliente possa preencher os dados da transferencia
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
                // neste caso o usuário não possui dinheiro suficiente para transferir
                if (conta.Saldo < dados.Valor)
                {
                    ViewBag.Error = "Saldo insuficiente para transferencia de R$ " + dados.Valor + " reais!";
                    return View(dados);
                }

                Conta contaDestino = this.db.Contas.Find(dados.NumeroContaDestino);
                // neste caso a conta destinho informada pelo usuário é inexistente
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

                // faz o reigistro no extrato
                Extrato linhaExtrato = new Extrato();
                linhaExtrato.Data = DateTime.Now;
                linhaExtrato.Valor = dados.Valor;
                linhaExtrato.SaldoAtual = conta.Saldo;
                linhaExtrato.Lancamento = "Transferencia";

                // associa a nova instancia no extrato
                conta.Extrato.Add(linhaExtrato);

                // salva no banco de dados
                this.db.SaveChanges();

                TempData["Sucesso"] = "Você transferiu R$ " + dados.Valor + " reais para " + contaDestino.Cliente.Nome + " com sucesso!";

                return RedirectToAction("Opcoes", new { numero = dados.NumeroConta });
            }

            return View(dados);
        }

        public ActionResult Extrato(int? numero)
        {
            if (numero == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);;
            }
            Conta conta = this.db.Contas.Find(numero);

            ExtratoModelView extrato = new ExtratoModelView();
            extrato.Extrato = conta.Extrato;
            extrato.NumeroConta = numero;
            extrato.NomeCliente = conta.Cliente.Nome;

            if (extrato == null)
            {
                return HttpNotFound();
            }
            return View(extrato);
        }

    }
}