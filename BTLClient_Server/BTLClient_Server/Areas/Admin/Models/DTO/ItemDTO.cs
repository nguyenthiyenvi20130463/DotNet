using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLClient_Server.Areas.Admin.Models.DTO
{
    public class ItemDTO
    {
        public int idDon { get; set; }
        public int idSp { get; set; }
        public double? giaBan { get; set; }
        public int soLuong { get; set; }
        public double? thanhTien { get; set; }
        public string ten { get; set; }
        public string anhDaiDien { get; set; }

    }
}