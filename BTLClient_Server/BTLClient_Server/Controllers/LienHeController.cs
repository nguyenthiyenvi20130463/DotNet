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

namespace LTTH_UI_UX.Controllers
{
    public class LienHeController : Controller
    {
        // GET: LienHe
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GuiFromLienHe(string txtHo, string txtTen, string txtEmail, string txtSDT, string txtNoiDung)
        {
            Message message = new Message();
            try
            {
                LoginInfor login = Session["Login"] as LoginInfor;
                string idkhach = "";
                if (login != null)
                {
                    idkhach = login.IdKhachHang.ToString();
                }
                PhanHoi phanhoi = new PhanHoi();
                phanhoi.txtHo = txtHo;
                phanhoi.txtTen = txtTen;
                phanhoi.txtEmail = txtEmail;
                phanhoi.txtSDT = txtSDT;
                phanhoi.txtNoiDung = txtNoiDung;
                phanhoi.idkhach = idkhach;
                using (var DbContext = new WebBanHangEntities())
                {
                    LienHe lienhe = new LienHe();
                    lienhe.Ho = phanhoi.txtHo;
                    lienhe.Ten = phanhoi.txtTen;
                    lienhe.Email = phanhoi.txtEmail;
                    lienhe.SDT = phanhoi.txtSDT;
                    lienhe.NoiDung = phanhoi.txtNoiDung;
                    lienhe.NgayTao = DateTime.Now;
                    if (!string.IsNullOrEmpty(phanhoi.idkhach))
                    {
                        lienhe.idKhach = int.Parse(phanhoi.idkhach);
                    }
                    DbContext.LienHes.Add(lienhe);
                    DbContext.SaveChanges();
                    message.Icon = "success";
                    message.Title = "Gửi liên hệ thành công!";
                }
            }
            catch(Exception e)
            {
                message.Icon = "error";
                message.Title = "Có lỗi : " + e.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
    }
}