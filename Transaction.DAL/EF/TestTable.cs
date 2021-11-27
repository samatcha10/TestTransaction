using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Transaction.DAL.EF
{
    public partial class TestTable
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("TransactionID")]
        [StringLength(50)]
        public string TransactionId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        [StringLength(5)]
        public string CurrencyCode { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? TransactionDate { get; set; }
        [StringLength(5)]
        public string Status { get; set; }
        [StringLength(5)]
        public string FileType { get; set; }
        [StringLength(10)]
        public string FileStatus { get; set; }
    }
}
