using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLClient_Server.Models
{
    public class ParamFree
    {
    }
    public class ParamFreeGiaoHangNhanh
    {
        public int from_district_id { get; set; }
        public int service_id { get; set; }
        public object service_type_id { get; set; }
        public int to_district_id { get; set; }
        public string to_ward_code { get; set; }
        public int height { get; set; }
        public int length { get; set; }
        public int weight { get; set; }
        public int width { get; set; }
        public int insurance_value { get; set; }
        public object coupon { get; set; }
    }

    public class ParamFreeGiaoTietKiem
    {
        public string pick_province { get; set; }
        public string pick_district { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public string address { get; set; }
        public int weight { get; set; }
        public int value { get; set; }
        public string transport { get; set; }
        public string deliver_option { get; set; }
        public List<int> tags { get; set; }
    }
}