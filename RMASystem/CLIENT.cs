//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RMASystem
{
    using System;
    using System.Collections.Generic;
    
    public partial class CLIENT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CLIENT()
        {
            this.APPLICATION = new HashSet<APPLICATION>();
        }
    
        public int ID { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string PHONE { get; set; }
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
        public Nullable<int> ROLE_ID { get; set; }
        public Nullable<int> ADRESS_ID { get; set; }
        public Nullable<int> BANKACCOUNT_ID { get; set; }
    
        public virtual ADRESS ADRESS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<APPLICATION> APPLICATION { get; set; }
        public virtual BANKACCOUNT BANKACCOUNT { get; set; }
        public virtual ROLE ROLE { get; set; }
    }
}
