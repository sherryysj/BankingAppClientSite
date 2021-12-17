using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcba.Models
{
    public record Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerID { get; init; }

        [Required, StringLength(50)]
        public string Name { get; init; }

        [StringLength(11)]
#nullable enable
        public string? TFN { get; init; }

        [StringLength(50)]
        public string? Address { get; init; }

        [StringLength(40)]
        public string? City { get; init; }

        [StringLength(20)]
        public string? State { get; init; }

        [StringLength(4)]
        public string? PostCode { get; init; }

        [StringLength(15)]
        public string? Phone { get; init; }

        [StringLength(40)]
        public string? Email { get; init; }

        [DataType(DataType.Date)]
        public DateTime? CheckDate { get; set; }
#nullable disable
        public virtual List<Account> Accounts { get; init; }
    }
}
