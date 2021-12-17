using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcba.Models
{
    public record Login
    {
        [Key, Required, StringLength(8)]
        [Display(Name = "Login ID")]
        public string LoginID { get; init; }

        [Required, ForeignKey("Customer")]
        public int CustomerID { get; init; }

        [Required, StringLength(64)]
        public string PasswordHash { get; init; }

        [Required, DataType(DataType.Date)]
        public DateTime ModifyDate { get; set; }

        public virtual Customer Customer { get; init; }
    }
}
