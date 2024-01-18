using BTLClient_Server.EF;
using BTLClient_Server.Areas.Admin.Security;
using LTTH_UI_UX.Models;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace BTLClient_Server.Areas.Admin.Controllers
{
    [AuthorizationAdmin]
    public class AdminTrademarkController : Controller
    {
        // GET: Admin/AdminTrademark
        public ActionResult Index(int pageNum = 1, int pageSize = 7)
        {
            IEnumerable<ThuongHieu> lst = null;
            using (var DbContext = new WebBanHangEntities())
            {
                lst = GetListTraOrder().ToPagedList<ThuongHieu>(pageNum, pageSize);
            }
            return View(lst);
        }

        public IEnumerable<ThuongHieu> GetListTraOrder()
        {
            using (var DbContext = new WebBanHangEntities())
            {
                return (from s in DbContext.ThuongHieux orderby s.idThuongHieu descending select s).ToList();
            }
        }

        [HttpPost]
        public ActionResult Create(string ten)
        {
            User u = (User)Session["user"];
            ThuongHieu t = new ThuongHieu();
            t.tenThuongHieu = ten;
            t.idUser = u.idUser;
            using (var DbContext = new WebBanHangEntities())
            {
                DbContext.ThuongHieux.Add(t);//luu tren RAM
                DbContext.SaveChanges();//luu vao o dia
            }
            return Redirect("~/Admin/AdminTrademark/Index");
        }
        public ActionResult Edit(int id)
        {
            ThuongHieu k = new ThuongHieu();
            using (var DbContext = new WebBanHangEntities())
            {
                k = DbContext.ThuongHieux.Find(id);
            }
            return View(k);
        }
        [HttpPost]
        public ActionResult Edit(int id, ThuongHieu t)
        {
            User u = (User)Session["user"];
            t.idUser = u.idUser;
            if (ModelState.IsValid)
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    ThuongHieu k = DbContext.ThuongHieux.Find(id);
                    if (k != null)
                    {
                        k.tenThuongHieu = t.tenThuongHieu;
                        k.idUser = t.idUser;
                        DbContext.SaveChanges();//luu vao o dia
                    }
                }
                return RedirectToAction("Index", "AdminTrademark");
            }
            return PartialView(t);
        }
        public ActionResult Delete(int id)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                ThuongHieu pro = DbContext.ThuongHieux.Find(id);
                if (pro != null)
                {
                    DbContext.ThuongHieux.Remove(pro);
                    DbContext.SaveChanges();
                }
            }
            return RedirectToAction("Index", "AdminTrademark");
        }
    }
}