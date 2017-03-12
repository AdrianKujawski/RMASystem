using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RMASystem;

namespace RMASystem.Controllers
{
	[Authorize(Roles = "Administrator")]
    public class AppTypesController : Controller
    {
        private RmaEntities db = new RmaEntities();

        // GET: AppTypes
        public ActionResult Index()
        {
            return View(db.AppType.ToList());
        }

        // GET: AppTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppType appType = db.AppType.Find(id);
            if (appType == null)
            {
                return HttpNotFound();
            }
            return View(appType);
        }

        // GET: AppTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AppTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] AppType appType)
        {
            if (ModelState.IsValid)
            {
                db.AppType.Add(appType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appType);
        }

        // GET: AppTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppType appType = db.AppType.Find(id);
            if (appType == null)
            {
                return HttpNotFound();
            }
            return View(appType);
        }

        // POST: AppTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] AppType appType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appType);
        }

        // GET: AppTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppType appType = db.AppType.Find(id);
            if (appType == null)
            {
                return HttpNotFound();
            }
            return View(appType);
        }

        // POST: AppTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppType appType = db.AppType.Find(id);
            db.AppType.Remove(appType);
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
