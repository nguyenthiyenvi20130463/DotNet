using BTLClient_Server.EF;
using LTTH_UI_UX.Models;
using LTTHAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace BTLClient_Server.Controllers
{
    public class LoginController : Controller
    {
        public JsonResult DangNhapKhachHang(string txtTenDangNhap,string txtMatKhau)
        {
            Message message = new Message();
            try
            {
                LoginInfor login = new LoginInfor();
                using (var DbContext = new WebBanHangEntities())
                {
                    var readJob = DbContext.KhachHangs.Where(e => e.tenDangNhap == txtTenDangNhap && e.matKhau == txtMatKhau).FirstOrDefault();
                    if(readJob==null)
                    {
                        message.Icon = "error";
                        message.Title = "Đăng nhập thất bại";
                        return Json(message, JsonRequestBehavior.AllowGet);
                    }    
                    login.tenDangNhap = readJob.tenDangNhap;
                    login.IdKhachHang = readJob.idKhach;
                    login.hoTen = readJob.hoTen;
                    login.email = readJob.email;
                    Session["Login"] = login;
                }
                message.Icon = "success";
                message.Title = "Đăng nhập thành công";
                message.Data = login.tenDangNhap;
            }
            catch(Exception e)
            {
                message.Icon = "error";
                message.Title = "Có lỗi: " + e.Message;
            }
            
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DangNhap()
        {
            LoginInfor login = new LoginInfor();
            login = Session["Login"] as LoginInfor;
            ViewBag.login = login;
            return View();
        }
        public JsonResult KiemTraDangNhap()
        {
            Message message = new Message();
            LoginInfor login = new LoginInfor();
            login = Session["Login"] as LoginInfor;
            message.Icon = "success";
            if (login==null)
            {
                message.Icon = "error";
                message.Title = "Ban cần đăng nhập để thực hiện chức năng này!";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DangKyTaiKhoan(string txtemail, string txtSDT, string txtTaiKhoan, string txtpassword, string txtHoTen, string txtNgaySinh, string GioiTinh,string txtdiachi="")
        {
            Message message = new Message();
            try
            {
                
                using (var DbContext = new WebBanHangEntities())
                {
                    var checkKH= DbContext.KhachHangs.Where(e => e.email == txtemail).FirstOrDefault(); ;
                    if (checkKH != null)
                    {
                        message.Icon = "error";
                        message.Title = "Email đã được đăng ký!";
                        return Json(message, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        KhachHang khach_Hang = new KhachHang();
                        khach_Hang.hoTen = txtHoTen;
                        khach_Hang.diaChi = txtdiachi;
                        khach_Hang.email = txtemail;
                        khach_Hang.sdt = txtSDT;
                        khach_Hang.tenDangNhap = txtTaiKhoan;
                        khach_Hang.matKhau = txtpassword;
                        DbContext.KhachHangs.Add(khach_Hang);
                        DbContext.SaveChanges();
                        message.Icon = "success";
                        message.Title = "Lưu khách hàng thành công!!";
                    }
                }
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                message.Icon = "error";
                message.Title = "Có lỗi: " + e.Message;
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DangXuat()
    {
        Message message = new Message();
        try
        {
            // Clear the session to log out the user
            Session["Login"] = null;
            message.Icon = "success";
            message.Title = "Đăng xuất thành công";
        }
        catch (Exception e)
        {
            message.Icon = "error";
            message.Title = "Có lỗi: " + e.Message;
        }

        return Json(message, JsonRequestBehavior.AllowGet);
    }
    }
}