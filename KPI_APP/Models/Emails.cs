namespace KPI_APP.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Emails")]
    public partial class Emails
    {
        public int Id { get; set; }
         
        public DateTime CreateDate { get; set; } 

        public byte EmailType { get; set; }
    }
}
