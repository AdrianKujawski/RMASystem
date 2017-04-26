using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMASystem.Models.ViewModel {
	public class EmailsViewModel {
		public IEnumerable<Email> ClientEmails { get; }

		public IEnumerable<Email> WorkerEmails { get; }

		public IEnumerable<Email> SortedEmails {
			get {
				var allEmails = ClientEmails.ToList();
				allEmails.AddRange(WorkerEmails);
				return allEmails.OrderBy(e => e.PostDate);
			}
		}

		public EmailsViewModel(IEnumerable<Email> emails, string clientEmail) {
			ClientEmails = emails.Where(e => e.Sender == clientEmail);
			WorkerEmails = emails.Where(e => e.Sender != clientEmail);
		}
	}
}