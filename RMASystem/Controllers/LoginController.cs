// -----------------------------------------------------------------------
// <copyright file="LoginController.cs">
//     Copyright (c) 2017, Adrian Kujawski.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using System.Web.Mvc;
using RMASystem.DbHelper;
namespace RMASystem.Controllers {

	public class LoginController : Controller {
		// GET: Login
		public ActionResult Index() {
			return View();
		}

		[HttpPost]
		public ActionResult Login(Models.User user) {
			var isCorrectData = Provider.Instance.LoginUser(user?.Email, user?.Password);
			if (!ModelState.IsValid || !isCorrectData) return View("Index");

			return View();
		}
	}

}
