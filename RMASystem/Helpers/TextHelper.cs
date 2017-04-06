using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RMASystem.Helpers {
	public static class TextHelper {

		public static string ChangeFirstLetterToUpper(string text) {
			return text.First().ToString().ToUpper() + text.Substring(1);
		}

	}
}