namespace KPI_APP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KPI")]
    public partial class KPI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KPI()
        {
            KPI_Item = new HashSet<KPI_Item>();
            KPI_Plan = new HashSet<KPI_Plan>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Title { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        [Required]
        [StringLength(4)]
        public string KPI_Year { get; set; }

        [Required]
        [StringLength(2)]
        public string KPI_Month { get; set; }
         
        public bool IsClose { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KPI_Item> KPI_Item { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KPI_Plan> KPI_Plan { get; set; }
    }
}
