// -----------------------------------------------------------------------
// <copyright file="AccountController.cs">
//     Copyright (c) 2017, Adrian Kujawski.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Channels.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RMASystem.Models;

namespace RMASystem.Controllers {

	[Authorize]
	public class AccountController : Controller {
		readonly UserAuthenctication _userAuthenctication;

		public AccountController() {
			_userAuthenctication = new UserAuthenctication();
		}

		// GET: Login
		[AllowAnonymous]
		public ActionResult Index() {
			if (!HttpContext.User.Identity.IsAuthenticated) return View();

			using (var context = new RmaEntities()) {
				var userEmail = HttpContext.User.Identity.Name;
				var currentUser = context.User.FirstOrDefault(u => u.Email == userEmail);
				ViewBag.User = currentUser;
				return View("UserPanel");
			}
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Login(ApplicationUser model) {
			if (!ModelState.IsValid) return View("Index");

			if (_userAuthenctication.CheckLogin(model.Email, model.Password)) {
				SetupFormsAuthTicket(model.Email, false);
				FormsAuthentication.SetAuthCookie(model.Email, false);
				return RedirectToAction("UserPanel");
;			}

			ModelState.AddModelError("", "Niepoprawny e-mail lub hasło");
			return RedirectToAction("Index");
		}

		public ActionResult UserPanel() {
			return RedirectToAction("Index");
		}

		public ActionResult Logout() {
			FormsAuthentication.SignOut();
			return RedirectToAction("Index");
		}

		void SetupFormsAuthTicket(string email, bool persistanceFlag) {
			User user;
			using (var context = new RmaEntities()) {
				user = context.User.FirstOrDefault(u => u.Email == email);
			}
			var userId = user.Id;
			var userData = userId.ToString(CultureInfo.InvariantCulture);
			var authTicket = new FormsAuthenticationTicket(1, 
								user.Email, 
								DateTime.Now,            
								DateTime.Now.AddMinutes(30), 
								persistanceFlag,
								userData);

			var encTicket = FormsAuthentication.Encrypt(authTicket);
			Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
		}
	}

}
