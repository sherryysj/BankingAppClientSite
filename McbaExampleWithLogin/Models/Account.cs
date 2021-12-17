using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Mcba.Models
{
    public enum AccountType
    {
        Checking = 1,
        Saving = 2
    }

    public record Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Number")] //Gets or sets a value that is used for display in the UI
        public int AccountNumber { get; init; }

        [Required, StringLength(1)]
        [Display(Name = "Type")]
        public AccountType AccountType { get; init; }

        [Required]
        public int CustomerID { get; init; }

        [Required, Column(TypeName = "money"), Range(0, double.MaxValue)]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime ModifyDate { get; set; }

        [Required]
        public bool Active { get; set; }

        public virtual Customer Customer { get; init; }

        public virtual List<Transaction> Transactions { get; init; }
        public virtual List<BillPay> BillPays { get; init; }
    }
}
