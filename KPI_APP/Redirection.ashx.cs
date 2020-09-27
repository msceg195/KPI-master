using KPI_APP.Helper;
using KPI_APP.Models;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace KPI_APP
{
    public class Redirection : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            StringBuilder script = new StringBuilder();
            script.AppendLine("<script>");

            using (var db = new Context())
            {
                var email = db.Emails.FirstOrDefault(q =>
                           q.CreateDate.Day == DateTime.Now.Day &&
                           q.CreateDate.Month == DateTime.Now.Month &&
                           q.CreateDate.Year == DateTime.Now.Year &&
                           q.EmailType == 1);

                if (email == null)
                {
                    if ((DateTime.Today.Day == Settings.Close) ||
                        (DateTime.Today.Day >= Settings.Start && DateTime.Today.Day < Settings.Close) ||
                        (DateTime.Today.Day >= Settings.Start && DateTime.Today.Day < Settings.Close))
                    {
                        var url = ConfigurationManager.AppSettings["url"];

                        Process.Start("IExplore.exe", url + "/Reports/Progress");
                    }
                }
            }

            if (DateTime.Now.Day == 1) CopySheet();

            script.AppendLine("</script>");

            context.Response.Write(script);
        }

        private static void CopySheet()
        {
            using (var db = new Context())
            {
                var date = DateTime.Now.Day > 15 ? DateTime.Now : DateTime.Now.AddMonths(-1);
                var year = date.Year.ToString();
                var month = date.Month.ToString();

                var kpi = db.KPI.OrderByDescending(q => q.Id).FirstOrDefault();

                var kpi_new = new KPI();

                var kpi_items = kpi.KPI_Item.ToList();
                kpi_new.CreateDate = DateTime.Now;
                kpi_new.IsClose = false;
                kpi_new.Title = kpi.Title;
                kpi_new.KPI_Month = (int.Parse(kpi.KPI_Month) < 12) ? (int.Parse(kpi.KPI_Month) + 1).ToString() : "1";
                kpi_new.KPI_Year = (int.Parse(kpi.KPI_Month) < 12) ? kpi.KPI_Year : (int.Parse(kpi.KPI_Year) + 1).ToString();

                db.Entry(kpi_new).State = EntityState.Added;

                db.SaveChanges();

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
                    db.SaveChanges();

                    var _folderPath = Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/" + kpi_new.KPI_Year + "_" + kpi_new.KPI_Month));

                    var _date = date.AddMonths(-1);
                    var oldPath = Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/" + _date.Year + "_" + _date.Month));

                    File.Copy(oldPath.GetFiles()[0].FullName, _folderPath.FullName + @"\" + oldPath.GetFiles()[0].Name, false);

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


                        }
                    }
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}