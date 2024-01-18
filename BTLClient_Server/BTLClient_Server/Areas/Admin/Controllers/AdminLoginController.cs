using BTLClient_Server.EF;
using LTTH_UI_UX.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace BTLClient_Server.Areas.Admin.Controllers
{
    public class AdminLoginController : Controller
    {
        // GET: Admin/AdminLogin
        public ActionResult Index()
       {
            User user = new User();
            if (Session["user"] != null)
                return RedirectToAction("Index", "AdminHome");
            return View(user);
        }
        [HttpPost]
        public ActionResult Index(User model)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                User user = DbContext.Users.Where(e => e.tenDangNhap == model.tenDangNhap && e.matKhau == model.matKhau && e.trangThai == true).FirstOrDefault();
                if (user != null)
                {
                    Session["user"] = user;
                    Session["username"] = user.hoTen;
                    Session["isAdmin"] = user.quyen;
                    return RedirectToAction("Index", "AdminHome");
                }
            }
            ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng!");
            return View(model);
        }

        public ActionResult Logout()
        {
            Session["user"] = null;
            return RedirectToAction("Index", "AdminLogin");
        }

    }
}