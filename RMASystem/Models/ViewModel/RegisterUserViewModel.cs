using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RMASystem.Models.ViewModel {
	public class RegisterUserViewModel {

		public int Id { get; set; }

		[Required]
		[Display(Name = "Imie")]
		[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Dozwolone są tylko litery")]
		[StringLength(30, MinimumLength = 2)]
		public string FirstName { get; set; }

		[Required]
		[Display(Name = "Nazwisko")]
		[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Dozwolone są tylko litery")]
		[StringLength(40, MinimumLength = 2)]
		public string LastName { get; set; }

		[Required]
		[Display(Name = "Telefon")]
		[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$", ErrorMessage = "Telefon musi być w formacie 123-456-789.")]
		[StringLength(20, MinimumLength = 9)]
		public string Phone { get; set; }

		[Required]
		[Display(Name = "E-mail")]
		[EmailAddress(ErrorMessage = "Niepoprawny adres e-mail.")]
		public string Email { get; set; }

		[Required]
		[Display(Name = "Hasło")]
		[DataType(DataType.Password)]
		[StringLength(30, MinimumLength = 8)]
		public string Password { get; set; }

		[Required]
		[Display(Name = "Miasto")]
		public string City { get; set; }

		[Required]
		[Display(Name = "Ulica")]
		public string Street { get; set; }

		[Required]
		[Display(Name = "Kod pocztowy")]
		[RegularExpression(@"^\(?([0-9]{2})\)?[-. ]?([0-9]{3})$", ErrorMessage = "Kod pocztowy musi być w formacie 00-000")]
		public string ZipCode { get; set; }

		[Required]
		[Display(Name = "Nazwa banku")]
		public string BankName { get; set; }

		[Required]
		[Display(Name = "Numer konta")]
		[StringLength(26, MinimumLength = 26, ErrorMessage = "Konto bankowe musi miec 26 cyfr.")]
		[RegularExpression("^[0-9]*$", ErrorMessage = "Tylko cyfry są dozwolone.")]
		public string AccountNumber { get; set; }
	}
}