using BTLClient_Server.EF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LapTrinhTichHop.Models
{
    public class CTDonHang
    {
        public string TenSanPham { get; set; }
        public double? DonGia { get; set; }
        public int? SoLuong { get; set; }
        public double? ThanhTien { get; set; }
        public string AnhDD { get; set; }
        [JsonIgnore]
        public List<SanPham> lstSanPhamNoiBat { get; set; }
        [JsonIgnore]
        public List<SanPham> lstDanhSachSanPham { get; set; }
        [JsonIgnore]
        public List<SanPham> lstGetAllSanPham { get; set; }
        public CTDonHang()
        {
            lstSanPhamNoiBat = new List<SanPham>();
            lstDanhSachSanPham = new List<SanPham>();
            lstGetAllSanPham = new List<SanPham>();
        }
        public CTDonHang(List<SanPham> lst_SanPhamNoiBat, List<SanPham> lst_DanhSachSanPham, List<SanPham> lst_GetAllSanPham)
        {
            lstSanPhamNoiBat = lst_SanPhamNoiBat;
            lstDanhSachSanPham = lst_DanhSachSanPham;
            lstGetAllSanPham = lst_GetAllSanPham;
        }
    }
}