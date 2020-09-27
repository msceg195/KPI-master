using KPI_APP.Models;
using Microsoft.Office.Interop.Excel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KPI_APP.Controllers
{
    [Authorize]
    public class KPIController : Controller
    {
        private Context db = new Context();
        Application xlApp = new Application();
        Workbook xlWorkbook = null;
        Worksheet xlWorksheet = null;
        Range xlRange = null;

        public async Task<ActionResult> Index()
        {
            return View(await db.KPI.ToListAsync());
        }

        public async Task<JsonResult> Download(int id)
        {
            var kpi = await db.KPI.FindAsync(id);

            var kpi_items = kpi.KPI_Item;

            var date = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year;
            var month = date.Month;

            var path = Path.Combine(Server.MapPath("~/Files/" + kpi.KPI_Year + "_" + kpi.KPI_Month), kpi.Title);

            Application xlApp = new Application();
            Workbook xlWorkbook = null;
            Worksheet xlWorksheet = null;
            Range xlRange = null;


            xlWorkbook = xlApp.Workbooks.Open(path);
            xlWorksheet = xlWorkbook.Sheets[1];
            xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;


            foreach (var item in kpi_items)
            {
                xlRange.Cells[item.Row, 13] = item.Jan;
                xlRange.Cells[item.Row, 14] = item.Feb;
                xlRange.Cells[item.Row, 15] = item.Mar;
                xlRange.Cells[item.Row, 16] = item.Apr;
                xlRange.Cells[item.Row, 17] = item.May;
                xlRange.Cells[item.Row, 18] = item.Jun;
                xlRange.Cells[item.Row, 19] = item.Jul;
                xlRange.Cells[item.Row, 20] = item.Aug;
                xlRange.Cells[item.Row, 21] = item.Sep;
                xlRange.Cells[item.Row, 22] = item.Oct;
                xlRange.Cells[item.Row, 23] = item.Nov;
                xlRange.Cells[item.Row, 24] = item.Dec;
            }

            xlWorkbook.Save();
            xlWorkbook.Close();
            xlApp.Quit();

            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);
            Marshal.ReleaseComObject(xlWorkbook);
            Marshal.ReleaseComObject(xlApp);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            //await Helper.Mail.SendEmail(
            //       Messages.KPI_Assigned_Subject,
            //       Messages.KPI_Assigned_Body,
            //       Messages.KPI_Admin_Email,
            //       Messages.KPI_Admin_Email,
            //       Messages.KPI_CC);

            return Json(("/Files/" + kpi.KPI_Year + "_" + kpi.KPI_Month + "/" + kpi.Title), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> Progress(int id)
        {
            var kpi = await db.KPI.FindAsync(id);

            var kpi_items = kpi.KPI_Item;

            var date = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year.ToString();
            var month = date.Month.ToString();

            var query = await (from user in db.AspNetUsers
                               join item in db.KPI_Item
                               on user.UserID equals item.UserID into ui
                               from x in ui.DefaultIfEmpty()
                               where x.KPI.KPI_Month == kpi.KPI_Month && x.KPI.KPI_Year == kpi.KPI_Year && x.KPI_ID == id
                               group user by new
                               {
                                   user.UserName,
                                   user.UserID
                               } into g
                               select new
                               {
                                   id = g.Key.UserID,
                                   name = g.Key.UserName,
                                   count = g.Count()
                               }).ToListAsync();

            var query2 = query.Select(q => new
            {
                q.id,
                name = q.name.Replace("@msc.com", "").Replace(".", " "),
                q.count,
                done = KPI_Done(q.id, int.Parse(kpi.KPI_Month))
            });

            return Json(query2, JsonRequestBehavior.AllowGet);
        }

        public int KPI_Done(int userid, int month)
        {
            var result = 0;
            switch (month)
            {
                case 1:
                    result = db.KPI_Item.Count(q => q.UserID == userid && !string.IsNullOrEmpty(q.Jan));
                    break;
                case 2:
                    result = db.KPI_Item.Count(q => q.UserID == userid && !string.IsNullOrEmpty(q.Feb));
                    break;
                case 3:
                    result = db.KPI_Item.Count(q => q.UserID == userid && !string.IsNullOrEmpty(q.Mar));
                    break;
                case 4:
                    result = db.KPI_Item.Count(q => q.UserID == userid && !string.IsNullOrEmpty(q.Apr));
                    break;
                case 5:
                    result = db.KPI_Item.Count(q => q.UserID == userid && !string.IsNullOrEmpty(q.May));
                    break;
                case 6:
                    result = db.KPI_Item.Count(q => q.UserID == userid && !string.IsNullOrEmpty(q.Jun));
                    break;
                case 7:
                    result = db.KPI_Item.Count(q => q.UserID == userid && !string.IsNullOrEmpty(q.Jul));
                    break;
                case 8:
                    result = db.KPI_Item.Count(q => q.UserID == userid && !string.IsNullOrEmpty(q.Aug));
                    break;
                case 9:
                    result = db.KPI_Item.Count(q => q.UserID == userid && !string.IsNullOrEmpty(q.Sep));
                    break;
                case 10:
                    result = db.KPI_Item.Count(q => q.UserID == userid && !string.IsNullOrEmpty(q.Oct));
                    break;
                case 11:
                    result = db.KPI_Item.Count(q => q.UserID == userid && !string.IsNullOrEmpty(q.Nov));
                    break;
                case 12:
                    result = db.KPI_Item.Count(q => q.UserID == userid && !string.IsNullOrEmpty(q.Dec));
                    break;
                default:
                    break;
            }
            return result;
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase file)
        {
            var lstItems = new List<KPI_Item>();
            string _FileName = "";

            var date = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year;
            var month = date.Month;

            try
            {
                #region Upload File
                string _path = "";
                if (file != null && file.ContentLength > 0)
                {
                    _FileName = Path.GetFileName(file.FileName);
                    DirectoryInfo _folderPath = null;
                    if (!Directory.Exists(Server.MapPath("~/Files/" + year + "_" + month)))
                        _folderPath = Directory.CreateDirectory(Server.MapPath("~/Files/" + year + "_" + month));

                    _path = Path.Combine(Server.MapPath("~/Files/" + year + "_" + month), _FileName);
                    file.SaveAs(_path);

                    TempData["FileName"] = _FileName;
                }
                #endregion

                if (string.IsNullOrEmpty(_path)) return View("Index", await db.KPI.ToListAsync());


                #region Read Excel File
                xlWorkbook = xlApp.Workbooks.Open(_path);
                xlWorksheet = xlWorkbook.Sheets[1];
                xlRange = xlWorksheet.UsedRange;

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;

                for (int i = 3; i < rowCount; i++)
                {
                    if ((xlRange.Cells[i, 3] != null &&
                        xlRange.Cells[i, 3].Value2 != null &&
                        xlRange.Cells[i, 3].Value2.ToString().ToLower() != "local")) continue;

                    var title = xlRange.Cells[i, 4].Value2;
                    if (title == null) continue;

                    var item = new KPI_Item
                    {
                        Row = i,
                        Format = ((Range)xlRange.Cells[i, 13]).NumberFormat
                    };

                    item.Format = item.Format == "[hh]:mm" ? "time" : "number";

                    if (xlRange.Cells[i, 4] != null && xlRange.Cells[i, 4].Value2 != null)
                        item.Remarks = xlRange.Cells[i, 4].Value2.ToString().ToUpper();

                    var department = "";
                    if (xlRange.Cells[i, 6] != null && xlRange.Cells[i, 6].Value2 != null)
                        department = xlRange.Cells[i, 6].Value2.ToString().ToUpper();

                    if (xlRange.Cells[i, 26] != null && xlRange.Cells[i, 26].Value2 != null)
                        item.Title = xlRange.Cells[i, 26].Value2.ToString().ToUpper();

                    var Department = await db.Excel_Department.FirstOrDefaultAsync(q => q.Name.ToLower().Contains(department.Trim().ToLower()));
                    item.CreateDate = DateTime.Now;
                    item.DepartmentID = Department == null ? -1 : Department.Id;
                    item.Remarks = item.Remarks;

                    lstItems.Add(item);
                }

                xlWorkbook.Close();
                xlApp.Quit();

                Marshal.ReleaseComObject(xlRange);
                Marshal.ReleaseComObject(xlWorksheet);
                Marshal.ReleaseComObject(xlWorkbook);
                Marshal.ReleaseComObject(xlApp);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                TempData["Items"] = lstItems;

                await Save(_FileName);

                return Redirect("/KPI/Index");
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
            finally
            {
                foreach (var process in Process.GetProcessesByName("Excel"))
                {
                    process.Kill();
                }
            }
            #endregion

            return View("Index", await db.KPI.ToListAsync());

        }

        [HttpPost]
        public async Task<JsonResult> Save(string fileName)
        {
            #region Write to data base

            var lstMonth = TempData["Items"] as List<KPI_Item>;

            using (var context = new Context())
            {
                try
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            KPI_Item kpi_item = null;
                            KPI kpi = null;
                            var filename = TempData["FileName"].ToString();

                            var date = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
                            var year = date.Year;
                            var month = date.Month;


                            kpi = new KPI()
                            {
                                CreateDate = DateTime.Now,
                                KPI_Month = month + "",
                                KPI_Year = year + "",
                                Title = fileName
                            };

                            context.Entry(kpi).State = EntityState.Added;

                            context.KPI.Add(kpi);
                            await context.SaveChangesAsync();


                            for (int i = 0; i < lstMonth.Count; i++)
                            {
                                KPI_Item item = lstMonth[i];
                                var _tempItem = await db.KPI_Item.FirstOrDefaultAsync(q => q.Row == kpi_item.Row);

                                kpi_item = new KPI_Item()
                                {
                                    KPI_ID = kpi.Id,
                                    DepartmentID = item.DepartmentID,
                                    Row = item.Row,
                                    Format = item.Format,
                                    Remarks = item.Remarks,
                                    Title = item.Title,
                                    UserID = _tempItem.UserID
                                };

                                try
                                {
                                    context.Entry(kpi_item).State = EntityState.Added;
                                    context.KPI_Item.Add(kpi_item);
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
                                }

                            }

                            transaction.Commit();
                        }
                        catch (DbEntityValidationException e)
                        {
                            transaction.Rollback();

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
            #endregion

            return Json("Done", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public async Task<JsonResult> Send(string base64String)
        {

            using (var db = new Context())
            {
                var email = await db.Emails.FirstOrDefaultAsync(q =>
               q.CreateDate.Day == DateTime.Now.Day &&
               q.CreateDate.Month == DateTime.Now.Month &&
               q.CreateDate.Year == DateTime.Now.Year &&
               q.EmailType == 1);

                if (email == null)
                {
                    var date = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
                    var year = date.Year.ToString();
                    var month = date.Month.ToString();

                    StringBuilder body = new StringBuilder();

                    string _Email_Text = System.IO.File.ReadAllText(Server.MapPath("~/Emails/Progress.html"));

                    _Email_Text = _Email_Text.Replace("{BODY}", body.ToString());
                    _Email_Text = _Email_Text.Replace("{TITLE}", "KPI " + date.Month + " - " + date.Year);

                    var split = base64String.Split(',')[1];
                    var image = Base64ToImage(split);
                    var stream = new MemoryStream();
                    image.Save(stream, ImageFormat.Jpeg);
                    stream.Position = 0;

                    var Attachment = new Attachment(stream, "chart_image.jpg", "image/jpg");

                    await Helper.Mail.SendEmail(
                            Messages.KPI_Assigned_Subject,
                            _Email_Text,
                            Messages.KPI_Admin_Email,
                            @"amr.mahmoud@msc.com",
                            Messages.KPI_CC,
                            Attachment);


                    db.Emails.Add(new Emails() { EmailType = 1, CreateDate = DateTime.Now });
                    db.SaveChanges();
                }
            }
            return Json("Done", JsonRequestBehavior.AllowGet);
        }

        public Image Base64ToImage(string base64String)
        {
            var imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                var image = Image.FromStream(ms, true);
                return image;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public async Task<ActionResult> CopySheet(int id)
        {
            var date = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year.ToString();
            var month = date.Month.ToString();

            var kpi = await db.KPI.FindAsync(id);
            if (kpi == null) return RedirectToAction("Index"); ;

            var kpi_new = new KPI();

            var kpi_items = kpi.KPI_Item.ToList();
            kpi_new.CreateDate = DateTime.Now;
            kpi_new.IsClose = false;
            kpi_new.Title = kpi.Title;
            kpi_new.KPI_Month = (int.Parse(kpi.KPI_Month) < 12) ? (int.Parse(kpi.KPI_Month) + 1).ToString() : "1";
            kpi_new.KPI_Year = (int.Parse(kpi.KPI_Month) < 12) ? kpi.KPI_Year : (int.Parse(kpi.KPI_Year) + 1).ToString();

            db.Entry(kpi_new).State = EntityState.Added;

            await db.SaveChangesAsync();

            foreach (var item in kpi_items)
            {
                var kpi_item = new KPI_Item()
                {
                    KPI_ID = kpi_new.Id,
                    DepartmentID = item.DepartmentID,
                    Row = item.Row,
                    Format = item.Format,
                    Remarks = item.Remarks,
                    Title = item.Title,
                    UserID = item.UserID,
                    Query = item.Query,
                    HasIbox = item.HasIbox
                };

                db.Entry(kpi_item).State = EntityState.Added;
                db.KPI_Item.Add(kpi_item);
            }

            try
            {
                await db.SaveChangesAsync();

                var _folderPath = Directory.CreateDirectory(Server.MapPath("~/Files/" + kpi_new.KPI_Year + "_" + kpi_new.KPI_Month));

                var _date = date.AddMonths(-1);
                var oldPath = Directory.CreateDirectory(Server.MapPath("~/Files/" + kpi.KPI_Year + "_" + kpi.KPI_Month));

                System.IO.File.Copy(oldPath.GetFiles()[0].FullName, _folderPath.FullName + @"\" + oldPath.GetFiles()[0].Name, false);

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
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> IBox()
        {
            ViewBag.Users = await db.AspNetUsers.Where(q => q.IsPIC.HasValue && q.IsPIC == true).ToListAsync();

            var date = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year;
            var month = date.Month;

            ViewBag.Grid = await db.KPI_Item
                .Where(q =>
                       q.KPI.KPI_Month == month + ""
                    && q.KPI.KPI_Year == year + ""
                    && q.HasIbox
                    && !string.IsNullOrEmpty(q.Query))
                .ToListAsync();

            ViewBag.KPIs = new List<KPI_Item>();
            return View();
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

                /*@ 
                    "SELECT a.osuser blocked_user, a.terminal blocked_terminal,
                    TO_CHAR(a.logon_time, 'DD/MM/YYYY HH24:MI:SS') login,
                    b.osuser blocked_by, b.terminal blocked_by_terminal, TO_CHAR(b.logon_time, 'DD/MM/YYYY HH24:MI:SS') blogin,
                    b.sid || ',' || b.serial# sidr FROM  gv$session a, gv$session b Where ROWNUM <= 100 "
                */

                if (orConnection.State == ConnectionState.Closed)
                    await orConnection.OpenAsync();

                dt.Load(await orCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection));

                TempData["dt"] = dt;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            var result = ToJson(dt);

            JavaScriptSerializer serializer = new JavaScriptSerializer
            {
                MaxJsonLength = int.MaxValue
            };

            return serializer.Serialize(result);
        }

        public string Calculate(string Aggregate)
        {
            var dt = TempData["dt"] as System.Data.DataTable;
            var result1 = new object();
            try
            {

                result1 = dt.Compute(Aggregate, "");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return result1.ToString();
        }

        public List<Dictionary<string, object>> ToJson(System.Data.DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }

            if (table.Rows.Count == 0)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, "");
                }
                parentRow.Add(childRow);
            }

            return parentRow;
        }

        public async Task<int> SaveQuery(int UserID, int KPI_ID, string Query, string Fields, string Table_View, string Type, Query_Parameter[] Parameters)
        {
            try
            {
                var item = await db.KPI_Item.FirstOrDefaultAsync(q => q.UserID == UserID && q.Id == KPI_ID);

                item.Query = Query;
                db.Entry(item).State = EntityState.Modified;
                await db.SaveChangesAsync();

                var query = await db.Query.FirstOrDefaultAsync(q => q.KPI_ID == item.Id);

                if (query != null)
                {
                    if (query != null)
                    {

                        db.Entry(query).State = EntityState.Deleted;
                        db.Query.Remove(query);
                    }
                    await db.SaveChangesAsync();

                    var _params = await db.Query_Parameter.Where(q => q.KPI_ID == item.Id).ToListAsync();
                    if (_params.Count() > 0)
                    {
                        foreach (var param in _params)
                        {
                            db.Entry(param).State = EntityState.Deleted;
                            db.Query_Parameter.Remove(param);
                        }
                    }
                    await db.SaveChangesAsync();
                }
                else
                {
                    var qq = new Query() { KPI_ID = item.Id, Fields = Fields, Table_View = Table_View, Type = Type };

                    db.Entry(qq).State = EntityState.Added;
                    db.Query.Add(qq);

                    await db.SaveChangesAsync();

                    foreach (var param in Parameters)
                    {
                        var p = new Query_Parameter() { KPI_ID = param.KPI_ID, Param_Name = param.Param_Name, Operator = param.Operator, Param_Value = param.Param_Value };
                        db.Entry(p).State = EntityState.Added;
                        db.Query_Parameter.Add(p);
                    }

                    await db.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            return -1;
        }

        public async Task<JsonResult> Filter(int Id)
        {
            var kpi_items = await db.KPI_Item.Where(q => q.UserID == Id && q.HasIbox).Select(q => new { q.Id, q.Title }).ToListAsync();

            return Json(kpi_items, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetQuery(int Id)
        {
            var kpi_item = await db.KPI_Item.FindAsync(Id);
            var query = await db.Query.FirstOrDefaultAsync(q => q.KPI_ID == Id);
            var _params = await db.Query_Parameter.Where(q => q.KPI_ID == Id).ToListAsync();

            return Json(new
            {
                kpi_item.Id,
                kpi_item.UserID,
                kpi_item.Title,
                kpi_item.Query,
                query.Fields,
                query.Table_View,
                query.Type,
                Parameters = _params
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
