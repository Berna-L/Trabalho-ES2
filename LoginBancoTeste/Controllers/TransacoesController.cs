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

        public ActionResult EmissaoCheque()
        {
            EmChequeViewModel ecvm = new EmChequeViewModel();
            return View(ecvm);
        }

        [HttpPost]
        public ActionResult ImpCheques(EmChequeViewModel ecvm)
        {
            if (ecvm.qtdCheque < 4 || ecvm.qtdCheque > 20)
            {
                return RedirectToAction("EmissaoCheque");
            }
            else {
                for (int i = 0; i < ecvm.qtdCheque; i++)
                {
                    Cheque cheque = new Cheque();
                    //Criar detalhes dos cheques aqui

                    PdfDocument pdf = new PdfDocument();
                    PdfPage pdfPage = pdf.AddPage();
                    XGraphics graph = XGraphics.FromPdfPage(pdfPage);
                    XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                    graph.DrawString("Exemplo de cheque", font, XBrushes.Black, new XRect(0, 0, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
                    String saida = ecvm.dirSaida + "cheque" + (i + 1) + ".pdf";

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