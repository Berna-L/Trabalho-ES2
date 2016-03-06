using LoginBancoTeste.DAL;
using LoginBancoTeste.Models;
using LoginBancoTeste.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace LoginBancoTeste.Controllers {
    public class TransacoesController : Controller {
        private BancoContext db = new BancoContext();

        // HOME: Transacoes
        public ActionResult Index(int? id) {
            Cliente cliente;

            if (User.Identity.IsAuthenticated) {
                cliente = this.db.Clientes.Where(s => User.Identity.Name == s.Username).SingleOrDefault();
                return View(cliente);
            }

            // neste caso ainda não logamos como cliente
            if (id == null) {
                return View();
            }

            // neste caso logamos como cliente
            cliente = this.db.Clientes.Find(id);
            if (cliente == null) {
                return HttpNotFound();
            }
            return View(cliente);
        }

        [Authorize]
        public ActionResult Opcoes(int? numero) {
            if (numero == null) {
                return HttpNotFound();
            }
            Conta conta = this.db.Contas.Find(numero);
            if (conta == null) {
                return HttpNotFound();
            }
            return View(conta);
        }

        [Authorize]
        public ActionResult Saldo(int? numero) {
            if (numero == null) {
                return View();
            }
            Conta conta = this.db.Contas.Find(numero);
            if (conta == null) {
                return HttpNotFound();
            }
            return View(conta);
        }

        [Authorize]
        public ActionResult Saque() {
            return View();
        }

        [Authorize]
        public ActionResult Investimentos(int? idCliente, int? idConta) {
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
            Investimento debug = this.db.Investimentos.Find(1);
            IList<Investimento> listaInvest = this.db.Investimentos.Where(inv => inv.cliente.Id == cliente.Id).ToList();
            foreach (Investimento inv in listaInvest) {
                if (inv.data_canc == null) {
                    inv.valor_acc = TipoInvestimentoAux.CalcularRendimento(inv, DateTime.Today);
                }
            }
            this.db.SaveChanges();
            return View(listaInvest);
        }

        [HttpPost]
        public ActionResult Investimentos(Investimento invest, int? idConta) {
            if (invest == null || idConta == null) {
                HttpNotFound();
            }
            return RedirectToAction("InvestimentoDetalhe", new { id = invest.Id, idConta = idConta });
        }

        [Authorize]
        public ActionResult InvestimentoDetalhe(int? id, int? idConta) {
            if (id == null || idConta == null) {
                return HttpNotFound();
            }
            Investimento invest = this.db.Investimentos.Find(id);
            invest.cliente = this.db.Contas.Find(idConta).Cliente; //Gambiarra porque já tô aqui com asp.net
            ViewBag.idConta = idConta;
            ViewBag.JurosMes = ((int)((Math.Pow(invest.tipo_invest.jurosDia, 30) - 1) * 10000)) / 100.0f;
            ViewBag.JurosAno = ((int)((Math.Pow(invest.tipo_invest.jurosDia, 365) - 1) * 10000)) / 100.0f;
            return View(invest);
        }

        public ActionResult InvestimentoCancelar(int? id, int? idConta) {
            if (id == null || idConta == null){
                return HttpNotFound();
            }
            Conta conta = this.db.Contas.Find(idConta);
            Investimento invest = this.db.Investimentos.Find(id);
            invest.cliente = conta.Cliente;
            conta.Saldo += invest.valor_acc;
            invest.data_canc = DateTime.Today;
            this.db.SaveChanges();
            TempData["Sucesso"] = "Investimento de R$ " + invest.valor_acc + " cancelado e valor adicionado à conta!";
            return RedirectToAction("Opcoes", new { numero = idConta });
        }


        [Authorize]
        public ActionResult InvestimentoCriar(int? idCliente, int? idConta) {
            if (idCliente == null || idConta == null) {
                return HttpNotFound();
            }
            InvestimentoViewModel invest = new InvestimentoViewModel();
            invest.confirmado = false;
            invest.numCliente = idCliente;
            invest.data = DateTime.Today;
            invest.contaADebitar = idConta;
            ViewBag.tipos = new SelectList(this.db.TiposInvestimento.ToList(), "Id", "nome", this.db.TiposInvestimento.Find(1));
            return View(invest);
        }

        [HttpPost]
        public ActionResult InvestimentoCriar(InvestimentoViewModel invest) {
            if (ModelState.IsValid && invest.valor > 0) {
                Conta conta = this.db.Contas.Find(invest.contaADebitar);
                if (invest.valor > conta.Saldo) {
                    ViewBag.Error = "Valor excede o saldo da conta.";
                    ViewBag.tipos = new SelectList(this.db.TiposInvestimento.ToList(), "Id", "nome", this.db.TiposInvestimento.Find(1));
                    return View(invest);
                }
                return RedirectToAction("InvestimentoCriarConf", invest);
            }
 else {
                ViewBag.Error = "Erro desconhecido.";
            }
            ViewBag.tipos = new SelectList(this.db.TiposInvestimento.ToList(), "Id", "nome", this.db.TiposInvestimento.Find(1));
            return View(invest);
        }

        [Authorize]
        public ActionResult InvestimentoCriarConf(InvestimentoViewModel invest) {
            if (ModelState.IsValid) {
                if (invest.confirmado) {
                    return RedirectToAction("InvestimentoCriarFinal", invest);
                }
                invest.confirmado = true;
                TipoInvestimento t = this.db.TiposInvestimento.Find(invest.tipo);
                ViewBag.Tipo = t.nome;
                ViewBag.JurosMes = ((int)((Math.Pow(t.jurosDia, 30) - 1) * 10000)) / 100.0f;
                ViewBag.SimulacaoMes = Auxiliares.Util.ConversorReal(TipoInvestimentoAux.CalcularRendimento(invest.valor, t, DateTime.Today, DateTime.Today.AddMonths(1)));
                ViewBag.JurosAno = ((int)((Math.Pow(t.jurosDia, 365) - 1) * 10000)) / 100.0f;
                ViewBag.SimulacaoAno = Auxiliares.Util.ConversorReal(TipoInvestimentoAux.CalcularRendimento(invest.valor, t, DateTime.Today, DateTime.Today.AddYears(1)));
                return View(invest);
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult InvestimentoCriarFinal(InvestimentoViewModel invest) {
            if (ModelState.IsValid) {
                Investimento i = new Investimento();
                i.cliente = this.db.Clientes.Find(invest.numCliente);
                i.data = invest.data;
                i.data_canc = null;
                i.tipo_invest = this.db.TiposInvestimento.Find(invest.tipo);
                i.valor_ini = invest.valor;
                ViewBag.idConta = invest.contaADebitar;
                Conta conta = this.db.Contas.Find(invest.contaADebitar);
                conta.Saldo -= invest.valor;
                this.db.Investimentos.Add(i);
                this.db.SaveChanges();
                TempData["Sucesso"] = "Investimento de R$ " + i.valor_ini + " realizado com sucesso!";
                return RedirectToAction("Opcoes", new { numero = invest.contaADebitar });
            }
            return View(invest);
        }

        public ActionResult Deposito(int? numero) {
            if (numero == null) {
                return View();
            }
            DepositoViewModel deposito = new DepositoViewModel();
            deposito.NumeroConta = numero;

            return View(deposito);
        }

        [HttpPost]
        public ActionResult Deposito(DepositoViewModel deposito) {
            if (ModelState.IsValid) {
                Conta conta = this.db.Contas.Find(deposito.NumeroConta);
                conta.Saldo += deposito.Valor;
                this.db.SaveChanges();
                TempData["Sucesso"] = "Seu deposito de R$ " + deposito.Valor + " reais foi realizado com sucesso!";
                return RedirectToAction("Opcoes", new { numero = deposito.NumeroConta });
            }

            return View(deposito);
        }

        [Authorize]
        public ActionResult Transferencia(int? numero) {
            if (numero == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TransferenciaViewModel dados = new TransferenciaViewModel();
            dados.NumeroConta = numero;

            return View(dados);
        }

        [HttpPost]
        public ActionResult Transferencia(TransferenciaViewModel dados) {
            if (ModelState.IsValid) {
                Conta conta = this.db.Contas.Find(dados.NumeroConta);
                if (conta.Saldo < dados.Valor) {
                    ViewBag.Error = "Saldo insuficiente para transferencia de R$ " + dados.Valor + " reais!";
                    return View(dados);
                }

                Conta contaDestino = this.db.Contas.Find(dados.NumeroContaDestino);
                if (contaDestino == null) {
                    ViewBag.Error = "Conta destino inexistente!";
                    return View(dados);
                }

                dados.ContaDestino = contaDestino;
                return View("TransferenciaConfirmacao", dados);
            }
            return View(dados);
        }

        [HttpPost]
        public ActionResult TransferenciaConfirmacao(TransferenciaViewModel dados) {
            if (ModelState.IsValid) {
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