using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace RMASystem.Models {
	public sealed class User {
		[Required(ErrorMessage = "Musisz podać e-mail")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Musisz podać hasło")]
		public string Password { get; set; }

	}
}