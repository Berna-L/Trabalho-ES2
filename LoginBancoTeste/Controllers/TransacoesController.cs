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
        public ActionResult Saque(int? numero)
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
        public ActionResult SaqueCustomizado(Conta conta)
        {
            if (conta == null)
            {
                return HttpNotFound();
            }
            return View("SaqueCustomizado", conta);
        }
        
        [Authorize]
        public ActionResult SaqueConfirm(int? numero, double valor) 
        {
            Conta conta = this.db.Contas.Find(numero);

            if ((valor <= 0) || (valor % 10 != 0))
            {
                TempData["Erro"] = "Valor incorreto. O caixa trabalha com notas de 10, 20, 50 e 100.";                

                return RedirectToAction("SaqueCustomizado", conta);
            }

            if (conta.Saldo < valor)
            {
                TempData["Erro"] = "Saldo insuficiente.";
                return RedirectToAction("Saque", conta);
            }

            EntradaSaqueViewModel entrada = new EntradaSaqueViewModel();
            entrada.NumeroConta = numero;
            entrada.Valor = valor;

            return View("SaqueConfirm", entrada);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SaqueConfirm(EntradaSaqueViewModel entrada)
        {
            String resposta = string.Empty;

            if (entrada.NumeroConta == null)
            {
                return HttpNotFound();
            }

            Conta conta = this.db.Contas.Find(entrada.NumeroConta);
            if (conta == null)
            {
                return HttpNotFound();
            }

            EstoqueViewModel qtdNotas = CalculaNotas(entrada.Valor);
            if (qtdNotas.Erro)
            {
                // neste caso não existe uma quantidade de notas suficiente para sacar
                TempData["Erro"] = "Notas insuficientes no caixa, escolha outro valor.";
                return RedirectToAction("Saque", new { numero = conta.Numero });
            }
            else
            {
                // desconta o saldo pelo saque efetuado
                conta.Saldo -= entrada.Valor;
                // salva as alterações no banco de dados
                db.SaveChanges();

                TempData["Sucesso"] = "Saque de R$ " + entrada.Valor + " efetuado com sucesso.";
                ViewBag.NumeroConta = conta.Numero;

                return View("ExibirTroco", qtdNotas);
            }

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

        public ActionResult Extrato()
        {
            return View();
        }

        private EstoqueViewModel CalculaNotas(double valor)
        {
            EstoqueViewModel troco = new EstoqueViewModel();
            Estoque repositorio = this.db.Estoque.First();

            // calcula o numero de notas de 100
            while (valor >= 100 && repositorio.QtdNotas100 > 0)
            {
                valor -= 100;
                troco.QtdNotas100++;
                repositorio.QtdNotas100--;
            }

            while (valor >= 50 && repositorio.QtdNotas50 > 0)
            {
                valor -= 50;
                troco.QtdNotas50++;
                repositorio.QtdNotas50--;
            }

            while (valor >= 20 && repositorio.QtdNotas20 > 0)
            {
                valor -= 20;
                troco.QtdNotas20++;
                repositorio.QtdNotas20--;
            }

            while (valor >= 10 && repositorio.QtdNotas10 > 0)
            {
                valor -= 10;
                troco.QtdNotas10++;
                repositorio.QtdNotas10--;
            }

            if (valor > 0)
            {
                troco.Erro = true;
                troco.DescricaoErro = "O banco não possui cedulas suficientes para completar a operação!";
            }

            return troco;
        }


    }
}