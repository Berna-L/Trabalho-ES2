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
    public class EstoquesController : Controller
    {
        private BancoContext db = new BancoContext();

        // GET: Estoques
        public ActionResult Index()
        {
            return View(db.Estoque.ToList());
        }

        // GET: Estoques/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estoque estoque = db.Estoque.Find(id);
            if (estoque == null)
            {
                return HttpNotFound();
            }
            return View(estoque);
        }

        // GET: Estoques/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Estoques/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EstoqueID,QtdNotas10,QtdNotas20,QtdNotas50,QtdNotas100,QtdCheques")] Estoque estoque)
        {
            if (ModelState.IsValid)
            {
                db.Estoque.Add(estoque);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(estoque);
        }

        // GET: Estoques/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estoque estoque = db.Estoque.Find(id);
            if (estoque == null)
            {
                return HttpNotFound();
            }
            return View(estoque);
        }

        // POST: Estoques/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EstoqueID,QtdNotas10,QtdNotas20,QtdNotas50,QtdNotas100,QtdCheques")] Estoque estoque)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estoque).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estoque);
        }

        // GET: Estoques/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estoque estoque = db.Estoque.Find(id);
            if (estoque == null)
            {
                return HttpNotFound();
            }
            return View(estoque);
        }

        // POST: Estoques/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Estoque estoque = db.Estoque.Find(id);
            db.Estoque.Remove(estoque);
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
