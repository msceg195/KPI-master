namespace KPI_APP.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Query")]
    public partial class Query
    {
        [Key]
        public int ID { get; set; }

        public int KPI_ID { get; set; }

        [StringLength(500)]
        public string Fields { get; set; }

        [StringLength(500)]
        public string Table_View { get; set; }

        [StringLength(500)]
        public string Type { get; set; }
    }

    [Table("Query_Parameter")]
    public partial class Query_Parameter
    {
        [Key]
        public int ID { get; set; }

        public int KPI_ID { get; set; }

        [StringLength(500)]
        public string Param_Name { get; set; }

        [StringLength(500)]
        public string Operator { get; set; }

        [StringLength(500)]
        public string Param_Value { get; set; }
    }
}
