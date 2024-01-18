using BTLClient_Server.Areas.Admin.Models.DTO;
using BTLClient_Server.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTLClient_Server.Areas.Admin.Controllers
{
    public class AdminLienHeController : Controller
    {
        // GET: Admin/LienHe
        public ActionResult Index(string keywords, string startTime, string endTime, int pageNum = 1, int pageSize = 7)
        {
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.keywords = keywords;
            
            if (keywords == null)
                keywords = "";
            DateTime? StartDdate = new DateTime(2020, 11, 10);
            if (!string.IsNullOrEmpty(startTime))
            {
                var ngayBatDau = startTime.Split('-');
                StartDdate = new DateTime(int.Parse(ngayBatDau[0]), int.Parse(ngayBatDau[1]), int.Parse(ngayBatDau[2]));
            }
            DateTime? EndDdate = DateTime.Now.AddDays(12);
            if (!string.IsNullOrEmpty(endTime))
            {
                var ngayKetThuc = endTime.Split('-');
                EndDdate = new DateTime(int.Parse(ngayKetThuc[0]), int.Parse(ngayKetThuc[1]), int.Parse(ngayKetThuc[2]));
            }
            IEnumerable<LienHeDTO> lst = null;
            using (var DbContext = new WebBanHangEntities())
            {
                var TaiKhoan = DbContext.KhachHangs.ToList();
                //lst = DbContext.Database.SqlQuery<LienHeDTO>(string.Format("listLienHe N'{0}', '{1}', '{2}'", keywords, startTime, endTime)).ToList<LienHeDTO>().ToPagedList<LienHeDTO>(pageNum, pageSize);
                lst = (from lh in DbContext.LienHes
                       where (string.IsNullOrEmpty(keywords) || lh.Email.Contains(keywords))
                       && (lh.NgayTao >= StartDdate) && (lh.NgayTao <= EndDdate)
                       select new LienHeDTO
                       {
                           IdLienHe=lh.IdLienHe,
                           NoiDung = lh.NoiDung,
                           Ho = lh.Ho,
                           Ten = lh.Ten,
                           Email = lh.Email,
                           SDT = lh.SDT,
                           idKhach = lh.idKhach,
                           NgayTao = lh.NgayTao
                       }).ToList();
                if (lst != null)
                {
                    foreach (var item in lst)
                    {
                        item.NgayTao_ = item.NgayTao.ToString("dd/MM/yyyy");
                        item.TaiKhoan = "";
                        if (item.idKhach != null)
                        {
                            item.TaiKhoan = TaiKhoan.Where(e => e.idKhach == item.idKhach).Select(e => e.tenDangNhap).FirstOrDefault();
                        }
                    }
                }
                
                lst = lst.ToPagedList<LienHeDTO>(pageNum, pageSize);
            }
            return View(lst);
        }

        public ActionResult Delete(int id)
        {
            using (var DbContext = new WebBanHangEntities())
            {
                LienHe pro = DbContext.LienHes.Find(id);
                if (pro != null)
                {
                    DbContext.LienHes.Remove(pro);
                    DbContext.SaveChanges();
                }
            }
            return Redirect("~/Admin/AdminLienHe/Index");
        }
    }
}