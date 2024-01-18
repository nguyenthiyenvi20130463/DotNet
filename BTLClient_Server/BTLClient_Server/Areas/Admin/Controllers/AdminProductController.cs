using BTLClient_Server.EF;
using BTLClient_Server.Areas.Admin.Models.DTO;
using BTLClient_Server.Areas.Admin.Security;
using LTTHAPI.Models;
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

    public class DanhGiaSanPham
    {
        public int IdKhach { get; set; }
        public string TenKhachHang { get; set; }
        public string NhanXet { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NgayTao_ { get; set; }
    }

    [AuthorizationAdmin]
    public class AdminProductController : Controller
    {
        // GET: Admin/AdminProduct
        public ActionResult Index(string keywords, string idThuongHieu, string proStatus, string minPrice, string maxPrice, int pageNum = 1, int pageSize = 8)
        {
            if (idThuongHieu == null)
                idThuongHieu = "0";
            int cateId = Convert.ToInt32(idThuongHieu);
            ViewBag.keywords = keywords;
            ViewBag.idThuongHieu = cateId;
            ViewBag.proStatus = proStatus;
            ViewBag.minPrice = minPrice;
            ViewBag.maxPrice = maxPrice;
            if (proStatus == null)
                proStatus = "0";
            double min, max;
            if (!double.TryParse(minPrice, out min))
                min = 0;
            if (!double.TryParse(maxPrice, out max))
                max = 100000000;
            if (keywords == null)
                keywords = "";

            IEnumerable<ProductDTO> lst = null;
            using (var DbContext = new WebBanHangEntities())
            {
                lst = GetListPro(keywords, int.Parse(idThuongHieu), proStatus, min, max).ToPagedList<ProductDTO>(pageNum, pageSize);
                ViewBag.lstTra = DbContext.ThuongHieux.ToList();
            }
            return View(lst);
        }

        public IEnumerable<ProductDTO> GetListPro(string proName, int categoryId, string proStatus, double minPrice, double maxPrice)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                var lst = DbContext.Database.SqlQuery<ProductDTO>(string.Format("listProduct N'{0}', {1}, {2}, {3}, {4}",
                proName, categoryId, proStatus, minPrice, maxPrice)
                ).ToList<ProductDTO>();
                return lst;
            }
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
                        SanPham pro = DbContext.SanPhams.Find(id);
                        if (pro != null)
                        {
                            DbContext.SanPhams.Remove(pro);
                            DbContext.SaveChanges();
                        }
                    }
                }
            }
            return Redirect("~/Admin/AdminProduct/Index");
        }

        public ActionResult Create()
        {
            using (var DbContext = new WebBanHangEntities())
            {
                ViewBag.lstThuongHieu = DbContext.ThuongHieux.ToList();
            }
            return View();
        }

        public ActionResult Delete(int id)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                SanPham pro = DbContext.SanPhams.Find(id);
                if (pro != null)
                {
                    DbContext.SanPhams.Remove(pro);
                    DbContext.SaveChanges();
                }
            }
            return Redirect("~/Admin/AdminProduct/Index");
        }

        public void LuuThong_TinHinhAnhXoa()
        {
            Session["AnhXoa"] = null;
        }

        public Message LuuThongTinHinhAnhXoa(string TenHinhAnh)
        {
            Message message = new Message();
            try
            {
                List<string> lstAnhXoa = Session["AnhXoa"] as List<String>;
                if (lstAnhXoa.Count() == 0)
                {
                    List<string> lstAnh_Xoa = new List<string>();
                    lstAnh_Xoa.Add(TenHinhAnh);
                    lstAnhXoa = lstAnh_Xoa;
                }
                else
                {
                    lstAnhXoa.Add(TenHinhAnh);
                    lstAnhXoa = lstAnhXoa.Distinct().ToList();
                }
                Session["AnhXoa"] = lstAnhXoa;
            }
            catch (Exception e)
            {
                message.Title = "Có lỗi: " + e.Message;
                message.Icon = "error";
            }
            return message;
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(List<HttpPostedFileBase> image, HttpPostedFileBase photo,string tensanpham, string motasanpham, string chitietsanpham, string giabansanpham, string giakhuyenmaisanpham, string thuonghieusanpham, string trangthaisanpham
            , string SoLuongSanPham)
        {
            User user = Session["user"] as User;
            SanPham sanpham = new SanPham();
            sanpham.gia = double.Parse(giabansanpham);
            if(!string.IsNullOrEmpty(giakhuyenmaisanpham))
            {
                sanpham.giaKm = double.Parse(giakhuyenmaisanpham);
            }
            sanpham.ten = tensanpham;
            sanpham.moTa = motasanpham;
            sanpham.chiTiet = chitietsanpham;
            sanpham.idThuongHieu = int.Parse(thuonghieusanpham);
            sanpham.trangThai = int.Parse(trangthaisanpham);
            sanpham.soLuong = int.Parse(SoLuongSanPham);
            sanpham.soLuong = int.Parse(SoLuongSanPham);
            sanpham.idUser = user.idUser;
            if (trangthaisanpham.Equals("2"))
            {
                double giagiam = (double.Parse(giabansanpham) - double.Parse(giakhuyenmaisanpham)) / double.Parse(giabansanpham);
                double Gia_Ban = Math.Round(giagiam);
                string lamT = "-" + Gia_Ban + "%";
                sanpham.trangThaiMieuTa = lamT;
            }
            if (trangthaisanpham.Equals("1"))
            {
                sanpham.trangThaiMieuTa = "NEW";
            }
            if (trangthaisanpham.Equals("5"))
            {
                sanpham.trangThaiMieuTa = "HOT";
            }
            if(photo!=null && photo.ContentLength>0)
            {
                photo.SaveAs(Path.Combine(Server.MapPath("~/Theme/Image/"), photo.FileName));
                sanpham.anhDaiDien = photo.FileName;
            }
            List<string> lstAnhXoa = Session["AnhXoa"] as List<string>;
            List<AnhSanPham> lstAnh =new List<AnhSanPham>();
            foreach (var item in image)
            {
                if (item != null && item.ContentLength > 0)
                {
                    if(lstAnhXoa!=null)
                    {
                        if(lstAnhXoa.IndexOf(item.FileName)==-1)
                        {
                            AnhSanPham anhsanpham = new AnhSanPham();
                            item.SaveAs(Path.Combine(Server.MapPath("~/Theme/Image/"), item.FileName));
                            anhsanpham.Anh = item.FileName;
                            lstAnh.Add(anhsanpham);
                        }    
                    }    
                    else
                    {
                        AnhSanPham anhsanpham = new AnhSanPham();
                        item.SaveAs(Path.Combine(Server.MapPath("~/Theme/Image/"), item.FileName));
                        anhsanpham.Anh = item.FileName;
                        lstAnh.Add(anhsanpham);
                    }    
                }
            }
            string json = JsonConvert.SerializeObject(sanpham);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            using (var DbContext = new WebBanHangEntities())
            {
                string IdSanPham = "";
                DbContext.SanPhams.Add(sanpham);
                DbContext.SaveChanges();
                IdSanPham = sanpham.idSp.ToString();

                if (lstAnh!=null)
                {
                    foreach(var item in lstAnh)
                    {
                        item.IdSanPham = IdSanPham;
                    }    
                }

                if (lstAnh != null)
                {
                    foreach (var item in lstAnh)
                    {
                        Anh anh = new Anh();
                        anh.link = item.Anh;
                        anh.idSp = int.Parse(item.IdSanPham);
                        DbContext.Anhs.Add(anh);
                    }
                }
                DbContext.SaveChanges();
            }
            return Redirect("/Admin/AdminProduct/Index");
        }

        public ActionResult Edit(string IdSanPham)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                ViewBag.lstThuongHieu = DbContext.ThuongHieux.ToList();

                SanPham sanpham = new SanPham();
                int Id_SanPham = int.Parse(IdSanPham);
                sanpham = DbContext.SanPhams.Where(e => e.idSp == Id_SanPham).FirstOrDefault();
                ViewBag.SanPham = sanpham;

                List<Anh> anh = new List<Anh>();
                ViewBag.AnhSanPham = DbContext.Anhs.Where(e => e.idSp == Id_SanPham).ToList();
            }
            return View();
        }

        public Message XoaAnhSanPham(string idAnh)
        {
            Message message = new Message();
            using (var DbContext = new WebBanHangEntities())
            {
                int id_Anh = int.Parse(idAnh);
                var anh = DbContext.Anhs.Where(e => e.idAnh == id_Anh).FirstOrDefault();
                DbContext.Anhs.Remove(anh);
                DbContext.SaveChanges();
            }
            return message;
        }

        public void LuuThong_TinHinhAnhXoaEdit()
        {
            Session["AnhXoaEdit"] = null;
        }

        public void LuuThongTinHinhAnhXoaEdit(string TenHinhAnh)
        {
            List<string> lstAnhXoa = Session["AnhXoaEdit"] as List<string>;
            try
            {
                if (lstAnhXoa.Count() == 0)
                {
                    List<string> lstAnh_Xoa = new List<string>();
                    lstAnh_Xoa.Add(TenHinhAnh);
                    lstAnhXoa = lstAnh_Xoa;
                }
                else
                {
                    lstAnhXoa.Add(TenHinhAnh);
                    lstAnhXoa = lstAnhXoa.Distinct().ToList();
                }
                Session["AnhXoa"] = lstAnhXoa;
            }
            catch (Exception e)
            {
            }
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(List<HttpPostedFileBase> image, HttpPostedFileBase photo, string tensanpham, string motasanpham, string chitietsanpham, string giabansanpham, string giakhuyenmaisanpham, string thuonghieusanpham, string trangthaisanpham
            , string SoLuongSanPham, string IdSanPham)
        {
            SanPham sanpham = new SanPham();
            using (var DbContext = new WebBanHangEntities())
            {
                int Id_SanPham = int.Parse(IdSanPham);
                sanpham = DbContext.SanPhams.Where(e => e.idSp == Id_SanPham).FirstOrDefault();
            }

            User user = Session["user"] as User;
            
            sanpham.gia = double.Parse(giabansanpham);
            if (!string.IsNullOrEmpty(giakhuyenmaisanpham))
            {
                sanpham.giaKm = double.Parse(giakhuyenmaisanpham);
            }
            sanpham.ten = tensanpham;
            sanpham.moTa = motasanpham;
            sanpham.chiTiet = chitietsanpham;
            sanpham.idThuongHieu = int.Parse(thuonghieusanpham);
            sanpham.trangThai = int.Parse(trangthaisanpham);
            sanpham.soLuong = int.Parse(SoLuongSanPham);
            sanpham.soLuong = int.Parse(SoLuongSanPham);
            sanpham.idUser = user.idUser;
            if (trangthaisanpham.Equals("2"))
            {
                double giagiam = (double.Parse(giabansanpham) - double.Parse(giakhuyenmaisanpham)) / double.Parse(giabansanpham);
                double Gia_Ban = Math.Round(giagiam);
                string lamT = "-" + Gia_Ban + "%";
                sanpham.trangThaiMieuTa = lamT;
            }
            if (trangthaisanpham.Equals("1"))
            {
                sanpham.trangThaiMieuTa = "NEW";
            }
            if (trangthaisanpham.Equals("5"))
            {
                sanpham.trangThaiMieuTa = "HOT";
            }
            if (photo != null && photo.ContentLength > 0)
            {
                photo.SaveAs(Path.Combine(Server.MapPath("~/Theme/Image/"), photo.FileName));
                sanpham.anhDaiDien = photo.FileName;
            }
            List<string> lstAnhXoa = Session["AnhXoaEdit"] as List<string>;
            List<AnhSanPham> lstAnh = new List<AnhSanPham>();
            foreach (var item in image)
            {
                if (item != null && item.ContentLength > 0)
                {
                    if (lstAnhXoa != null)
                    {
                        if (lstAnhXoa.IndexOf(item.FileName) == -1)
                        {
                            AnhSanPham anhsanpham = new AnhSanPham();
                            item.SaveAs(Path.Combine(Server.MapPath("~/Theme/Image/"), item.FileName));
                            anhsanpham.Anh = item.FileName;
                            lstAnh.Add(anhsanpham);
                        }
                    }
                    else
                    {
                        AnhSanPham anhsanpham = new AnhSanPham();
                        item.SaveAs(Path.Combine(Server.MapPath("~/Theme/Image/"), item.FileName));
                        anhsanpham.Anh = item.FileName;
                        lstAnh.Add(anhsanpham);
                    }
                }
            }
            string json = JsonConvert.SerializeObject(sanpham);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            using (var DbContext = new WebBanHangEntities())
            {
                var san_pham = DbContext.SanPhams.Where(e => e.idSp == sanpham.idSp).FirstOrDefault();
                san_pham.gia = sanpham.gia;
                san_pham.ten = sanpham.ten;
                san_pham.moTa = sanpham.moTa;
                san_pham.chiTiet = sanpham.chiTiet;
                san_pham.idThuongHieu = sanpham.idThuongHieu;
                san_pham.trangThai = sanpham.trangThai;
                san_pham.soLuong = sanpham.soLuong;
                san_pham.soLuong = sanpham.soLuong;
                san_pham.idUser = sanpham.idUser;
                san_pham.trangThaiMieuTa = sanpham.trangThaiMieuTa;
                san_pham.anhDaiDien = sanpham.anhDaiDien;
                DbContext.SaveChanges();

                if (lstAnh != null)
                {
                    foreach (var item in lstAnh)
                    {
                        item.IdSanPham = IdSanPham;
                    }
                }

                if (lstAnh != null)
                {
                    foreach (var item in lstAnh)
                    {
                        Anh anh = new Anh();
                        anh.link = item.Anh;
                        anh.idSp = int.Parse(item.IdSanPham);
                        DbContext.Anhs.Add(anh);
                    }
                }
            }
            return Redirect("/Admin/AdminProduct/Index");
        }
        }
}