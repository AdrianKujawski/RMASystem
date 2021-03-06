//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc.Html;

namespace RMASystem
{
    using System;
    using System.Collections.Generic;
    
    public partial class Adress
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Adress()
        {
            this.User = new HashSet<User>();
        }
    
        public int Id { get; set; }

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
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> User { get; set; }
    }
}
