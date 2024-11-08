using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSach.Models
{
    public class Cart
    {
        QLBANSACHEntities db = new QLBANSACHEntities();

        public int iMaSach {  get; set; }
        public string sTenSach { get; set; }
        public string sAnhBia { get; set; }
        public Double dDonGia { get; set; }
        public int iSoLuong {  get; set; }
        public Double dThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }

        public Cart(int MaSach)
        {
            iMaSach = MaSach;
            SACH sach = db.SACHes.Single(n => n.Masach == iMaSach);
            sTenSach = sach.Tensach;
            sAnhBia = sach.Hinhminhhoa;
            dDonGia = double.Parse(sach.Dongia.ToString());
            iSoLuong = 1;
        }
    }
}