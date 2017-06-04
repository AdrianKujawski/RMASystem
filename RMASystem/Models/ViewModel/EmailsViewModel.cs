using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMASystem.Models.ViewModel {
	public class EmailsViewModel {
		public int ApplicationId { get; }
		public string MessageContent { get; set; }
		public string MessageSubject { get; set; }
		
		public IEnumerable<Email> ClientEmails { get; }

		public IEnumerable<Email> WorkerEmails { get; }

		public IEnumerable<Email> SortedEmails {
			get {
				var allEmails = ClientEmails.ToList();
				allEmails.AddRange(WorkerEmails);
				return allEmails.OrderByDescending(e => e.PostDate);
			}
		}

		public string WorkerEmail {
			get { return WorkerEmails.FirstOrDefault(w => w.Sender != null).Sender; }
		}
		
		public string ClientEmail {
			get { return ClientEmails.FirstOrDefault(w => w.Sender != null).Sender; }
		}
		
		public EmailsViewModel(IEnumerable<Email> emails, string clientEmail, int applicationId) {
			ClientEmails = emails.Where(e => e.Sender == clientEmail);
			WorkerEmails = emails.Where(e => e.Sender != clientEmail);
			ApplicationId = applicationId;
		}
	}
}