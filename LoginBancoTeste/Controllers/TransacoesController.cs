using LoginBancoTeste.DAL;
using LoginBancoTeste.Models;
using LoginBancoTeste.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LoginBancoTeste.Models.ViewModels;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

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
        [HttpGet]
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

        public ActionResult EmissaoCheque(int numero)
        {            
            EmChequeViewModel ecvm = new EmChequeViewModel();
            ecvm.numConta = numero;
            return View(ecvm);
        }

        [HttpPost]
        public ActionResult ImpCheques(EmChequeViewModel ecvm, int numConta)
        {
            if (ecvm.qtdCheque < 4 || ecvm.qtdCheque > 20)
            {
                return RedirectToAction("EmissaoCheque", new {numero = ecvm.qtdCheque});
            }
            else {
                Conta conta = this.db.Contas.Find(numConta);
                if (conta == null)
                {
                    return HttpNotFound();
                }
                long numCheque;

                for (int i = 0; i < ecvm.qtdCheque; i++)
                {
                    Cheque cheque = new Cheque();
                    String detalhes;

                    if (ModelState.IsValid)
                    {
                        db.Cheques.Add(cheque);
                        db.SaveChanges();
                    }

                    PdfDocument pdf = new PdfDocument();
                    PdfPage pdfPage = pdf.AddPage();
                    XGraphics graph = XGraphics.FromPdfPage(pdfPage);
                    XFont font = new XFont("Verdana", 14, XFontStyle.Bold);

                    numCheque = cheque.numCheque;
                    detalhes = "Agência "+ conta.agencia.numAgencia + "| Banco " + conta.agencia.banco.numBanco + "| Conta " + conta.Numero + "| Número do Cheque " + numCheque;
                    graph.DrawString(detalhes, font, XBrushes.Black, new XRect(0, 0, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                    detalhes = "Pague por este cheque a quantia de";
                    graph.DrawString(detalhes, font, XBrushes.Black, new XRect(0, 50, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                    detalhes = "____________________________________________________________";
                    graph.DrawString(detalhes, font, XBrushes.Black, new XRect(0, 55, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                    detalhes = "e centavos acima";
                    graph.DrawString(detalhes, font, XBrushes.Black, new XRect(pdfPage.Width.Point - 150, 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                    detalhes = "____________________________________________________________";
                    graph.DrawString(detalhes, font, XBrushes.Black, new XRect(0, 110, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                    detalhes = "ou à sua ordem";
                    graph.DrawString(detalhes, font, XBrushes.Black, new XRect(pdfPage.Width.Point - 150, 160, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                    detalhes = "____________________________________________________________";
                    graph.DrawString(detalhes, font, XBrushes.Black, new XRect(0, 165, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                    detalhes = ",       de                        de 20";
                    graph.DrawString(detalhes, font, XBrushes.Black, new XRect(pdfPage.Width.Point - 250, 215, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                    detalhes = "____________________________________";
                    graph.DrawString(detalhes, font, XBrushes.Black, new XRect(pdfPage.Width.Point - 350, 220, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                    detalhes = "____________________________________";
                    graph.DrawString(detalhes, font, XBrushes.Gray, new XRect(pdfPage.Width.Point - 350, 270, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

                    String saida = ecvm.dirSaida + "cheque" + numCheque + ".pdf";

                    try
                    {
                        pdf.Save(saida);
                        ecvm.msgControle = "Arquivo gerado";
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        ecvm.msgControle = "Impossível gerar arquivo no diretório solicitado";
                        return View(ecvm);
                    }
                    catch (System.IO.IOException io)
                    {
                        ecvm.msgControle = "Erro na escrita dos dados";
                        return View(ecvm);
                    }
                }

                return View(ecvm);
            }
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

                // faz o reigistro no extrato
                Extrato linhaExtrato = new Extrato();
                linhaExtrato.Data = DateTime.Now;
                linhaExtrato.Valor = entrada.Valor;
                linhaExtrato.SaldoAtual = conta.Saldo;
                linhaExtrato.Lancamento = "Saque";

                // associa a nova instancia no extrato
                conta.Extrato.Add(linhaExtrato);

                // salva as alterações no banco de dados
                this.db.SaveChanges();

                TempData["Sucesso"] = "Saque de R$ " + entrada.Valor + " efetuado com sucesso.";
                ViewBag.NumeroConta = conta.Numero;

                return View("ExibirTroco", qtdNotas);
            }
        }

        [Authorize]
        public ActionResult Investimentos(int? idCliente, int? idConta)
        {
            if (idCliente == null || idConta == null)
            {
                return HttpNotFound("deu id nulo mermão");
            }
            Cliente cliente = this.db.Clientes.Find(idCliente);
            Conta conta = this.db.Contas.Find(idConta);
            if (cliente == null || conta == null)
            {
                return HttpNotFound("deu objeto nulo rapá");
            }
            ViewBag.idCliente = idCliente;
            ViewBag.idConta = idConta;
            Investimento debug = this.db.Investimentos.Find(1);
            IList<Investimento> listaInvest = this.db.Investimentos.Where(inv => inv.cliente.Id == cliente.Id).ToList();
            foreach (Investimento inv in listaInvest)
            {
                if (inv.data_canc == null)
                {
                    inv.valor_acc = TipoInvestimentoAux.CalcularRendimento(inv, DateTime.Today);
                }
            }
            this.db.SaveChanges();

            ViewBag.NumeroConta = idConta;

            return View(listaInvest);
        }

        [HttpPost]
        public ActionResult Investimentos(Investimento invest, int? idConta)
        {
            if (invest == null || idConta == null)
            {
                HttpNotFound();
            }
            return RedirectToAction("InvestimentoDetalhe", new { id = invest.Id, idConta = idConta });
        }

        [Authorize]
        public ActionResult InvestimentoDetalhe(int? id, int? idConta)
        {
            if (id == null || idConta == null)
            {
                return HttpNotFound();
            }
            Investimento invest = this.db.Investimentos.Find(id);
            invest.cliente = this.db.Contas.Find(idConta).Cliente; //Gambiarra porque já tô aqui com asp.net
            ViewBag.idConta = idConta;
            ViewBag.JurosMes = ((int)((Math.Pow(invest.tipo_invest.jurosDia, 30) - 1) * 10000)) / 100.0f;
            ViewBag.JurosAno = ((int)((Math.Pow(invest.tipo_invest.jurosDia, 365) - 1) * 10000)) / 100.0f;
            return View(invest);
        }

        public ActionResult InvestimentoCancelar(int? id, int? idConta)
        {
            if (id == null || idConta == null)
            {
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
        public ActionResult InvestimentoCriar(int? idCliente, int? idConta)
        {
            if (idCliente == null || idConta == null)
            {
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
        public ActionResult InvestimentoCriar(InvestimentoViewModel invest)
        {
            if (ModelState.IsValid && invest.valor > 0)
            {
                Conta conta = this.db.Contas.Find(invest.contaADebitar);
                if (invest.valor > conta.Saldo)
                {
                    ViewBag.Error = "Valor excede o saldo da conta.";
                    ViewBag.tipos = new SelectList(this.db.TiposInvestimento.ToList(), "Id", "nome", this.db.TiposInvestimento.Find(1));
                    return View(invest);
                }
                return RedirectToAction("InvestimentoCriarConf", invest);
            }
            else
            {
                ViewBag.Error = "Erro desconhecido.";
            }
            ViewBag.tipos = new SelectList(this.db.TiposInvestimento.ToList(), "Id", "nome", this.db.TiposInvestimento.Find(1));
            return View(invest);
        }

        public ActionResult InvestimentoCriarConf(InvestimentoViewModel invest, int? idConta)
        {
            if (ModelState.IsValid)
            {
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
        public ActionResult InvestimentoCriarConf(InvestimentoViewModel invest)
        {
            if (ModelState.IsValid)
            {
                Investimento i = new Investimento();
                i.cliente = this.db.Clientes.Find(invest.numCliente);
                i.data = DateTime.Today;
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
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); ;
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