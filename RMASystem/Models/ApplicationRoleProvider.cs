using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace RMASystem.Models {
	public class ApplicationRoleProvider : RoleProvider{
		public override bool IsUserInRole(string email, string roleName) {
			var foundUser = GetUser(email);
			if (foundUser == null) return false;

			return foundUser.Role.Name == roleName;
		}

		public override string[] GetRolesForUser(string email) {
			var foundUser = GetUser(email);
			return new[] { foundUser.Role.Name };
		}

		public override void CreateRole(string roleName) {
			using (var context = new RmaEntities()) {
				context.Role.Add(new Role { Name = roleName });
			}
		}

		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
			throw new NotImplementedException();
		}

		public override bool RoleExists(string roleName) {
			throw new NotImplementedException();
		}

		public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
			throw new NotImplementedException();
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
			throw new NotImplementedException();
		}
		public override string[] GetUsersInRole(string roleName) {
			throw new NotImplementedException();
		}

		public override string[] GetAllRoles() {
			using (var context = new RmaEntities()) {
				return context.Role.Select(r => r.Name).ToArray();
			}
		}

		public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
			throw new NotImplementedException();
		}

		public override string ApplicationName { get; set; }

		User GetUser(string email) {
			var context = new RmaEntities();
			return context.User.FirstOrDefault(u => u.Email == email);
			
		}
	}
}