namespace KPI_APP.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("User")]
    public partial class User
    {
        public int Id { get; set; }
         
        public string Name { get; set; }

        public string Username { get; set; }

        public int DepartmentID { get; set; }
    }
}
