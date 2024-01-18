using BTLClient_Server.EF;
using BTLClient_Server.Areas.Admin.Models.DTO;
using BTLClient_Server.Areas.Admin.Security;
using LTTH_UI_UX.Models;
using Newtonsoft.Json;
using PagedList;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace BTLClient_Server.Areas.Admin.Controllers
{
    public class AdminHomeController : Controller
    {
        static bool ch = false;
        [AuthorizationAdmin]
        // GET: Admin/AdminHome
        public ActionResult Index(string tenK, string tt, string tu, string den, int pageNum = 1, int pageSize = 10)
        {
            Session["cartAdmin"] = null;
            ViewBag.tenK = tenK;
            ViewBag.tt = tt;
            ViewBag.tu = tu;
            ViewBag.den = den;
            if (tt == null)
                tt = "0";
            if (tu == null || tu == "")
                tu = new DateTime(2020, 11, 19).ToString("yyyy-MM-dd HH:mm:ss");
            if (den == null || den == "")
                den = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");

            IEnumerable<OrderDTO> lst = GetList_Order(tenK, int.Parse(tt), tu, den);
            lst = lst.ToPagedList<OrderDTO>(pageNum, pageSize);
            return View(lst);
        }

        public IEnumerable<OrderDTO> GetList_Order(string tenK, int tt, string tu, string den)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                if (tenK == null)
                    tenK = "";
                var lst = DbContext.Database.SqlQuery<OrderDTO>(string.Format("GetListDonHang N'{0}', '{1}', '{2}', '{3}'",
                    tenK, tt, tu, den)
                    ).ToList();
                for (int item = 0; item < lst.Count; item++)
                {
                    List<ItemDTO> listItem = DbContext.Database.SqlQuery<ItemDTO>(
                        string.Format("SELECT I.idSp, I.giaBan, I.soLuong, I.thanhTien, P.ten, p.anhDaiDien"
                        + " FROM dbo.CT_DonHang I, SanPham P WHERE I.idDon = {0} AND I.idSp = P.idSp", lst[item].idDon)
                    ).ToList();
                    if (listItem.Count > 0)
                    {
                        foreach (var i in listItem)
                        {
                            lst[item].listItem.Add(i);
                        }
                    }
                    else
                    {
                        lst = lst.Where(l => l.idDon != lst[item].idDon).ToList();
                        item--;
                    }
                }
                return lst;
            }
        }

            [HttpPost]
        public ActionResult Index(FormCollection data, int pageNum = 1, int pageSize = 6)
        {
            if (data.Count > 0)
            {
                string[] ids = data["checkBoxId"].Split(new char[] { ',' });
                foreach (string id in ids)
                {
                    using (var DbContext = new WebBanHangEntities())
                    {
                        DonHang pro = DbContext.DonHangs.Find(id);
                        if (pro != null)
                        {
                            DbContext.DonHangs.Remove(pro);
                        }
                    }
                }
            }
            return Redirect("~/Admin/AdminHome/Index");
        }

        public ActionResult CountOrder()
        {
            int c = 0;
            using (var DbContext = new WebBanHangEntities())
            {
                c = DbContext.DonHangs.Where(x => x.trangThai != 3).Count();
            }
            return PartialView(c);
        }

        [AuthorizationAdmin]
        public ActionResult Create()
        {
            using (var DbContext = new WebBanHangEntities())
            {
                DonHang donhang = new DonHang();
                var lstKhachHang = DbContext.KhachHangs.Where(e => e.trangThai == true).ToList();
                ViewBag.lstKhachHang = lstKhachHang;
                Session["cartAdmin"] = null;
                return View(donhang);
            }
        }
        [HttpPost]
        public ActionResult Create(DonHang dh, string idUs)
        {
            dh.ngayDat = DateTime.Now;
            dh.trangThai = 1;
            dh.idKhach = int.Parse(idUs);
            OrderDTO o = (OrderDTO)Session["cartAdmin"];
            dh.tongTien = o.getTotalMoney();

            using (var DbContext = new WebBanHangEntities())
            {
                DbContext.DonHangs.Add(dh);//luu tren RAM
                DbContext.SaveChanges();
                dh.idDon = dh.idDon;
            }
            //insert Item
            foreach (ItemDTO item in o.listItem)
            {
                CT_DonHang ct = new CT_DonHang();
                ct.idDon = dh.idDon;
                ct.idSp = item.idSp;
                ct.giaBan = item.giaBan;
                ct.soLuong = item.soLuong;
                ct.thanhTien = item.thanhTien;
                using (var DbContext = new WebBanHangEntities())
                {
                    DbContext.CT_DonHang.Add(ct);//luu tren RAM
                    DbContext.SaveChanges();//luu vao o dia
                }
            }
            return RedirectToAction("Index", "AdminHome");
        }

        public JsonResult ListProduct(string key)
        {
            IEnumerable<ProductDTO> lst = null;
            using (var DbContext = new WebBanHangEntities())
            {
                if (key == null)
                    key = "";
                lst = DbContext.Database.SqlQuery<ProductDTO>(string.Format("lstSearchProByNameInOrder N'{0}'", key)
                    ).ToList();
                foreach (var item in lst)
                {
                    if (item.trangThai == 2)
                    {
                        if (item.giaKm != null)
                            item.gia = item.giaKm;
                    }
                }
            }
            var data = lst;
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddItem(int id)
        {
            OrderDTO order = (OrderDTO)Session["cartAdmin"];
            if (order == null)
                order = new OrderDTO();
            ItemDTO item = new ItemDTO();
            SanPham sp = new SanPham();
            using (var DbContext = new WebBanHangEntities())
            {
                sp = DbContext.SanPhams.Find(id);
            }
            item.anhDaiDien = sp.anhDaiDien;
            item.idSp = sp.idSp;
            if (sp.trangThai == 2 && sp.giaKm != null)
                item.giaBan = sp.giaKm;
            else
                item.giaBan = sp.gia;
            item.soLuong = 1;
            item.ten = sp.ten;
            item.thanhTien = item.giaBan;

            order.addItem(item);
            Session["cartAdmin"] = order;
            ch = true;
            return Json(new
            {
                data = new ResultSum
                {
                    id = id,
                    value = item.thanhTien.HasValue ? item.thanhTien.Value : 0,
                    totalMoney = order.getTotalMoney()
                },
                status = true
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteItem(int id)
        {
            OrderDTO order = (OrderDTO)Session["cartAdmin"];
            order.deleteItem(id);
            Session["cartAdmin"] = order;
            ch = true;
            return Json(new
            {
                data = new ResultSum
                {
                    totalMoney = order.getTotalMoney()
                },
                status = true
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateItem(int id, int quantity)
        {
            OrderDTO order = (OrderDTO)Session["cartAdmin"];
            order.updateItem(id, quantity);
            Session["cartAdmin"] = order;
            ch = true;
            return Json(new
            {
                data = new ResultSum
                {
                    id = id,
                    value = order.getSum(id),
                    totalMoney = order.getTotalMoney()
                },
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListFullName(string key)
        {
            IEnumerable<KhachHang> lst = null;
            using (var DbContext = new WebBanHangEntities())
            {
                lst = DbContext.Database.SqlQuery<KhachHang>(string.Format("lstSearchCusByNameInOrder N'{0}'", key)
                ).ToList<KhachHang>();
            }
            var data = lst;
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizationAdmin]
        public ActionResult Edit(int id)
        {
            Session["cartAdmin"] = null;
            OrderDTO dh = new OrderDTO();
            using (var DbContext = new WebBanHangEntities())
            {
                var lst = DbContext.Database.SqlQuery<OrderDTO>(
                string.Format("SELECT O.idDon, O.ngayDat, O.diaChiGiao, O.tongTien, O.idKhach, O.trangThai, O.donViGiao, O.moTa, U.hoTen, U.sdt"
                                + " FROM[dbo].DonHang O, [dbo].KhachHang U"
                                + " WHERE O.idDon = {0} AND O.idKhach = U.idKhach", id)
                ).ToList();
                for (int item = 0; item < lst.Count; item++)
                {
                    List<ItemDTO> listItem = DbContext.Database.SqlQuery<ItemDTO>(
                        string.Format("SELECT I.idDon, I.idSp, I.giaBan, I.soLuong, I.thanhTien, P.ten, p.anhDaiDien"
                        + " FROM dbo.CT_DonHang I, SanPham P WHERE I.idDon = {0} AND I.idSp = P.idSp", lst[item].idDon)
                    ).ToList();
                    foreach (var i in listItem)
                    {
                        lst[item].listItem.Add(i);
                    }
                }
                dh = lst[0];
            }
            Session["cartAdmin"] = dh;
            return View(dh);
        }
        [HttpPost]
        public ActionResult Edit(OrderDTO o, string idUs, string tt)
        {
            if (idUs != "")
                o.idKhach = Convert.ToInt32(idUs);
            User u = (User)Session["user"];
            OrderDTO order = (OrderDTO)Session["cartAdmin"];

            DonHang dh = new DonHang();
            dh.idDon = o.idDon;
            dh.idKhach = o.idKhach;
            dh.idUser = u.idUser;
            dh.moTa = o.moTa;
            dh.trangThai = int.Parse(tt);
            dh.diaChiGiao = o.diaChiGiao;
            dh.donViGiao = o.donViGiao;
            dh.ngayDat = order.ngayDat;
            if (ch)
                dh.tongTien = order.getTotalMoney();
            else
                dh.tongTien = order.tongTien;

            //update Order
            using (var DbContext = new WebBanHangEntities())
            {
                DonHang DH = DbContext.DonHangs.Find(o.idDon);
                if (DH != null)
                {
                    DH.diaChiGiao = dh.diaChiGiao;
                    DH.idKhach = dh.idKhach;
                    DH.idUser = dh.idUser;
                    DH.moTa = dh.moTa;
                    DH.tongTien = dh.tongTien;
                    DH.trangThai = dh.trangThai;
                    DbContext.SaveChanges();//luu vao o dia
                }
            }
            //update Item
            if (ch)
            {
                foreach (ItemDTO item in order.listItem)
                {
                    CT_DonHang ct = new CT_DonHang();
                    ct.idDon = item.idDon;
                    ct.idSp = item.idSp;
                    ct.soLuong = item.soLuong;
                    ct.giaBan = item.giaBan;
                    ct.thanhTien = item.thanhTien;
                    using (var DbContext = new WebBanHangEntities())
                    {
                        DbContext.CT_DonHang.Add(ct);//luu tren RAM
                        DbContext.SaveChanges();//luu vao o dia
                    }
                }
            }
            ch = false;
            return RedirectToAction("Index", "AdminHome");
        }

        public ActionResult Delete(int id)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                DonHang pro = DbContext.DonHangs.Find(id);
                if (pro != null)
                {
                    DbContext.DonHangs.Remove(pro);
                    DbContext.SaveChanges();
                }
            }
            return Redirect("~/Admin/AdminHome/Index");
        }

        public ActionResult Print(int id)
        {
            return new ActionAsPdf("Invoice", new { id = id });
        }
        public ActionResult Invoice(int id)
        {
            Session["cartAdmin"] = null;
            OrderDTO dh = new OrderDTO();
            using (var DbContext = new WebBanHangEntities())
            {

               var lst = DbContext.Database.SqlQuery<OrderDTO>(
               string.Format("SELECT O.idDon, O.ngayDat, O.diaChiGiao, O.tongTien, O.idKhach, O.trangThai, O.donViGiao, O.moTa, U.hoTen, U.sdt"
                               + " FROM[dbo].DonHang O, [dbo].KhachHang U"
                               + " WHERE O.idDon = {0} AND O.idKhach = U.idKhach", id)
               ).ToList();
                for (int item = 0; item < lst.Count; item++)
                {
                    List<ItemDTO> listItem = DbContext.Database.SqlQuery<ItemDTO>(
                        string.Format("SELECT I.idDon, I.idSp, I.giaBan, I.soLuong, I.thanhTien, P.ten, p.anhDaiDien"
                        + " FROM dbo.CT_DonHang I, SanPham P WHERE I.idDon = {0} AND I.idSp = P.idSp", lst[item].idDon)
                    ).ToList();
                    foreach (var i in listItem)
                    {
                        lst[item].listItem.Add(i);
                    }
                }
                dh = lst[0];
            }
            return View(dh);
        }
    }
    public class ResultSum
    {
        public int id { get; set; }
        public double value { get; set; }
        public double totalMoney { get; set; }
    }
}