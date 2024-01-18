using LTTH_UI_UX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTLClient_Server.Areas.Admin.Models.DTO
{
    public class OrderDTO
    {
        public int idDon { get; set; }
        public Nullable<System.DateTime> ngayDat { get; set; }
        public string diaChiGiao { get; set; }
        public string moTa { get; set; }
        public Nullable<double> tongTien { get; set; }
        public Nullable<int> idKhach { get; set; }
        public Nullable<int> trangThai { get; set; }
        public string hoTen { get; set; }
        public string sdt { get; set; }
        public Nullable<int> idUser { get; set; }
        public string donViGiao { get; set; }

        public List<ItemDTO> listItem = new List<ItemDTO>();

        public OrderDTO()
        {

        }
        public void addItem(ItemDTO tmp)
        {
            bool co = false;
            foreach (ItemDTO i in listItem)
                if (i.idSp == tmp.idSp)
                {
                    i.soLuong += tmp.soLuong;
                    co = true;
                    break;
                }
            if (!co)
                listItem.Add(tmp);
        }
        public void updateItem(int id, int quantity)
        {
            foreach (ItemDTO i in listItem)
                if (i.idSp == id)
                {
                    i.soLuong = quantity;
                    i.thanhTien = i.soLuong * i.giaBan;
                    return;
                }
        }
        public void deleteItem(int id)
        {
            for (int i = 0; i < listItem.Count; i++)
            {
                if (listItem[i].idSp == id)
                {
                    listItem.Remove(listItem[i]);
                    i--;
                }
            }
        }
        public double getSum(int id)
        {
            double tong = 0;
            foreach (ItemDTO i in listItem)
                if(i.idSp == id)
                {
                    tong = Convert.ToDouble(i.thanhTien);
                    break;
                }
            return tong;
        }
        public double getTotalMoney()
        {
            double tong = 0;
            foreach (ItemDTO i in listItem)
                tong += Convert.ToDouble(i.soLuong * i.giaBan);
            return tong;
        }
        //public OrderDTO FindOrderByID(int id)
        //{
        //    Order o = new OrderDAO().FindOrderByID(id);
        //    OrderDTO oRes = new OrderDTO();
        //    oRes.OrderID = id;
        //    oRes.Amount = o.Amount;
        //    oRes.Daytime = o.Daytime;
        //    oRes.DeliveryAddress = o.DeliveryAddress;
        //    oRes.OrderDescription = o.OrderDescription;
        //    oRes.OrderStatus = o.OrderStatus;
        //    oRes.UserID = o.UserID;
        //    User u = new UserDAO().FindUserByID(Convert.ToInt32(oRes.UserID));
        //    oRes.Phone = u.Phone;
        //    oRes.FullName = u.FullName;
        //    oRes.Email = u.Email;
        //    oRes.listItem = new ItemDAO().GetListItem(oRes.OrderID, "");
        //    return oRes;
        //}
    }
}