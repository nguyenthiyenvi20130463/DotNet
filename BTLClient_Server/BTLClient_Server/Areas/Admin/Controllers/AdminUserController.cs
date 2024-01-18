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
    public class AdminUserController : Controller
    {
        [IsAdmin]
        // GET: Admin/AdminUser
        public ActionResult Index(string tendn, string quyen, int pageNum = 1, int pageSize = 6)
        {
            ViewBag.tendn = tendn;
            ViewBag.quyen = quyen;
            
            IEnumerable<User> lst = null;
            using (var DbContext = new WebBanHangEntities())
            {
                lst = GetListUser(tendn, quyen).ToPagedList<User>(pageNum, pageSize);
            }
            return View(lst);
        }

        public IEnumerable<User> GetListUser(string tendn, string quyen)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                var lst = (from s in DbContext.Users
                           where (tendn == null || s.tenDangNhap.Contains(tendn))
                                    && (quyen == null || s.quyen.ToString().Equals(quyen))
                           orderby s.idUser descending
                           select s)
                .ToList();
                return lst;
            }
        }

        [HttpPost]
        public ActionResult Create(string username, string password, string adminType)
        {
            User u = new User();
            u.tenDangNhap = username;
            u.matKhau = password;
            u.quyen = int.Parse(adminType);
            using (var DbContext = new WebBanHangEntities())
            {
                DbContext.Users.Add(u);//luu tren RAM
                DbContext.SaveChanges();//luu vao o dia
            }
            return RedirectToAction("Index", "AdminUser");
        }

        public ActionResult Edit(int id)
        {
            User k = new User();
            using (var DbContext = new WebBanHangEntities())
            {
                k = DbContext.Users.Find(id);
            }
            return View(k);
        }
        [HttpPost]
        public ActionResult Edit(int id, string passwordEdit, string adminTypeEdit, string tt)
        {
            User u = new User();
            u.matKhau = passwordEdit;
            u.quyen = int.Parse(adminTypeEdit);
            if (tt == "true")
                u.trangThai = true;
            else
                u.trangThai = false;
            if (ModelState.IsValid)
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    if(passwordEdit == "")
                    {
                        User k = DbContext.Users.Find(id);
                        if (k != null)
                        {
                            k.quyen = u.quyen;
                            k.trangThai = u.trangThai;
                            DbContext.SaveChanges();//luu vao o dia
                        }
                    }
                    else
                    {
                        User k = DbContext.Users.Find(id);
                        if (k != null)
                        {
                            k.matKhau = u.matKhau;
                            k.quyen = u.quyen;
                            k.trangThai = u.trangThai;
                            DbContext.SaveChanges();//luu vao o dia
                        }
                    }
                }
                return RedirectToAction("Index", "AdminUser");
            }
            return PartialView(u);
        }

        [HttpGet]
        public ActionResult Profile()
        {
            User u = (User)Session["user"];
            return View(u);
        }
        [HttpPost]
        public ActionResult Profile(string ten, string usernameProfile, string passwordEditProfile)
        {
            User u = (User)Session["user"];
            u.hoTen = ten;
            u.tenDangNhap = usernameProfile;
            if (passwordEditProfile != "")
                u.matKhau = passwordEditProfile;
            if (ModelState.IsValid)
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    User k = DbContext.Users.Find(u.idUser);
                    if (k != null)
                    {
                        k.hoTen = u.hoTen;
                        k.tenDangNhap = u.tenDangNhap;
                        k.matKhau = u.matKhau;
                        DbContext.SaveChanges();//luu vao o dia
                    }
                }
            }
            return RedirectToAction("Profile", "AdminUser");
        }

        public JsonResult CheckPass(string passwordProfile)
        {
            User u = (User)Session["user"];
            bool res = false;
            if (u.matKhau == passwordProfile)
                res = true;
            return Json(new
            {
                status = res
            }, JsonRequestBehavior.AllowGet);
        }
    }
}