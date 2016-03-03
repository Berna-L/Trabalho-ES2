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
    public class AdminController : Controller
    {
        private BancoContext db = new BancoContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Estatisticas()
        {
            return View(); 
        }

        public JsonResult GetEstatisticas()
        {
            EstatisticasViewModel estatisticas = new EstatisticasViewModel();
            estatisticas.ContasPoupanca = this.db.Contas.Where(s => s.TipoDeConta == TipoDeConta.Poupanca).Count();
            estatisticas.ContasCorrente = this.db.Contas.Where(s => s.TipoDeConta == TipoDeConta.Corrente).Count();

            return Json(estatisticas, JsonRequestBehavior.AllowGet);
        } 
    }
}