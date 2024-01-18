using BTLClient_Server.Areas.Admin.Controllers;
using BTLClient_Server.EF;
using BTLClient_Server.Models;
using LapTrinhTichHop.Models;
using LTTH_UI_UX.Models;
using LTTHAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CartItem = BTLClient_Server.Models.CartItem;

namespace BTLClient_Server.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: SanPham
        public ActionResult SanPhamThuongHieu(string IdThuongHieu)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                int Id_ThuongHieu = int.Parse(IdThuongHieu);
                ViewBag.lstSanPham = DbContext.SanPhams.Where(e => e.idThuongHieu == Id_ThuongHieu).ToList();
            }
            return View();
        }

        [HttpPost]
        public ActionResult TimKiemSanpham(string TuKhoa)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                ViewBag.lstSanPham = (from sp in DbContext.SanPhams
                                      where sp.ten.Contains(TuKhoa)
                                      select sp).ToList();
            }
            return View();
        }

        [HttpPost]
        public JsonResult ThemSanPhamVaoGioHang(string IdSanPham)
        {
            Message message = new Message();
            try
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    int Id_SanPham = int.Parse(IdSanPham);
                    var CheckSlSP = DbContext.SanPhams.Where(e => e.idSp == Id_SanPham).Select(e => e.soLuong).FirstOrDefault();
                    if (CheckSlSP == 0)
                    {
                        message.Icon = "error";
                        message.Title = "Không đủ sản phẩm để bạn đăt hàng!";
                        return Json(message, JsonRequestBehavior.AllowGet);
                    }

                    List<CartItem> GioiHang = Session["GioiHang"] as List<CartItem>;
                    if (GioiHang != null)
                    {
                        var checkSPTrongGioi = GioiHang.Where(e => e.sanpham.idSp == Id_SanPham).FirstOrDefault();
                        if (checkSPTrongGioi != null)
                        {
                            var check_SPTrongGioi = checkSPTrongGioi.soluong;
                            if (check_SPTrongGioi + 1 > CheckSlSP)
                            {
                                message.Icon = "error";
                                message.Title = "Không đủ sản phẩm để bạn đăt hàng!";
                                return Json(message, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }


                    Cart cart = new Cart();
                    GioiHang = cart.ThemSanPham(GioiHang, IdSanPham, 1);
                    Session["GioiHang"] = GioiHang;
                    message.Icon = "success";
                    message.Title = "Thêm sản phẩm vào trong giỏ hàng thành công!";
                }
            }
            catch (Exception e)
            {
                message.Icon = "error";
                message.Title = "Có lỗi : " + e.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChiTietGioHang()
        {
            Message message = new Message();
            try
            {
                List<CartItem> GioiHang = Session["GioiHang"] as List<CartItem>;
                Cart cart = new Cart();

                float TongTien = 0;
                int TongSanPham = 0;
                string DataChiTietGioHang = "";
                if (GioiHang != null)
                {
                    TongTien = (float)cart.TongTien(GioiHang);
                    TongSanPham = GioiHang.Count();
                    foreach (var itemGioHang in GioiHang)
                    {
                        string Gia = "";
                        if (itemGioHang.sanpham.trangThai == 2)
                        {
                            Gia = "<div class=\"product__price__old\">" + itemGioHang.sanpham.gia.Value.ToString("N0") + " <span class=\"unit\">đ</span></div>\r\n"
                                    + "<div class=\"product__price__regular\">" + itemGioHang.sanpham.giaKm.Value.ToString("N0") + " <span class=\"unit\">đ</span></div>\r\n";
                        }
                        else
                        {
                            Gia = "<div class=\"product__price__regular\">" + itemGioHang.sanpham.gia.Value.ToString("N0") + " <span class=\"unit\">đ</span></div>\r\n";
                        }
                        DataChiTietGioHang += "<div class=\"product__list\">\r\n"
                            + "	<div class=\"product__list__item\">\r\n"
                            + "		<div class=\"product__img\">\r\n"
                            + "			<img src=\"/Theme/Image/" + itemGioHang.sanpham.anhDaiDien + "\" alt=\"Name your Product \">\r\n"
                            + "		</div>\r\n"
                            + "		<div class=\"product__name\">\r\n"
                            + "			" + itemGioHang.sanpham.ten + "\r\n"
                            + "			<a class=\"product__delete\" onclick=\"XoaSanPhamChiTietGioHang(" + itemGioHang.sanpham.idSp + ")\">\r\n"
                            + "				<span class=\"icon -ap icon-trash2\"></span>\r\n"
                            + "			</a>\r\n"
                            + "		</div>\r\n"
                            + "		\r\n"
                            + "		<div class=\"product__price\">\r\n"
                            + Gia
                            + "			<div class=\"product__quantity\">\r\n"
                            + "				<button class=\"btn-minus btn\" onclick=\"GiamSanPhamGioHang(" + itemGioHang.sanpham.idSp + ")\">\r\n"
                            + "					-\r\n"
                            + "				</button>\r\n"
                            + "				<input disabled id=\"SanPhamGioHang" + itemGioHang.sanpham.idSp + "\" onkeypress=\"return (event.charCode !=8 && event.charCode ==0 || ( event.charCode == 46 || (event.charCode >= 48 && event.charCode <= 57)))\" type=\"text\" class=\"input-number\" value=\"" + itemGioHang.soluong + "\" style=\"padding: 0;\">\r\n"
                            + "				<button class=\"btn-plus btn\"onclick=\"TangSanPhamGioHang(" + itemGioHang.sanpham.idSp + ")\">\r\n"
                            + "					+\r\n"
                            + "				</button>\r\n"
                            + "				<div class=\"clearfix\"></div>\r\n"
                            + "			</div>\r\n"
                            + "			\r\n"
                            + "		</div>\r\n"
                            + "\r\n"
                            + "		<div class=\"clearfix\"></div>\r\n"
                            + "	</div>\r\n"
                            + "</div>";
                    }
                }
                message.Icon = "success";
                message.TongTien = TongTien;
                message.TongSanPham = TongSanPham;
                message.DataChiTietGioHang = DataChiTietGioHang;
                message.Data = DataChiTietGioHang;
            }
            catch (Exception e)
            {
                message.Icon = "error";
                message.Title = "Có lỗi : " + e.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult XoaSanPhamTrongGioHang(string IdSanPham)
        {
            Message message = new Message();
            try
            {
                List<CartItem> GioiHang = Session["GioiHang"] as List<CartItem>;
                Cart cart = new Cart();
                GioiHang = cart.XoaSanPham(GioiHang, IdSanPham);
                Session["GioiHang"] = GioiHang;
                message.Icon = "success";
            }
            catch (Exception e)
            {
                message.Icon = "error";
                message.Title = "Có lỗi : " + e.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CapNhatSanPhamTrongGioHang(string IdSanPham, string soLuongSanPham)
        {
            Message message = new Message();
            try
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    int Id_SanPham = int.Parse(IdSanPham);
                    int soLuong_SanPham = int.Parse(soLuongSanPham);
                    var CheckSlSP = DbContext.SanPhams.Where(e => e.idSp == Id_SanPham).Select(e => e.soLuong).FirstOrDefault();
                    if (soLuong_SanPham > CheckSlSP)
                    {
                        message.Icon = "error";
                        message.Title = "Không đủ sản phẩm để bạn đăt hàng!";
                        return Json(message, JsonRequestBehavior.AllowGet);
                    }

                    List<CartItem> GioiHang = Session["GioiHang"] as List<CartItem>;
                    Cart cart = new Cart();
                    GioiHang = cart.CapNhatSanPham(GioiHang, IdSanPham, int.Parse(soLuongSanPham));
                    Session["GioiHang"] = GioiHang;
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

        [HttpPost]
        public JsonResult ThemSanPhamVao_GioHang(string IdSanPham, string SoLuong)
        {
            Message message = new Message();
            try
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    int So_Luong = int.Parse(SoLuong);
                    int Id_SanPham = int.Parse(IdSanPham);
                    var CheckSlSP = DbContext.SanPhams.Where(e => e.idSp == Id_SanPham).Select(e => e.soLuong).FirstOrDefault();
                    if (CheckSlSP == 0)
                    {
                        message.Icon = "error";
                        message.Title = "Không đủ sản phẩm để bạn đăt hàng!";
                        return Json(message, JsonRequestBehavior.AllowGet);
                    }

                    List<CartItem> GioiHang = Session["GioiHang"] as List<CartItem>;

                    if (GioiHang != null)
                    {
                        var checkSPTrongGioi = GioiHang.Where(e => e.sanpham.idSp == Id_SanPham).FirstOrDefault();
                        if (checkSPTrongGioi != null)
                        {
                            var check_SPTrongGioi = checkSPTrongGioi.soluong;
                            if (check_SPTrongGioi + So_Luong > CheckSlSP)
                            {
                                message.Icon = "error";
                                message.Title = "Không đủ sản phẩm để bạn đăt hàng!";
                                return Json(message, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                    Cart cart = new Cart();
                    GioiHang = cart.ThemSanPham(GioiHang, IdSanPham, int.Parse(SoLuong));
                    Session["GioiHang"] = GioiHang;
                    message.Icon = "success";
                    message.Title = "Thêm sản phẩm vào trong giỏ hàng thành công!";
                }

            }
            catch (Exception e)
            {
                message.Icon = "error";
                message.Title = "Có lỗi : " + e.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChiTiet_GioHang()
        {
            Message message = new Message();
            try
            {
                List<CartItem> GioiHang = Session["GioiHang"] as List<CartItem>;
                Cart cart = new Cart();

                float TongTien = 0;
                int TongSanPham = 0;
                string DataChiTietGioHang = "";
                if (GioiHang != null)
                {
                    TongTien = (float)cart.TongTien(GioiHang);
                    TongSanPham = GioiHang.Count();
                    foreach (var itemGioHang in GioiHang)
                    {
                        string Gia = "";
                        if (itemGioHang.sanpham.trangThai == 2)
                        {
                            Gia = "<div class=\"product__price__old\">" + itemGioHang.sanpham.gia.Value.ToString("N0") + " <span class=\"unit\">đ</span></div>\r\n"
                                    + "<div class=\"product__price__regular\">" + itemGioHang.sanpham.giaKm.Value.ToString("N0") + " <span class=\"unit\">đ</span></div>\r\n";
                        }
                        else
                        {
                            Gia = "<div class=\"product__price__regular\">" + itemGioHang.sanpham.gia.Value.ToString("N0") + " <span class=\"unit\">đ</span></div>\r\n";
                        }

                        DataChiTietGioHang += " <div class=\"product__list__item\">\r\n"
                    + "     <div class=\"product__img\">\r\n"
                    + "         <img src=\"/Theme/Image/" + itemGioHang.sanpham.anhDaiDien + "\" alt=\"Name your Product \">\r\n"
                    + "     </div>\r\n"
                    + "     <div class=\"product__name\" style=\"text-align: left;\">\r\n"
                    + "         <a href=\"#\" >" + itemGioHang.sanpham.ten + "</a>\r\n"
                    + "         <a class=\"product__delete\"  href=\"#\" onclick=\"XoaSanPham(" + itemGioHang.sanpham.idSp + ")\">\r\n"
                    + "             <span class=\"icon -ap icon-trash2\"></span>\r\n"
                    + "         </a>\r\n"
                    + "     </div>\r\n"
                    + "     \r\n"
                    + "     <div class=\"product__price\">\r\n"
                    + Gia
                    + "         <div class=\"product__quantity\">\r\n"
                    + "             <button class=\"btn-minus btn\" id=\"GiamSoLuong" + itemGioHang.sanpham.idSp + "\" onclick=\"GiamSoLuong(" + itemGioHang.sanpham.idSp + ")\">\r\n"
                    + "                 -\r\n"
                    + "             </button>\r\n"
                    + "             <input type=\"text\" disabled onkeypress=\"return (event.charCode !=8 && event.charCode ==0 || ( event.charCode == 46 || (event.charCode >= 48 && event.charCode <= 57)))\" style=\"padding: 0;\" class=\"input-number\" value=\"" + itemGioHang.soluong + "\" id=\"SoLuong" + itemGioHang.sanpham.idSp + "\">\r\n"
                    + "             <button class=\"btn-plus btn\" id=\"TangSoLuong" + itemGioHang.sanpham.idSp + "\" onclick=\"TangSoLuong(" + itemGioHang.sanpham.idSp + ")\">\r\n"
                    + "                 +\r\n"
                    + "             </button>\r\n"
                    + "             <div class=\"clearfix\"></div>\r\n"
                    + "         </div>\r\n"
                    + "         \r\n"
                    + "     </div>\r\n"
                    + "\r\n"
                    + "     <div class=\"clearfix\"></div>\r\n"
                    + " </div>";
                    }
                }
                message.Icon = "success";
                message.TongTien = TongTien;
                message.TongSanPham = TongSanPham;
                message.DataChiTietGioHang = DataChiTietGioHang;
                message.Data = DataChiTietGioHang;
            }
            catch (Exception e)
            {
                message.Icon = "error";
                message.Title = "Có lỗi : " + e.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChiTietGioHangRutGon()
        {
            Message message = new Message();
            try
            {
                List<CartItem> GioiHang = Session["GioiHang"] as List<CartItem>;
                Cart cart = new Cart();

                float TongTien = 0;
                int TongSanPham = 0;
                string DataChiTietGioHang = "";
                if (GioiHang != null)
                {
                    TongTien = (float)cart.TongTien(GioiHang);
                    TongSanPham = GioiHang.Count();

                    foreach (var itemGioHang in GioiHang)
                    {
                        string Gia = "";
                        if (itemGioHang.sanpham.trangThai == 2)
                        {
                            Gia = "<div class=\"product__price__old\">" + itemGioHang.sanpham.gia.Value.ToString("N0") + " <span class=\"unit\">đ</span></div>\r\n"
                                    + "<div class=\"product__price__regular\">" + itemGioHang.sanpham.giaKm.Value.ToString("N0") + " <span class=\"unit\">đ</span></div>\r\n";
                        }
                        else
                        {
                            Gia = "<div class=\"product__price__regular\">" + itemGioHang.sanpham.gia.Value.ToString("N0") + " <span class=\"unit\">đ</span></div>\r\n";
                        }

                        DataChiTietGioHang += "<div class=\"product__list__item\">\r\n"
                        + "                                    <div class=\"product__img\">\r\n"
                        + "                                        <img src=\"/Theme/Image/" + itemGioHang.sanpham.anhDaiDien + "\" alt=\"Name your Product \">\r\n"
                        + "                                    </div>\r\n"
                        + "                                    <div class=\"product__name\" style=\"text-align: left;\">\r\n"
                        + "                                        " + itemGioHang.sanpham.ten + "\r\n"
                        + "                                    </div>\r\n"
                        + "                                    \r\n"
                        + "                                    <div class=\"product__price\">\r\n"
                        + Gia
                        + "                                    </div>\r\n"
                        + "                                    <div class=\"clearfix\"></div>\r\n"
                        + "                                </div>";
                    }
                }
                message.Icon = "success";
                message.TongTien = TongTien;
                message.TongSanPham = TongSanPham;
                message.DataChiTietGioHang = DataChiTietGioHang;
                message.Data = DataChiTietGioHang;
            }
            catch (Exception e)
            {
                message.Icon = "error";
                message.Title = "Có lỗi : " + e.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult XemDonHangDaMua()
        {
            LoginInfor login = new LoginInfor();
            login = Session["Login"] as LoginInfor;

            using (var DbContext = new WebBanHangEntities())
            {
                ViewBag.lstDonHang = DbContext.DonHangs.Where(e => e.idKhach == login.IdKhachHang).OrderByDescending(e=>e.idDon).ToList();
            }
            return View();
        }

        public ActionResult ChiTiet_DonHang(string IdDonHang)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                int IdDon_Hang = int.Parse(IdDonHang);
                ViewBag.lstChiTietDonHang = (from ctdonhang in DbContext.CT_DonHang
                                             join sp in DbContext.SanPhams on ctdonhang.idSp equals sp.idSp
                                             where ctdonhang.idDon == IdDon_Hang
                                             select new CTDonHang
                                             {
                                                 TenSanPham = sp.ten,
                                                 DonGia = ctdonhang.giaBan,
                                                 SoLuong = ctdonhang.soLuong,
                                                 ThanhTien = ctdonhang.thanhTien,
                                                 AnhDD = sp.anhDaiDien,
                                             }).ToList();
            }
            return View();
        }

        [HttpPost]
        public JsonResult HuyDonDatHang(string IdSanPham)
        {
            Message message = new Message();
            try
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    int Id_SanPham = int.Parse(IdSanPham);
                    var GoiHang = DbContext.DonHangs.Where(e => e.idDon == Id_SanPham).FirstOrDefault();
                    if (GoiHang != null)
                    {
                        GoiHang.trangThai = 4;
                        DbContext.SaveChanges();
                        message.Icon = "success";
                        message.Title = "Hủy đơn hàng thành công";
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

        [HttpPost]
        public JsonResult NhanDonHang(string IdSanPham)
        {
            Message message = new Message();
            try
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    int Id_SanPham = int.Parse(IdSanPham);
                    var GoiHang = DbContext.DonHangs.Where(e => e.idDon == Id_SanPham).FirstOrDefault();
                    if (GoiHang != null)
                    {
                        GoiHang.trangThai = 5;
                        DbContext.SaveChanges();
                        message.Icon = "success";
                        message.Title = "Thay đổi trạng thái đơn hàng thành công";
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

        [HttpPost]
        public JsonResult DatHang(string IdSanPham)
        {
            Message message = new Message();
            try
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    int Id_SanPham = int.Parse(IdSanPham);
                    var GoiHang = DbContext.DonHangs.Where(e => e.idDon == Id_SanPham).FirstOrDefault();
                    if (GoiHang != null)
                    {
                        GoiHang.trangThai = 1;
                        DbContext.SaveChanges();
                        message.Icon = "success";
                        message.Title = "Thay đổi trạng thái đơn hàng thành công";
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

        public ActionResult XemDonHangDaMuaLoad()
        {
            LoginInfor login = new LoginInfor();
            login = Session["Login"] as LoginInfor;

            using (var DbContext = new WebBanHangEntities())
            {
                ViewBag.lstDonHang = DbContext.DonHangs.Where(e => e.idKhach == login.IdKhachHang).OrderByDescending(e => e.idDon).ToList();
            }
            return View();
        }

        public ActionResult XemPhanHoiSanPham(string idSanPham)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                var lstKhachHang = DbContext.KhachHangs.ToList();
                int id_SanPham = int.Parse(idSanPham);
                var query = (from dg in DbContext.DanhGias
                             join sp in DbContext.SanPhams on dg.idSp equals sp.idSp
                             join kh in DbContext.KhachHangs on dg.idKhach equals kh.idKhach
                             where (sp.idSp == id_SanPham)
                             select new DanhGiaSanPham
                             {
                                 IdKhach = dg.idKhach,
                                 NhanXet = dg.noiDung,
                                 NgayTao = dg.ngayTao
                             }).ToList();
                if (query != null)
                {
                    foreach (var item in query)
                    {
                        item.TenKhachHang = lstKhachHang.Where(e => e.idKhach == item.IdKhach).Select(e => e.hoTen).FirstOrDefault();
                        if (item.NgayTao != null)
                        {
                            DateTime ngayDG = (DateTime)item.NgayTao;
                            item.NgayTao_ = ngayDG.ToString("dd/MM/yyyy");
                        }
                    }
                }
                ViewBag.query = query;
                return View();
            }
        }
    }
}