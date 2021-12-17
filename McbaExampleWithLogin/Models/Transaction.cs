using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcba.Models
{
    public enum TransactionType
    {
        Deposit = 1,
        Withdraw = 2,
        Transfer = 3,
        ServiceCharge = 4,
        BillPay = 5
    }

    public record Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionID { get; init; }

        [Required, StringLength(1)]
        public TransactionType TransactionType { get; init; }

        [Required, ForeignKey("Account")]
        public int AccountNumber { get; init; }
        public virtual Account Account { get; init; }

        [ForeignKey("DestinationAccount")]
        public int? DestinationAccountNumber { get; init; }
        public virtual Account DestinationAccount { get; init; }

        [Required, Column(TypeName = "money"), Range(0, double.MaxValue)]
        public decimal? Amount { get; init; }
#nullable enable
        [StringLength(255)]
        public string? Comment { get; init; }
#nullable disable
        [Required, DataType(DataType.Date)]
        public DateTime? ModifyDate { get; set; }
    }
}