using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RMASystem.Abstract;

namespace RMASystem.Models {
	public class UserAuthenctication : IAuthentication{

		public bool CheckLogin(string email, string password) {
			using (var context = new RmaEntities()) {
				var isCorrectData = context.User.FirstOrDefault(u => u.Email == email && u.Password == password);
				return isCorrectData != null;
			}
		}

		public bool Logout() {
			return true;
		}
	}
}