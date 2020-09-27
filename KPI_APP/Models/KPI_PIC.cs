namespace KPI_APP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPI_PIC
    {
        public int Id { get; set; }

        public int Item_ID { get; set; }

        public int UserID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(10)]
        public string Grade { get; set; }

        public virtual KPI_Item KPI_Item { get; set; }
    }
}
