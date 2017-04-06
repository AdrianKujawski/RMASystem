using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RMASystem.Models.ViewModel {
	public class ChangePasswordViewModel {

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Aktualne hasło")]
		public string OldPassword { get; set; }

		[Required]
		[Display(Name = "Nowe hasło")]
		[DataType(DataType.Password)]
		[StringLength(30, MinimumLength = 8)]
		public string NewPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Powtórz nowe hasło")]
		public string NewPasswordRepeated { get; set; }
	}
}