using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLClient_Server.Models
{
    public class ModelTinhThanhQuanHuyen
    {
    }
    public class Datum
    {
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public int CountryID { get; set; }
        public string Code { get; set; }
        public List<string> NameExtension { get; set; }
        public int IsEnable { get; set; }
        public int RegionID { get; set; }
        public int UpdatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public bool CanUpdateCOD { get; set; }
        public int Status { get; set; }
        public string UpdatedIP { get; set; }
        public int? UpdatedEmployee { get; set; }
        public string UpdatedSource { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class TinhThanh
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<Datum> data { get; set; }
    }

    public class WhiteListClient
    {
        public object From { get; set; }
        public object To { get; set; }
        public object Return { get; set; }
    }

    public class WhiteListDistrict
    {
        public object From { get; set; }
        public object To { get; set; }
    }

    public class DataQuanHuyen
    {
        public int DistrictID { get; set; }
        public int ProvinceID { get; set; }
        public string DistrictName { get; set; }
        public string Code { get; set; }
        public int Type { get; set; }
        public int SupportType { get; set; }
        public List<string> NameExtension { get; set; }
        public int IsEnable { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public bool CanUpdateCOD { get; set; }
        public WhiteListClient WhiteListClient { get; set; }
        public WhiteListDistrict WhiteListDistrict { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonMessage { get; set; }
        public int? UpdatedBy { get; set; }
        public int? Status { get; set; }
        public string UpdatedIP { get; set; }
        public int? UpdatedEmployee { get; set; }
        public string UpdatedSource { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class QuanHuyen
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<DataQuanHuyen> data { get; set; }
    }

    public class WhiteListWard
    {
        public object From { get; set; }
        public object To { get; set; }
    }

    public class DataXaPhuong
    {
        public string WardCode { get; set; }
        public int DistrictID { get; set; }
        public string WardName { get; set; }
        public List<string> NameExtension { get; set; }
        public int IsEnable { get; set; }
        public bool CanUpdateCOD { get; set; }
        public int UpdatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public int SupportType { get; set; }
        public WhiteListClient WhiteListClient { get; set; }
        public WhiteListWard WhiteListWard { get; set; }
        public int Status { get; set; }
        public string ReasonCode { get; set; }
        public string ReasonMessage { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class XaPhuong
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<DataXaPhuong> data { get; set; }
    }
}