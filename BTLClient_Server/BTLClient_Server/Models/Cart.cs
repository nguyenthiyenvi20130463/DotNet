using BTLClient_Server.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace BTLClient_Server.Models
{
    public class CartItem
    {
        public SanPham sanpham { get; set; }
        public int soluong { get; set; }
        public int IdDonHang { get; set; }

        public CartItem()
        {
            sanpham = new SanPham();
            soluong = 0;
            IdDonHang = 0;
        }
    }
    public class Cart
    {
        public List<CartItem> ThemSanPham(List<CartItem> gioiHang,string IdSanPham,int SoLuong)
        {
            using (var DbContext = new WebBanHangEntities())
            {

                int IdSan_Pham = int.Parse(IdSanPham);
                var sanpham = DbContext.SanPhams.Where(e => e.idSp == IdSan_Pham).FirstOrDefault();
                var ThuongHieu = DbContext.ThuongHieux.Where(e => e.idThuongHieu == sanpham.idThuongHieu).FirstOrDefault();
                sanpham.ThuongHieu.tenThuongHieu = ThuongHieu.tenThuongHieu;
            
                if (gioiHang!=null)
                {
                    int Id_SanPham = int.Parse(IdSanPham);
                    var check = gioiHang.Where(e => e.sanpham.idSp == Id_SanPham).FirstOrDefault();
                    if(check!=null)
                    {
                        check.soluong = check.soluong + SoLuong;
                    }
                    else
                    {
                        CartItem cartitem = new CartItem();
                        cartitem.sanpham = sanpham;
                        cartitem.soluong = SoLuong;
                        gioiHang.Add(cartitem);
                    }
                }
                else
                {
                    List<CartItem> lstCart = new List<CartItem>();
                    CartItem cartitem = new CartItem();
                    cartitem.sanpham = sanpham;
                    cartitem.soluong = SoLuong;
                    lstCart.Add(cartitem);
                    gioiHang = lstCart;
                }
            }
            return gioiHang;
        }

        public List<CartItem> XoaSanPham(List<CartItem> gioiHang, string IdSanPham)
        {
            if(gioiHang!=null)
            {
                int Id_SanPham = int.Parse(IdSanPham);
                var check = gioiHang.Where(e => e.sanpham.idSp == Id_SanPham).FirstOrDefault();
                if(check!=null)
                {
                    gioiHang.Remove(check);
                }
            }
            return gioiHang;
        }

        public List<CartItem> CapNhatSanPham(List<CartItem> gioiHang, string IdSanPham, int SoLuong)
        {
            if (gioiHang != null)
            {
                int Id_SanPham = int.Parse(IdSanPham);
                var check = gioiHang.Where(e => e.sanpham.idSp == Id_SanPham).FirstOrDefault();
                if (check != null)
                {
                    check.soluong =  SoLuong;
                }
            }
            return gioiHang;
        }

        public double? TongTien(List<CartItem> gioiHang)
        {
            double? Tong = 0;
            if(gioiHang!=null)
            {
                foreach(var item in gioiHang)
                {
                    if(item.sanpham.trangThai==2)
                    {
                        Tong += item.sanpham.giaKm * item.soluong;
                    }
                    else
                    {
                        Tong += item.sanpham.gia * item.soluong;
                    }
                }
            }
            return Tong;
        }
    }
}