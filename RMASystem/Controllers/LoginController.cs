using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RMASystem.Models;

namespace RMASystem.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
	    public ActionResult Login(User user) {
		    if (!ModelState.IsValid) return View("Index");

		    if (user.Email == "A" && user.Password == "B") {
			    return View(user);
		    }

		    return View("Index");
	    }
    }
}