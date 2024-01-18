using BTLClient_Server.EF;
using BTLClient_Server.Areas.Admin.Models.DTO;
using BTLClient_Server.Areas.Admin.Security;
using LTTH_UI_UX.Models;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace BTLClient_Server.Areas.Admin.Controllers
{
    [AuthorizationAdmin]
    public class AdminNewsController : Controller
    {
        // GET: Admin/AdminNews
        public ActionResult Index(string keywords, string startTime, string endTime, int pageNum = 1, int pageSize = 7)
        {
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.keywords = keywords;
            if (startTime == null || startTime == "")
                startTime = new DateTime(2020, 11, 19).ToString("yyyy-MM-dd HH:mm:ss");
            if (endTime == null || endTime == "")
                endTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            if (keywords == null)
                keywords = "";

            IEnumerable<NewsDTO> lst = null;
            using (var DbContext = new WebBanHangEntities())
            {
                lst = DbContext.Database.SqlQuery<NewsDTO>(string.Format("getListNews N'{0}', '{1}', '{2}'",
                keywords, startTime, endTime)
                ).ToList<NewsDTO>().ToPagedList<NewsDTO>(pageNum, pageSize);
            }
            return View(lst);
        }
        [HttpPost]
        public ActionResult Index(FormCollection data)
        {
            if (data.Count > 0)
            {
                string[] ids = data["checkBoxId"].Split(new char[] { ',' });
                foreach (string id in ids)
                {
                    using (var DbContext = new WebBanHangEntities())
                    {
                        TinTuc pro = DbContext.TinTucs.Find(id);
                        if (pro != null)
                        {
                            DbContext.TinTucs.Remove(pro);
                            DbContext.SaveChanges();
                        }
                    }
                }
            }
            return Redirect("~/Admin/AdminNews/Index");
        }

        public ActionResult Create()
        {
            return View(new TinTuc());
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(TinTuc tmp, HttpPostedFileBase photo)
        {
            if (ModelState.IsValid)
            {
                if (photo != null && photo.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Theme/Image/"), System.IO.Path.GetFileName(photo.FileName));
                    photo.SaveAs(path);
                    tmp.anh = photo.FileName;
                }
                User u = (User)Session["user"];
                tmp.idUser = u.idUser;
                tmp.ngayTao = DateTime.Now;

                string json = JsonConvert.SerializeObject(tmp);
                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                using (var DbContext = new WebBanHangEntities())
                {
                    DbContext.TinTucs.Add(tmp);//luu tren RAM
                    DbContext.SaveChanges();//luu vao o dia
                }
            }
            return Redirect("~/Admin/AdminNews/Index");
        }

        public ActionResult Edit(int id)
        {
            TinTuc t = new TinTuc();
            using (var DbContext = new WebBanHangEntities())
            {
                t = DbContext.TinTucs.Find(id);
            }
            return View(t);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(TinTuc tmp, HttpPostedFileBase photo)
        {
            if (ModelState.IsValid)
            {
                if (photo != null && photo.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Theme/Image/"), System.IO.Path.GetFileName(photo.FileName));
                    photo.SaveAs(path);
                    tmp.anh = photo.FileName;
                }
                tmp.ngayTao = DateTime.Now;
                User u = (User)Session["user"];
                tmp.idUser = u.idUser;
                using (var DbContext = new WebBanHangEntities())
                {
                    TinTuc news = DbContext.TinTucs.Find(tmp.idTin);
                    if (news != null)
                    {
                        news.tieuDe = tmp.tieuDe;
                        news.noiDung = tmp.noiDung;
                        news.anh = tmp.anh;
                        news.idUser = tmp.idUser;
                        news.ngayTao = tmp.ngayTao;
                        DbContext.SaveChanges();//luu vao o dia
                    }
                }
                return RedirectToAction("Index", "AdminNews");
            }
            return View(tmp);
        }

        public ActionResult Delete(int id)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                TinTuc pro = DbContext.TinTucs.Find(id);
                if (pro != null)
                {
                    DbContext.TinTucs.Remove(pro);
                    DbContext.SaveChanges();
                }
            }
            return Redirect("~/Admin/AdminNews/Index");
        }
    }
}