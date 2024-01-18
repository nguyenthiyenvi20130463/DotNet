using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLClient_Server.Areas.Admin.Models.DTO
{
    public class ProductDTO
    {
        public int idSp { get; set; }
        public string ten { get; set; }
        public Nullable<double> gia { get; set; }
        public Nullable<double> giaKm { get; set; }
        public Nullable<int> trangThai { get; set; }
        public string anhDaiDien { get; set; }
        public string tenThuongHieu { get; set; }
        public int Count { get; set; }
    }
}