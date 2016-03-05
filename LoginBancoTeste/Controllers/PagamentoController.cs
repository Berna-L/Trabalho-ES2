using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoginBancoTeste.Controllers
{
    public class PagamentoController : Controller
    {
        // GET: Pagamento
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PgmtoConfirma()
        {
            return View();
        }

        public ActionResult PgmtoVencido()
        {
            return View();
        }

        public ActionResult PgmtoVerifica()
        {
            return View();
        }

        public ActionResult PgmtoVlrVenc()
        {
            return View();
        }
    }
}