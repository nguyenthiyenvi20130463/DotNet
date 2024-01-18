using BTLClient_Server.EF;
using LTTH_UI_UX.Models;
using LTTHAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTTH_UI_UX.Controllers
{
    public class GioiThieuController : Controller
    {
        // GET: GioiThieu
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Account()
        {
            LoginInfor login = Session["Login"] as LoginInfor;
            using (var DbContext = new WebBanHangEntities())
            {
                if (login != null)
                {
                    ViewBag.khachhang = DbContext.KhachHangs.Where(e => e.idKhach == login.IdKhachHang).FirstOrDefault();
                }
            }
            return View();
        }

        public JsonResult AccountEdit(string txtSDT, string txtTen, string txtTaiKhoan, string txtPass, string txtEmail, string txtDiaChi, string idKhachHang)
        {
            Message message = new Message();
            try
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    int id_KhachHang = int.Parse(idKhachHang);
                    var CheckMail = DbContext.KhachHangs.Where(e => e.email.Trim().ToLower() == txtEmail.Trim().ToLower()&&e.idKhach!=id_KhachHang).FirstOrDefault();
                    if (CheckMail != null)
                    {
                        message.Title = "Email đã được đăng ký!";
                        message.Icon = "error";
                        return Json(message, JsonRequestBehavior.AllowGet);
                    }
                    var khachhang = DbContext.KhachHangs.Where(e => e.idKhach == id_KhachHang).FirstOrDefault();
                    if (khachhang != null)
                    {
                        khachhang.sdt = txtSDT;
                        khachhang.hoTen = txtTen;
                        khachhang.diaChi = txtDiaChi;
                        khachhang.email = txtEmail;
                        khachhang.matKhau = txtPass;
                        khachhang.tenDangNhap = txtTaiKhoan;
                        DbContext.SaveChanges();
                    }
                    message.Icon = "success";
                    message.Title = "Thay đổi thông tin tài khoản thành công";
                    message.Data = txtTaiKhoan;
                }
            }
            catch (Exception e)
            {
                message.Icon = "error";
                message.Title = "Có lỗi : " + e.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
    }
}