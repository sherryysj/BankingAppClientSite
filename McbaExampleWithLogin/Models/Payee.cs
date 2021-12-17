using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcba.Models
{
    public record Payee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PayeeID { get; init; }

        [Required, StringLength(50)]
        public string PayeeName { get; init; }
#nullable enable
        [StringLength(50)]
        public string? Address { get; init; }

        [StringLength(40)]
        public string? City { get; init; }

        [StringLength(20)]
        public string? State { get; init; }

        [StringLength(10)]
        public string? PostCode { get; init; }
#nullable disable
        [Required, StringLength(15), DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; init; }
    }
}
