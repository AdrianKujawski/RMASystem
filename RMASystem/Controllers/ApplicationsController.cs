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
	public class ApplicationsController : Controller
    {
        private RmaEntities db = new RmaEntities();

        // GET: Applications
        public ActionResult Index()
        {
            var application = db.Application.Include(a => a.AppType).Include(a => a.User).Include(a => a.User1).Include(a => a.Product).Include(a => a.Realization).Include(a => a.Result).Include(a => a.Statue);
            return View(application.ToList());
        }

        // GET: Applications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Application.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        // GET: Applications/Create
        public ActionResult Create()
        {
            ViewBag.AppType_Id = new SelectList(db.AppType, "Id", "Name");
            ViewBag.Client_Id = new SelectList(db.User, "Id", "FirstName");
            ViewBag.Employee_Id = new SelectList(db.User, "Id", "FirstName");
            ViewBag.Product_Id = new SelectList(db.Product, "Id", "Name");
            ViewBag.Realization_Id = new SelectList(db.Realization, "Id", "Name");
            ViewBag.Result_Id = new SelectList(db.Result, "Id", "Name");
            ViewBag.Statue_Id = new SelectList(db.Statue, "Id", "Name");
            return View();
        }

        // POST: Applications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,InvoiceNo,Purschace,Content,Description,Expectations,Cost,Start,Pending,InProgress,End,Product_Id,AppType_Id,Realization_Id,Statue_Id,Result_Id,Client_Id,Employee_Id")] Application application)
        {
            if (ModelState.IsValid)
            {
                db.Application.Add(application);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AppType_Id = new SelectList(db.AppType, "Id", "Name", application.AppType_Id);
            ViewBag.Client_Id = new SelectList(db.User, "Id", "FirstName", application.Client_Id);
            ViewBag.Employee_Id = new SelectList(db.User, "Id", "FirstName", application.Employee_Id);
            ViewBag.Product_Id = new SelectList(db.Product, "Id", "Name", application.Product_Id);
            ViewBag.Realization_Id = new SelectList(db.Realization, "Id", "Name", application.Realization_Id);
            ViewBag.Result_Id = new SelectList(db.Result, "Id", "Name", application.Result_Id);
            ViewBag.Statue_Id = new SelectList(db.Statue, "Id", "Name", application.Statue_Id);
            return View(application);
        }

        // GET: Applications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Application.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            ViewBag.AppType_Id = new SelectList(db.AppType, "Id", "Name", application.AppType_Id);
            ViewBag.Client_Id = new SelectList(db.User, "Id", "FirstName", application.Client_Id);
            ViewBag.Employee_Id = new SelectList(db.User, "Id", "FirstName", application.Employee_Id);
            ViewBag.Product_Id = new SelectList(db.Product, "Id", "Name", application.Product_Id);
            ViewBag.Realization_Id = new SelectList(db.Realization, "Id", "Name", application.Realization_Id);
            ViewBag.Result_Id = new SelectList(db.Result, "Id", "Name", application.Result_Id);
            ViewBag.Statue_Id = new SelectList(db.Statue, "Id", "Name", application.Statue_Id);
            return View(application);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,InvoiceNo,Purschace,Content,Description,Expectations,Cost,Start,Pending,InProgress,End,Product_Id,AppType_Id,Realization_Id,Statue_Id,Result_Id,Client_Id,Employee_Id")] Application application)
        {
            if (ModelState.IsValid)
            {
                db.Entry(application).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AppType_Id = new SelectList(db.AppType, "Id", "Name", application.AppType_Id);
            ViewBag.Client_Id = new SelectList(db.User, "Id", "FirstName", application.Client_Id);
            ViewBag.Employee_Id = new SelectList(db.User, "Id", "FirstName", application.Employee_Id);
            ViewBag.Product_Id = new SelectList(db.Product, "Id", "Name", application.Product_Id);
            ViewBag.Realization_Id = new SelectList(db.Realization, "Id", "Name", application.Realization_Id);
            ViewBag.Result_Id = new SelectList(db.Result, "Id", "Name", application.Result_Id);
            ViewBag.Statue_Id = new SelectList(db.Statue, "Id", "Name", application.Statue_Id);
            return View(application);
        }

        // GET: Applications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application application = db.Application.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Application application = db.Application.Find(id);
            db.Application.Remove(application);
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
