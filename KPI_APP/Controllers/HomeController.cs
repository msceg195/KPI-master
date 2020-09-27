using KPI_APP.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KPI_APP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        Context db = new Context();

        public async Task<ActionResult> Index()
        {

            if (!(Session["user"] is ApplicationUser user))
                return Redirect("~/Account/LogOff");

            var dep_id = user.DepartmentID;

            DateTime dateTime = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var date = dateTime;
            var year = date.Year;
            var month = date.Month;

            var kPI_Item = await db.KPI_Item
                .Where(q =>
                       q.UserID == user.UserID
                    && q.KPI.KPI_Month == month.ToString()
                    && q.KPI.KPI_Year == year.ToString()
                    && q.IsClose == false)
                .ToListAsync();

            ViewBag.Date = date;

            foreach (var item in kPI_Item)
            {
                if (item.HasIbox && !string.IsNullOrEmpty(item.Query))
                {
                    var result = await Query(item.Query);

                    if (month == 1) item.Jan = result;
                    if (month == 2) item.Feb = result;
                    if (month == 3) item.Mar = result;
                    if (month == 4) item.Apr = result;
                    if (month == 5) item.May = result;
                    if (month == 6) item.Jun = result;
                    if (month == 7) item.Jul = result;
                    if (month == 8) item.Aug = result;
                    if (month == 9) item.Sep = result;
                    if (month == 10) item.Oct = result;
                    if (month == 11) item.Nov = result;
                    if (month == 12) item.Dec = result;
                }
            }

            return View(kPI_Item);
        }

        public JsonResult Send()
        {
            using (var db = new Context())
            {
                var email = db.Emails.FirstOrDefault(q =>
               q.CreateDate.Day == DateTime.Now.Day &&
               q.CreateDate.Month == DateTime.Now.Month &&
               q.CreateDate.Year == DateTime.Now.Year &&
               q.EmailType == 1);

                if (email == null)
                {
                    //string url = "/Reports/Index";


                    db.Emails.Add(new Emails() { EmailType = 1, CreateDate = DateTime.Now });
                    db.SaveChanges();
                }
            }
            return Json("done", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Search(string Year, string Month)
        {
            var lstKPI_Item = new List<KPI_Item>();
            try
            {
                var user = Session["user"] as ApplicationUser;
                var dep_id = user.DepartmentID;
                lstKPI_Item = await db.KPI_Item
                    .Where(q => q.DepartmentID == dep_id && q.KPI.KPI_Year == Year && q.KPI.KPI_Month == Month)
                    .Include(k => k.KPI).ToListAsync();
            }
            catch (Exception)
            {

            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetIbox(int id)
        {
            try
            {
                var q = await db.KPI_Item.FindAsync(id);
            }
            catch (Exception)
            {

            }

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Save(KPI_Item[] Items, bool IsClose = false)
        {
            var date = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year;
            var month = date.Month;

            using (var context = new Context())
            {
                try
                {
                    for (int i = 0; i < Items.Length; i++)
                    {
                        var item = Items[i];

                        var kpi_item = await context.KPI_Item.FirstOrDefaultAsync(q => q.Id == item.Id);
                         
                        if (kpi_item.Format.ToLower() == "time" && !string.IsNullOrEmpty(item.Jan))
                        {
                            var sec_num = decimal.Parse(item.Jan.Replace(":", ".")); // don't forget the second param
                            var hours = Math.Truncate(sec_num);
                            var minutes = Math.Ceiling((sec_num - Math.Truncate(sec_num)) * 100);

                            string _h = "", _m = "";

                            if (hours < 10) { _h = "0" + hours.ToString(); } else { _h = hours.ToString(); }
                            if (minutes < 10) { _m = "0" + minutes.ToString(); } else { _m = minutes.ToString(); }

                            item.Jan = _h + ':' + _m;
                        }

                        if (month == 1) kpi_item.Jan = item.Jan;
                        if (month == 2) kpi_item.Feb = item.Jan;
                        if (month == 3) kpi_item.Mar = item.Jan;
                        if (month == 4) kpi_item.Apr = item.Jan;
                        if (month == 5) kpi_item.May = item.Jan;
                        if (month == 6) kpi_item.Jun = item.Jan;
                        if (month == 7) kpi_item.Jul = item.Jan;
                        if (month == 8) kpi_item.Aug = item.Jan;
                        if (month == 9) kpi_item.Sep = item.Jan;
                        if (month == 10) kpi_item.Oct = item.Jan;
                        if (month == 11) kpi_item.Nov = item.Jan;
                        if (month == 12) kpi_item.Dec = item.Jan;

                        try
                        {
                            kpi_item.IsClose = IsClose;

                            context.Entry(kpi_item).State = EntityState.Modified;

                            await context.SaveChangesAsync();
                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);

                                    ViewBag.Error = ve.ErrorMessage;
                                }
                            }
                            continue;
                        }
                    }
                    ViewBag.Success = "Save Done";
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);

                            ViewBag.Error = ve.ErrorMessage; 
                        }
                    }
                }
            }
             
            var user = Session["user"] as ApplicationUser;
            var dep_id = user.DepartmentID;
 
            var kPI_Item = await db.KPI_Item
                .Where(q => q.UserID == user.UserID
                        && q.KPI.KPI_Month == month.ToString()
                        && q.KPI.KPI_Year == year.ToString()
                        && q.IsClose == true)
                .ToListAsync();

            ViewBag.Items = kPI_Item;

            return Json("Finished", JsonRequestBehavior.AllowGet);
        }

        public async Task<string> Query(string Statement)
        {
            var dt = new System.Data.DataTable();
            try
            {
                var oraConnection = ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
                var orConnection = new OracleConnection(oraConnection);
                var orCommand = orConnection.CreateCommand();
                orCommand.CommandText = Statement;
                if (orConnection.State == ConnectionState.Closed)
                    await orConnection.OpenAsync();

                dt.Load(await orCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return dt.Rows.Count > 0 ? dt.Rows[0][0].ToString() : "";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}