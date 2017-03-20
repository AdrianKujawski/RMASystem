using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using RMASystem.Helpers;
using Xunit;

namespace RMASystem.Tests {
	public class Send_Email {

		async Task execute(EmailHelper emailHelper, MailMessage message) {
			await emailHelper.Send(message);
		}

		[Fact]
		void send_email_to_user() {
			var emailHelper = new EmailHelper(true);
			var message = new MailMessage();
			message.To.Add(new MailAddress("adrian.kujawski@outlook.com"));
			message.From = new MailAddress(Settings.EmailAddress);
			message.Subject = "Rejestracja w systemie RMA";
			message.Body = $"Witaj Adrian!{Environment.NewLine}Kliknij link poniżej aby potwierdzić adres e-mail.";

			Task.Run(async () => {
				await execute(emailHelper, message);
			}).GetAwaiter().GetResult();

		}
	}
}
