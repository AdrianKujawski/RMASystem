﻿// -----------------------------------------------------------------------
// <copyright file="AccountController.cs">
//     Copyright (c) 2017, Adrian Kujawski.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RMASystem.Models;
using RMASystem.Models.ViewModel;

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
				SignInUser(model.Email);
				return RedirectToAction("UserPanel");
			}

			ModelState.AddModelError("", "Niepoprawny e-mail lub hasło");
			return View("Index");
		}

		public ActionResult UserPanel() {
			return RedirectToAction("Index");
		}

		public ActionResult Logout() {
			FormsAuthentication.SignOut();
			return RedirectToAction("Index");
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Register(RegisterUserViewModel model) {
			if (!ModelState.IsValid) return View();

			if (IsEmailNotUnique(model.Email)) ModelState.AddModelError("", "Ten e-mail jest już w bazie.");
			var addressId = AddNewAddress(model);
			var bankId = AddNewBank(model);
			AddNewUser(model, addressId, bankId);
			SignInUser(model.Email);
			return RedirectToAction("UserPanel");
		}

		void AddNewUser(RegisterUserViewModel model, int addressId, int bankId) {
			using (var context = new RmaEntities()) {
				var newUser = new User {
					FirstName = model.FirstName,
					LastName = model.LastName,
					Email = model.Email,
					Password = model.Password,
					Phone = model.Phone,
					Adress_Id = addressId,
					BankAccount_Id = bankId,
					Role_Id = context.Role.First(r => r.Name == "Klient").Id
				};
				context.User.Add(newUser);
				context.SaveChanges();
			}
		}

		int AddNewAddress(RegisterUserViewModel model) {
			using (var context = new RmaEntities()) {
				var newAddress = new Adress {
					City = model.City,
					Street = model.Street,
					ZipCode = model.ZipCode
				};
				context.Adress.Add(newAddress);
				context.SaveChanges();
				return newAddress.Id;
			}
		}

		int AddNewBank(RegisterUserViewModel model) {
			using (var context = new RmaEntities()) {
				var newBank = new BankAccount {
					Name = model.BankName,
					AccountNumber = model.AccountNumber
				};
				context.BankAccount.Add(newBank);
				context.SaveChanges();
				return newBank.Id;
			}
		}

		static bool IsEmailNotUnique(string newEmail) {
			using (var context = new RmaEntities()) {
				return context.User.Any(u => u.Email == newEmail);
			}
		}

		[AllowAnonymous]
		public ActionResult Register() {
			return View();
		}

		void SignInUser(string email) {
			SetupFormsAuthTicket(email, false);
			FormsAuthentication.SetAuthCookie(email, false);
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
