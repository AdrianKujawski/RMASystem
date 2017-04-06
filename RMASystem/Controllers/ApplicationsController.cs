// -----------------------------------------------------------------------
// <copyright file="ApplicationsController.cs">
//     Copyright (c) 2017, Adrian Kujawski.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using RMASystem.Helpers;

namespace RMASystem.Controllers {

	[Authorize]
	public class ApplicationsController : Controller {
		RmaEntities db = new RmaEntities();

		// GET: Applications
		[Authorize(Roles = "Administrator,Serwisant")]
		public ActionResult Index() {
			var application =
				db.Application.Include(a => a.AppType).Include(a => a.User).Include(a => a.User1).Include(a => a.Product).Include(a => a.Realization).Include(a => a.Result).Include(a => a.Statue);
			return View(application.ToList());
		}

		// GET: Applications/Details/5
		[Authorize(Roles = "Klient, Administrator,Serwisant")]
		public ActionResult Details(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var application = db.Application.Find(id);
			if (application == null) {
				return HttpNotFound();
			}

			return View(application);
		}

		// GET: Applications/Create
		[Authorize(Roles = "Klient, Administrator,Serwisant")]
		public ActionResult Create() {
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
		[Authorize(Roles = "Klient, Administrator,Serwisant")]
		public ActionResult Create(
			[Bind(Include = "Id,Name,InvoiceNo,Purschace,Content,Description,Expectations,Cost,Start,Pending,InProgress,End,Product_Id,AppType_Id,Realization_Id,Statue_Id,Result_Id,Client_Id,Employee_Id")] Application application) {
			if (ModelState.IsValid) {
				db.Application.Add(application);
				CreateCode(application);
				if (HttpContext.User.IsInRole("Klient")) SetClient(application);
				SetStatus(application);
				db.SaveChanges();
				return HttpContext.User.IsInRole("Klient")
							? RedirectToAction("UserApplication")
							: RedirectToAction("Index");
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
			var userEmail = HttpContext.User.Identity.Name;
			var currentUser = RMASystem.User.GetLogged(userEmail);
			application.Client_Id = currentUser.Id;
		}

		void CreateCode(Application application) {
			int newId;
			using (var context = new RmaEntities()) {
				newId = context.Application.Max(u => u.Id);
			}
			application.Name = $"{newId + 1}/{DateTime.Today.Month}/{DateTime.Today.Year}";
		}

		// GET: Applications/Edit/5
		[Authorize(Roles = "Administrator,Serwisant")]
		public ActionResult Edit(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var application = db.Application.Find(id);
			if (application == null) {
				return HttpNotFound();
			}

			ViewBag.Old = application.Statue.Name;
			ViewBag.AppType_Id = new SelectList(db.AppType, "Id", "Name", application.AppType_Id);
			ViewBag.Client_Id = new SelectList(db.User.Where(u => u.Role.Name == "Klient"), "Id", "Identificator", application.Client_Id);
			ViewBag.Employee_Id = new SelectList(db.User.Where(u => u.Role.Name == "Serwisant"), "Id", "Identificator", application.Employee_Id);
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
		[Authorize(Roles = "Administrator,Serwisant")]
		public async System.Threading.Tasks.Task<ActionResult> Edit(
			[Bind(Include = "Id,Name,InvoiceNo,Purschace,Content,Description,Expectations,Cost,Start,Pending,InProgress,End,Product_Id,AppType_Id,Realization_Id,Statue_Id,Result_Id,Client_Id,Employee_Id")] Application application, string oldStatue) {
			if (ModelState.IsValid) {
				db.Entry(application).State = EntityState.Modified;
				if (IsStatueChanged(application, oldStatue)) {
					var emailHelper = new EmailHelper();
					await emailHelper.Send(PrepareChangeStatusMessage(application));
				}
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.AppType_Id = new SelectList(db.AppType, "Id", "Name", application.AppType_Id);
			ViewBag.Client_Id = new SelectList(db.User.Where(u => u.Role.Name == "Klient"), "Id", "Identificator", application.Client_Id);
			ViewBag.Employee_Id = new SelectList(db.User.Where(u => u.Role.Name == "Serwisant"), "Id", "Identificator", application.Employee_Id);
			ViewBag.Product_Id = new SelectList(db.Product, "Id", "Name", application.Product_Id);
			ViewBag.Realization_Id = new SelectList(db.Realization, "Id", "Name", application.Realization_Id);
			ViewBag.Result_Id = new SelectList(db.Result, "Id", "Name", application.Result_Id);
			ViewBag.Statue_Id = new SelectList(db.Statue, "Id", "Name", application.Statue_Id);
			return View(application);
		}

		MailMessage PrepareChangeStatusMessage(Application application) {
			var message = new MailMessage();
			var user = db.User.Find(application.Client_Id);
			if (user != null) message.To.Add(new MailAddress(user.Email));
			message.From = new MailAddress(Settings.EmailAddress);
			message.Subject = $"Zmiana statusu zgłoszenia {application.Name}";

			var newState = db.Statue.Find(application.Statue_Id)?.Name;
			var employee = db.User.Find(application.Employee_Id)?.Identificator;
			message.Body =
				$"Status twojego zgłoszenia został zmieniony na {newState}.{Environment.NewLine}Twoim zgłoszeniem zajmuje się {employee}.{Environment.NewLine}{Environment.NewLine}Pozdrawiamy!";

			return message;
		}

		bool IsStatueChanged(Application application, string oldStatue) {
			var statue = db.Statue.Find(application.Statue_Id);
			return statue != null && statue.Name != oldStatue;
		}

		// GET: Applications/Delete/5
		[Authorize(Roles = "Administrator,Serwisant")]
		public ActionResult Delete(int? id) {
			if (id == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var application = db.Application.Find(id);
			if (application == null) {
				return HttpNotFound();
			}

			return View(application);
		}

		// POST: Applications/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrator,Serwisant")]
		public ActionResult DeleteConfirmed(int id) {
			var application = db.Application.Find(id);
			db.Application.Remove(application);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult UserApplication() {
			var userEmail = HttpContext.User.Identity.Name;
			var currentUser = RMASystem.User.GetLogged(userEmail);
			var application =
				db.Application.Where(a => a.Client_Id == currentUser.Id)
				.Include(a => a.AppType)
				.Include(a => a.User)
				.Include(a => a.User1)
				.Include(a => a.Product)
				.Include(a => a.Realization)
				.Include(a => a.Result)
				.Include(a => a.Statue);
			return View("Index", application.ToList());
		}
	}

}
