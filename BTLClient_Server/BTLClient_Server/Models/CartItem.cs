using BTLClient_Server.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LapTrinhTichHop.Models
{
    public class CartItem
    {
        public SanPham sanpham { get; set; }
        public int soluong { get; set; }
        public int IdDonHang { get; set; }
    }
   
}