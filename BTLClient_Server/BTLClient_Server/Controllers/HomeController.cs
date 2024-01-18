using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using BTLClient_Server.EF;
using LTTHAPI.Models;
using LTTH_UI_UX.Models;

namespace LTTH_UI_UX.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var DbContext = new WebBanHangEntities())
            {

                var lstSanPham_ = DbContext.SanPhams.OrderBy(e => e.ten).ToList();
                var lstSanPham = lstSanPham_.Where(e => e.trangThai == 1 || e.trangThai == 2 || e.trangThai == 5).ToList();
                var lst_SanPham = lstSanPham_.OrderBy(e => e.ten).Skip(0 * 8).Take(8).ToList();

                ViewBag.lstDanhSachSanPham = lst_SanPham;
                ViewBag.lstSanPhamNoiBat = lstSanPham;
                ViewBag.SoLuongSanPham = lstSanPham_.Count;
            }
            return View();
        }

        public ActionResult XemThemSanPham(string page, string size = "8")
        {
            using (var DbContext = new WebBanHangEntities())
            {
                int Page = int.Parse(page);
                int Size = int.Parse(size);
                ViewBag.lstDanhSachSanPham = DbContext.SanPhams.OrderBy(e => e.ten).Skip(Page * 8).Take(Size).ToList();
            }
            return View();
        }

        public ActionResult XemNhanhSanPham(string IdSanPham)
        {
            using (var DbContext = new WebBanHangEntities())
            {

                int IdSan_Pham = int.Parse(IdSanPham);
                var sanpham = DbContext.SanPhams.Where(e => e.idSp == IdSan_Pham).FirstOrDefault();
                var ThuongHieu = DbContext.ThuongHieux.Where(e => e.idThuongHieu == sanpham.idThuongHieu).FirstOrDefault();
                ViewBag.tenThuongHieu = ThuongHieu.tenThuongHieu;
                ViewBag.IdSanPham = sanpham;
                ViewBag.lstAnhSanPham = DbContext.Anhs.Where(e => e.idSp == IdSan_Pham).ToList();
            }
            return View();
        }

        public ActionResult DanhSachThuongHieu()
        {
            using (var DbContext = new WebBanHangEntities())
            {
                ViewBag.lstThuongHieu = DbContext.ThuongHieux.ToList();
            }
            return View();
        }

        public ActionResult ChiTietSanPham(string IdSanPham)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                ViewBag.IdSanPham = IdSanPham;
                int IdSan_Pham = int.Parse(IdSanPham);
                var sanpham = DbContext.SanPhams.Where(e => e.idSp == IdSan_Pham).FirstOrDefault();
                var ThuongHieu = DbContext.ThuongHieux.Where(e => e.idThuongHieu == sanpham.idThuongHieu).FirstOrDefault();
                ViewBag.tenThuongHieu = ThuongHieu.tenThuongHieu;
                ViewBag.SanPham = sanpham;

                int Id_SanPham = int.Parse(IdSanPham);
                ViewBag.lstDanhGia = (from dg in DbContext.DanhGias
                                      join kh in DbContext.KhachHangs on dg.idKhach equals kh.idKhach
                                      where dg.idSp == Id_SanPham
                                      orderby dg.ngayTao
                                      select new PhanHoi
                                      {
                                          ngayTao = dg.ngayTao,
                                          noiDung = dg.noiDung,
                                          tenKhachHang = kh.tenDangNhap
                                      }).ToList();

                ViewBag.lstAnhSanPham = DbContext.Anhs.Where(e => e.idSp == IdSan_Pham).ToList();
                ViewBag.RandomSanPham = DbContext.SanPhams.OrderBy(x => Guid.NewGuid()).Skip(0).Take(8).ToList();
            }
            return View();
        }

        [ValidateInput(false)]
        public JsonResult GuiNoiDungPhanHoi(string IdSanPham, string NoiDungPhanHoi)
        {
            Message message = new Message();
            try
            {
                LoginInfor login = Session["Login"] as LoginInfor;
                if (login == null)
                {
                    message.Icon = "error";
                    message.Title = "Bạn phải đăng nhập để thực hiện chức năng này";
                    message.Data = "error";
                    return Json(message, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    using (var DbContext = new WebBanHangEntities())
                    {
                        int Id_SanPham = int.Parse(IdSanPham);
                        var checkDonHang = DbContext.DonHangs.Where(e => e.idKhach == login.IdKhachHang).Select(e => e.idDon).ToList();
                        var checksp = DbContext.CT_DonHang.Where(e => e.idSp == Id_SanPham && checkDonHang.Contains(e.idDon)).ToList();
                        if (checksp.Count == 0)
                        {
                            message.Icon = "error";
                            message.Title = "Bạn cần mua sản phẩm để đánh giá";
                            return Json(message, JsonRequestBehavior.AllowGet);
                        }
                    }

                    PhanHoi phanhoi = new PhanHoi();
                    phanhoi.IdSanPham = IdSanPham;
                    phanhoi.NoiDungPhanHoi = NoiDungPhanHoi;
                    phanhoi.idkhach = login.IdKhachHang.ToString();
                    using (var DbContext = new WebBanHangEntities())
                    {
                        DanhGia danhgia = new DanhGia();
                        danhgia.idSp = int.Parse(phanhoi.IdSanPham);
                        danhgia.noiDung = phanhoi.NoiDungPhanHoi;
                        danhgia.idKhach = int.Parse(phanhoi.idkhach);
                        danhgia.ngayTao = DateTime.Now;
                        DbContext.DanhGias.Add(danhgia);
                        DbContext.SaveChanges();
                        message.Icon = "success";
                        message.Title = "Gửi đánh giá thành công!";

                        message.Data = "<div class='DanhGia'>\r\n"
                        + "	  <p style='margin: 0;'><span>" + login.tenDangNhap + "</span></p>\r\n"
                        + "		  <div class='news__date'>\r\n"
                        + "			<span class='-ap icon icon-access_time' style='font-size: 12px;'></span>\r\n"
                        + "			<span style='font-size: 12px;'>" + danhgia.ngayTao + "</span>\r\n"
                        + "		  </div>\r\n"
                        + "	  <p style='margin: 0;'>" + NoiDungPhanHoi + "</p>\r\n"
                        + " </div>";
                    }
                }
            }
            catch (Exception e)
            {
                message.Icon = "error";
                message.Title = "Có lỗi : " + e.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }



        public JsonResult KiemTraSoLuongSanPham(string IdSanPham)
        {
            Message message = new Message();
            try
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    int Id_SanPham = int.Parse(IdSanPham);
                    var checkSP = DbContext.SanPhams.Where(e => e.idSp == Id_SanPham).Select(e => e.soLuong).FirstOrDefault();
                    message.Data = checkSP.ToString();
                    message.Icon = "success";
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