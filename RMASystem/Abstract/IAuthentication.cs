using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMASystem.Abstract {
	public interface IAuthentication {
		bool CheckLogin(string email, string password);
		bool Logout();
	}
}