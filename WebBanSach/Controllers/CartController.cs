using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models;

namespace WebBanSach.Controllers
{
    public class CartController : BaseController
    {
        QLBANSACHEntities db = new QLBANSACHEntities();

        public List<Cart> GetCart() 
        { 
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if(lstCart == null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }

            return lstCart;
        }

        //Them hang vao gio
        public ActionResult AddCart(int iMasach, string strURL)
        {
            //Lay ra Session gio hang
            List<Cart> lstCart = GetCart();
            //Kiem tra sach nay ton tai trong Session["Cart"] chua?
            Cart sanpham = lstCart.Find(n => n.iMaSach == iMasach);
            if (sanpham == null)
            {
                sanpham = new Cart(iMasach);
                lstCart.Add(sanpham);
                //return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                //return Redirect(strURL);
            }
            return RedirectToAction("Cart", "Cart");
        }

        //Tong so luong
        private int SumAmount()
        {
            int iTongSoLuong = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                iTongSoLuong = lstCart.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }

        //Tinh tong tien
        private double TotalMoney()
        {
            double dTongTien = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                dTongTien = lstCart.Sum(n => n.dThanhTien);
            }
            return dTongTien;
        }

        //Xay dung trang Cart
        public ActionResult Cart()
        {
            List<Cart> lstCart = GetCart();
            ViewBag.TongSoLuong = SumAmount();
            ViewBag.TongTien = TotalMoney();
            //if (lstCart.Count == 0)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            return View(lstCart);
        }

        public ActionResult CartPartial()
        {
            ViewBag.TongSoLuong = SumAmount();
            ViewBag.TongTien = TotalMoney();
            return PartialView();
        }

        public ActionResult DeleteCart(int iMaSach)
        {
            List<Cart> lstCart = GetCart();
            Cart sp = lstCart.SingleOrDefault(n => n.iMaSach == iMaSach);
            if (sp != null)
            {
                lstCart.RemoveAll(n => n.iMaSach == iMaSach);
            }
            return RedirectToAction("Cart");
        }

        public ActionResult UpdateCart(int iMaSach, FormCollection f)
        {
            List<Cart> lstCart = GetCart();

            Cart sp = lstCart.SingleOrDefault(n => n.iMaSach == iMaSach);


            if (sp != null)
            {
                sp.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }

            return RedirectToAction("Cart");
        }

        public ActionResult DeleteAllCart()
        {
            List<Cart> lstCart = GetCart();
            lstCart.Clear();
            return RedirectToAction("Cart");
        }
        [HttpGet]
        public ActionResult Order()
        {
            if (Session["Account"] == null || Session["Account"].ToString() == "")
            {
                return RedirectToAction("Sign In", "User");
            }
            List<Cart> lstCart = GetCart();
            ViewBag.TongSoLuong = SumAmount();
            ViewBag.TongTien = TotalMoney();

            return View(lstCart);
        }

        // In GioHangController.cs

        public ActionResult Order(FormCollection collection)
        {
            // Thêm Đơn hàng
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Account"];
            List<Cart> lstCart = GetCart();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDH = DateTime.Now;
            var NgayGiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);
            ddh.Ngaygiaohang = DateTime.Parse(NgayGiao);
            ddh.Dagiao = false;
            ddh.HTThanhtoan = false;
            db.DONDATHANGs.Add(ddh);
            db.SaveChanges();
            // Thêm chi tiết đơn hàng
            foreach (var item in lstCart)
            {
                CTDATHANG ctdh = new CTDATHANG();
                ctdh.SoDH = ddh.SoDH;
                ctdh.Masach = item.iMaSach;
                ctdh.Soluong = item.iSoLuong;
                ctdh.Dongia = (decimal)item.dDonGia;
                db.CTDATHANGs.Add(ctdh);
            }
            db.SaveChanges();
            Session["Cart"] = null;
            return RedirectToAction("Confirm", "Cart");
        }

        // Method Xacnhandonhang in GioHangController
        public ActionResult Confirm()
        {
            return View();
        }

    }

}