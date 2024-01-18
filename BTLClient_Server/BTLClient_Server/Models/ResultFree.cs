using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLClient_Server.Models
{
    public class ResultFree
    {
    }
    public class DataGiaoHangNhanh
    {
        public int total { get; set; }
        public int service_fee { get; set; }
        public int insurance_fee { get; set; }
        public int pick_station_fee { get; set; }
        public int coupon_value { get; set; }
        public int r2s_fee { get; set; }
    }

    public class GiaoHangNhanh
    {
        public int code { get; set; }
        public string message { get; set; }
        public DataGiaoHangNhanh data { get; set; }
    }

    public class ExtFee
    {
        public string display { get; set; }
        public string title { get; set; }
        public int amount { get; set; }
        public string type { get; set; }
    }

    public class Fee
    {
        public string name { get; set; }
        public int fee { get; set; }
        public int insurance_fee { get; set; }
        public int include_vat { get; set; }
        public int cost_id { get; set; }
        public string delivery_type { get; set; }
        public string a { get; set; }
        public string dt { get; set; }
        public List<ExtFee> extFees { get; set; }
        public int ship_fee_only { get; set; }
        public string promotion_key { get; set; }
        public bool delivery { get; set; }
    }

    public class GiaoHangTietKiem
    {
        public bool success { get; set; }
        public string message { get; set; }
        public Fee fee { get; set; }
    }
}