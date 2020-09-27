namespace KPI_APP.Models
{
    using System.Data.Entity;

    public class Context : DbContext
    {
        public Context()
            : base("name=Context")
        {
        }

        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Excel_Department> Excel_Department { get; set; }
        public virtual DbSet<KPI> KPI { get; set; }
        public virtual DbSet<KPI_Item> KPI_Item { get; set; }
        public virtual DbSet<KPI_PIC> KPI_PIC { get; set; }
        public virtual DbSet<KPI_Plan> KPI_Plan { get; set; }
        public virtual DbSet<Plan> Plan { get; set; }
        public virtual DbSet<Setting> Setting { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Emails> Emails { get; set; }
        public virtual DbSet<Query> Query { get; set; }
        public virtual DbSet<Query_Parameter> Query_Parameter { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetRoles)
                .WithMany(e => e.AspNetUsers)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("UserId").MapRightKey("RoleId"));

            modelBuilder.Entity<Excel_Department>()
                .HasMany(e => e.Department)
                .WithOptional(e => e.Excel_Department)
                .HasForeignKey(e => e.ParentID);

            modelBuilder.Entity<Excel_Department>()
                .HasMany(e => e.KPI_Item)
                .WithOptional(e => e.Excel_Department)
                .HasForeignKey(e => e.DepartmentID);

            modelBuilder.Entity<KPI>()
                .HasMany(e => e.KPI_Item)
                .WithRequired(e => e.KPI)
                .HasForeignKey(e => e.KPI_ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<KPI>()
                .HasMany(e => e.KPI_Plan)
                .WithRequired(e => e.KPI)
                .HasForeignKey(e => e.KPI_ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<KPI_Item>()
                .HasMany(e => e.KPI_PIC)
                .WithRequired(e => e.KPI_Item)
                .HasForeignKey(e => e.Item_ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Plan>()
                .HasMany(e => e.KPI_Plan)
                .WithRequired(e => e.Plan)
                .HasForeignKey(e => e.Plan_ID)
                .WillCascadeOnDelete(false);
        }
    }
}
