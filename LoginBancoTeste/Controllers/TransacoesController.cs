using LoginBancoTeste.DAL;
using LoginBancoTeste.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult Transferencia()
        {
            return View();
        }

    }
}