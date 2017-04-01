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
	[Authorize]
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
		[Authorize(Roles = "Klient, Administrator")]
        public ActionResult Create()
        {
            ViewBag.AppType_Id = new SelectList(db.AppType, "Id", "Name");
            ViewBag.Client_Id = new SelectList(db.User.Where(u => u.Role.Name == "Klient"), "Id", "Identificator");
            ViewBag.Employee_Id = new SelectList(db.User.Where(u => u.Role.Name == "Serwisant"), "Id", "Identificator");
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
				CreateCode(application);
				if(HttpContext.User.IsInRole("Klient")) SetClient(application);
	            SetStatus(application);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AppType_Id = new SelectList(db.AppType, "Id", "Name", application.AppType_Id);
            ViewBag.Client_Id = new SelectList(db.User, "Id", "Identificator", application.Client_Id);
            ViewBag.Employee_Id = new SelectList(db.User, "Id", "Identificator", application.Employee_Id);
            ViewBag.Product_Id = new SelectList(db.Product, "Id", "Name", application.Product_Id);
            ViewBag.Realization_Id = new SelectList(db.Realization, "Id", "Name", application.Realization_Id);
            ViewBag.Result_Id = new SelectList(db.Result, "Id", "Name", application.Result_Id);
            ViewBag.Statue_Id = new SelectList(db.Statue, "Id", "Name", application.Statue_Id);
            return View(application);
        }

	    void SetStatus(Application application) {
		    using (var context = new RmaEntities()) {
			    var statue = context.Statue.FirstOrDefault(s => s.Name == EStatue.Niepotwierdzony.ToString());
			    if (statue != null)
				    application.Statue_Id = statue.Id;
		    }
	    }

	    void SetClient(Application application) {
			using (var context = new RmaEntities()) {
				var userEmail = HttpContext.User.Identity.Name;
				var currentUser = context.User.FirstOrDefault(u => u.Email == userEmail);
				application.Client_Id = currentUser.Id;
			}
		}

	    void CreateCode(Application application) {
		    int newId;
		    using (var context = new RmaEntities()) {
				newId = context.Application.Max(u => u.Id);
		    }
		    application.Name = $"{newId+1}/{DateTime.Today.Month}/{DateTime.Today.Year}";
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
            ViewBag.Client_Id = new SelectList(db.User.Where(u => u.Role.Name == "Klient"), "Id", "Identificator", application.Client_Id);
            ViewBag.Employee_Id = new SelectList(db.User.Where(u => u.Role.Name == "Administrator"), "Id", "Identificator", application.Employee_Id);
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
            ViewBag.Client_Id = new SelectList(db.User.Where(u => u.Role.Name == "Klient"), "Id", "Identificator", application.Client_Id);
            ViewBag.Employee_Id = new SelectList(db.User.Where(u => u.Role.Name == "Administrator"), "Id", "Identificator", application.Employee_Id);
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

	    public ActionResult UserApplication() {
		    int userId;
		    using (var context = new RmaEntities()) {
			    var userEmail = HttpContext.User.Identity.Name;
			    var currentUser = context.User.FirstOrDefault(u => u.Email == userEmail);
			    userId = currentUser.Id;
		    }
		    var application = db.Application.Where(a => a.Client_Id == userId).Include(a => a.AppType).Include(a => a.User).Include(a => a.User1).Include(a => a.Product).Include(a => a.Realization).Include(a => a.Result).Include(a => a.Statue);
			return View("Index",application.ToList());
		}
    }
}
