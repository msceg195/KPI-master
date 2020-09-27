namespace KPI_APP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPI_Item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KPI_Item()
        {
            KPI_PIC = new HashSet<KPI_PIC>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { get; set; }

        public int KPI_ID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        public int Row { get; set; }

        [Required]
        [StringLength(10)]
        public string Format { get; set; }

        [Required]
        [StringLength(500)]
        public string Remarks { get; set; }

        public int? DepartmentID { get; set; }

        public int? UserDepartmentID { get; set; }

        public int UserID { get; set; }

        [StringLength(50)]
        public string Jan { get; set; }

        [StringLength(50)]
        public string Feb { get; set; }

        [StringLength(50)]
        public string Mar { get; set; }

        [StringLength(50)]
        public string Apr { get; set; }

        [StringLength(50)]
        public string May { get; set; }

        [StringLength(50)]
        public string Jun { get; set; }

        [StringLength(50)]
        public string Jul { get; set; }

        [StringLength(50)]
        public string Aug { get; set; }

        [StringLength(50)]
        public string Sep { get; set; }

        [StringLength(50)]
        public string Oct { get; set; }

        [StringLength(50)]
        public string Nov { get; set; }

        [StringLength(50)]
        public string Dec { get; set; }

        public bool IsClose { get; set; }

        public bool HasIbox { get; set; }

        public bool HasProductivity { get; set; }
         
        public string IboxID { get; set; }

        public string Query { get; set; }

        public virtual Excel_Department Excel_Department { get; set; }

        public virtual KPI KPI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KPI_PIC> KPI_PIC { get; set; }
    }
}
