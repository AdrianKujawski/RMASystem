using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMASystem.DbHelper {
	public sealed class Provider : RmaEntities {

		static Provider _provider = null;
		static readonly object _lock= new object();

		public static Provider Instance {
			get {
				lock(_lock) {
					return _provider ?? (_provider = new Provider());
				}
			}
		}

		Provider() {}

		public bool FindUser(string email, string password) {
			var foundUser = Instance.User.FirstOrDefault(u => u.Email == email && u.Password == password);
			return foundUser != null;
		}
	}
}