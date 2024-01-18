using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTTHAPI.Models
{
    public class Message
    {
		public string Icon { get; set; }
		public string Title { get; set; }
		public string Data { get; set; }
		public float TongTien { get; set; }
		public float TongSanPham { get; set; }
		public string DataChiTietGioHang { get; set; }
		public int Thang { get; set; }
		public int Nam { get; set; }
	}
}