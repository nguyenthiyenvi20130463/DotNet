using BTLClient_Server.EF;
using BTLClient_Server.Areas.Admin.Models.DTO;
using BTLClient_Server.Areas.Admin.Security;
using LTTH_UI_UX.Models;
using Newtonsoft.Json;
using PagedList;
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
    public class AdminCustomerController : Controller
    {
        // GET: Admin/AdminCustome
        public ActionResult Index(string ten, string dc, int pageNum = 1, int pageSize = 9)
        {
            ViewBag.ten = ten;
            ViewBag.dc = dc;
            if (ten == null)
                ten = "";
            if (dc == null)
                dc = "";

            IEnumerable<CustomerDTO> lst = null;
            using (var DbContext = new WebBanHangEntities())
            {
                lst = GetListCus(ten, dc).ToPagedList<CustomerDTO>(pageNum, pageSize);
            }
            return View(lst);
        }

        public IEnumerable<CustomerDTO> GetListCus(string ten, string dc)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                var lst = DbContext.Database.SqlQuery<CustomerDTO>(string.Format("listCustomer N'{0}', N'{1}'", ten, dc)).ToList<CustomerDTO>();
                return lst;
            }

        }

        [HttpPost]
        public ActionResult Index(FormCollection data)
        {
            if (data.Count > 0)
            {
                string[] ids = data["checkBoxId"].Split(new char[] { ',' });
                foreach (string id in ids)
                {
                    using (var DbContext = new WebBanHangEntities())
                    {
                        KhachHang pro = DbContext.KhachHangs.Find(id);
                        if (pro != null)
                        {
                            DbContext.KhachHangs.Remove(pro);
                            DbContext.SaveChanges();
                        }
                    }
                }
            }
            return Redirect("~/Admin/AdminCustomer/Index");
        }

        public ActionResult Create()
        {
            KhachHang kh = new KhachHang();
            return View(kh);
        }
        [HttpPost]
        public ActionResult Create(KhachHang k)
        {
            string json = JsonConvert.SerializeObject(k);
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            using (var DbContext = new WebBanHangEntities())
            {
                try
                {
                    DbContext.KhachHangs.Add(k);//luu tren RAM
                    DbContext.SaveChanges();//luu vao o dia
                    return RedirectToAction("Index", "AdminCustomer");
                }
                catch (Exception e)
                {
                    return View(k);
                }
            }
        }
        public ActionResult Edit(int id)
        {
            KhachHang k = new KhachHang();
            using (var DbContext = new WebBanHangEntities())
            {
                k = DbContext.KhachHangs.Find(id);
            }
            return View(k);
        }
        [HttpPost]
        public ActionResult Edit(int id, KhachHang tmp)
        {
            if (ModelState.IsValid)
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    KhachHang k = DbContext.KhachHangs.Find(id);
                    if (k != null)
                    {
                        k.hoTen = tmp.hoTen;
                        k.diaChi = tmp.diaChi;
                        k.email = tmp.email;
                        k.sdt = tmp.sdt;
                        k.tenDangNhap = tmp.tenDangNhap;
                        DbContext.SaveChanges();//luu vao o dia
                        return RedirectToAction("Index", "AdminCustomer");
                    }
                }
            }
            return PartialView(tmp);
        }

        public ActionResult EditPassword(int id)
        {
            return View(id);
        }
        [HttpPost]
        public ActionResult EditPassword(int id, string password)
        {
            if (ModelState.IsValid)
            {
                using (var DbContext = new WebBanHangEntities())
                {
                    KhachHang k = DbContext.KhachHangs.Find(id);
                    if (k != null)
                    {
                        k.matKhau = password;
                        DbContext.SaveChanges();//luu vao o dia
                        return RedirectToAction("Index", "AdminCustomer");
                    }
                }
            }
            return PartialView(id);
        }
        public ActionResult ListOrder(int id, string startTime, string endTime)
        {
            ViewBag.userId = id;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            if (startTime == null || startTime == "")
                startTime = new DateTime(2020, 11, 19).ToString("yyyy-MM-dd HH:mm:ss");
            if (endTime == null || endTime == "")
                endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            IEnumerable<DonHang> lst = null;
            using (var DbContext = new WebBanHangEntities())
            {
                lst = DbContext.Database.SqlQuery<DonHang>(string.Format("SELECT * FROM DonHang WHERE idKhach = {0}"
                + " AND (ngayDat >= '{1}' AND ngayDat <= '{2}') ORDER BY idDon DESC", id, startTime, endTime)
                ).ToList<DonHang>().ToPagedList<DonHang>(1, 9);
            }
            return View(lst);
        }
        public ActionResult Delete(int id)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                KhachHang pro = DbContext.KhachHangs.Find(id);
                if (pro != null)
                {
                    DbContext.KhachHangs.Remove(pro);
                }
            }
            return Redirect("~/Admin/AdminCustomer/Index");
        }
    }
}