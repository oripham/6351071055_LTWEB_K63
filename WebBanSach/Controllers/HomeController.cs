using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models;

namespace WebBanSach.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        private readonly QLBANSACHEntities _context = new QLBANSACHEntities();
        public ActionResult Index()
        {

            var sachList = _context.SACHes.ToList();
            ViewBag.sachList = sachList;

            return View();
        }

        public ActionResult ChuDe(int maCD=0)
        {
            var sachList = _context.SACHes
                            .Where(s => s.MaCD == maCD)
                            .ToList();

            ViewBag.title = _context.CHUDEs
                        .Where(s => s.MaCD == maCD)
                        .Select(s => s.TenChuDe)
                        .FirstOrDefault();

            ViewBag.sachList = sachList;

            return View("ChuDe");
        }

        public ActionResult NhaXuatBan(int maNXB=0)
        {
            var sachList = _context.SACHes
                            .Where(s => s.MaNXB == maNXB)
                            .ToList();

            ViewBag.title = _context.NHAXUATBANs
                        .Where(s => s.MaNXB == maNXB)
                        .Select(s => s.TenNXB)
                        .FirstOrDefault();

            ViewBag.sachList = sachList;
            ViewBag.chudeList = _context.CHUDEs.ToList();
            ViewBag.nhaxuatbanList = _context.NHAXUATBANs.ToList();

            return View("NhaXuatBan");
        }

        public ActionResult Detail(int id=0) 
        {
           

            var book = _context.SACHes.Where(s => s.Masach == id).FirstOrDefault();
            ViewBag.book = book;

            return View("Detail");
        }
    }
}