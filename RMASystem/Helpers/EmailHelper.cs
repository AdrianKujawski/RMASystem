﻿// -----------------------------------------------------------------------
// <copyright file="EmailHelper.cs">
//     Copyright (c) 2017, Adrian Kujawski.
// </copyright>
// -----------------------------------------------------------------------

using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RMASystem.Helpers {

	public class EmailHelper {
		readonly NetworkCredential _credential;
		readonly bool _enableSsl;

		public EmailHelper(bool enableSsl = true) {
			_enableSsl = enableSsl;
			_credential = new NetworkCredential {
				UserName = Settings.EmailAddress,
				Password = Settings.EmailPassword
			};
		}

		public async Task Send(MailMessage message) {
			using (var smtp = new SmtpClient()) {
				smtp.Credentials = _credential;
				smtp.Host = Settings.EmailHost;
				smtp.Port = Settings.EmailPort;
				smtp.EnableSsl = _enableSsl;
				await smtp.SendMailAsync(message);
			}
		}
	}

}
