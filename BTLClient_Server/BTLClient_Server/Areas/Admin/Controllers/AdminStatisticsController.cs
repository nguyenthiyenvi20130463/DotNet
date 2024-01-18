using BTLClient_Server.Areas.Admin.Models.DTO;
using BTLClient_Server.Areas.Admin.Security;
using BTLClient_Server.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace BTLClient_Server.Areas.Admin.Controllers
{
    [AuthorizationAdmin]
    public class AdminStatisticsController : Controller
    {
        // GET: Admin/AdminStatistics
        public ActionResult Index(int? year, int? month)
        {
            if (year == null)
                year = DateTime.Now.Year;
            if (month == null)
                month = DateTime.Now.Month;

            IEnumerable<StatisticsDTO> lst = null;
            using (var DbContext = new WebBanHangEntities())
            {
                lst = DbContext.Database.SqlQuery<StatisticsDTO>(String.Format("SELECT * FROM Func_ThongKeDoanhThu({0}, {1})", year, month)
                    ).ToList();
                ViewBag.year = DbContext.Database.SqlQuery<int>("SELECT DISTINCT YEAR(ngayDat) FROM [dbo].[DonHang] ORDER BY YEAR(ngayDat) DESC"
                    ).ToList();
                ViewBag.total = DbContext.Database.SqlQuery<double>(String.Format("SELECT SUM(DoanhThu) FROM Func_ThongKeDoanhThu({0}, {1})", year, month)
                    ).FirstOrDefault() ;
            }

            List<int> ngay = new List<int>();
            List<double> doanhThu = new List<double>();
            foreach (var item in lst)
            {
                ngay.Add(item.Ngay);
                doanhThu.Add(item.DoanhThu);
            }
            ViewBag.days = ngay;
            ViewBag.revenues = doanhThu;
            ViewBag.yearSelected = year;
            ViewBag.month = month;
            return View();
        }

        public ActionResult HotProduct(int? year, int? month)
        {
            if (year == null)
                year = DateTime.Now.Year;
            if (month == null)
                month = DateTime.Now.Month;

            IEnumerable<ProductDTO> lst = null;
            using (var DbContext = new WebBanHangEntities())
            {
                lst = DbContext.Database.SqlQuery<ProductDTO>(String.Format("lstSearchHotProduct {0}, {1}", year, month)
                     ).ToList();
                ViewBag.year = DbContext.Database.SqlQuery<int>("SELECT DISTINCT YEAR(ngayDat) FROM [dbo].[DonHang] ORDER BY YEAR(ngayDat) DESC"
                    ).ToList();
            }
            ViewBag.yearSelected = year;
            ViewBag.month = month;
            return View(lst);
        }
    }
}