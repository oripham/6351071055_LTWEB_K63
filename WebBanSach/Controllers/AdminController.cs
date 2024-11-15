using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models;

using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Web.UI.WebControls;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Collections.ObjectModel;

namespace WebBanSach.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin

        private readonly QLBANSACHEntities _context = new QLBANSACHEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ManageBook(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;

            return View(_context.SACHes.ToList().OrderBy(n => n.Masach).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var username = collection["username"];
            var password = collection["password"];
            if (String.IsNullOrEmpty(username))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(password))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {

                Admin ad = _context.Admins.SingleOrDefault(n => n.UserAdmin == username && n.PassAdmin == password);
                if (ad != null)
                {
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công!";
                    Session["AccountAdmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            ViewData["username"] = username;
            return View();
        }
        [HttpGet]
        public ActionResult CreateNewBook()
        {
            ViewBag.MaCD = new SelectList(_context.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(_context.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateNewBook(SACH book, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaCD = new SelectList(_context.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(_context.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                        ViewBag.Thongbao = "Hình ảnh đã được tải lên thành công!";
                    }

                    book.Hinhminhhoa = fileName;

                    // Chuyển đổi giá trị Ngaycapnhat
                    DateTime ngayCapNhat;
                    if (DateTime.TryParse(Request.Form["Ngaycapnhat"], out ngayCapNhat))
                    {
                        book.Ngaycapnhat = ngayCapNhat;
                    }
                    else
                    {
                        book.Ngaycapnhat = DateTime.Now; // Gán giá trị mặc định nếu không hợp lệ
                    }

                    _context.SACHes.Add(book);
                    _context.SaveChanges();
                }
                return RedirectToAction("ManageBook");
            }
            else
            {
                ViewBag.Thongbao = "Vui lòng chọn một file.";
                return View();
            }

        }
        public ActionResult DetailBook(int id)
        {
            SACH book = _context.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = book.Masach;
            if (book == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(book);
        }
        [HttpGet]
        public ActionResult DeleteBook(int id)
        {
            SACH book = _context.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.MaSach = book.Masach;
            if (book == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(book);
        }

        [HttpPost, ActionName("DeleteBook")]

        public ActionResult ConfirmDelete(int id)
        {
            SACH book = _context.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.MaSach = book.Masach;
            if (book == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            _context.SACHes.Remove(book);
            _context.SaveChanges();
            return RedirectToAction("ManageBook");
        }

        [HttpGet]
        public ActionResult EditBook(int id)
        {
            //if (Session["AccountAdmin"] == null)
            //{
            //    return RedirectToAction("Login", "Admin");
            //}
            SACH sach = _context.SACHes.SingleOrDefault(n => n.Masach == id);

            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaCD = new SelectList(_context.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(_context.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View(sach);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditBook(SACH sach, HttpPostedFileBase fileUpload)
        {
            //if (Session["AccountAdmin"] == null)
            //{
            //    return RedirectToAction("Login", "Admin");
            //}
            ViewBag.MaCD = new SelectList(_context.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(_context.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            if (ModelState.IsValid)
            {
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                        return View(sach);
                    }
                    else
                    {
                        try
                        {
                            fileUpload.SaveAs(path);
                            sach.Hinhminhhoa = fileName;
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Thongbao = "Không thể lưu hình ảnh: " + ex.Message;
                            return View(sach);
                        }
                    }
                }
                else
                {
                    sach.Hinhminhhoa = _context.SACHes
                        .AsNoTracking()
                        .FirstOrDefault(s => s.Masach == sach.Masach)?.Hinhminhhoa;
                }

                sach.Ngaycapnhat = DateTime.Now;

                try
                {
                    _context.Entry(sach).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("ManageBook");
                }
                catch (Exception ex)
                {
                    ViewBag.Thongbao = "Không thể cập nhật sách: " + ex.Message;
                    return View(sach);
                }
            }
            else
            {
                ViewBag.Thongbao = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(sach);
            }


        }
        public ActionResult BookStatistics()
        {
            var data = _context.SACHes
                .GroupBy(s => s.MaCD)
                .Select(g => new
                {
                    CategoryName = _context.CHUDEs.FirstOrDefault(cd => cd.MaCD == g.Key).TenChuDe,
                    Quantity = g.Count()
                })
                .ToList();

            ViewBag.ChartData = data;
            return View();
        }
    }
}
