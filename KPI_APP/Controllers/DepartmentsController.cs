using KPI_APP.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KPI_APP.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private Context db = new Context();

        public async Task<ActionResult> Index()
        {
            ViewBag.Department = await db.Excel_Department.ToListAsync();

            return View(await db.Department.Include(k => k.AspNetUsers).ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Department.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,CreateDate")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Department.Add(department);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(department);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Department.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,CreateDate")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Department.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Department department = await db.Department.FindAsync(id);
            db.Department.Remove(department);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Save(int? id, bool? isPIC)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            AspNetUsers user = await db.AspNetUsers.FirstOrDefaultAsync(q => q.UserID == id);
            db.Entry(user).State = EntityState.Modified;

            if (user == null)
                return HttpNotFound();
            else
                user.IsPIC = isPIC;

            await db.SaveChangesAsync();

            return Json("Done", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SaveParent(int? parentid, int? depid)
        {
            if (parentid == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var department = await db.Department.FirstOrDefaultAsync(q => q.ID == depid);
            if (parentid.HasValue)
            {
                department.ParentID = parentid;
                db.Entry(department).State = EntityState.Modified;
            }

            await db.SaveChangesAsync();

            return Json("Done", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Users()
        {
            ViewBag.Users = await db.AspNetUsers.Where(q => q.IsPIC.HasValue && q.IsPIC == true).ToListAsync();
            ViewBag.Departments = await db.Excel_Department.ToListAsync();

            var date = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year;
            var month = date.Month;

            var kPI_Item = await db.KPI_Item
                .Where(q =>
                       q.KPI.KPI_Month == month + ""
                    && q.KPI.KPI_Year == year + "")
                .ToListAsync();

            ViewBag.Date = date;

            return View(kPI_Item);
        }

        public async Task<ActionResult> SaveUser(int Itemid, int UserId)
        {
            var item = await db.KPI_Item.FirstOrDefaultAsync(q => q.Id == Itemid);
            //var user = await db.AspNetUsers.FirstOrDefaultAsync(q => q.Id.Contains(UserId));

            if (item == null)
                return HttpNotFound();
            else
            {
                db.Entry(item).State = EntityState.Modified;

                item.UserID = UserId;
            }
            await db.SaveChangesAsync();

            return Json("Done", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CloseItem(int Itemid, bool IsClose)
        {
            var item = await db.KPI_Item.FirstOrDefaultAsync(q => q.Id == Itemid);
            //var user = await db.AspNetUsers.FirstOrDefaultAsync(q => q.Id.Contains(UserId));

            if (item == null)
                return HttpNotFound();
            else
            {
                db.Entry(item).State = EntityState.Modified;

                item.IsClose = IsClose;
            }
            await db.SaveChangesAsync();

            return Json("Done", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AssignIbox(int Itemid, int UserId, bool HasIbox)
        {
            var item = await db.KPI_Item.FirstOrDefaultAsync(q => q.Id == Itemid);

            if (item == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.Entry(item).State = EntityState.Modified;

                item.HasIbox = HasIbox;
            }
            await db.SaveChangesAsync();

            return Json("Done", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Productivity(int Itemid, int UserId, bool HasProductivity)
        {
            var item = await db.KPI_Item.FirstOrDefaultAsync(q => q.Id == Itemid);

            if (item == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.Entry(item).State = EntityState.Modified;

                item.HasProductivity = HasProductivity;
            }
            await db.SaveChangesAsync();

            return Json("Done", JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Send()
        {
            var date = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year;
            var month = date.Month;

            if (DateTime.Now.Day == 14)
            {
                StringBuilder body = null;

                using (Context db = new Context())
                {
                    var users = await db.AspNetUsers.Where(q => q.IsPIC == true).ToListAsync();

                    foreach (var user in users)
                    {
                        var kPI_Items = await db.KPI_Item.Where(q =>
                            q.UserID == user.UserID
                         && q.KPI.KPI_Month == month + ""
                         && q.KPI.KPI_Year == year + ""
                         && !q.IsClose).ToListAsync();

                        if (kPI_Items.Count > 0)
                        {
                            body = new StringBuilder("<p>Please fill following points </p>");

                            body.AppendLine("<ol>");
                            foreach (var item in kPI_Items)
                            {
                                body.AppendLine("<li>" + item.Title + "</li>");
                            }

                            body.AppendLine("</ol>");

                            string _Email_Text = System.IO.File.ReadAllText(Server.MapPath("~/Emails/Remain Items.html"));
                            _Email_Text = _Email_Text.Replace("{BODY}", body.ToString());
                            _Email_Text = _Email_Text.Replace("{TITLE}", "KPI " + date.Month + " - " + date.Year);

                            await Helper.Mail.SendEmail(
                                  Messages.KPI_Assigned_Subject,
                                  _Email_Text,
                                  Messages.KPI_Admin_Email,
                                  user.Email,
                                  Messages.KPI_CC);
                        }
                    }
                }
            }

            return Json("done", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Manage()
        {
            ViewBag.Users = await db.AspNetUsers.Where(q => q.IsPIC.HasValue && q.IsPIC == true).ToListAsync();
            ViewBag.Departments = await db.Excel_Department.ToListAsync();

            var date = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year;
            var month = date.Month;

            var kPI_Item = await db.KPI_Item
                .Where(q =>
                       q.KPI.KPI_Month == month + ""
                    && q.KPI.KPI_Year == year + "")
                .ToListAsync();

            ViewBag.Date = date;

            return View(kPI_Item);
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
