using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LoginBancoTeste.DAL;
using LoginBancoTeste.Models;

namespace LoginBancoTeste.Controllers
{
    public class InvestimentosController : Controller
    {
        private BancoContext db = new BancoContext();

        // GET: Investimentos
        public ActionResult Index(int? id)
        {
            if (id == null) {
                return HttpNotFound();
            }
            return View(db.Investimentos.Where(i => i.cliente.Id == id).ToList());
        }

        // GET: Investimentos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Investimento investimento = db.Investimentos.Find(id);
            if (investimento == null)
            {
                return HttpNotFound();
            }
            return View(investimento);
        }

        // GET: Investimentos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Investimentos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,valor_ini,valor_acc,data,data_canc")] Investimento investimento)
        {
            if (ModelState.IsValid)
            {
                db.Investimentos.Add(investimento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(investimento);
        }

        // GET: Investimentos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Investimento investimento = db.Investimentos.Find(id);
            if (investimento == null)
            {
                return HttpNotFound();
            }
            return View(investimento);
        }

        // POST: Investimentos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,valor_ini,valor_acc,data,data_canc")] Investimento investimento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(investimento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(investimento);
        }

        // GET: Investimentos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Investimento investimento = db.Investimentos.Find(id);
            if (investimento == null)
            {
                return HttpNotFound();
            }
            return View(investimento);
        }

        // POST: Investimentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Investimento investimento = db.Investimentos.Find(id);
            db.Investimentos.Remove(investimento);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
