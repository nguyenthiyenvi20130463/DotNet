using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLClient_Server.Areas.Admin.Models.DTO
{
    public class CustomerDTO
    {
        public int idKhach { get; set; }
        public string hoTen { get; set; }
        public string diaChi { get; set; }
        public string email { get; set; }
        public string sdt { get; set; }
        public string tenDangNhap { get; set; }
        public string matKhau { get; set; }
        public int Count { get; set; }
    }
}