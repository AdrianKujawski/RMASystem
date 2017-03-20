using RMASystem.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RMASystem.Controllers.Tests {
	public class Login_User {

		bool execute(string email, string password) {
			using (var context = new RmaEntities()) {
				var isCorrectData = context.User.FirstOrDefault(u => u.Email == email && u.Password == password);
				return isCorrectData != null;
			}
		}

		[Fact]
		public void login_user_from_db() {
			var email = "adrian.kujawski@outlook.com";
			var password = "adrian123";
			var result = execute(email, password);

			Assert.True(result);
		}

		[Fact]
		public void login_user_from_db_no_email_and_password() {
			var email = string.Empty;
			var password = string.Empty;
			var result = execute(email, password);

			Assert.False(result);
		}
	}
}