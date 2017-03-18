using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RMASystem.Validator {
	public class NotFutureDate : ValidationAttribute {
		public override bool IsValid(object obj) {
			if (obj == null) return true;
			var date = (DateTime)obj;
			return date <= DateTime.Now;
		}
	}
}