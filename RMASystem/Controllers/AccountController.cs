// -----------------------------------------------------------------------
// <copyright file="AccountController.cs">
//     Copyright (c) 2017, Adrian Kujawski.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RMASystem.Helpers;
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
				var userEmail = HttpContext.User.Identity.Name;
				var currentUser = RMASystem.User.GetLogged(userEmail);
				ViewBag.User = currentUser;
				return View("UserPanel");
			
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Login(ApplicationUser model) {
			if (!ModelState.IsValid) return View("Index");

			if (_userAuthenctication.CheckLogin(model.Email, model.Password)) {
				SignInUser(model.Email);
				return RedirectToAction("Index");
			}

			ModelState.AddModelError("", "Niepoprawny e-mail lub hasło");
			return View("Index");
		}

		public ActionResult Logout() {
			FormsAuthentication.SignOut();
			return RedirectToAction("Index");
		}

		[AllowAnonymous]
		public ActionResult Register() {
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> Register(RegisterUserViewModel model) {
			if (!ModelState.IsValid) return View();

			if (IsEmailNotUnique(model.Email)) ModelState.AddModelError("", "Ten e-mail jest już w bazie.");
			var addressId = AddNewAddress(model);
			var bankId = AddNewBank(model);
			AddNewUser(model, addressId, bankId);
			await SendVerificationEmail(model);
			SignInUser(model.Email);
			return RedirectToAction("Index", new { register = true });
		}

		async Task SendVerificationEmail(RegisterUserViewModel userEmail) {
			var message = new MailMessage();
			PrepareMessage(message, userEmail);
			var emailHelper = new EmailHelper(true);
			await emailHelper.Send(message);
		}


		void PrepareMessage(MailMessage message, RegisterUserViewModel model) {
			var token = HashHelper.Encrypt(model.Id.ToString(), model.Email);
			message.To.Add(new MailAddress(model.Email));  
			message.From = new MailAddress(Settings.EmailAddress);  
			message.Subject = "Rejestracja w systemie RMA";
			message.Body = $"Witaj {model.FirstName}!{Environment.NewLine}Kliknij link poniżej aby potwierdzić adres e-mail." +
							$"{Environment.NewLine}{Url.Action("ConfirmEmail", "Account", new { Token = token, model.Email }, Request.Url.Scheme)}";
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
				model.Id = newUser.Id;
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

		public ActionResult ConfirmEmail(string token, string email) {
			using (var context = new RmaEntities()) {
				var userId = int.Parse(HashHelper.Decrypt(token, email));
				var user = context.User.FirstOrDefault(u => u.Id == userId && u.Email == email);
				if (user == null) return null;

				user.EmailConfirmed = true;
				context.Entry(user).Property(p => p.EmailConfirmed).IsModified = true;
				context.SaveChanges();
				return RedirectToAction("Index");
			}
		}

		public ActionResult ChangePassword() {
			return View();
		}

		[HttpPost]
		public ActionResult ChangePassword(ChangePasswordViewModel model) {

			if (model.OldPassword != model.OldPasswordRepeated) ModelState.AddModelError("", "Hasła różnią się od siebie.");

			var userEmail = HttpContext.User.Identity.Name;
			var currentUser = RMASystem.User.GetLogged(userEmail);

			if(currentUser.Password != model.OldPassword) ModelState.AddModelError("", "Obecne hasło jest niepoprawne.");

			if (!ModelState.IsValid) return View();
			
			RMASystem.User.ChangePassword(userEmail, model.NewPassword);
			return RedirectToAction("Index");
		}
	}

}
