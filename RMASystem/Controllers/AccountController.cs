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
			ViewBag.City = currentUser.Adress?.City;
			ViewBag.Street = currentUser.Adress?.Street;
			ViewBag.ZipCode = currentUser.Adress?.ZipCode;
			return View("UserPanel");
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Login(ApplicationUser model) {

#if DEBUG
			if (string.IsNullOrEmpty(model.Email)) {
				SignInUser("adrian.kujawski@outlook.com");
				return RedirectToAction("Index");
			}
#endif
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
			var emailHelper = new EmailHelper();
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

			if (currentUser.Password != model.OldPassword) ModelState.AddModelError("", "Obecne hasło jest niepoprawne.");

			if (!ModelState.IsValid) return View();

			RMASystem.User.ChangePassword(userEmail, model.NewPassword);
			return RedirectToAction("Index");
		}

		public ActionResult EditDate(int? userId) {
			if (userId == null) {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var context = new RmaEntities();
			var user = context.User.Find(userId);
			if (user == null) {
				return HttpNotFound();
			}

			return View(user);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditDate([Bind(Include = "Id,FirstName,LastName,Phone,Email,Password,Role_Id,Adress_Id,BankAccount_Id")] User user, [Bind(Include = "City,Street,ZipCode")] Adress adress,
									[Bind(Include = "Name,AccountNumber")] BankAccount bankAccount) {
			if (!ModelState.IsValid) return View(user);

			if (!ChangeValues(user, adress, bankAccount)) return View(user);

			var isEmailChanged = HttpContext.User.Identity.Name != user.Email;
			if (!isEmailChanged) return RedirectToAction("Index");

			FormsAuthentication.SignOut();
			SignInUser(user.Email);
			return RedirectToAction("Index");
		}

		bool ChangeValues(User user, Adress address, BankAccount bankAccount) {
			using (var context = new RmaEntities()) {
				var oldUser = context.User.Find(user.Id);
				if (oldUser != null) {
					if (context.User.Any(m => m.Email == user.Email && m.Id != user.Id)) {
						ModelState.AddModelError("", "Podany e-mail już istnieje!");
						return false;
					}
					ChangeValues(user, address, bankAccount, oldUser);
				}

				context.SaveChanges();
				return true;
			}
		}

		void ChangeValues(User user, Adress address, BankAccount bankAccount, User oldUser) {
			RMASystem.User.SetFirstLetterOfNameToUpper(user);
			oldUser.FirstName = user.FirstName;
			oldUser.LastName = user.LastName;
			oldUser.Phone = user.Phone;
			oldUser.Email = user.Email;

			if (oldUser.Adress_Id == null) oldUser.Adress_Id = CreateAddress(address);
			else {
				oldUser.Adress.City = address.City;
				oldUser.Adress.Street = address.Street;
				oldUser.Adress.ZipCode = address.ZipCode;
			}

				if (oldUser.BankAccount_Id == null) oldUser.BankAccount_Id = CreateBankAccount(bankAccount);
				else {
				oldUser.BankAccount.Name = bankAccount.Name;
				oldUser.BankAccount.AccountNumber = bankAccount.AccountNumber;
			}
		}

		int CreateBankAccount(BankAccount bankAccount) {
			using (var context = new RmaEntities()) {
				var addedBankAccount = context.BankAccount.Add(bankAccount);
				context.SaveChanges();
				return addedBankAccount.Id;
			}
		}

		int CreateAddress(Adress address) {
			using (var context = new RmaEntities()) {
				var addedAddress = context.Adress.Add(address);
				context.SaveChanges();
				return addedAddress.Id;
			}
		}
	}

}
