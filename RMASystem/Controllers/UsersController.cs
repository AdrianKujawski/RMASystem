using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RMASystem;
using RMASystem.Helpers;

namespace RMASystem.Controllers
{
	[Authorize(Roles = "Administrator")]
	public class UsersController : Controller
    {
        private RmaEntities db = new RmaEntities();

        // GET: Users
        public ActionResult Index()
        {
            var user = db.User.Include(u => u.Adress).Include(u => u.BankAccount).Include(u => u.Role);
            return View(user.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.Adress_Id = new SelectList(db.Adress, "Id", "City");
            ViewBag.BankAccount_Id = new SelectList(db.BankAccount, "Id", "Name");
            ViewBag.Role_Id = new SelectList(db.Role, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Phone,Email,Password,Role_Id,Adress_Id,BankAccount_Id")] User user)
        {
            if (ModelState.IsValid) {
				RMASystem.User.SetFirstLetterOfNameToUpper(user);
				db.User.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Adress_Id = new SelectList(db.Adress, "Id", "City", user.Adress_Id);
            ViewBag.BankAccount_Id = new SelectList(db.BankAccount, "Id", "Name", user.BankAccount_Id);
            ViewBag.Role_Id = new SelectList(db.Role, "Id", "Name", user.Role_Id);
            return View(user);
        }

	    

	    // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.Adress_Id = new SelectList(db.Adress, "Id", "City", user.Adress_Id);
            ViewBag.BankAccount_Id = new SelectList(db.BankAccount, "Id", "Name", user.BankAccount_Id);
            ViewBag.Role_Id = new SelectList(db.Role, "Id", "Name", user.Role_Id);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Phone,Email,Password,Role_Id,Adress_Id,BankAccount_Id")] User user)
        {
            if (ModelState.IsValid)
            {
				RMASystem.User.SetFirstLetterOfNameToUpper(user);
	            db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Adress_Id = new SelectList(db.Adress, "Id", "City", user.Adress_Id);
            ViewBag.BankAccount_Id = new SelectList(db.BankAccount, "Id", "Name", user.BankAccount_Id);
            ViewBag.Role_Id = new SelectList(db.Role, "Id", "Name", user.Role_Id);
            return View(user);
        }

	    

	    // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.User.Find(id);
            db.User.Remove(user);
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
