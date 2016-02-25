using LoginBancoTeste.DAL;
using LoginBancoTeste.Models;
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
        public ActionResult SaqueConfirm(int? numero, double valor)
        {
            String resp = "";
            
            
            if (numero == null)
            {
                return View();
            }
            Conta conta = this.db.Contas.Find(numero);
            if (conta == null)
            {
                return HttpNotFound();
            }
            if (conta.Saldo<valor)
            {
                resp = "Saldo insuficiente.";
            }
            else
            {
                resp = "Saque efetuado com sucesso.";
                conta.Saldo -= valor;
                db.SaveChanges();
                

            }


            ViewBag.Sucesso = resp;
            return View(conta);
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

    }
}