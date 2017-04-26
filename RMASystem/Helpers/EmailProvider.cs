using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace RMASystem.Helpers {
	public class EmailProvider {

		Email _emailMessage;

		public EmailProvider(Email emailMessage) {
			_emailMessage = emailMessage;
		}

		public void SaveToDb() {
			using (var context = new RmaEntities()) {
				context.Email.Add(_emailMessage);
				context.SaveChanges();
			}
		}
	}
}