using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RMASystem;

namespace RMASystem.Views
{
	[Authorize(Roles = "Administrator")]
	public class RealizationsController : Controller
    {
        private RmaEntities db = new RmaEntities();

        // GET: Realizations
        public ActionResult Index()
        {
            return View(db.Realization.ToList());
        }

        // GET: Realizations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Realization realization = db.Realization.Find(id);
            if (realization == null)
            {
                return HttpNotFound();
            }
            return View(realization);
        }

        // GET: Realizations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Realizations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Realization realization)
        {
            if (ModelState.IsValid)
            {
                db.Realization.Add(realization);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(realization);
        }

        // GET: Realizations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Realization realization = db.Realization.Find(id);
            if (realization == null)
            {
                return HttpNotFound();
            }
            return View(realization);
        }

        // POST: Realizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Realization realization)
        {
            if (ModelState.IsValid)
            {
                db.Entry(realization).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(realization);
        }

        // GET: Realizations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Realization realization = db.Realization.Find(id);
            if (realization == null)
            {
                return HttpNotFound();
            }
            return View(realization);
        }

        // POST: Realizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Realization realization = db.Realization.Find(id);
            db.Realization.Remove(realization);
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
