using BTLClient_Server.EF;
using LTTH_UI_UX.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace LTTH_UI_UX.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        public ActionResult Index()
        {
            using (var DbContext = new WebBanHangEntities())
            {
                ViewBag.SoLuongTinTuc = DbContext.TinTucs.OrderBy(e => e.ngayTao).ToList().Count();
                ViewBag.lstTinTuc = DbContext.TinTucs.OrderBy(e => e.ngayTao).Skip(0 * 4).Take(4).ToList();
            }
            return View();
        }

        public ActionResult XemThemTinTuc(string ViTri,string size="8")
        {
            using (var DbContext = new WebBanHangEntities())
            {
                int Page = int.Parse(ViTri);
                int Size = int.Parse(size);
                ViewBag.lstXemThemTinTuc = DbContext.TinTucs.OrderBy(e => e.ngayTao).Skip(Page * 4).Take(Size).ToList();
            }
            return View();
        }

        public ActionResult TinTucChiTiet(string idTinTuc)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                int id_Tin = int.Parse(idTinTuc);
                ViewBag.ChiTietTinTuc = DbContext.TinTucs.Where(e => e.idTin == id_Tin).FirstOrDefault();
                ViewBag.lstRandomTinTuc = DbContext.TinTucs.OrderBy(x => Guid.NewGuid()).Skip(0).Take(3).ToList();
            }
            return View();
        }
    }
}