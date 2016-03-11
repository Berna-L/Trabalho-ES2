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

            EntradaSaqueViewModel entrada = new EntradaSaqueViewModel();
            entrada.NumeroConta = numero;
            entrada.Valor = valor*10;

            return View(entrada);
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

            if (conta.Saldo < entrada.Valor)
            {
                resposta = "Saldo insuficiente.";
            }
            else
            {
                EstoqueViewModel qtdNotas = calcularNotas(entrada.Valor);
                if (!qtdNotas.Sucesso)
                {
                    // neste caso não existe uma quantidade de notas suficiente para sacar
                    resposta = "Notas insuficientes no caixa, escolha outro valor.";
                }
                else
                {
                    // desconta o saldo pelo saque efetuado
                    conta.Saldo -= entrada.Valor;
                    // salva as alterações no banco de dados
                    db.SaveChanges();

                    TempData["Sucesso"] = "Saque de R$ " + entrada.Valor + " efetuado com sucesso.";

                    return RedirectToAction("Opcoes", "Transacoes", new { numero = entrada.NumeroConta });
                }
            }

            ViewBag.Sucesso = resposta;
            return View(entrada);
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

        private EstoqueViewModel calcularNotas(double valor)
        {
            double valorTemp = (valor % 100);
            Estoque est = this.db.Estoque.First();
            valor = valor - valorTemp;
            EstoqueViewModel respEst = new EstoqueViewModel();
            int temp = (int)valor / 100;
            //Calculo das notas de 100
            if (temp <= est.QtdNotas100)
            {
                respEst.QtdNotas100 = temp;
                est.QtdNotas100 -= temp;
            }
            else
            {
                respEst.QtdNotas100 = est.QtdNotas100;
                est.QtdNotas100 = 0;
                valorTemp += valor - (respEst.QtdNotas100 * 100);
            }
            //Calculo das notas de 50
            valorTemp = valor % 50;
            valor = valor - valorTemp;

            temp = (int)valor / 50;

            if (temp <= est.QtdNotas50)
            {
                respEst.QtdNotas50 = temp;
                est.QtdNotas50 -= temp;
            }
            else
            {
                respEst.QtdNotas50 = est.QtdNotas50;
                est.QtdNotas50 = 0;
                valorTemp += valor - (respEst.QtdNotas50 * 50);
            }
            //Calculo das notas de 20
            valorTemp = valor % 20;
            valor = valor - valorTemp;

            temp = (int)valor / 20;

            if (temp <= est.QtdNotas20)
            {
                respEst.QtdNotas20 = temp;
                est.QtdNotas20 -= temp;
            }
            else
            {
                respEst.QtdNotas20 = est.QtdNotas20;
                est.QtdNotas20 = 0;
                valorTemp += valor - (respEst.QtdNotas20 * 20);
            }
            //Calculo das notas de 10
            valorTemp = valor % 10;
            valor = valor - valorTemp;

            temp = (int)valor / 10;

            if (temp <= est.QtdNotas10)
            {
                respEst.QtdNotas10 = temp;
                est.QtdNotas10 -= temp;
                respEst.Sucesso = true;
            }
            else
            {
                respEst.Sucesso = false;
            }
            return respEst;
        }

    }
}