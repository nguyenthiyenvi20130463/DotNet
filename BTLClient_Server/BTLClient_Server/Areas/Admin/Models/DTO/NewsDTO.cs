using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLClient_Server.Areas.Admin.Models.DTO
{
    public class NewsDTO
    {
        public int idTin { get; set; }
        public string hoTen { get; set; }
        public string anh { get; set; }
        public string tieuDe { get; set; }
        public Nullable<System.DateTime> ngayTao { get; set; }
    }

    public class LienHeDTO
    {
        public int IdLienHe { get; set; }
        public string Ho { get; set; }
        public string Ten { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public string NoiDung { get; set; }
        public int? idKhach { get; set; }
        public DateTime NgayTao { get; set; }
        public string NgayTao_ { get; set; }
        public string TaiKhoan { get; set; }

    }
}