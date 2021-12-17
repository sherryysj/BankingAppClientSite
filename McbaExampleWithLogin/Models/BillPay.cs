using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Mcba.Models
{
    public record BillPay
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillPayID { get; init; }

        [Required, ForeignKey("Account")]
        public int AccountNumber { get; init; }

        [Required, ForeignKey("Payee")]
        public int PayeeID { get; init; }

        [Required, Column(TypeName = "money"), Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime ScheduleDate { get; set; }

        [Required, StringLength(1)]
        public string Period { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime ModifyDate { get; set; }

        [Required]
        public bool Active { get; set; }

        public virtual Payee Payee { get; init; }

        public virtual Account Account { get; init; }
    }
}
