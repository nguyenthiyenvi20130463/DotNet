using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTTHAPI.Models
{
    public class PhanHoi
    {
        public string txtHo { get; set; }
        public string txtTen { get; set; }
        public string txtEmail { get; set; }
        public string txtSDT { get; set; }
        public string txtNoiDung { get; set; }

        public string idkhach { get; set; }

        public string tenKhachHang { get; set; }

        public string noiDung { get; set; }

        public Nullable<System.DateTime> ngayTao { get; set; }
        public string IdSanPham { get; set; }
        public string NoiDungPhanHoi { get; set; }
        public string txtQuanHuyen { get; set; }
        public string txtDiaChi { get; set; }
        public string HinhThucThanhToan { get; set; }
        public string txtTinhThanh { get; set; }
        public string txtXaPhuong { get; set; }
        public string HTThanhToan { get; set; }
        public string HinhThucGiaoHang { get; set; }
        public string PhiGiaoHang { get; set; }
    }
    public class AnhSanPham
    {
        public string IdSanPham { get; set; }
        public string Anh { get; set; }
    }
}