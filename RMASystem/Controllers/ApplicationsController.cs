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
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using RMASystem.Helpers;
using RMASystem.Models.ViewModel;

namespace RMASystem.Controllers {

	[Authorize]
	public class ApplicationsController : Controller {
		RmaEntities db = new RmaEntities();

		// GET: Applications
		[Authorize(Roles = "Administrator,Serwisant")]
		public ActionResult Index() {
			var application =
				db.Application.Include(a => a.AppType).Include(a => a.User).Include(a => a.User1).Include(a => a.Product).Include(a => a.Realization).Include(a => a.Result).Include(a => a.Statue);
			return View(application.OrderByDescending(a => a.Start));
		}

		[HttpPost]
		[Authorize(Roles = "Administrator,Serwisant, Klient")]
		public ActionResult Index(string status) {
			if (HttpContext.User.IsInRole("Klient")) {
				return RedirectToAction("UserApplication", "Applications",new {state = status});
			}
			var application =
					db.Application.Include(a => a.AppType).Include(a => a.User).Include(a => a.User1).Include(a => a.Product).Include(a => a.Realization).Include(a => a.Result).Include(a => a.Statue).ToList();

			switch (status) {
				case "0":
					return View(application.Where(a => a.Statue.EName == EStatue.NotConfirmed));
				case "1":
					return View(application.Where(a => a.Statue.EName == EStatue.Pending));
				case "2":
					return View(application.Where(a => a.Statue.EName == EStatue.InProgrss));
				case "3":
					return View(application.Where(a => a.Statue.EName == EStatue.Sended));
			}
			return View(application.OrderByDescending(a => a.Start));
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
		public async Task<ActionResult> Create(
			[Bind(Include = "Id,Name,InvoiceNo,Purschace,Content,Description,Expectations,Cost,Start,Pending,InProgress,End,Product_Id,AppType_Id,Realization_Id,Statue_Id,Result_Id,Client_Id,Employee_Id")] Application application) {
			if (ModelState.IsValid) {
				db.Application.Add(application);
				CreateCode(application);
				if (HttpContext.User.IsInRole("Klient")) SetClient(application);
				SetStatus(application);
				SetStatusDate(application);
				db.SaveChanges();
				
				var mailMessage = PrepareNewMessage(application);
				var mailEntity = new Email {
					Sender = mailMessage.From.ToString(),
					Reciper = mailMessage.To.ToString(),
					Subject = mailMessage.Subject,
					Content = mailMessage.Body,
					Application_Id = application.Id,
					PostDate = DateTime.Now
				};
				await SendMessage(mailMessage);
				SaveMessage(mailEntity);

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
				var statusName = Statue.StatusDictionary[EStatue.NotConfirmed];
				var statue = context.Statue.FirstOrDefault(s => s.Name == statusName);
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
		public async Task<ActionResult> Edit(
			[Bind(Include = "Id,Name,InvoiceNo,Purschace,Content,Description,Expectations,Cost,Start,Pending,InProgress,End,Product_Id,AppType_Id,Realization_Id,Statue_Id,Result_Id,Client_Id,Employee_Id")] Application application, string oldStatue) {

			if (IsPreviousStatus(application, oldStatue)) {
				ModelState.AddModelError(string.Empty, "Nie możesz zmienić statusu na poprzedni.");
			}

			if (application.Employee_Id == null) {
				ModelState.AddModelError(string.Empty, "Nie wybrano serwisanta.");
			}

			if (ModelState.IsValid) {
				db.Entry(application).State = EntityState.Modified;
				if (IsStatueChanged(application, oldStatue)) {

					SetStatusDate(application);
					var mailMessage = PrepareChangeStatusMessage(application);
					var mailEntity = new Email {
						Sender = mailMessage.From.ToString(),
						Reciper = mailMessage.To.ToString(),
						Subject = mailMessage.Subject,
						Content = mailMessage.Body,
						Application_Id = application.Id,
						PostDate = DateTime.Now
					};
					await SendMessage(mailMessage);
					SaveMessage(mailEntity);
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

		static void SaveMessage(Email mailEntity) {
			var emailProvider = new EmailProvider(mailEntity);
			emailProvider.SaveToDb();
		}

		static async Task SendMessage(MailMessage mailMessage) {
			var emailHelper = new EmailHelper();
			await emailHelper.Send(mailMessage);
		}

		MailMessage PrepareNewMessage(Application application) {
			var message = PrepareMessage(application);
			message.Subject = $"Przyjęcie zgłoszenia {application.Name}";
			message.Body = $"Dziękujemy za złożenie zgłoszenia!{Environment.NewLine}Już niedługo jeden z naszych pracowników zajmie się twoim zgłoszeniem.{Environment.NewLine}{Environment.NewLine}Pozdrawiamy!";
			return message;
		}

		MailMessage PrepareChangeStatusMessage(Application application) {
			var message = PrepareMessage(application);
			message.Subject = $"Zmiana statusu zgłoszenia {application.Name}";
			var newState = db.Statue.Find(application.Statue_Id)?.Name;
			var employee = db.User.Find(application.Employee_Id)?.Identificator;
			message.Body =
				$"Status twojego zgłoszenia został zmieniony na {newState}.{Environment.NewLine}Twoim zgłoszeniem zajmuje się {employee}.{Environment.NewLine}{Environment.NewLine}Pozdrawiamy!";

			return message;
		}

		MailMessage PrepareMessage(Application application) {
			var message = new MailMessage();
			var user = db.User.Find(application.Client_Id);
			if (user != null) message.To.Add(new MailAddress(user.Email));
			message.From = new MailAddress(Settings.EmailAddress);
			return message;
		}

		bool IsStatueChanged(Application application, string oldStatue) {
			var statue = db.Statue.Find(application.Statue_Id);
			return statue != null && statue.Name != oldStatue;
		}

		bool IsPreviousStatus(Application application, string oldStatue) {
			var status = db.Statue.Find(application.Statue_Id);
			var oldStatus = Statue.StatusDictionary.FirstOrDefault(s => s.Value == oldStatue).Key;
			var newStatus = Statue.StatusDictionary.FirstOrDefault(s => s.Value == status.Name).Key;
			return newStatus < oldStatus;

		}

		void SetStatusDate(Application application) {
			var statusName = db.Statue.Find(application.Statue_Id).Name;
			var status = Statue.StatusDictionary.FirstOrDefault(s => s.Value == statusName).Key;
			switch (status) {
				case EStatue.NotConfirmed: application.Start = DateTime.Now;
					break;
				case EStatue.Pending: application.Pending = DateTime.Now;
					break;
				case EStatue.InProgrss: application.InProgress = DateTime.Now;
					break;
				case EStatue.Sended: application.End = DateTime.Now;
					break;
			}

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
			var application = db.Application.Include(a => a.Email).FirstOrDefault(a => a.Id == id);
			db.Email.RemoveRange(application.Email);
			db.Application.Remove(application);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult UserApplication(string state) {
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
				.Include(a => a.Statue).ToList();

			switch (state) {
				case "0":
					return View("Index", application.Where(a => a.Statue.EName == EStatue.NotConfirmed));
				case "1":
					return View("Index", application.Where(a => a.Statue.EName == EStatue.Pending));
				case "2":
					return View("Index", application.Where(a => a.Statue.EName == EStatue.InProgrss));
				case "3":
					return View("Index", application.Where(a => a.Statue.EName == EStatue.Sended));
				default: return View("Index", application.OrderByDescending(a => a.Start));
			}
		}

		public ActionResult MessageHistory(int applicationId, string clientEMail) {
			var context = new RmaEntities();
				var emails = context.Email.Where(e => e.Application_Id == applicationId).ToList();
				var model = new EmailsViewModel(emails, clientEMail);
				return View(model);
			
		}
	}

}
