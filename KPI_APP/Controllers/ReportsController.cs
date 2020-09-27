using KPI_APP.Helper;
using KPI_APP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KPI_APP.Controllers
{
    public class ReportsController : Controller
    {
        Context db = new Context();

        public async Task<ActionResult> Index()
        {
            var date = DateTime.Now.Day > Settings.Close ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year.ToString();
            var month = date.Month.ToString();

            ApplicationUser user = Session["user"] as ApplicationUser;
            var dep_id = user.DepartmentID;
            var dep = await db.Department.FindAsync(dep_id);
            int? parentID = -1;

            if (dep != null) parentID = dep.ParentID;

            var kPI_Item = await db.KPI_Item
               .Where(q =>
                      q.DepartmentID == parentID
                   && q.KPI.KPI_Month == month.ToString()
                   && q.KPI.KPI_Year == year.ToString())
               .ToListAsync();

            return View(kPI_Item);
        }

        public ActionResult Progress() => View();

        public ActionResult Productivity() => View();

        public async Task<JsonResult> KPI_Productivity()
        {
            var date = DateTime.Now.Day > Settings.Close ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year.ToString();
            var month = date.Month.ToString();

            var query = await (from item in db.KPI_Item
                               where item.HasProductivity
                               group item by new
                               {
                                   item.Remarks,
                                   item.Title,
                                   item.Jan,
                                   item.Feb,
                                   item.Mar,
                                   item.Apr,
                                   item.May,
                                   item.Jun,
                                   item.Jul,
                                   item.Aug,
                                   item.Sep,
                                   item.Oct,
                                   item.Nov,
                                   item.Dec
                               } into g
                               let p = new
                               {
                                   g.Key.Remarks,
                                   g.Key.Title,
                                   Jan = g.Key.Jan ?? "0",
                                   Feb = g.Key.Feb ?? "0",
                                   Mar = g.Key.Mar ?? "0",
                                   Apr = g.Key.Apr ?? "0",
                                   May = g.Key.May ?? "0",
                                   Jun = g.Key.Jun ?? "0",
                                   Jul = g.Key.Jul ?? "0",
                                   Aug = g.Key.Aug ?? "0",
                                   Sep = g.Key.Sep ?? "0",
                                   Oct = g.Key.Oct ?? "0",
                                   Nov = g.Key.Nov ?? "0",
                                   Dec = g.Key.Dec ?? "0"
                               }
                               select p).ToListAsync();
            //try
            //{
            var result = query.GroupBy(l => l.Title)
                                            .Select(g => new
                                            {
                                                g.FirstOrDefault().Remarks,
                                                Jan = g.Select(q => q.Jan).FirstOrDefault(c => c != "0") ?? "0",
                                                Feb = g.Select(q => q.Feb).FirstOrDefault(c => c != "0") ?? "0",
                                                Mar = g.Select(q => q.Mar).FirstOrDefault(c => c != "0") ?? "0",
                                                Apr = g.Select(q => q.Apr).FirstOrDefault(c => c != "0") ?? "0",
                                                May = g.Select(q => q.May).FirstOrDefault(c => c != "0") ?? "0",
                                                Jun = g.Select(q => q.Jun).FirstOrDefault(c => c != "0") ?? "0",
                                                Jul = g.Select(q => q.Jul).FirstOrDefault(c => c != "0") ?? "0",
                                                Aug = g.Select(q => q.Aug).FirstOrDefault(c => c != "0") ?? "0",
                                                Sep = g.Select(q => q.Sep).FirstOrDefault(c => c != "0") ?? "0",
                                                Oct = g.Select(q => q.Oct).FirstOrDefault(c => c != "0") ?? "0",
                                                Nov = g.Select(q => q.Nov).FirstOrDefault(c => c != "0") ?? "0",
                                                Dec = g.Select(q => q.Dec).FirstOrDefault(c => c != "0") ?? "0",
                                            }).ToList();
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}


            return Json(data: result, behavior: JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> CurrentProgress()
        {
            var date = DateTime.Now.Day > Settings.Close ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year.ToString();
            var month = date.Month.ToString();

            var query = await (from user in db.AspNetUsers
                               join item in db.KPI_Item
                               on user.UserID equals item.UserID into ui
                               from x in ui.DefaultIfEmpty()
                               where x.KPI.KPI_Month == month && x.KPI.KPI_Year == year
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
                done = KPI_Done(q.id, int.Parse(month))
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

        public int KPI_Empty(int userid, int month, int kpi)
        {
            var result = 0;
            switch (month)
            {
                case 1:
                    result = db.KPI_Item.Count(q => q.KPI_ID == kpi && q.UserID == userid && string.IsNullOrEmpty(q.Jan));
                    break;
                case 2:
                    result = db.KPI_Item.Count(q => q.KPI_ID == kpi && q.UserID == userid && string.IsNullOrEmpty(q.Feb));
                    break;
                case 3:
                    result = db.KPI_Item.Count(q => q.KPI_ID == kpi && q.UserID == userid && string.IsNullOrEmpty(q.Mar));
                    break;
                case 4:
                    result = db.KPI_Item.Count(q => q.KPI_ID == kpi && q.UserID == userid && string.IsNullOrEmpty(q.Apr));
                    break;
                case 5:
                    result = db.KPI_Item.Count(q => q.KPI_ID == kpi && q.UserID == userid && string.IsNullOrEmpty(q.May));
                    break;
                case 6:
                    result = db.KPI_Item.Count(q => q.KPI_ID == kpi && q.UserID == userid && string.IsNullOrEmpty(q.Jun));
                    break;
                case 7:
                    result = db.KPI_Item.Count(q => q.KPI_ID == kpi && q.UserID == userid && string.IsNullOrEmpty(q.Jul));
                    break;
                case 8:
                    result = db.KPI_Item.Count(q => q.KPI_ID == kpi && q.UserID == userid && string.IsNullOrEmpty(q.Aug));
                    break;
                case 9:
                    result = db.KPI_Item.Count(q => q.KPI_ID == kpi && q.UserID == userid && string.IsNullOrEmpty(q.Sep));
                    break;
                case 10:
                    result = db.KPI_Item.Count(q => q.KPI_ID == kpi && q.UserID == userid && string.IsNullOrEmpty(q.Oct));
                    break;
                case 11:
                    result = db.KPI_Item.Count(q => q.KPI_ID == kpi && q.UserID == userid && string.IsNullOrEmpty(q.Nov));
                    break;
                case 12:
                    result = db.KPI_Item.Count(q => q.KPI_ID == kpi && q.UserID == userid && string.IsNullOrEmpty(q.Dec));
                    break;
                default:
                    break;
            }
            return result;
        }

        [HttpPost]
        public async Task<JsonResult> Send(string base64String)
        {
            DateTime date = DateTime.Now.Day > Settings.Close ? DateTime.Now : DateTime.Now.AddMonths(-1);
            var year = date.Year.ToString();
            var month = date.Month.ToString();
            string _Email_Text = System.IO.File.ReadAllText(Server.MapPath("~/Emails/Progress.html"));

            var split = base64String.Split(',')[1];
            var image = Base64ToImage(split);
            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Jpeg);
            stream.Position = 0;

            var Attachment = new Attachment(stream, "chart_image.png", "image/png");

            var adminemail = Settings.Email;
            var cc = Settings.CC;

            var emails = await db.AspNetUsers.ToListAsync();

            string to = "";
            foreach (var item in emails.Where(q => q.IsPIC == true).Select(q => q.Email)) to += item + ", ";

            var allpoints = await db.KPI_Item.Where(q => q.KPI.KPI_Month == month && q.KPI.KPI_Year == year).ToListAsync();

            var points = allpoints.Where(q => !q.IsClose).ToList();


            var body = new StringBuilder();
            if (DateTime.Today.Day == Settings.Close)
            {
                var myemail3 = await db.Emails.FirstOrDefaultAsync(q =>
                          q.CreateDate.Day == Settings.Close &&
                          q.CreateDate.Month == DateTime.Now.Month &&
                          q.CreateDate.Year == DateTime.Now.Year &&
                          q.EmailType == 3);

                if (myemail3 == null)
                {
                    if (points.Count > 0)
                    {
                        body = new StringBuilder("<ul>");

                        var query = await (from user in db.AspNetUsers
                                           join item in db.KPI_Item
                                           on user.UserID equals item.UserID into ui
                                           from x in ui.DefaultIfEmpty()
                                           where x.KPI.KPI_Month == month && x.KPI.KPI_Year == year
                                           group user by new
                                           {
                                               x.KPI_ID,
                                               user.UserID,
                                               user.Department.Name
                                           } into g
                                           select new
                                           {
                                               g.Key.KPI_ID,
                                               g.Key.UserID,
                                               Department = g.Key.Name,
                                               Count = g.Count()
                                           }).ToListAsync();

                        var query1 = await (from user in db.AspNetUsers
                                            join item in db.KPI_Item
                                            on user.UserID equals item.UserID into ui
                                            from x in ui.DefaultIfEmpty()
                                            where x.KPI.KPI_Month == month && x.KPI.KPI_Year == year
                                            group user by new
                                            {
                                                x.KPI_ID,
                                                user.DepartmentID,
                                                user.Department.Name
                                            } into g
                                            select new
                                            {
                                                Department = g.Key.Name,
                                                Count = g.Count()
                                            }).ToListAsync();

                        var query2 = query.Select(q => new
                        {
                            q.Department,
                            done = KPI_Empty(q.UserID, int.Parse(month), q.KPI_ID)
                        }).Where(q => q.done > 0).ToList();

                        var result = query2
                                     .GroupBy(l => l.Department)
                                     .Select(cl => new
                                     {
                                         cl.FirstOrDefault().Department,
                                         done = cl.Sum(c => c.done).ToString(),
                                     }).ToList();


                        for (int i = 0; i < result.Count(); i++)
                        {
                            body.Append("<li>" + result[i].Department + "    (" + result[i].done + ") of " + query1.FirstOrDefault(q => q.Department == result[i].Department).Count + "</li>");
                        }

                        body.Append("</ul>");

                        _Email_Text = _Email_Text.Replace("{BODY}", body.ToString());
                        _Email_Text = _Email_Text.Replace("{TITLE}", Messages.KPI_Not_Closed);

                        await Mail.SendEmail(
                            Messages.KPI_Assigned_Subject,
                            _Email_Text,
                            adminemail,
                            to.Trim(),
                           "mohamed.ghallab@msc.com, amr.mahmoud@msc.com, " + Settings.CC.Trim() + ", ",
                            Attachment, 3);
                    }
                }
            }

            if (DateTime.Today.Day >= Settings.Start && DateTime.Today.Day < Settings.Close)
            {
                var kpi = await db.KPI.FirstOrDefaultAsync(q => q.KPI_Month == date.Month.ToString() && q.KPI_Year == date.Year.ToString());

                var myemail2 = await db.Emails.FirstOrDefaultAsync(q =>
                         q.CreateDate.Day == DateTime.Now.Day &&
                         q.CreateDate.Month == DateTime.Now.Month &&
                         q.CreateDate.Year == DateTime.Now.Year &&
                         q.EmailType == 2);

                if (myemail2 == null && kpi.IsClose == false)
                {
                    if (points.Count == 0)
                    {
                        body = new StringBuilder("<ul>");

                        body.Append("<ul>");
                        var departments = await db.Department.ToListAsync();

                        foreach (var department in departments)
                        {
                            var dep_points = points.Where(q => q.DepartmentID == department.ParentID).ToList();
                            if (dep_points.Any()) body.Append("<li>" + department.Name + "    (" + dep_points.Count() + ") of " + allpoints.Count(q => q.DepartmentID == department.ParentID) + "</li>");
                        }

                        body.Append("</ul>");

                        _Email_Text = _Email_Text.Replace("{BODY}", body.ToString());
                        _Email_Text = _Email_Text.Replace("{TITLE}", Messages.KPI_Closed);

                        await Mail.SendEmail(
                            Messages.KPI_Assigned_Subject,
                           _Email_Text,
                            adminemail,
                            to,
                            "mohamed.ghallab@msc.com, amr.mahmoud@msc.com, " + Settings.CC + ", ",
                            Attachment, 2);

                        kpi.IsClose = true;
                        await db.SaveChangesAsync();
                    }
                }
            }

            if (DateTime.Today.Day >= Settings.Start && DateTime.Today.Day < Settings.Close)
            {
                var myemail1 = await db.Emails.FirstOrDefaultAsync(q =>
                         q.CreateDate.Day == DateTime.Now.Day &&
                         q.CreateDate.Month == DateTime.Now.Month &&
                         q.CreateDate.Year == DateTime.Now.Year &&
                         q.EmailType == 1);

                if (myemail1 == null)
                {
                    var days = Math.Abs(date.Subtract(new DateTime(date.Year, date.Month, Settings.Close)).Days);

                    _Email_Text = _Email_Text.Replace("{BODY}", body.ToString());
                    _Email_Text = _Email_Text.Replace("{TITLE}",
                     days == 0 ? "Final Reminder" : (days.ToString() + " Days Remaining"));

                    await Mail.SendEmail(
                        Messages.KPI_Assigned_Subject,
                        _Email_Text,
                        adminemail,
                        to,
                        "mohamed.ghallab@msc.com, amr.mahmoud@msc.com, " + Settings.CC + ", ",
                        Attachment, 1);
                }
            }

            StringBuilder sb = new StringBuilder(" \n\r ");
            sb.Append("Sent " + DateTime.Now.ToString() + " \n\r ");
            System.IO.File.AppendAllText(Server.MapPath("~/") + "log.txt", sb.ToString());
            sb.Clear();

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

        public async Task<ActionResult> Emails() => View(await new Context().Emails.ToListAsync());
    }
}
