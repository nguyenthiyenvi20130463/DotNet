using BTLClient_Server.EF;
using BTLClient_Server.Models;
using BTLClient_Server.Models;
using LTTH_UI_UX.Models;
using LTTHAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace BTLClient_Server.Controllers
{
    public class ThanhToanController : Controller
    {
        // GET: ThanhToan
        public ActionResult TienHanhThanhToan()
        {
            LoginInfor login = new LoginInfor();
            login = Session["Login"] as LoginInfor;
            ViewBag.login = login;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://online-gateway.ghn.vn");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("token", ConfigurationManager.AppSettings["tokenGiaoHangNhanh"]);

                var responeTask = client.GetAsync("/shiip/public-api/master-data/province");
                responeTask.Wait();

                if (responeTask.Result.IsSuccessStatusCode)
                {
                    var readJob = responeTask.Result.Content.ReadAsAsync<TinhThanh>();
                    readJob.Wait();
                    ViewBag.lstTinhThanh = readJob.Result.data;
                }
            }

            return View();
        }

        public ActionResult DataQuanHuyen(string IdTinh)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://online-gateway.ghn.vn");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("token", ConfigurationManager.AppSettings["tokenGiaoHangNhanh"]);

                var responeTask = client.GetAsync("/shiip/public-api/master-data/district?province_id=" + IdTinh);
                responeTask.Wait();

                if (responeTask.Result.IsSuccessStatusCode)
                {
                    var readJob = responeTask.Result.Content.ReadAsAsync<QuanHuyen>();
                    readJob.Wait();
                    ViewBag.lstQuanHuyen = readJob.Result.data;
                }
            }
            return View();
        }

        public ActionResult DataXaPhuong(string IdQuanHuyen)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://online-gateway.ghn.vn");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("token", ConfigurationManager.AppSettings["tokenGiaoHangNhanh"]);

                var responeTask = client.GetAsync("/shiip/public-api/master-data/ward?district_id=" + IdQuanHuyen);
                responeTask.Wait();

                if (responeTask.Result.IsSuccessStatusCode)
                {
                    var readJob = responeTask.Result.Content.ReadAsAsync<XaPhuong>();
                    readJob.Wait();
                    ViewBag.lstXaPhuong = readJob.Result.data;
                }
            }
            return View();
        }

        public string TinhPhiGiaoHangTietKiem(string TenTinh, string TenQuanHuyen, string TenXaPhuong)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://services.giaohangtietkiem.vn");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("token", ConfigurationManager.AppSettings["tokenGiaoHangTietKiem"]);

                ParamFreeGiaoTietKiem GiaoHangTietKiem = new ParamFreeGiaoTietKiem();
                GiaoHangTietKiem.pick_province = "Hà Nội";
                GiaoHangTietKiem.pick_district = "Cầu giấy";

                GiaoHangTietKiem.province = TenTinh;
                GiaoHangTietKiem.district = TenQuanHuyen;
                GiaoHangTietKiem.address = TenXaPhuong;

                GiaoHangTietKiem.weight = 1000;
                GiaoHangTietKiem.value = 3000000;
                GiaoHangTietKiem.transport = "road";
                GiaoHangTietKiem.deliver_option = "xteam";

                string json = JsonConvert.SerializeObject(GiaoHangTietKiem);
                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var responeTask = client.PostAsync("/services/shipment/fee", httpContent);
                responeTask.Wait();

                if (responeTask.Result.IsSuccessStatusCode)
                {
                    var readJob = responeTask.Result.Content.ReadAsAsync<GiaoHangTietKiem>();
                    readJob.Wait();
                    return readJob.Result.fee.ship_fee_only.ToString();
                }
            }
            return "30000";
        }

        public string TinhPhiGiaoHangNhanh(string IdTenTinh, string IdTenQuanHuyen, string IdTenXaPhuong)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://online-gateway.ghn.vn");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("token", "c07a24a1-6853-11ec-bde8-6690e1946f41");

                ParamFreeGiaoHangNhanh Giaohanhnhanh = new ParamFreeGiaoHangNhanh();

                Giaohanhnhanh.from_district_id = 1485;
                Giaohanhnhanh.service_id = 53320;
                Giaohanhnhanh.service_type_id = 2;

                Giaohanhnhanh.to_district_id = int.Parse(IdTenQuanHuyen);
                Giaohanhnhanh.to_ward_code = IdTenXaPhuong;

                Giaohanhnhanh.height = 50;
                Giaohanhnhanh.length = 20;
                Giaohanhnhanh.weight = 200;
                Giaohanhnhanh.width = 20;
                Giaohanhnhanh.insurance_value = 10000;
                Giaohanhnhanh.coupon = null;

                string json = JsonConvert.SerializeObject(Giaohanhnhanh);
                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var responeTask = client.PostAsync("/shiip/public-api/v2/shipping-order/fee", httpContent);
                responeTask.Wait();

                if (responeTask.Result.IsSuccessStatusCode)
                {
                    var readJob = responeTask.Result.Content.ReadAsAsync<GiaoHangNhanh>();
                    readJob.Wait();
                    return readJob.Result.data.service_fee.ToString();
                }
            }
            return "30000";
        }

        public JsonResult ThucHienThanhToan(string txtSDT, string txtTen, string txtEmail, string txtDiaChi, string txtTinhThanh, string txtQuanHuyen, string txtNoiDung, string HinhThucThanhToan,
            string txtXaPhuong, string HTThanhToan, string HinhThucGiaoHang, string PhiGiaoHang)
        {
            Message message = new Message();
            try
            {
                PhanHoi thanhtoan = new PhanHoi();
                thanhtoan.txtSDT = txtSDT;
                thanhtoan.txtTen = txtTen;
                thanhtoan.txtEmail = txtEmail;
                thanhtoan.txtDiaChi = txtDiaChi;
                thanhtoan.txtTinhThanh = txtTinhThanh;
                thanhtoan.txtQuanHuyen = txtQuanHuyen;
                thanhtoan.txtNoiDung = txtNoiDung;
                thanhtoan.txtXaPhuong = txtXaPhuong;
                thanhtoan.HinhThucThanhToan = HinhThucThanhToan;
                thanhtoan.HTThanhToan = HTThanhToan;
                thanhtoan.HinhThucGiaoHang = HinhThucGiaoHang;
                thanhtoan.PhiGiaoHang = PhiGiaoHang;
                Session["ThanhToan"] = thanhtoan;
                message.Icon = "success";
            }
            catch (Exception e)
            {
                message.Icon = "error";
                message.Title = "Có lỗi:" + e.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public ActionResult XacNhanThanhToan()
        {
            PhanHoi thanhtoan = Session["ThanhToan"] as PhanHoi;
            ViewBag.thanhtoan = thanhtoan;
            LoginInfor login = new LoginInfor();
            login = Session["Login"] as LoginInfor;
            ViewBag.login = login;
            return View();
        }

        public ActionResult ThanhThoanVNPay()
        {
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma website
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
            Cart cart = new Cart();
            List<CartItem> GioiHang = Session["GioiHang"] as List<CartItem>;
            long IdOrder = DateTime.Now.Ticks;
            Session["DonHang"] = IdOrder;

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (cart.TongTien(GioiHang) * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + IdOrder);
            vnpay.AddRequestData("vnp_OrderType", "billpayment"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", IdOrder.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Redirect(paymentUrl);
        }

        public ActionResult ThanhThoanThanhCong()
        {
            LoginInfor login = new LoginInfor();
            login = Session["Login"] as LoginInfor;
            List<CartItem> GioiHang = Session["GioiHang"] as List<CartItem>;
            Cart cart = new Cart();
            PhanHoi thanhtoan = Session["ThanhToan"] as PhanHoi;

            DonHang donhang = new DonHang();
            donhang.ngayDat = DateTime.Now;
            donhang.diaChiGiao = thanhtoan.txtXaPhuong + " - " + thanhtoan.txtQuanHuyen + " - " + thanhtoan.txtTinhThanh;
            donhang.moTa = thanhtoan.txtNoiDung;
            donhang.tongTien = cart.TongTien(GioiHang) + double.Parse(thanhtoan.PhiGiaoHang);
            donhang.idKhach = login.IdKhachHang;
            donhang.trangThai = 1;
            if (thanhtoan.HinhThucGiaoHang == "0")
            {
                donhang.donViGiao = "Giao hàng tiết kiệm";
            }
            else
            {
                donhang.donViGiao = "Giao hàng nhanh";
            }

            string json = JsonConvert.SerializeObject(donhang);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            string IdDonHang = "";
            using (var DbContext = new WebBanHangEntities())
            {
                DbContext.DonHangs.Add(donhang);
                DbContext.SaveChanges();
                IdDonHang = donhang.idDon.ToString();
                List<CartItem> GioiHang_ = new List<CartItem>();
                foreach (var item in GioiHang)
                {
                    CartItem cartItem = new CartItem();
                    cartItem.sanpham.idSp = item.sanpham.idSp;
                    cartItem.sanpham.trangThai = item.sanpham.trangThai;
                    cartItem.sanpham.giaKm = item.sanpham.giaKm;
                    cartItem.sanpham.gia = item.sanpham.gia;
                    cartItem.soluong = item.soluong;
                    cartItem.IdDonHang = int.Parse(IdDonHang);
                    GioiHang_.Add(cartItem);
                }

                foreach (var item in GioiHang_)
                {
                    CT_DonHang ctDonHang = new CT_DonHang();
                    ctDonHang.idDon = item.IdDonHang;
                    ctDonHang.idSp = item.sanpham.idSp;
                    if (item.sanpham.trangThai == 2)
                    {
                        ctDonHang.giaBan = item.sanpham.giaKm;
                        ctDonHang.thanhTien = item.sanpham.giaKm * item.soluong;
                    }
                    else
                    {
                        ctDonHang.giaBan = item.sanpham.gia;
                        ctDonHang.thanhTien = item.sanpham.gia * item.soluong;
                    }
                    ctDonHang.soLuong = item.soluong;
                    DbContext.CT_DonHang.Add(ctDonHang);

                    var sp = DbContext.SanPhams.Where(e => e.idSp == item.sanpham.idSp).FirstOrDefault();
                    if (sp != null)
                    {
                        sp.soLuong = sp.soLuong - item.soluong;
                    }
                }
                DbContext.SaveChanges();

            }
            Session["GioiHang"] = null;
            Session["ThanhToan"] = null;

            return View();
        }

        public ActionResult ThanhThoanThanhCongVNPay()
        {
            LoginInfor login = new LoginInfor();
            login = Session["Login"] as LoginInfor;
            List<CartItem> GioiHang = Session["GioiHang"] as List<CartItem>;
            Cart cart = new Cart();
            PhanHoi thanhtoan = Session["ThanhToan"] as PhanHoi;

            DonHang donhang = new DonHang();
            donhang.ngayDat = DateTime.Now;
            donhang.diaChiGiao = thanhtoan.txtXaPhuong + " - " + thanhtoan.txtQuanHuyen + " - " + thanhtoan.txtTinhThanh;
            donhang.moTa = thanhtoan.txtNoiDung;
            donhang.tongTien = cart.TongTien(GioiHang) + double.Parse(thanhtoan.PhiGiaoHang);
            donhang.idKhach = login.IdKhachHang;
            donhang.trangThai = 2;
            if (thanhtoan.HinhThucGiaoHang == "0")
            {
                donhang.donViGiao = "Giao hàng tiết kiệm";
            }
            else
            {
                donhang.donViGiao = "Giao hàng nhanh";
            }

            string json = JsonConvert.SerializeObject(donhang);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            using (var DbContext = new WebBanHangEntities())
            {
                DbContext.DonHangs.Add(donhang);
                DbContext.SaveChanges();

                List<CartItem> GioiHang_ = new List<CartItem>();
                foreach (var item in GioiHang)
                {
                    CartItem cartItem = new CartItem();
                    cartItem.sanpham.idSp = item.sanpham.idSp;
                    cartItem.sanpham.trangThai = item.sanpham.trangThai;
                    cartItem.sanpham.giaKm = item.sanpham.giaKm;
                    cartItem.sanpham.gia = item.sanpham.gia;
                    cartItem.soluong = item.soluong;
                    cartItem.IdDonHang = donhang.idDon;
                    GioiHang_.Add(cartItem);
                }

                foreach (var item in GioiHang_)
                {
                    CT_DonHang ctDonHang = new CT_DonHang();
                    ctDonHang.idDon = item.IdDonHang;
                    ctDonHang.idSp = item.sanpham.idSp;
                    if (item.sanpham.trangThai == 2)
                    {
                        ctDonHang.giaBan = item.sanpham.giaKm;
                        ctDonHang.thanhTien = item.sanpham.giaKm * item.soluong;
                    }
                    else
                    {
                        ctDonHang.giaBan = item.sanpham.gia;
                        ctDonHang.thanhTien = item.sanpham.gia * item.soluong;
                    }
                    ctDonHang.soLuong = item.soluong;
                    DbContext.CT_DonHang.Add(ctDonHang);
                    var sp = DbContext.SanPhams.Where(e => e.idSp == item.sanpham.idSp).FirstOrDefault();
                    if (sp != null)
                    {
                        sp.soLuong = sp.soLuong - item.soluong;
                    }

                    DbContext.SaveChanges();
                }
                

                //string json_ = JsonConvert.SerializeObject(GioiHang_);
                //StringContent httpContent_ = new StringContent(json_, System.Text.Encoding.UTF8, "application/json");

                //var responeTask_ = client.PostAsync("SanPhamAPI/LuuChiTietDonHang", httpContent_);
                //responeTask_.Wait();

                //if (responeTask_.Result.IsSuccessStatusCode)
                //{
                //    var readJob_ = responeTask_.Result.Content.ReadAsAsync<Message>();
                //    readJob_.Wait();
                //}
            }
            Session["GioiHang"] = null;
            Session["ThanhToan"] = null;

            return View();
        }
    }
}