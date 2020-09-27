namespace KPI_APP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class KPI_Plan
    {
        public int Id { get; set; }

        public int Plan_ID { get; set; }

        public int KPI_ID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        public virtual KPI KPI { get; set; }

        public virtual Plan Plan { get; set; }
    }
}
